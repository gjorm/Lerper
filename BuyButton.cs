using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyButton : MonoBehaviour {

    public AudioClip acClickOn, acNoMoney;

    private Button button;
    private Sprite charSprite;
    private MenuButton menuButton;
    private LerpGameController gScript;
    private PlayerController pScript;
    private AudioSource audioOut;
    private Text charCostText;
    private int charCost, coinBankG;
    private int charIndex;
    private Text totCoinsField;
    private Animator anim;
    private Image img;
    private Text buyButText;
    private bool wiggle = false;
    private WatchAdsButton watchAdsBut;

    // Use this for initialization
    void Start()
    {
        menuButton = GameObject.FindGameObjectWithTag("MenuButton").GetComponent<MenuButton>();
        if (menuButton == null)
            Debug.Log("BuyButton couldnt get a reference to the Menu Button");


        button = GetComponent<Button>();
        if (button == null)
            Debug.Log("Menu Button script could not get a reference to the button sript.");
        else
            button.onClick.AddListener(BuyItem);
    

        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<LerpGameController>();
        if (gScript == null)
            Debug.Log("Buy Button script could not get a reference to the game controller.");

        pScript = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        if (pScript == null)
            Debug.Log("Buy Button script could not get a reference to the player controller.");

        audioOut = GetComponent<AudioSource>();
        if (audioOut == null)
            Debug.Log("Buy Button couldnt get its Audio Source component.");

        charCostText = GameObject.FindGameObjectWithTag("uiCharCostField").GetComponent<Text>();
        if (charCostText == null)
            Debug.Log("Buy button script could not get a reference to the Character Cost Text component.");

        totCoinsField = GameObject.FindGameObjectWithTag("uiTotalCoins").GetComponent<Text>();
        if (totCoinsField == null)
            Debug.Log("Buy button script couldnt get a reference to the Panels Total Coins Field");

        anim = GetComponent<Animator>();
        if (anim == null)
            Debug.Log("Buy Button couldnt get reference to its animator component");

        img = GetComponent<Image>();
        if (img == null)
            Debug.Log("Buy Button couldnt get reference to its Image component");

        buyButText = GetComponentInChildren<Text>();
        if (buyButText == null)
            Debug.Log("Buy Button Script couldnt get reference to the Text child component.");

        watchAdsBut = GameObject.FindGameObjectWithTag("uiWatchAds").GetComponent<WatchAdsButton>();
        if (watchAdsBut == null)
            Debug.Log("Buy Button script couldnt get reference to the watch ads button.");

        charIndex = 0;
        charSprite = null;
    }

    private void Update()
    {
        
        Image img = GetComponent<Image>();
        Color cA = pScript.GetCharColorA();
        Color cB = pScript.GetCharColorB();
        Color foo = Color.Lerp(cB, cA, pScript.GetLerpVal());
        foo.a = 255f;
        img.color = foo;


        if (wiggle == false)
        {
            anim.Play("Disabled");
            img.enabled = false;
            buyButText.enabled = false;
        }
        else
        {
            anim.Play("Normal");
            img.enabled = true;
            buyButText.enabled = true;
        }
    }

    public void BuyItem()
    {

        if (charIndex >= 0 && charIndex < pScript.GetNumModels() && !pScript.GetCharPurchased(charIndex))
        {
            charCost = pScript.GetCharCost(charIndex);
            coinBankG = gScript.GetBigCoinBank();
            if(charCost <= coinBankG)
            {
                gScript.SetBigCoinBank(coinBankG - charCost);
                string str = "" + gScript.GetBigCoinBank().ToString();
                totCoinsField.text = str;
                gScript.SetCoinCount();

                pScript.SetCharacterBought(charIndex);
                menuButton.SetCharSelected(charIndex, charSprite);
                audioOut.PlayOneShot(acClickOn, 0.8f);

                menuButton.ClearCharFields();

                // make sure to save this to saved game data
                GameSaver.gSaver.charBought[charIndex] = true;
                GameSaver.gSaver.SaveData();

                SetWiggle(false);
                button.enabled = false;
            }
            else
            {
                charCostText.text = "Not Enough";
                audioOut.PlayOneShot(acNoMoney, 0.8f);
                SetWiggle(false);
                watchAdsBut.SetWiggle(true);
            }
        }

    }

    public void SetCharIndex(int i, Sprite img)
    {
        charSprite = img;
        charIndex = i;
    }

    public void SetWiggle(bool b)
    {
        wiggle = b;
    }
}
