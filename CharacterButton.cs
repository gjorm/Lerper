using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour {

    public int charIndex;
    public AudioClip acClickOn, acClickErr, acClickSelect;

    private PlayerController pScript;
    private Text charName;
    private Text charCost;
    private Text charDist;
    private Text charJump;
    private Image charSelected;
    private AudioSource audioOut;
    private Button button;
    private GameObject buyButton;
    private MenuButton menuButton;
    private WatchAdsButton watchAdsBut;
    private GameObject scrollView;
    private ScrollRect scrollRect;
    private BuyPanelScript bPanel;
    private GameObject charSelectHighLight;
    private Canvas buyCanvas;
    private float scale;
    private bool selectOneShot;
    private bool updatedOneShot;
    private float scrollUnitsPerPixel;
    private bool playSound, saveOneShot;
    private DownArrowScript daScript;

    private void Awake()
    {
        charSelectHighLight = GameObject.FindGameObjectWithTag("uiCharHighLight");
        if (charSelectHighLight == null)
            Debug.Log("Couldnt get reference to the Character buttons Char Select HighLight Panel");

        charName = GetComponentInChildren<Text>();
        if (charName == null)
            Debug.Log("Character button script could not get a reference to its own Text component.");

        charCost = GameObject.FindGameObjectWithTag("uiCharCostField").GetComponent<Text>();
        if (charCost == null)
            Debug.Log("Character button script could not get a reference to the Character Cost Text component.");

        charJump = GameObject.FindGameObjectWithTag("uiJump").GetComponent<Text>();
        if (charJump == null)
            Debug.Log("Character button script could not get a reference to the Character Jump Text component.");

        charDist = GameObject.FindGameObjectWithTag("uiDistance").GetComponent<Text>();
        if (charDist == null)
            Debug.Log("Character button script could not get a reference to the Character Dist Text component.");

        daScript = GameObject.FindGameObjectWithTag("uiDownArrow").GetComponent<DownArrowScript>();
        if (daScript == null)
            Debug.Log("Character button script couldnt get reference to the down arrow script.");

        saveOneShot = false;
    }

    // Use this for initialization
    void Start () {

        selectOneShot = false;

        pScript = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        if (pScript == null)
            Debug.Log("Character Button script could not get a reference to the player controller.");

        charName.text = pScript.GetCharName(charIndex);

        charSelected = GameObject.FindGameObjectWithTag("uiCharSelected").GetComponent<Image>();
        if (charSelected == null)
            Debug.Log("Character button script couldnt get reference to the ui element for Character Selected");

        audioOut = GetComponent<AudioSource>();
        if (audioOut == null)
            Debug.Log("Character Button couldnt get its Audio Source component.");

        scrollView = GameObject.FindGameObjectWithTag("ScrollView");
        if (scrollView == null)
            Debug.Log("Character button could not get the scrollview reference.");
        else
        {
            scrollRect = scrollView.GetComponent<ScrollRect>();;
        }

        bPanel = GameObject.FindGameObjectWithTag("uiPanel").GetComponent<BuyPanelScript>();
        if (bPanel == null)
            Debug.Log("Character Button Couldnt Get reference to the ui Buy Panel");

        button = GetComponent<Button>();
        if (button == null)
            Debug.Log("Character Button script could not get a reference to the button sript.");
        else
            button.onClick.AddListener(SelectChar);

        buyButton = GameObject.FindGameObjectWithTag("uiBuyButton");
        if (buyButton == null)
        {
            Debug.Log("Character button couldnt get a reference to the Buy Button script.");
        }

        menuButton = GameObject.FindGameObjectWithTag("MenuButton").GetComponent<MenuButton>();
        if(menuButton == null)
        {
            Debug.Log("Character button couldnt get a reference to the Menu Button script.");
        }

        buyCanvas = GetComponentInParent<Canvas>();
        if (buyCanvas == null)
            Debug.Log("Character Buttons couldnt get reference to the Buy Canvas");

        updatedOneShot = false;
    }

    // Update is called once per frame
    void Update () {

        if (updatedOneShot == false)
            updatedOneShot = true;

        if(IsOnScreen())
        {

            // change the normal colors so that the button can still be interactible
            if (pScript.GetCharPurchased(charIndex) || IsInCenter())
            {
                ColorBlock b = button.colors;
                Color foo = Color.white;
                b.normalColor = foo;
                button.colors = b;
            }
            else
            {
                ColorBlock b = button.colors;
                Color foo = button.colors.disabledColor;
                b.normalColor = foo;
                button.colors = b;
            }

            // change the scaling if it is in center of screen
            scale = transform.position.x / Screen.width; // scale down to 0 - 1f
            scale *= Mathf.PI; // scale up to half circle radians aka PI
            scale = Mathf.Sin(scale); // apply to Sin function
            scale = (scale / 1.5f) + 0.333f; // only want 0.66 to 1 as a scale
            scale = Mathf.Clamp(scale, 0.666f, 1.0f);
            transform.localScale = new Vector3(scale, scale, transform.localScale.z);

            if(IsInCenter() && selectOneShot == false && bPanel.IsPanelMovedIn()) // fire the selectchar and tell scroll buttons to stop moving
            {
                selectOneShot = true;

                playSound = true;

                ForceToCenter();

                EnableCharHighLight();

                SelectChar();
            }
            else if(!IsInCenter())
            {
                if (selectOneShot == true)
                {
                    selectOneShot = false;
                    DisableCharHighLight();
                }
                    
            }

            // since moving the text fields to the new charselect panel, they dont update now.
            if (IsInCenter() && selectOneShot == true && bPanel.IsPanelMovedIn())
            {
                if (charSelectHighLight.activeSelf == false)
                {
                    charSelectHighLight.SetActive(true);
                    SelectChar();
                }

                if (!pScript.GetCharPurchased(charIndex))
                {
                    charCost.text = "" + pScript.GetCharCost(charIndex).ToString();
                    charDist.text = "" + pScript.GetCharDistance(charIndex).ToString();
                    charJump.text = "" + pScript.GetCharJumpTimes(charIndex).ToString();
                }
                else
                {
                    charCost.text = "";
                    charDist.text = "";
                    charJump.text = "";
                }
            }
        }
        else
        {
            transform.localScale = new Vector3(0.66f, 0.66f, transform.localScale.z);
        }
	}

    public Sprite GetSprite()
    {
        return gameObject.GetComponent<Image>().sprite;
    }

    public void SelectChar()
    {
        if(bPanel.IsPanelMovedIn())
        {
            watchAdsBut = GameObject.FindGameObjectWithTag("uiWatchAds").GetComponent<WatchAdsButton>();
            if (watchAdsBut == null)
                Debug.Log("Character Button script couldnt get reference to the watch ads button.");
            watchAdsBut.SetWiggle(false);

            
            if (!IsInCenter())
            {
                ForceToCenter();

                charSelectHighLight.SetActive(true);

                //playSound = false;
            }
            else
            {
                playSound = true;
            }
            

            // save the scroll position to saved data
            GameSaver.gSaver.scrollViewPosition = scrollRect.horizontalNormalizedPosition;

            if (!pScript.GetCharPurchased(charIndex))
            {
                Button btn = buyButton.GetComponent<Button>();
                if (btn == null)
                    Debug.Log("Character Button couldnt get a reference to the Buy Button Button Script");
                btn.enabled = true;

                BuyButton bbtn = buyButton.GetComponent<BuyButton>();
                if (btn == null)
                    Debug.Log("Menu Button couldnt get a reference to the Buy Button Button component.");
                else
                {
                    bbtn.SetWiggle(true);
                    Sprite img = gameObject.GetComponent<Image>().sprite;
                    bbtn.SetCharIndex(charIndex, img);
                }


                
                audioOut.PlayOneShot(acClickSelect, 0.9f);

                //get rid of arrow
                daScript.UnPop();

                menuButton.ShowCharFields();
                charCost.text = "" + pScript.GetCharCost(charIndex).ToString();
                charDist.text = "" + pScript.GetCharDistance(charIndex).ToString();
                charJump.text = "" + pScript.GetCharJumpTimes(charIndex).ToString();                
            }
            else
            {
                Button btn = buyButton.GetComponent<Button>();
                if (btn == null)
                    Debug.Log("Character Button couldnt get a reference to the Buy Button");
                btn.enabled = false;
                BuyButton bbtn = buyButton.GetComponent<BuyButton>();
                if (btn == null)
                    Debug.Log("Menu Button couldnt get a reference to the Buy Button Button component.");
                else
                    bbtn.SetWiggle(false);


                
                audioOut.PlayOneShot(acClickOn, 0.8f);

                //make the arrow pop
                daScript.Pop();

                menuButton.ClearCharFields();
                charCost.text = "";
                charDist.text = "";
                charJump.text = "";                

                Sprite img = gameObject.GetComponentInChildren<Image>().sprite;
                menuButton.SetCharSelected(charIndex, img);
            }
        }
    }

    public bool IsOnScreen()
    {
        bool result = false;

        if(transform.position.x > 0 && transform.position.y <= Screen.width)
            result = true;

        return result;
    }

    public bool IsInCenter()
    {
        bool result = false;

        if (transform.position.x >= ((Screen.width / 2) - 60) && transform.position.x <= ((Screen.width / 2) + 60))
        {
            result = true;
            ForceToCenter();
        }

        if(!result)
        {
            saveOneShot = false;
        }

        return result;
    }

    public void EnableCharHighLight()
    {
        if (charSelectHighLight != null && charSelectHighLight.activeSelf == false)
        {
            charSelectHighLight.SetActive(true);
        }
    }

    public void DisableCharHighLight()
    {
        if (charSelectHighLight != null && charSelectHighLight.activeSelf == true)
        {
            charSelectHighLight.SetActive(false);
        }
    }

    public void ForceToCenter()
    {
        //Debug.Log("ForceToCenter()");

        RectTransform rect = GetComponent<RectTransform>();
        if (rect != null)
        {
            scrollUnitsPerPixel = 1 / ((rect.rect.width * buyCanvas.scaleFactor) * 64); //64 is roughly how many Character buttons fit on the scroll view
        }
        else
        {
            Debug.Log("Char button couldnt get reference to its own rect transform.");
        }

        //stop the scrolling.
        scrollRect.velocity = new Vector2(0, 0);
        //so after the scrolling stops because oncenter was detected, the scroll position is adjusted to "perfect" center afterwards. This
        //allows for the widths of the oncenter catch to be widened to help with scrolling
        scrollRect.horizontalNormalizedPosition = scrollRect.horizontalNormalizedPosition + ((transform.position.x - (Screen.width / 2)) * scrollUnitsPerPixel);

        if(saveOneShot == false)
        {
            // save the scroll position to saved data
            GameSaver.gSaver.scrollViewPosition = scrollRect.horizontalNormalizedPosition;
            GameSaver.gSaver.SaveData();

            saveOneShot = true;
        }

        selectOneShot = true;
        playSound = true;
    }
}
