using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using EasyMobile;
	
public class LerpGameController : MonoBehaviour {
    public LerpGameController gScript;

	public GameObject block_base;
	public GameObject player_prefab;
	public GameObject coin;
	public GameObject bigCoin;
	public GameObject hourGlass;
	public GameObject diamond;
	public GameObject checkPoint;
	public GameObject bomb;
	public GameObject dragon;
    public GameObject explosionEffect;
    public GameObject explosionEffect2;
    public GameObject trophy;
    public GameObject crystal;
    public AudioClip acExpl1;
    public AudioClip acExpl2;
    public AudioClip acHGPickup;
    public AudioClip acHGUse;
    public AudioClip acHGUseEnd;
    public AudioClip acDiamondPickup;
    public AudioClip acDiamondUse;
    public AudioClip acCPPickup;
    public AudioClip acCPUse;
    public AudioClip acDragonSpawn;
    public AudioClip acNoPowerUp;
    public AudioClip acNewRoundStart;
    public AudioClip acCrystalActivate;
    public string[] endQuotes;
    public string[] beginQuotes;
    public string[] winQuotes;

    public int numBlocksX;
	public int numBlocksY;
	public int lerpDivs;
	public int maxLerpTicks;
	public int lerpTickOffset;
	public int upperFadeIn;
	public int lowerFadeOut;
	public int maxBlinkTicks;
	public float maxFreezeTime = 10.0f;

    public int whichPlayerIndexActive = 0;
    [System.NonSerialized]
    public Stack<PowerUp> PowerUps;
    [System.NonSerialized]
    public Stack<Vector2Int> CheckPoints;

    private GameObject player;
    private GameObject pUpDisplay;
    private PowerUpController pUpDispScript;
    private Vector3 pUpDispLoc;
    private Dictionary<int, GameObject> MasterPowerUpList;
	private float[,] lerpMapA;
	private float[,] lerpMapB;
	private GameObject[,] blockMap;
	private float pSpawnX, pSpawnY;
    [System.NonSerialized]
	private int lerpTick;
	private bool lerpTickUp;
	private int lerpOffsetCounter;
	private int blinkTick;
	private Vector2Int pLand;
	private Vector2Int pLandStored;
	private Vector2Int mPoint;
	private int pUpCtr = 0;
    private bool pUpToggle;
	private Color colorA;
	private Color colorB;
    private Color fColor;
    [System.NonSerialized]
	private bool morphFreeze = false;
    [System.NonSerialized]
	private float morphFreezeTimer = 0.0f;
    private GameObject dragHolder;
	private long nextDragonTime;
    private bool dragonToggle;
    [System.NonSerialized]
    private int maxDragonTime = 15;
    [System.NonSerialized]
    private bool gamePaused;
    private Text uiRoundOver;
    private Text uiRoundNew;
    private Text uiRoundOverWin;
    private bool roundNew = false;
    private long roundNewUiTime = 0;
    private bool newGameOneShot = true;
    private bool roundWasWon = false;
    private int trophyX, trophyY;
    MenuButton menuB;
    private int dragonsAvoided;
    private int dragonsSpawned;
    private long roundTimeStart;
    private long timeTaken;
    private bool roundIsOver;
    private BuyPanelScript newRoundPanel;
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
    private bool touchOn;
    private bool touchOneShot;
#endif
    private GameObject uiNumCheckPoints;

    private int levelCoins;
    private int levelCoinsCollected;

    [System.NonSerialized]
    public int bigCoinBank;

	private Plane plane;

	private PlayerController pScript;
	private BlockController blockScript;
	private CameraController cScript;

    private AudioSource audioOut;

    private GameObject tutPanel;
    private WinPanel tutPanelScript;
    private bool needTutorial;
    private bool keepTutorialAway;
    private bool tutorialDone;

    private BuyPanelScript bPanel;

    [System.NonSerialized]
    private bool newCoinDay;
    [System.NonSerialized]
    private GameObject newCoinPanel;
    [System.NonSerialized]
    private bool newCoinDayStart;
    [System.NonSerialized]
    private int newCoins;
    [System.NonSerialized]
    private bool newCoinDayFinished;

    private long crystalTime;
    private bool crystalActive;
    private int crystalX;
    private float crystalVal;

    // Use this for initialization
    void Awake()
    {
        needTutorial = false;
        keepTutorialAway = false;
        tutorialDone = false;
        newCoinDay = false;
        newCoinDayStart = false;
        newCoinDayFinished = false;
        newCoins = 0;

        crystalActive = false;
        crystalTime = 0;
        crystalX = -1;

        //DontDestroyOnLoad(gameObject);
        gScript = this;

        //-10f, 0.0f, -14.75f
        pUpDispLoc.x = -10f;
        pUpDispLoc.y = 0.3f;
        pUpDispLoc.z = -14.75f;

        //tX = numBlocksX / 2;
        //tY = numBlocksY - 4;

        lerpTick = 0;
        lerpTickUp = true;
        lerpOffsetCounter = 0;
        blinkTick = 0;

        //oops dont crash Unity
        if (lerpDivs == 0)
        {
            lerpDivs = 4;
        }
        if (numBlocksX == 0 || numBlocksY == 0)
        {
            numBlocksX = 12;
            numBlocksY = 12;
        }
        if (maxLerpTicks == 0)
        {
            maxLerpTicks = 16;
        }
        if (lerpTickOffset == 0)
        {
            lerpTickOffset = 3;
        }
        if (upperFadeIn == 0)
        {
            upperFadeIn = 100;
        }
        if (lowerFadeOut == 0)
        {
            lowerFadeOut = 72;
        }
        if (maxBlinkTicks == 0)
        {
            maxBlinkTicks = 20;
        }
        if(maxFreezeTime <= 0)
        {
            maxFreezeTime = 5;
        }

        dragHolder = null;

        audioOut = GetComponent<AudioSource>();
        bigCoinBank = 0;

        // I think Unity automatically seeds the generator with entropy but sometimes it appears to not be the case. Will force a seed anyways
        System.DateTime epochStart = new System.DateTime(1986, 3, 25, 0, 0, 0, System.DateTimeKind.Utc);
        int curTime = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        Random.InitState(curTime);
        //Debug.Log (curTime);

        pUpToggle = true;
        roundIsOver = false;

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        touchOneShot = false;
        touchOn = false;
#endif

        // init the goddamn power up stack and check point stack, before player spawning
        PowerUps = new Stack<PowerUp>();
        PowerUps.Clear();
        CheckPoints = new Stack<Vector2Int>();
        CheckPoints.Clear();
        MasterPowerUpList = new Dictionary<int, GameObject>();
        MasterPowerUpList.Clear();

        //Instantiate player root object for manipulation, then Spawn player character model
        // next time im omitting this
        player = (GameObject)Instantiate(player_prefab, new Vector3(-2, 3, -2), Quaternion.identity); // this is the empty gameobject with a script for manipulating the player
        pScript = player.GetComponent<PlayerController>();

        // init camera
        cScript = Camera.main.GetComponent<CameraController>();
        if (cScript != null)
            cScript.InitCamera(player, numBlocksX, numBlocksY);
        else
            Debug.Log("Game controller couldnt get reference to camera");

        // init the plane for the GetMousePos. There are now 3 planes at any time....
        plane = new Plane(Vector3.up, new Vector3(0, 0.5f, 0));

        //get menu button reference
        menuB = GameObject.FindGameObjectWithTag("MenuButton").GetComponent<MenuButton>();
        if (menuB == null)
            Debug.Log("LerpGameController couldnt get reference to Menu button");

        tutPanel = GameObject.FindGameObjectWithTag("uiTutorialPanel");
        if (tutPanel == null)
            Debug.Log("Lerp Game Controller couldnt get reference to the Tutorial Panel Game Object");
        else
        {
            tutPanelScript = tutPanel.GetComponent<WinPanel>();
            if (tutPanelScript == null)
                Debug.Log("Lerp Game Controller couldnt get reference to the Tut Panel Win Panel Script. Does that make sense?");
        }


        // start the timer for the dragon
        maxDragonTime = 8;
        SetDragonTime();
        dragonToggle = true;

        // get reference to Round Over ui element
        uiRoundOver = GameObject.FindGameObjectWithTag("uiRoundOver").GetComponent<Text>();
        if (uiRoundOver == null)
            Debug.Log("Game controller couldnt get reference to the ui Round Over Text");
        else
            uiRoundOver.enabled = false;

        // get reference to Round Over Win ui element
        uiRoundOverWin = GameObject.FindGameObjectWithTag("uiRoundOverWin").GetComponent<Text>();
        if (uiRoundOverWin == null)
            Debug.Log("Game controller couldnt get reference to the ui Round Over Win Text");
        else
            uiRoundOverWin.enabled = false;

        // get reference to New Round ui element
        uiRoundNew = GameObject.FindGameObjectWithTag("uiNewRound").GetComponent<Text>();
        if (uiRoundNew == null)
            Debug.Log("Game controller couldnt get reference to the ui Round Over Text");
        else
            uiRoundOver.enabled = true;

        // get reference to New Round Panel
        newRoundPanel = GameObject.FindGameObjectWithTag("uiNewRoundPanel").GetComponent<BuyPanelScript>();
        if (newRoundPanel == null)
            Debug.Log("Game controller Couldnt Get reference to the ui New Round Panel");

        uiNumCheckPoints = GameObject.FindGameObjectWithTag("uiNumCheckPoints");
        if (uiNumCheckPoints == null)
            Debug.Log("GameController Couldnt get reference to the ui Num Check Points field");

        newCoinPanel = GameObject.FindGameObjectWithTag("uiNewCoinDayPanel");
        if (newCoinPanel == null)
            Debug.Log("Lerp Game controller couldnt get reference to its parent panel object.");


    }

