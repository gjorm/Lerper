using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour {

    public AudioClip acClickOn, acClickOff;

    private GameObject scrollView;
    private GameObject panel;
    private GameObject backGroundFade;
    private GameObject charSelectSub;
    private Button button;
    private Button playButton;
    private GameObject buyButton;
    private Image charSelected;
    private Sprite charSprite;
    private int charIndex;
    private bool scrollIsActive = false;
    private LerpGameController gScript;
    private PlayerController pScript;
    private AudioSource audioOut;
    private bool roundOver;
    private WatchAdsButton watchAdsBut;
    private BuyPanelScript bPanel;
    private Button backButton;
    private bool dontSoundOnStartUp;
    private bool buyPanelActive;
    private bool disableMenuButton;
    private Text currentName;
    private Text currentJump;
    private Text currentDistance;
    private Text currentLevel;
    private Text charCost;
    private Text charJump;
    private Text charDist;
    private bool closeMenuOneShot;
    private GameObject winPanel;
    private Text winTime;
    private Text winCoins;
    private Text mbText;
    private GameObject subWinPanel;
    private ScrollRect scrollRect;
    private float alpha;
    private GameObject charSelectHighLight;
    private FinalPlayPanel fpPanel;
    private DownArrowScript daScript;

    private void Awake()
    {
        playButton = GameObject.FindGameObjectWithTag("uiPlayButton").GetComponent<Button>();
        if (playButton == null)
            Debug.Log("Menu Button couldnt get a reference to the Play Button");

        fpPanel = GameObject.FindGameObjectWithTag("uiFinalPlayPanel").GetComponent<FinalPlayPanel>();
        if (fpPanel == null)
            Debug.Log("Menu button couldnt get a reference to the Final Play Panel Script.");

        daScript = GameObject.FindGameObjectWithTag("uiDownArrow").GetComponent<DownArrowScript>();
        if (daScript == null)
            Debug.Log("Menu button script couldnt get reference to the down arrow script.");

        charSelectSub = GameObject.FindGameObjectWithTag("uiCharSubPanel");
        if (charSelectSub == null)
            Debug.Log("Menu Button Script couldnt get a reference to the ui Char Select Sub Panel.");
    }

    // Use this for initialization
    void Start () {

        // charSelected may need to be first, as the below code will disable the parent game object
        charSelected = GameObject.FindGameObjectWithTag("uiCharSelected").GetComponent<Image>();
        if (charSelected == null)
            Debug.Log("Menu button script couldnt get reference to the ui element for Character Selected");

        

        backButton = GameObject.FindGameObjectWithTag("uiBackButton").GetComponent<Button>();
        if (backButton == null)
            Debug.Log("Menu Button couldnt get a reference to the Back Button");

        buyButton = GameObject.FindGameObjectWithTag("uiBuyButton");
        if (buyButton == null)
            Debug.Log("Menu Button couldnt get a reference to the Buy Button");

        Animator anim = buyButton.GetComponent<Animator>();
        if (anim == null)
            Debug.Log("Menu Button couldnt get a reference to the Buy Button Button component.");

        scrollView = GameObject.FindGameObjectWithTag("ScrollView");
        if (scrollView == null)
            Debug.Log("Menu Button could not get the scrollview reference.");
        else
        {
            scrollRect = scrollView.GetComponent<ScrollRect>();
            if (scrollRect != null)
                scrollRect.horizontalNormalizedPosition = GameSaver.gSaver.scrollViewPosition;
        }

        panel = GameObject.FindGameObjectWithTag("uiPanel");
        if (panel == null)
            Debug.Log("Menu Button could not get the panel reference.");

        backGroundFade = GameObject.FindGameObjectWithTag("uiFade");
        if (backGroundFade == null)
            Debug.Log("Menu Button could not get the backGroundFade reference.");

        button = GetComponent<Button>();
        if (button == null)
            Debug.Log("Menu Button script could not get a reference to the button sript.");
        else
        {
            button.onClick.AddListener(OpenMenu);
            mbText = button.gameObject.GetComponentInChildren<Text>();
            if (mbText == null)
                Debug.Log("Menu Button Text Could not be referenced");
        }

        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<LerpGameController>();
        if (gScript == null)
            Debug.Log("Menu Button script could not get a reference to the game controller.");

        pScript = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        if (pScript == null)
            Debug.Log("Menu Button script could not get a reference to the player controller.");

        audioOut = GetComponent<AudioSource>();
        if (audioOut == null)
            Debug.Log("Menu Button couldnt get its Audio Source component.");

        bPanel = GameObject.FindGameObjectWithTag("uiPanel").GetComponent<BuyPanelScript>();
        if (bPanel == null)
            Debug.Log("Menu Button Couldnt Get reference to the ui Buy Panel");

        winPanel = GameObject.FindGameObjectWithTag("uiWinPanel");
        if (winPanel == null)
            Debug.Log("Menu Button Couldnt Get reference to the win Panel");
        else
            winPanel.SetActive(true);

        subWinPanel = GameObject.FindGameObjectWithTag("uiSubWinPanel");
        if (subWinPanel == null)
            Debug.Log("OpenWInPanel in MenuButton coulnt get a reference to the Sub Win Panel");


        // currently selecter lerper text fields
        currentName = GameObject.FindGameObjectWithTag("uiCurrentName").GetComponent<Text>();
        if (currentName == null)
            Debug.Log("Menu Button couldnt get reference to the Current Name field");

        currentJump = GameObject.FindGameObjectWithTag("uiCurrentJump").GetComponent<Text>();
        if (currentJump == null)
            Debug.Log("Menu Button couldnt get reference to the Current Jump field");

        currentDistance = GameObject.FindGameObjectWithTag("uiCurrentDistance").GetComponent<Text>();
        if (currentDistance == null)
            Debug.Log("Menu Button couldnt get reference to the Current Distance field");

        currentLevel = GameObject.FindGameObjectWithTag("uiCurrentLevel").GetComponent<Text>();
        if (currentLevel == null)
            Debug.Log("Menu Button couldnt get reference to the Current Level field");

        // candidate purchase lerper text fields
        charCost = GameObject.FindGameObjectWithTag("uiCharCostField").GetComponent<Text>();
        if (charCost == null)
            Debug.Log("Character button script could not get a reference to the Character Cost Text component.");

        charJump = GameObject.FindGameObjectWithTag("uiJump").GetComponent<Text>();
        if (charJump == null)
            Debug.Log("Character button script could not get a reference to the Character Jump Text component.");

        charDist = GameObject.FindGameObjectWithTag("uiDistance").GetComponent<Text>();
        if (charDist == null)
            Debug.Log("Character button script could not get a reference to the Character Dist Text component.");
        /*
        charName = GameObject.FindGameObjectWithTag("uiCharName").GetComponent<Text>();
        if (charName == null)
            Debug.Log("Character button script could not get a reference to the Buy Panel Character Name Text component.");
            */

        // ui win panel text fields
        winTime = GameObject.FindGameObjectWithTag("uiTimeTaken").GetComponent<Text>();
        if (winTime == null)
            Debug.Log("Character button script could not get a reference to the Win Panel Time Taken component.");

        winCoins = GameObject.FindGameObjectWithTag("uiCoinsCollected").GetComponent<Text>();
        if (charDist == null)
            Debug.Log("Character button script could not get a reference to the Win Coins Collected text component.");

        charSelectHighLight = GameObject.FindGameObjectWithTag("uiCharHighLight");
        if (charSelectHighLight == null)
            Debug.Log("Menu Button Couldnt get reference to the Character buttons Char Select HighLight Panel");

        //fpPanel.CloseFinalPanel();

        disableMenuButton = false;
        dontSoundOnStartUp = true;
        buyPanelActive = true;
        closeMenuOneShot = true;
        CloseMenu();
        scrollIsActive = false;
        roundOver = false;
	}
	
	// Update is called once per frame
	void Update () {
        if(bPanel.IsPanelMovedOut() && buyPanelActive == false)
        {
            if(closeMenuOneShot == true)
            {
                scrollView.SetActive(false);
                buyButton.SetActive(false);

                charSelectHighLight.SetActive(false);
                panel.SetActive(false);
                scrollIsActive = false;
                playButton.enabled = false;

                closeMenuOneShot = false;
            }
        }

        if (roundOver == true)
            alpha = 0.0f;
        else
            alpha = 255f;

        Image img = GetComponent<Image>();
        Color cA = pScript.GetCharColorA();
        Color cB = pScript.GetCharColorB();
        Color foo = Color.Lerp(cB, cA, pScript.GetLerpVal());
        foo.a = alpha;
        img.color = foo;

        foo = mbText.color;
        foo.a = alpha;
        mbText.color = foo;

        if (Input.GetKeyDown(KeyCode.Escape) && roundOver == false)
            CloseMenu();
	}

    public void SetCharSelected(int index, Sprite img)
    {
        daScript.Pop();

        charSelected.enabled = true;
        charSprite = img;
        charSelected.sprite = charSprite;
        charIndex = index;

        currentName.text = "" + pScript.GetCharName(index);
        currentJump.text = "" + pScript.GetCharJumpTimes(index).ToString();
        currentDistance.text = "" + pScript.GetCharDistance(index).ToString();
        currentLevel.text = "" + pScript.GetCharLevel(index).ToString();
    }

    //when in doubt, cheat cheat cheat
    public int GetIndex()
    {
        return charIndex;
    }

    public Sprite GetCharSprite()
    {
        return charSprite;
    }

    public string GetCharName()
    {
        return pScript.GetCharName(charIndex);
    }

    public void RoundOverMenu() //calling from this function does the same as Open Menu, but disables the menu button until the player clicks new game button
    {

        //next make the button alpha channel faded
        // this doesnt appear to work
        Image img = GetComponent<Image>();
        Color foo = img.color;
        foo.a = 128f;
        img.color = foo;

        //win panel can be called like this
        winPanel.SetActive(true);
        WinPanel script = winPanel.GetComponent<WinPanel>();
        if (script == null)
            Debug.Log("OpenWinPanel in Menubutton couldnt get reference to the win panel script.");
        else
            script.SetMovePanelOut();

        DisableMenuButton(false);
        roundOver = true;
        OpenMenu();
    }


    public void OpenMenu()
    {
        if(scrollView != null && !gScript.IsRoundNewTime())
        {
            if(disableMenuButton == false)
            {
                if (scrollIsActive == false)
                {
                    gScript.PauseGame(true);

                    buyPanelActive = true;

                    panel.SetActive(true);

                    audioOut.PlayOneShot(acClickOn, 0.8f);
                    backGroundFade.SetActive(true);
                    
                    buyButton.SetActive(true);

                    BackButton blarg = backButton.GetComponentInParent<BackButton>();
                    if (roundOver == true)
                    {
                        backButton.enabled = false;
                        if (blarg != null)
                            blarg.SetAlpha(0f);
                    }
                    else
                    {
                        backButton.enabled = true;
                        if (blarg != null)
                            blarg.SetAlpha(255f);
                    }

                    // move the canvas in
                    bPanel.SetMovePanelIn();

                    watchAdsBut = GameObject.FindGameObjectWithTag("uiWatchAds").GetComponent<WatchAdsButton>();
                    if (watchAdsBut == null)
                        Debug.Log("Menu Button script couldnt get reference to the watch ads button.");
                    watchAdsBut.SetWiggle(false);

                    Button btn = buyButton.GetComponent<Button>();
                    if (btn == null)
                        Debug.Log("Menu Button couldnt get a reference to the Buy Button Button component.");
                    else
                        btn.enabled = false;

                    BuyButton bbtn = buyButton.GetComponent<BuyButton>();
                    if (btn == null)
                        Debug.Log("Menu Button couldnt get a reference to the Buy Button Button component.");
                    else
                        bbtn.SetWiggle(false);

                    

                    // get reference to the text that shows the coin bank and update
                    Text coinB = GameObject.FindGameObjectWithTag("uiTotalCoins").GetComponent<Text>();
                    if (coinB != null)
                    {
                        string str = "" + gScript.GetBigCoinBank().ToString();
                        coinB.text = str;
                    }

                    // update current lerper character fields
                    currentName.text = "" + pScript.GetCharName(charIndex);
                    currentJump.text = "" + pScript.GetCharJumpTimes(charIndex).ToString();
                    currentDistance.text = "" + pScript.GetCharDistance(charIndex).ToString();
                    currentLevel.text = "" + pScript.GetCharLevel(charIndex).ToString();

                    // close the winpanel
                    //winPanel.SetActive(false);

                    playButton.enabled = true;
                    scrollView.SetActive(true);
                    scrollIsActive = true;


                    GameSaver.gSaver.SaveData();
                    Debug.Log("bugger");
                }
                else
                {
                    if (roundOver == false)
                    {
                        CloseMenu();
                    }
                }
            }
        }
    }

    public void CloseMenu()
    {
        if(dontSoundOnStartUp == false && buyPanelActive == true)
        {
            audioOut.PlayOneShot(acClickOff, 0.8f);
        }
        else
        {
            dontSoundOnStartUp = false;
        }

        fpPanel.CloseFinalPanel();

        //win panel can be called like this
        WinPanel script = winPanel.GetComponent<WinPanel>();
        if (script == null)
            Debug.Log("OpenWinPanel in Menubutton couldnt get reference to the win panel script.");
        else
            script.SetMovePanelOut();

        // move the canvas out
        backGroundFade.SetActive(false);
        //winPanel.SetActive(false);
        bPanel.SetMovePanelOut();
        buyPanelActive = false;
        closeMenuOneShot = true;
        DisableMenuButton(false);

        gScript.PauseGame(false);
    }

    public int GetCharIndex()
    {
        return charIndex;
    }

    public void ClearCharFields()
    {
        if (charSelectSub.activeSelf == true)
            charSelectSub.SetActive(false);
    }

    public void ShowCharFields()
    {
        if (charSelectSub.activeSelf == false)
            charSelectSub.SetActive(true);
    }

    public void ResetRoundVar()
    {
        roundOver = false;
    }

    public void OpenWinPanel(bool win)
    {
        
        winPanel.SetActive(true);
        WinPanel script = winPanel.GetComponent<WinPanel>();
        if (script == null)
            Debug.Log("OpenWinPanel in Menubutton couldnt get reference to the win panel script.");
        else
            script.SetMovePanelIn();

        roundOver = true;
        DisableMenuButton(true);
        gScript.StopRoundTimer();

        if(win == true)
        {
            subWinPanel.SetActive(true);
            long total = gScript.GetTimeTaken();
            long minutes = total / 60;
            long seconds = total % 60;
            winTime.text = "You Earned Level " + pScript.GetCharLevel(charIndex).ToString() + " with " + pScript.GetCharName(charIndex) + "!";
            winCoins.text = "" + gScript.GetLevelCoinsCollected().ToString() + "/" + gScript.GetLevelCoins();
        }
        else
        {
            subWinPanel.SetActive(false);
        }
    }

    public void DisableMenuButton(bool b)
    {
        disableMenuButton = b;
    }

    public bool IsMenuOpen()
    {
        return scrollIsActive;
    }
}
