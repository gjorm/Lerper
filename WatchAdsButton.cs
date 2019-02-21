using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Advertisements;

public class WatchAdsButton : MonoBehaviour
{
#if UNITY_IOS
private string gameId = "1655986";
#elif UNITY_ANDROID
    private string gameId = "1655987";
#else
    private string gameId = "1655987";
#endif

    public string placementId = "rewardedVideo";

    public AudioClip acClickOn, acCoinsRecieved;

    private Button button;
    private MenuButton menuButton;
    private LerpGameController gScript;
    private PlayerController pScript;
    private AudioSource audioOut;
    private Animator anim;
    private bool wiggle = false;

    // Use this for initialization
    void Start()
    {
        menuButton = GameObject.FindGameObjectWithTag("MenuButton").GetComponent<MenuButton>();
        if (menuButton == null)
            Debug.Log("WatchAdsButton couldnt get a reference to the Menu Button");


        button = GetComponent<Button>();
        if (button == null)
            Debug.Log("Menu Button script could not get a reference to the button sript.");
        else
            button.onClick.AddListener(PlayAd);


        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<LerpGameController>();
        if (gScript == null)
            Debug.Log("WatchAds Button script could not get a reference to the game controller.");

        pScript = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        if (pScript == null)
            Debug.Log("WatchAds Button script could not get a reference to the player controller.");

        audioOut = GetComponent<AudioSource>();
        if (audioOut == null)
            Debug.Log("WatchAds Button couldnt get its Audio Source component.");

        anim = GetComponent<Animator>();
        if (anim == null)
            Debug.Log("WatchAds Button couldnt get reference to its animator component");

        if (Advertisement.isSupported)
        {
            Advertisement.Initialize(gameId, true);
        }

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
        }
        else
        {
            anim.Play("Normal");
        }

        if(button != null)
            button.interactable = Advertisement.IsReady(placementId);
    }

    public void PlayAd()
    {
        wiggle = false;

        ShowOptions options = new ShowOptions();
        options.resultCallback = HandleShowResult;

        Advertisement.Show(placementId, options);
    }

    void HandleShowResult(ShowResult result)
    {
        if (result == ShowResult.Finished)
        {
            pScript.CollectCoin(150);
            Text totCoins = GameObject.FindGameObjectWithTag("uiTotalCoins").GetComponent<Text>();
            if (totCoins != null)
                totCoins.text = "" + gScript.GetBigCoinBank();
            else
                Debug.Log("Couldnt update the Total Coins field after watching an ad.");
        }
        else if (result == ShowResult.Skipped)
        {
            Debug.LogWarning("Video was skipped - Do NOT reward the player");

        }
        else if (result == ShowResult.Failed)
        {
            Debug.LogError("Video failed to show");
        }
    }

    public void SetWiggle(bool b)
    {
        wiggle = b;
    }
}