    private void Start() {

        bPanel = GameObject.FindGameObjectWithTag("uiPanel").GetComponent<BuyPanelScript>();
        if (bPanel == null)
            Debug.Log("Lerp Game Controller Couldnt Get reference to the ui Buy Panel");

        // in order for saved game data (which is loaded in OnEnable in the game saver) to work, loading saved information needs to happen in start
        bigCoinBank = GameSaver.gSaver.numCoins;
        whichPlayerIndexActive = GameSaver.gSaver.charIndexActive;
        /*
        int sdf;
        for(int i = 0; i < pScript.GetNumModels(); i++)
        {
            sdf = GameSaver.gSaver.charLevel[i];
            pScript.SetCharLevel(i, sdf);
        }
        */

        newCoinDay = CheckIfNewCoinDay();

        if (GameSaver.gSaver.firstTimePlaying == true)
        {
            OpenTutorial();
        }
        else
        {
            tutPanel.SetActive(false);
        }

        if (newCoinDay)
        {
            newCoinPanel.SetActive(true);
            newCoins = 0;
            //newCoinDay = false;
        }
        else
        {
            newCoinPanel.SetActive(false);
        }

        NewRound(whichPlayerIndexActive);
    }




    void FixedUpdate () {
        // Debug Dump
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log(morphFreeze + " " + lerpTick + " " + maxLerpTicks + " " + lerpTickUp);
        }

        //check for new coin day
        if (newCoinDayStart)
        {
            if (newCoins % 3 == 0)
                pScript.CollectCoin(-1);

            newCoins++;

            if (newCoins >= 45)
            {
                newCoinDay = false;
                newCoinDayStart = false;
                newCoinDayFinished = true;
                GameSaver.gSaver.SaveData();
            }
        }

        IsRoundNewTime();

        // check if new tutorial is needed
        if (needTutorial == true)
        {
            needTutorial = false;
            PauseGame(true);
            tutPanel.SetActive(true);
            tutPanelScript.SetMovePanelIn();
        }
        

        if (keepTutorialAway == false && tutorialDone == true)
        {
            
            keepTutorialAway = true;
            //tutPanelScript.SetMovePanelOut(); //setmovepanelout() is already called by the ok button
        }

        if (keepTutorialAway == true && tutorialDone == true && tutPanelScript.IsPanelMovedOut())
        {
            tutPanel.SetActive(false);
        }

        //check for crystal timer to be done
        if(crystalActive == true)
        {
            System.DateTime epochStart = new System.DateTime(1986, 3, 25, 0, 0, 0, System.DateTimeKind.Utc);
            long curTime = (long)(System.DateTime.UtcNow - epochStart).TotalSeconds;

            if(curTime >= (crystalTime + 4))
            {
                DeactivateCrystal();
            }
        }

        // Morpth the Lerp Maps///////
        morphLerpMaps();


        // this section allows for the morphing between the lerpMaps. It is frozen whenever the player activates the hour glass power up
        if (morphFreeze)
        {
            if (morphFreezeTimer > maxFreezeTime)
            {
                // play end of freezing sound and stop everything
                audioOut.PlayOneShot(acHGUseEnd, 0.8f);
                UnFreezeMorphing();
                //also reset dragon time now that morphFreeze bool is part of conditional logic for spawning
                SetDragonTime();
            }
            morphFreezeTimer += Time.deltaTime;
        }
        else
        {

            updateLerpTick();

            updateBlinkTick();
        }
    }




	void Update() {
        //capture touch input oneshot logic
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        CaptureTap();
#endif

        //one shot for the New Round graphic at beginning of game start. Putting it in here because any where else it wont work or glitches
        if (newGameOneShot == true)
        {
            newGameOneShot = false;
            SetRoundNewTime();
        }


        if(gamePaused == false)
        {
            
            if (pScript.IsAlive())
            { // timescale is set to zero when the game is paused.
              // detect if player character landed on a block and start the block move shit
                if (pScript.HasLanded() == true)
                {
                    pLand = RoundPlayerPos(pScript.GetPlayerPosition());
                    pLandStored = pLand;
                    if (pLand.x >= 0 && pLand.x < numBlocksX && pLand.y >= 0 && pLand.y < numBlocksY)
                    {
                        blockScript = blockMap[pLand.x, pLand.y].GetComponent<BlockController>();
                        blockScript.PlayerLandMove(true, player);
                        //give the player the blockscript of the block that is landed on
                        pScript.GivePlayerBlockScript(blockScript); // this may be glitchy as hell
                    }
                }
                else
                {
                    pLand = RoundPlayerPos(pScript.GetPlayerPosition());
                    if (pLand.x >= 0 && pLand.x < numBlocksX && pLand.y >= 0 && pLand.y < numBlocksY)
                    {
                        blockScript = blockMap[pLandStored.x, pLandStored.y].GetComponent<BlockController>();
                        blockScript.PlayerLandMove(false, player);
                    }
                }



                // detect if player clicked on a block and start block move shit
                if (IsMouseOrTouchOn() && !pScript.HasReachedJumpClickLimit() && !IsRoundNewTime())
                {
                    mPoint = RoundPlayerPos(GetInputInWorld()); // RoundPlayerPos() works for what I need with the mouse click position
                    if (mPoint.x >= 0 && mPoint.x < numBlocksX && mPoint.y >= 0 && mPoint.y < numBlocksY)
                    {
                        blockScript = blockMap[mPoint.x, mPoint.y].GetComponent<BlockController>();
                        blockScript.ClickBlockMove();
                    }
                }

                // for now get input from keyboard, but later this will be from a tap or mouse click on a UI item
                if (Input.GetKeyDown(KeyCode.Space) || UserPressedPowerUp())
                {
                    if (PowerUps.Count > 0)
                    {
                        PowerUp foo = PowerUps.Peek();

                        switch (foo)
                        {
                            case PowerUp.HourGlass:
                                FreezeMorphing();
                                break;
                            case PowerUp.Diamond:
                                DiamondBlast();
                                break;
                            case PowerUp.Nothing:
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        audioOut.PlayOneShot(acNoPowerUp, 0.8f);
                    }
                }

                // detect if weird shit happened with character controller
                if (!pScript.WithinPlayField())
                {
                    pScript.KillPlayer(GetNumCheckPoints());
                }


                if(pScript.GetLevel() > 5)
                {
                    // check if dragon time is ready and need to spawn a new dragon
                    if (IsDragonTime())
                    { //IsDragonTime resets the timer
                        SpawnDragon();
                    }
                }
            }
            else
            {
                // allows me to respawn player on death. if checkpoints are present then it automatically will use that
                if (CheckPoints.Count > 0)
                { // || pScript.GetNumLives() >= 0) {
                  // need to implement a timer after death. timer should be in player script and give a bool when ready to be respawned
                  // this will allow enough time to clear out after killing player, so player can spawn again and not immediately die

                    if (pScript.IsReadyForSpawn()) // wait for dragon to die. maybe other timers etc, later
                    {
                        SpawnNewPlayerFromGC(whichPlayerIndexActive);
                    }
                }
            }
        }
	}

    // Late Update
    private void LateUpdate()
    {

        if (PowerUps.Count <= 0)
        {
            if(pUpDisplay != null)
                RemovePowerUpFromMasterList(pUpDisplay.GetInstanceID());

        } else
        {
            switch(PowerUps.Peek())
            {
                case PowerUp.Diamond:

                    if(pUpDisplay != null)
                    {
                        pUpDispScript = pUpDisplay.GetComponent<PowerUpController>();
                        if(pUpDispScript.PowerUpType != PowerUp.Diamond)
                        {
                            RemovePowerUpFromMasterList(pUpDisplay.GetInstanceID());
                            pUpDisplay = (GameObject)Instantiate(diamond, pUpDispLoc, Quaternion.identity);
                        }
                    }
                    else
                    {
                        pUpDisplay = (GameObject)Instantiate(diamond, pUpDispLoc, Quaternion.identity);
                    }
                    break;

                case PowerUp.HourGlass:
                    if (pUpDisplay != null)
                    {
                        pUpDispScript = pUpDisplay.GetComponent<PowerUpController>();
                        if (pUpDispScript.PowerUpType != PowerUp.HourGlass)
                        {
                            RemovePowerUpFromMasterList(pUpDisplay.GetInstanceID());
                            pUpDisplay = (GameObject)Instantiate(hourGlass, pUpDispLoc, Quaternion.identity);
                        }
                    }
                    else
                    {
                        pUpDisplay = (GameObject)Instantiate(hourGlass, pUpDispLoc, Quaternion.identity);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    //
    // Generate The Map Values
    //
    void generateMap() {

        //destroy any blocks at the current numBlocks values
        if(blockMap != null)
        {
            for (int x = 0; x < numBlocksX; x++)
            {
                for (int y = 0; y < numBlocksY; y++)
                {
                    if (blockMap[x, y] != null)
                    {
                        GameObject.Destroy(blockMap[x, y]);
                    }
                }
            }
        }

        //set the numBlocksY variable according to player level achieved
        numBlocksY = 60 + (pScript.GetCharLevel(whichPlayerIndexActive) * 20);
        trophyX = numBlocksX / 2;
        trophyY = numBlocksY - 4;

        InstantiateBlocks(); 

        // InstantiateBlockMap must be called only once and before generateMap
        if (blockMap [0, 0] == null) {
			Debug.Log ("InstantiateBlockMap() failed or was not called before generateMap()");
			return;
		}

		float area1, area2, area3, area4;
		float totArea = lerpDivs * lerpDivs;
		float heightA, heightB;
		int subx, suby;
        int cpCtr = 0;
        int lowCtrA = 0, lowCtrB = 0, hiCtrA = 0, hiCtrB = 0;
        float valA, valB;
        GameObject foo;


        //first jump along at the frequency (lerpDivs) given and generate a random value
        lerpMapA = new float[numBlocksX + 1, numBlocksY + 1]; // do a plus one to store a sample value, so the bilerp can use something for the far right side
		lerpMapB = new float[numBlocksX + 1, numBlocksY + 1];

		for (int y = 0; y <= numBlocksY; y += lerpDivs) {
			for (int x = 0; x <= numBlocksX; x += lerpDivs) {

                //then remaining calculations use floats
                valA = Random.Range(0, 8) * 32;
                valB = Random.Range(0, 8) * 32;

                /*
                if (valA > upperFadeIn)
                {
                    hiBalA--;
                    loBalA++;
                }
                if (valB > upperFadeIn)
                {
                    hiBalB--;
                    loBalB++;
                }
                if (valA < lowerFadeOut)
                {
                    hiBalA++;
                    loBalA--;
                }
                if (valB <lowerFadeOut)
                {
                    hiBalB++;
                    loBalB--;
                }
                */

                //to prevent large areas of negative or large areas of positive values, need to skew this way too
                // try to detect the highly repetitive low or high values. after two many of them, force a high or low value as needd
                if (valA <= lowerFadeOut)
                {
                    lowCtrA++;
                    hiCtrA = 0;
                }

                if (valB <= lowerFadeOut)
                {
                    lowCtrB++;
                    hiCtrB = 0;
                }

                if (valA >= upperFadeIn)
                {
                    hiCtrA++;
                    lowCtrA = 0;
                }

                if (valB >= upperFadeIn)
                {
                    hiCtrB++;
                    lowCtrB = 0;
                }

                //force the high value if too many lows accrued
                if (lowCtrA >= numBlocksX + (numBlocksX / 2))
                {
                    valA = 255f;
                    //valB = 128f;
                    //not resetting the counters here just yet. still want them to be reset naturally
                }
                if (lowCtrB >= numBlocksX + (numBlocksX / 2))
                {
                    valB = 255f;
                    //not resetting the counters here just yet. still want them to be reset naturally
                }
                //force the low value if too many high values accrued
                if (hiCtrA >= numBlocksX + (numBlocksX / 2))
                {
                    valA = 0f;
                    // not resetting the counters here just yet. still want them to be reset naturally
                }
                if (hiCtrB >= numBlocksX + (numBlocksX / 2))
                {
                    //valA = 0f;
                    valB = 0f;
                    // not resetting the counters here just yet. still want them to be reset naturally
                }
                

                lerpMapA[x, y] = valA;
                lerpMapB[x, y] = valB;

                


                // the y > lerpDivs prevents the map from spawning stuff near the start pad
                if (y >= lerpDivs * 4)
                {
                    // spawn the coins around the map with power ups interspersed in between
                    // since lerpMaps is +1 on both x and y axes (in reference to blockMap), the bounds must be checked before assigning anything with the blockMap
                    // this will find blocks that will eventually fade out, no matter what part of the cycle it is in
                    if (x < numBlocksX && y < numBlocksY)
                    {
                        //bombs spawn in places where there should always be a solid block
                        // now alternating between the two to ensure even nonrandom placement
                        if (lerpMapA[x, y] > upperFadeIn && lerpMapB[x, y] > upperFadeIn) // making y greater than 8 removes the chance of a bomb being placed by spawn
                        {
                            if (cpCtr <= 1)
                            {
                                if (cpCtr == 1)
                                {
                                    foo = (GameObject)Instantiate(bomb, new Vector3(x, 1.3f, y), Quaternion.identity);
                                    foo.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                                }
                                cpCtr++;
                            }
                            else
                            {
                                cpCtr = 0;
                            }
                        }

                        // big coins
                        if (lerpMapA[x, y] < lowerFadeOut && lerpMapB[x, y] < lowerFadeOut)
                        {
                            if (Random.Range(0, 32) == 1)
                            {
                                foo = (GameObject)Instantiate(bigCoin, new Vector3(x, 1.3f, y), Quaternion.identity);
                                //foo.transform.localScale.Scale(new Vector3(0.75f, 0.75f, 0.75f));
                            }
                        }

                        // regular coins and Power Ups
                        if ((lerpMapA[x, y] > upperFadeIn && lerpMapB[x, y] < lowerFadeOut) || (lerpMapB[x, y] > upperFadeIn && lerpMapA[x, y] < lowerFadeOut))
                        {
                            if (pUpCtr <= 12)
                            { //power up counter allows the beneficial power ups to be placed intermittently and at lower frequencies than the coins
                              //only spawning coins on even numbers to cut down on number of coins spawned
                                if (pUpCtr % 3 == 0)
                                {
                                    foo = (GameObject)Instantiate(coin, new Vector3(x, 1.3f, y), Quaternion.AngleAxis(-90, Vector3.right));
                                    foo.transform.localScale = new Vector3(0.68f, 0.68f, 0.68f);
                                }

                                pUpCtr++;
                            }
                            else
                            {

                                switch (pUpToggle)
                                { //the high range value is exclusive
                                    case true:
                                        foo = (GameObject)Instantiate(hourGlass, new Vector3(x, 1.3f, y), Quaternion.identity);
                                        foo.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

                                        pUpCtr = 0;
                                        pUpToggle = false;
                                        break;
                                    case false:
                                        foo = (GameObject)Instantiate(diamond, new Vector3(x, 1.3f, y), Quaternion.identity);
                                        foo.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

                                        pUpCtr = 0;
                                        pUpToggle = true;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
			}
		}

        int loc;
        //put in checkpoints at even locations
        for(int i = lerpDivs; i < numBlocksY; i += lerpDivs)
        {
            if(i % 40 == 0 && i != numBlocksY) // should end up having a checkpoint every 40 blocks
            {
                // find a random number between 0 and half of numblocks
                loc = Random.Range(0, (numBlocksX / 2));
                loc *= 2; // then multiply by two to ensure it is roughly square
                lerpMapA[loc, i] = 255f;
                lerpMapB[loc, i] = 255f;

                foo = (GameObject)Instantiate(checkPoint, new Vector3(loc, 1.3f, i), Quaternion.identity);
                foo.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            }
        }
        //put in crystals at even locations
        for (int i = 20; i < numBlocksY; i += (lerpDivs * 22)) // have to watch the 20 here. a lerpDivs = 2 seems like its hear to stay but it could change
        {
            // find a random number between 0 and half of numblocks
            loc = Random.Range(0, (numBlocksX / 2));
            loc *= 2; // then multiply by two to ensure it is roughly square
            lerpMapA[loc, i] = 255f;
            lerpMapB[loc, i] = 255f;

            foo = (GameObject)Instantiate(crystal, new Vector3(loc, 1.3f, i), Quaternion.identity);
        }

        // put in start pad
        // lerpDivs is set at 2 and its here to stay
        //lerpMapA[4, 0] = 255;
        //lerpMapB[4, 0] = 255;
        //lerpMapA[4, 2] = upperFadeIn;
        //lerpMapB[4, 2] = upperFadeIn;
        lerpMapA[6, 0] = 255;
        lerpMapB[6, 0] = 255;
        lerpMapA[6, 2] = 255;
        lerpMapB[6, 2] = 255;
        //lerpMapA[8, 0] = 255;
        //lerpMapB[8, 0] = 255;
        //lerpMapA[8, 2] = upperFadeIn;
        //lerpMapB[8, 2] = upperFadeIn;

        // force a good spawn point
        //lerpMapA [0, 0] = 255f;
		//lerpMapB [0, 0] = 255f;
		

		//then move along to each point (aka block) in the map and generate the value given using bilinear interpolation
		for (int x = 0; x < numBlocksX; x += lerpDivs) {
			for (int y = 0; y < numBlocksY; y += lerpDivs) {

				for (subx = 0; subx < lerpDivs; subx++) {
					for (suby = 0; suby < lerpDivs; suby++) {
						area1 = subx * suby;
						area2 = (lerpDivs - subx) * suby;
						area3 = subx * (lerpDivs - suby);
						area4 = (lerpDivs - subx) * (lerpDivs - suby);

						heightA = ((lerpMapA [x , y] * area4) + (lerpMapA [x + lerpDivs , y] * area3) + (lerpMapA [x , y + lerpDivs] * area2) + (lerpMapA [x + lerpDivs , y + lerpDivs] * area1)) / totArea;
						heightB = ((lerpMapB [x , y] * area4) + (lerpMapB [x + lerpDivs , y] * area3) + (lerpMapB [x , y + lerpDivs] * area2) + (lerpMapB [x + lerpDivs , y + lerpDivs] * area1)) / totArea;

						lerpMapA [x + subx , y + suby] = heightA;
						lerpMapB [x + subx , y + suby] = heightB;
					}
				}
			}
		}
    }

    public void PutInTrophy()
    {
        Instantiate(trophy, new Vector3(trophyX, 1.3f, trophyY), Quaternion.identity);

        SetTrophyArea(new Vector2Int(trophyX, trophyY));
    }


	public void InstantiateBlocks() {
		//instantiate the blockMap array
		blockMap = new GameObject[numBlocksX, numBlocksY];
        BlockController bScript;

		for (int x = 0; x < numBlocksX; x++) {
			for (int y = 0; y < numBlocksY; y++) {
                bScript = null;
				blockMap [x, y] = (GameObject)Instantiate (block_base, new Vector3(x, 0, y), Quaternion.identity);
                bScript = blockMap[x, y].GetComponent<BlockController>();
                if(bScript != null)
                {
                    bScript.SetFrozen(false);
                    bScript.SetTrophyArea(false);
                    bScript.SetColorA(colorA);
                    bScript.SetColorB(colorB);
                    bScript.SetFrozenAndTrophyColors(fColor);
                }
            }
		}
	}


	//
	// Linealy interpolate between the two datasets to produce one map that the renderer will use
	//
	void morphLerpMaps() {
		float slope, value, y1, y2;
		float lerpColor, maxLerpColor;
		float fade;
		int x, y;
		int isBlockFadeOut = 0;
        float preVal;

		// determine the number of blocks to consider when running the calculations (for performance(which may actually be negligible))
		Vector2 limits = cScript.GetViewBlockLimits ();

		float flowLimit = Mathf.Clamp (limits.x - 3.0f, 0, numBlocksY); // the hourglass powerup messes with this. if the player ventures up far enough, the blocks were never morphed and glitch out. May need to scrap this.
		float fupLimit = Mathf.Clamp (limits.y + 3.0f, 0, numBlocksY);

		int lowLimit = Mathf.RoundToInt (flowLimit); // x was set in GetViewBlockLimits as the bottom ray cast 2d y value
		int upLimit = Mathf.RoundToInt (fupLimit); // y was set as the upper raycast 2d y value

		// and run the morphing calcs
		for (x = 0; x < numBlocksX; x++) {
			for (y = lowLimit; y < upLimit; y++) {
				//compute our lerp values
				y1 = lerpMapA [x, y];
				y2 = lerpMapB [x, y];

				slope = ((y2 - y1) / (maxLerpTicks));

				value = ((slope * lerpTick) + y1);
                preVal = value;

				//determine if the block we have is fading in or out
				if(lerpTickUp == true) { //going from lerpMapA to lerpMapB
					if(lerpMapB[x, y] < lerpMapA[x, y]) {
						isBlockFadeOut = 1;
					}
					else {
						isBlockFadeOut = 0;
					}
				}
				else { //going  from lerpMapB to lerpMapA
					if(lerpMapA[x, y] < lerpMapB[x, y]) {
						isBlockFadeOut = 1;
					}
					else {
						isBlockFadeOut = 0;
					}
				}

				//get reference to the current block for setting the block fade
				blockScript = blockMap[x, y].GetComponent<BlockController> ();


				//blockScript.SetColorA (colorA);
				//blockScript.SetColorB (colorB);

				// maybe include the commented out section here as a nightmare mode?
				/*
				if (value > 128) {
					if (value > 192) { // 192 > 256
						blockMap [x, y].SetActive (false);
						blockScript.setBlockFade (0.0f); //setBlockFade accepts a value between 0.0f and 1.0f
					} else { // 128 > 192
						blockMap [x, y].SetActive (true);
						blockScript.setBlockFade (1.0f); //setBlockFade accepts a value between 0.0f and 1.0f 
						// make the block pop up for a neat effect
						if (isBlockFadeOut == 1) {
							blockScript.ClickBlockMove ();
						}
					}
				} else {
					if (value < 64) { // 0 >64
						blockMap [x, y].SetActive (true);
						blockScript.setBlockFade (1.0f); //setBlockFade accepts a value between 0.0f and 1.0f 
						// make the block pop up for a neat effect
						if (isBlockFadeOut == 1) {
							blockScript.ClickBlockMove ();
						}
					} else { // 64 > 128
						blockMap [x, y].SetActive (false);
						blockScript.setBlockFade (0.0f); //setBlockFade accepts a value between 0.0f and 1.0f
					}
				}
				*/

                //apply as solid if crystal is activated for particular x
                if(crystalActive == true && x == crystalX)
                {
                    value = Mathf.Clamp(crystalVal, preVal, 255);
                    blockScript.SetVerticalPath(true);
                }
                crystalVal -= (Time.deltaTime * 0.225f);

                if(!crystalActive)
                {
                    blockScript.SetVerticalPath(false);
                }


				//adjust fade per block
				if(value > upperFadeIn) {
					blockMap [x, y].SetActive (true);
					blockScript.setBlockFade (1.0f); //setBlockFade accepts a value between 0.0f and 1.0f
				}
				else {
					if(value < lowerFadeOut) {
						blockMap [x, y].SetActive (false);
						blockScript.setBlockFade (0.0f); //setBlockFade accepts a value between 0.0f and 1.0f
					}
					else { //then we are somewhere in between and need to fade the block
						blockMap [x, y].SetActive (true);

						if(blinkTick < (maxBlinkTicks / 2) && lerpOffsetCounter == 0 && isBlockFadeOut == 1) { //only blink during the transitioning/morphing phase and only for the blocks that are fading out
                            fade = (value - lowerFadeOut) / (upperFadeIn - lowerFadeOut);// / 3; // this was set to blink to 0.0f, however having it just go to a third of its value is easier on eyes
						}
						else {
							fade = (value - lowerFadeOut) / (upperFadeIn - lowerFadeOut);
							//fade = 1.0f;
						}
						blockScript.setBlockFade (fade); //setBlockFade accepts a value between 0.0f and 1.0f

						// make the block rumble for a neat effect
						if (isBlockFadeOut == 1 && !morphFreeze) {
							//blockScript.ClickBlockMove ();
						}
					}
				}


				//adjust the color of the block (set to lerp with lerpTick
				lerpColor = lerpTick;
				maxLerpColor = maxLerpTicks;
				blockScript.setBlockLerp (lerpColor / maxLerpColor);
                pScript.SetLerpVal(lerpColor / maxLerpColor);
			}
		}
	}

    public void NewRound(int charIndex)
    {

        cScript.StopMusic();

        uiRoundOver.enabled = false;
        //kill character, if still alive
        if(pScript.IsAlive())
        {
            pScript.KillPlayer(-5); // total hack. -5 indicates to KillPlayer() that it is being called from this function
        }

        // destroy all coins and power ups including power up avatar
        RemoveAllPowerUpsFromMasterList();

        whichPlayerIndexActive = charIndex;
        //save the player active index
        GameSaver.gSaver.charIndexActive = whichPlayerIndexActive;

        roundWasWon = false;

        // section for finding the dragon and killing it
        
        DragonController drag;

        if (dragHolder != null)
        {
            drag = dragHolder.GetComponent<DragonController>();
            if (drag != null)
                drag.KillDragon();
        }

        // check if more than one dragon spawned despite attempts to only spawn one dragon
        GameObject[] d = GameObject.FindGameObjectsWithTag("Dragon");
        if (d != null)
        {
            foreach(GameObject blegh in d)
            {
                drag = blegh.GetComponent<DragonController>();
                if (drag != null)
                {
                    drag.KillDragon();
                }
            }
        }
        dragHolder = null;
        pScript.SetDragonIsDead(true);

        //reset all power ups and checkpoints
        CheckPoints.Clear();
        PowerUps.Clear();
        UpdateNumPowerUpsField();

        levelCoinsCollected = 0;
        levelCoins = 0;

        // reset lerpticks
        UnFreezeMorphing();
        morphFreeze = false;
        crystalActive = false;
        lerpTick = 0;
        maxLerpTicks = 280;
        lerpTickOffset = 35;
        lerpTickUp = false;

        //reset menu button
        menuB.enabled = true;

        SetNumCheckPointsUI();

        //spawn new player
        //pScript.ResetNumLives();
        SpawnNewPlayerFromGC(charIndex);

        // run generateMap() here. Needs to be called before ResetAllBlockStates and PutInTophy. GenerateMap Modifies numBlocksY
        generateMap();

        //ResetAllBlockStates(); // applies new character color information. must be called after spawn new player
        PutInTrophy();

        //must re init camera after new map generation
        if(cScript != null)
        {
            cScript.InitCamera(player, numBlocksX, numBlocksY);
        }

        SetCoinCount();

        timeTaken = 0;
        dragonsAvoided = 0;
        dragonsSpawned = 0;

        SetRoundTimer();

        PauseGame(false);

        // reset dragontimer
        maxDragonTime = 8;
        dragonToggle = true;
        SetDragonTime();

        // reset ui Texts
        roundNew = false;
        SetRoundNewTime();
    }

    //unneccesary now
    public void ResetAllBlockStates()
    {
        BlockController bScript;
        for(int i = 0; i < numBlocksX; i++)
        {
            for(int j = 0; j < numBlocksY; j++)
            {
                bScript = blockMap[i, j].GetComponent<BlockController>();
                if(bScript != null)
                {
                    bScript.SetFrozen(false);
                    bScript.SetTrophyArea(false);
                    bScript.SetColorA(colorA);
                    bScript.SetColorB(colorB);
                    bScript.SetFrozenAndTrophyColors(fColor);
                }
            }
        }
    }

	//
	// Footwork with the ticking mechanism for morphLerpMaps()

	void updateLerpTick () {

		if(IsGamePaused() == false)
        {
            //Reset Down
            if (lerpTick >= maxLerpTicks && lerpTickUp == true)
            { //if the tick has arrived at its upper value while counting up
                if (lerpOffsetCounter < lerpTickOffset)
                { //then allow for a little over hang so that the morphing "pauses" at the peak of each cycle
                    lerpOffsetCounter++;
                }
                else
                {
                    lerpTickUp = false;
                    lerpOffsetCounter = 0;
                }
            }
            //Reset Up
            if (lerpTick <= 0 && lerpTickUp == false)
            {
                if (lerpOffsetCounter < lerpTickOffset)
                { //then allow for a little over hang so that the morphing "pauses" at the peak of each cycle
                    lerpOffsetCounter++;
                }
                else
                {
                    lerpTickUp = true;
                    lerpOffsetCounter = 0;
                }
            }

            //Accumulate Up
            if (lerpTickUp == true && lerpOffsetCounter == 0)
            { // dont accumulate while the offsetcounter is doing its thing to allow for overhang
                lerpTick++;
            }
            //Accumulate Down
            if (lerpTickUp == false && lerpOffsetCounter == 0)
            { // dont accumulate while the offsetcounter is doing its thing to allow for overhang
                lerpTick--;
            }
        }
	}

	public int getLerpTicks() {
		return lerpTick;
	}

	public void updateBlinkTick () {
		if(blinkTick < maxBlinkTicks) {
			blinkTick++;
		}
		if(blinkTick == maxBlinkTicks) {
			blinkTick = 0;
		}
	}

	public Vector2Int RoundPlayerPos(Vector2 plrPos) {
        float subx = Mathf.Clamp(plrPos.x, 0, numBlocksX);
        float suby = Mathf.Clamp(plrPos.y, 0, numBlocksY);

        int x = Mathf.RoundToInt (subx);
		int y = Mathf.RoundToInt (suby);
		Vector2Int val = new Vector2Int (x, y);
		return val;
	}

	public Vector2Int GetNumBlocks() {
		return new Vector2Int (numBlocksX, numBlocksY);
	}

	public void SpawnNewPlayerFromGC(int index) {
		if (pScript != null) {

            pSpawnX = ((float)numBlocksX / 2) - 0.5f;
            pSpawnY = 1.5f;

            SetCoinCount();

            SetDragonTime(); // reset the time so that if the player is dead long enough to elapse the current dragon time, the dragon wont spawn when player spawns

            // now that there is no respawning without checkpoints, px and py and now be floats and accurately place the player in the center on new rounds
            float pX = pSpawnX, pY = pSpawnY; //load up pX and pY with default spawn values, if there are no checkpoints

			//if spawned from a check point, kill that checkpoint
			if (CheckPoints.Count > 0) {
				//Debug.Log ("Stack size: " + CheckPoints.Count);
				Vector2Int spawn = CheckPoints.Peek ();
				pX = spawn.x;
				pY = spawn.y;

                // ensure a landing spot in case a bomb or dragon took out the check point
                if (lerpMapA[spawn.x, spawn.y] <= lowerFadeOut || lerpMapB[spawn.x, spawn.y] <= lowerFadeOut)
                {
                    lerpMapA[spawn.x, spawn.y] = 255;
                    lerpMapB[spawn.x, spawn.y] = 255;
                }

                //very innefficient but oh damn well
                RemovePowerUpFromMasterList(FindPowerUpIDByMapLocation(spawn));

                audioOut.PlayOneShot(acCPUse, 0.8f);

                CheckPoints.Pop ();
            }
            else
            {
                audioOut.PlayOneShot(acNewRoundStart, 0.9f);
            }

            

            pScript.SpawnPlayer (index, pX, pY);
			colorA = pScript.GetCharColorA (); // in order for the color information for the blocks to work, the player must be spawned first
			colorB = pScript.GetCharColorB ();
            fColor = pScript.GetFrozenAndTrophyColors();
		} else {
			Debug.Log ("Called on SpawnNewPlayerFromGC with null script pointer.");
		}

        SetNumCheckPointsUI();
    }

	public void SetPowerUpType(PowerUp pUpType) {
		PowerUps.Push (pUpType);
        UpdateNumPowerUpsField();

        // switch to correct sound effect
        switch (pUpType)
        {
            case PowerUp.HourGlass:
                audioOut.PlayOneShot(acHGPickup, 0.8f);
                break;
            case PowerUp.Diamond:
                audioOut.PlayOneShot(acDiamondPickup, 0.8f);
                break;
            case PowerUp.Nothing:
                break;
            default:
                break;
        }
    }

	public void FreezeMorphing() {
		morphFreeze = true;
		morphFreezeTimer = 0.0f;
        // play audio source
        audioOut.PlayOneShot(acHGUse, 0.7f);
        //reset when done to use once
        PowerUps.Pop();
        UpdateNumPowerUpsField();
    }

    public void UnFreezeMorphing()
    {
        morphFreeze = false;
        morphFreezeTimer = 0.0f;
    }

	public void DiamondBlast() {
		BlockController bScript;
		Vector2Int pos = RoundPlayerPos (pScript.GetPlayerPosition());
		int x = pos.x;
		int ctr = 0;
		int xdiff = 0;

        // audio 
        audioOut.PlayOneShot(acDiamondUse, 1f);

        // make a diamond shape around player
        for (int y = pos.y + 3; y >= pos.y - 3; y--) {
			for (int i = (x - xdiff); i <= (x + xdiff); i++) {
				if (i >= 0 && i < numBlocksX && y >= 0 && y < numBlocksY) {
					lerpMapA [i, y] = 255f;
					lerpMapB [i, y] = 255f;
					bScript = blockMap [i, y].GetComponent<BlockController> ();
					bScript.SetFrozen (true);
				}
			}
			if (ctr < 3) {
				xdiff += 1;
				ctr++;
			} else {
				xdiff -= 1;
				ctr++;
			}
		}

		//reset when done to use once
		PowerUps.Pop();
        UpdateNumPowerUpsField();
    }

    public void SetTrophyArea(Vector2Int pos)
    {
        BlockController bScript;
        int x = pos.x;
        int ctr = 0;
        int xdiff = 0;


        // make a diamond shape around the position
        for (int y = pos.y + 3; y >= pos.y - 3; y--)
        {
            for (int i = (x - xdiff); i <= (x + xdiff); i++)
            {
                if (i >= 0 && i < numBlocksX && y >= 0 && y < numBlocksY)
                {
                    lerpMapA[i, y] = 255f;
                    lerpMapB[i, y] = 255f;
                    bScript = blockMap[i, y].GetComponent<BlockController>();
                    bScript.SetTrophyArea(true);
                }
            }
            if (ctr < 3)
            {
                xdiff += 1;
                ctr++;
            }
            else
            {
                xdiff -= 1;
                ctr++;
            }
        }
    }

    public void BombExplosion(Vector2 bLoc)
    {
        Vector2Int pos = RoundPlayerPos(bLoc);

        // actual audio and visual explosion
        Instantiate(explosionEffect2, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
        audioOut.PlayOneShot(acExpl2, 1f);

        int x = pos.x;
        int ctr = 0;
        int xdiff = 0;

        // make a diamond shape around the bomb
        for (int y = pos.y + 3; y >= pos.y - 3; y--)
        {
            for (int i = (x - xdiff); i <= (x + xdiff); i++)
            {
                if (i >= 0 && i < numBlocksX && y >= 0 && y < numBlocksY)
                {
                    lerpMapA[i, y] = 0f;
                    lerpMapB[i, y] = 0f;
                }
            }
            if (ctr < 3)
            {
                xdiff += 1;
                ctr++;
            }
            else
            {
                xdiff -= 1;
                ctr++;
            }
        }
    }

	public void SetCheckPoint(Vector2 pos) {
		Vector2Int cp = RoundPlayerPos (pos);
		if (CheckPoints.Count > 0) {

			// since the physics engine may trigger the onCollider method more than once for what appears to be a single contact, it will attempt to push more than one of the same checkpoint
			// onto the stack. need to check for this and only allow for one checkpoint to be pushed
			if (!CheckPoints.Contains(cp)) {
				CheckPoints.Push (cp);
                audioOut.PlayOneShot(acCPPickup, 0.8f);
            }

		} else {
			CheckPoints.Push (cp);
            audioOut.PlayOneShot(acCPPickup, 0.8f);
        }

        SetNumCheckPointsUI();
    }

	public void DisableBlock(Vector2 pos) {

		pos.x = Mathf.Clamp (pos.x, 0, numBlocksX);
		pos.y = Mathf.Clamp (pos.y, 0, numBlocksY);

		Vector2Int foo = RoundPlayerPos (pos);

		lerpMapA [foo.x, foo.y] = 0.0f;
		lerpMapB [foo.x, foo.y] = 0.0f;

        // actual visual and audio effect
        Instantiate(explosionEffect, new Vector3(foo.x, 0, foo.y), Quaternion.identity);
        audioOut.PlayOneShot(acExpl1, Random.Range(0.3f, 0.6f));
    }

    public void MakeDragonSpawnFaster()
    {
        //dragon time is now decreased everytime the dragon is spawned and the player level is 9 or higher
        maxDragonTime = Mathf.Clamp(maxDragonTime / 2, 1, 8);
    }

    public void SetDragonTime() {
		System.DateTime epochStart = new System.DateTime(1986, 3, 25, 0, 0, 0, System.DateTimeKind.Utc);
		long curTime = (long)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        curTime = curTime + maxDragonTime;
		nextDragonTime = curTime;
    }

	public bool IsDragonTime() {
		bool result = false;
		System.DateTime epochStart = new System.DateTime(1986, 3, 25, 0, 0, 0, System.DateTimeKind.Utc);
		long curTime = (long)(System.DateTime.UtcNow - epochStart).TotalSeconds;

		if (curTime > nextDragonTime && pScript.GetLevel() > 5 && pScript.IsDragonDead()) {
			result = true;

            // since SetDragonTime() is now reset when the dragon dies, setting nextDragonTime to a spot way in the future should ensure that the dragon reset its on death
            nextDragonTime = curTime + 8;
        }

        return result;
	}

    public void SetRoundOverTime(bool roundWon)
    {
        if(roundWon == true)
        {
            PauseGame(true);
            uiRoundOver.enabled = false;
            uiRoundOverWin.enabled = true; //  not that it matters much, but want to make sure I can set the text
            uiRoundOverWin.text = winQuotes[Random.Range(0, winQuotes.Length)];

            // increment the level
            int lvl = pScript.GetCharLevel(whichPlayerIndexActive) + 1;
            pScript.SetLevel(lvl);
            GameSaver.gSaver.charLevel[whichPlayerIndexActive] = lvl;
            GameSaver.gSaver.numCoins = bigCoinBank;

            cScript.SetWinMusic();

            menuB.OpenWinPanel(true);
        }
        else
        {
            PauseGame(true);
            menuB.OpenWinPanel(false);

            cScript.SetLoseMusic();

            uiRoundOver.enabled = true;
            uiRoundOver.text = endQuotes[Random.Range(0, endQuotes.Length)];
        }

        roundWasWon = roundWon;
        roundIsOver = true;
    }

    public void SetRoundNewTime()
    {
        if (roundNew == false)
        {
            System.DateTime epochStart = new System.DateTime(1986, 3, 25, 0, 0, 0, System.DateTimeKind.Utc);
            long curTime = (long)(System.DateTime.UtcNow - epochStart).TotalSeconds;
            curTime += 2;
            roundNewUiTime = curTime;
            roundNew = true;

            uiRoundNew.text = "Level " + pScript.GetLevel().ToString() + "!";
            newRoundPanel.SetMovePanelIn();
            roundIsOver = false;

            GameSaver.gSaver.numCoins = bigCoinBank;
            GameSaver.gSaver.SaveData();
        }
    }

    public bool IsRoundNewTime()
    {
        bool result = false;
        if (roundNew == true)
        {
            System.DateTime epochStart = new System.DateTime(1986, 3, 25, 0, 0, 0, System.DateTimeKind.Utc);
            long curTime = (long)(System.DateTime.UtcNow - epochStart).TotalSeconds;
            result = true;

            if (curTime > roundNewUiTime)
            {
                result = false;
                roundNew = false;

                SetDragonTime();

                newRoundPanel.SetMovePanelOut();
            }
        }

        return result;
    }

    

    // this is called when the player dies. big Coin bank is permanent and is used for buying new characters
    public void AddToBigCoinBank(int coins)
    {
        bigCoinBank += coins;
        levelCoinsCollected++;
        GameSaver.gSaver.numCoins = bigCoinBank;
        SetCoinCount();
    }

    public int GetBigCoinBank()
    {
        return bigCoinBank;
    }

    public int GetNumCheckPoints()
    {
        return CheckPoints.Count;
    }

    public void SetBigCoinBank(int num)
    {
        bigCoinBank = num;
        GameSaver.gSaver.numCoins = num;
        SetCoinCount();
    }

    public bool GetDragonToggle()
    {
        return dragonToggle;
    }

    public void SetDragonToggle(bool b)
    {
        dragonToggle = b;
    }

    public bool IsRoundOver()
    {
        return roundIsOver;
    }

    public int GetPlayerIndex()
    {
        return whichPlayerIndexActive;
    }

    public int GetNumChars()
    {
        return pScript.GetNumModels();
    }

    private void UpdateNumPowerUpsField()
    {
        Text pUps = GameObject.FindGameObjectWithTag("uiNumPowerUps").GetComponent<Text>();
        if (pUps == null)
            Debug.Log("UpdateNumPowerUpsField in Game Controller couldnt get reference to the ui num Power ups field");

        string field = "";

        if(PowerUps.Count < 2)
        {
            pUps.text = field;
        } else
        {
            int num = PowerUps.Count - 1;
            field = "+" + num.ToString();
            pUps.text = field;
        }
    }

    public void IncreaseGameSpeed()
    {
        int mod = pScript.GetLevel();
        // decrease and clamp lerptick
        int num = (maxLerpTicks - 5);
        maxLerpTicks = Mathf.Clamp(num, (165 - (mod * 3)), 280);

        num = lerpTickOffset - 1;
        lerpTickOffset = Mathf.Clamp(num, 20, 35);
    }

    public void SetCoinCount()
    {
        // finding the fucking text field for every call only needs to happen with the coinfield for some goddamn fucking reason fuck you
        GameObject uiCoin = GameObject.FindGameObjectWithTag("UICoin");
        Text coinCount = uiCoin.GetComponent<Text>();
        if (coinCount != null)
        {
            coinCount.text = "" + bigCoinBank.ToString();
        }
    }

    public void AddPowerUpToMasterList(GameObject go)
    {
        int instId = go.GetInstanceID();
        bool fail = false;

        if (MasterPowerUpList.ContainsKey(instId))
            fail = true;

        if(go != null && fail == false)
            MasterPowerUpList.Add(instId, go);
    }

    public int FindPowerUpIDByMapLocation(Vector2Int loc)
    {
        GameObject go = null;
        int result = 0;
        Vector2Int test;

        foreach(int key in MasterPowerUpList.Keys)
        {
            go = null;
            if(MasterPowerUpList.TryGetValue(key, out go))
            {
                if(go != null)
                {
                    test = RoundPlayerPos(new Vector2(go.transform.position.x, go.transform.position.z));
                    if (loc.x == test.x && loc.y == test.y)
                        result = key;
                }
            }
            
        }

        return result;
    }

    public GameObject FindPowerUpByInstId(int instId)
    {
        GameObject result = null;

        if (MasterPowerUpList.ContainsKey(instId))
            MasterPowerUpList.TryGetValue(instId, out result);

        return result;
    }

    public void RemovePowerUpFromMasterList(int instId)
    {
        GameObject go = null;
        bool succ = false;

        if (MasterPowerUpList.ContainsKey(instId))
            succ = MasterPowerUpList.TryGetValue(instId, out go);

        if (succ == true)
        {
            if(go != null)
            {
                CoinController coinC = go.GetComponent<CoinController>();
                PowerUpController pUpC = go.GetComponent<PowerUpController>();

                if (coinC != null)
                {
                    coinC.KillThisCoin();
                }

                if (pUpC != null)
                {
                    pUpC.KillThisPowerUp();
                }

                MasterPowerUpList.Remove(instId);
            }

            GameObject.Destroy(go);
        }
    }

    public void RemoveAllPowerUpsFromMasterList()
    {
        foreach(GameObject go in MasterPowerUpList.Values)
        {
            if(go != null)
            {
                CoinController coinC = go.GetComponent<CoinController>();
                PowerUpController pUpC = go.GetComponent<PowerUpController>();

                if (coinC != null)
                {
                    coinC.KillThisCoin();
                }

                if (pUpC != null)
                {
                    pUpC.KillThisPowerUp();
                }
            }

            GameObject.Destroy(go);
        }

        MasterPowerUpList.Clear();
    }

    public int GetLevelCoins()
    {
        return levelCoins;
    }

    public void RegisterCoin(int value)
    {
        levelCoins += value;
    }

    public void SubtractLevelCoins(int value)
    {
        levelCoins -= value;
    }

    public int GetLevelCoinsCollected()
    {
        return levelCoinsCollected;
    }

    public long GetTimeTaken()
    {
        return timeTaken;
    }

    public int GetNumDragonsAvoided()
    {
        return dragonsSpawned - dragonsAvoided; // done this way so that it still counts even if the round was won before dragon could die
    }

    public void IncrementDragonAvoided()
    {
        dragonsAvoided++;
    }

    public int GetNumDragonsSpawned()
    {
        return dragonsSpawned;
    }

    public void IncrementDragonsSpawned()
    {
        dragonsSpawned++;
    }

    public void SetRoundTimer()
    {
        System.DateTime epochStart = new System.DateTime(1986, 3, 25, 0, 0, 0, System.DateTimeKind.Utc);
        roundTimeStart = (long)(System.DateTime.UtcNow - epochStart).TotalSeconds;
    }

    public void StopRoundTimer()
    {
        System.DateTime epochStart = new System.DateTime(1986, 3, 25, 0, 0, 0, System.DateTimeKind.Utc);
        long curTime = (long)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        timeTaken = curTime - roundTimeStart;
    }

    public bool GetRoundWon()
    {
        return roundWasWon;
    }

    // new input methods including mobile
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
    public void CaptureTap() {
        if(Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            touchOn = false;

            if (touchOneShot == false)
            {
                if (t.phase == TouchPhase.Began)
                {
                    touchOn = true;
                    touchOneShot = true;
                }
            }

            if (touchOneShot == true)
            {
                if (t.phase == TouchPhase.Ended)
                {
                    touchOneShot = false;
                }
            }
        }
    }

#endif
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
    
    public bool IsTouchOn()
    {
        return touchOn;
    }
#else
    public bool IsTouchOn()
    {
        return false;
    }
#endif

#if UNITY_STANDALONE || UNITY_WEBPLAYER
    public bool IsMouseOn()
    {

        return Input.GetMouseButtonDown(0);

    }
#else
    public bool IsMouseOn()
    {

        return false;

    }
#endif

    public bool IsMouseOrTouchOn()
    {
        return ((IsTouchOn() || IsMouseOn()) && !IsGamePaused() && (!newCoinDay || newCoinDayFinished));
    }

    public Vector2 GetTouchPosition()
    {
        Touch t = Input.GetTouch(0);

        return t.position;
    }

    public bool UserPressedPowerUp()
    {
        bool result = false;
        float maxX = Screen.width * 0.15f;
        float maxY = Screen.height * 0.58f;
        float minY = Screen.height * 0.35f;

        if(IsMouseOrTouchOn())
        {
            Vector2 pos = GetInputInScreen();
            if (pos.x < maxX && pos.y > minY && pos.y < maxY)
                result = true;
        }


        return result;
    }

    public Vector2 GetInputInScreen()
    {
        Vector2 result = new Vector2(0, 0);

        if(IsMouseOn())
        {
            result = Input.mousePosition;
        }

        if(IsTouchOn())
        {
            Touch t = Input.GetTouch(0);
            result = t.position;
        }

        return result;
    }

    public Vector2 GetInputInWorld()
    {
        Vector2 result = new Vector2(0,0);

        if(IsMouseOn())
        {
            float rayDist;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            plane.Raycast(ray, out rayDist);
            Vector3 lookDir = ray.GetPoint(rayDist);
            result.x = lookDir.x;
            result.y = lookDir.z;
        }

        if(IsTouchOn())
        {
            Touch t = Input.GetTouch(0);
            float rayDist;
            Ray ray = Camera.main.ScreenPointToRay(t.position);
            plane.Raycast(ray, out rayDist);
            Vector3 lookDir = ray.GetPoint(rayDist);
            result.x = lookDir.x;
            result.y = lookDir.z;
        }

        return result;
    }

    public void SetNumCheckPointsUI()
    {
        Text foo = uiNumCheckPoints.GetComponent<Text>();
        if (foo != null)
            foo.text = "" + CheckPoints.Count.ToString();
    }

    public void SetTutorialDone()
    {
        tutorialDone = true;
    }

    public void PauseGame(bool p)
    {

        if (p == true)
        {
            //audioOut.Pause();
            //Time.timeScale = 0;

        }
        else
        {
            //Time.timeScale = 1;
            audioOut.Play();
            if (p == false && gamePaused == true)
                SetDragonTime();
        }
        gamePaused = p;
    }

    public bool IsGamePaused()
    {
        return gamePaused;
    }

    public void SetNewCoinDayCollect()
    {
        newCoinDayStart = true;
        newCoinDay = false;
        newCoins = 0;
    }

    public bool CheckIfNewCoinDay()
    {
        bool result = false;

        System.DateTime today = System.DateTime.Today;
        System.DateTime lastTime = GameSaver.gSaver.lastCoinDay.Date;

        int check = today.Date.CompareTo(lastTime.Date);

        result = check != 0;// || GameSaver.gSaver.fileReadSuccess == false;

        return result;
    }

    public void ResetDragHolder()
    {
        dragHolder = null;
    }

    public void OpenTutorial()
    {
        needTutorial = true;
        tutorialDone = false;
    }

    public void ActivateCrystal(float xSpot)
    {
        audioOut.PlayOneShot(acCrystalActivate, 0.9f);
        System.DateTime epochStart = new System.DateTime(1986, 3, 25, 0, 0, 0, System.DateTimeKind.Utc);
        crystalTime = (long)(System.DateTime.UtcNow - epochStart).TotalSeconds;

        crystalActive = true;
        crystalVal = 255f;

        //doing this twice
        IncreaseGameSpeed();
        IncreaseGameSpeed();

        Vector2Int foo = RoundPlayerPos(new Vector2(xSpot, 0));

        crystalX = foo.x;
    }

    public void DeactivateCrystal()
    {
        crystalActive = false;
        crystalX = -1;
    }

    public bool IsCrystalActive()
    {
        return crystalActive;
    }

    public bool IsMorphFroze()
    {
        return morphFreeze;
    }

    public void MakeSmallExplosion(Vector3 foo)
    {
        Instantiate(explosionEffect2, new Vector3(foo.x, foo.y, foo.z), Quaternion.identity);
        audioOut.PlayOneShot(acExpl2, Random.Range(0.3f, 0.6f));
    }

    public void SpawnDragon()
    {
        if(pScript.GetLevel() > 1 && dragHolder == null && !IsGamePaused() && !roundWasWon && !crystalActive && !morphFreeze && pScript.IsDragonDead())
        {
            // play the spawning sound effect
            audioOut.PlayOneShot(acDragonSpawn, 0.8f);

            pScript.SetDragonIsDead(false);

            Vector2 bar = pScript.GetPlayerPosition();
            int num = 9;
            if (dragonToggle == false) // after lerpGameController is instantiated, the dragon controller takes over the toggling of dragonToggle
            {
                num = -5; // now that the dragon controller pauses for 1 second, it can spawn closer to the blocks
            }
            else
            {
                num = numBlocksX + 5;
            }

            Instantiate(dragon, new Vector3(num, -0.65f, bar.y), Quaternion.identity);

            if (pScript.GetLevel() > 5)
            {
                MakeDragonSpawnFaster();
            }
        }
    }
}
