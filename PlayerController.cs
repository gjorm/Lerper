using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyMobile;
using Borodar.FarlandSkies.CloudyCrownPro;

public class PlayerController : MonoBehaviour {
    public static PlayerController pScript;

    public GameObject player;
    public GameObject distHalo;
    public GameObject dragonWarning;
    public GameObject leftArrow;
    public GameObject rightArrow;
    public GameObject[] models;
    public int numChars;
    public float gravity = 10F;
    public float runMult = 6.0f;
    public float jumpMult = 6.0f;
    public float velClamp = 10.0f;
    public float jumpClamp = 10.0f;
    public float gravEpsilon = 0.2f;
    public float maxDistance;
    public float smoothTime = 0.3f;
    public AudioClip acCoin;
    public AudioClip acCoinBig;
    public AudioClip acJump;
    public AudioClip acPlayerDie;
    

    private AudioSource audioOut;
    private GameObject uiName;
    private Text charNameText;
    private int modelIndex;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 lookDir = Vector3.zero;
    private Vector3 moveDirClamp = Vector3.zero;
    private Vector3 lookDirClamp = Vector3.zero;
    private float moveVel = 0.0f;
    private Plane plane;
    private bool setPlaneOnce = true;
    private Ray ray;
    private float rayDist;
    private CharacterController charControl;
    private BoxCollider box;
    private Camera cam;
    private int jumpCtr = 0;
    private float jumpAmp = 2.0f;
    private bool landOnce = true;
    private float jumpVel = 0.0f;
    private float jumpDist = 0.0f;
    private float saveDist = 0.0f;
    private Animator anim;
    private int animId;
    private float yStandCapture = 0.0f;
    private bool yStandLatch = true;
    private BlockController bScript;
    private LerpGameController gScript;
    private GameObject avatar;
    private long playerStuckTime;
    private bool playerStuckOnce = false;
    private bool playerDieSoundOnce = false;
    private float lerpVal = 1.0f;
    private GameObject halo;
    private GameObject dragWarn;
    private GameObject lArr;
    private GameObject rArr;
    private bool isDragonDead;
    private bool dragWarnShot;
    [System.NonSerialized]
    private int charLevel;
    private float lastY;

    private int endLevelCoinCounter;
    private int maxEndRoundCoinCount = 150;

    [System.NonSerialized]
    private bool isSpawned = false;

    private SkyboxController skyBox;

	private float scaleFactor = 1.0f;
	private string cName = "Derp Derp Derp";
	private Color colorA;
	private Color colorB;
    private Color frozen;
	private bool DoAvatarScaling;
    private int musicLoop;
    private int jumpTimes;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        pScript = this;
        numChars = models.Length;
        dragWarnShot = false;
        charLevel = 0;

        audioOut = GetComponent<AudioSource>();

        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<LerpGameController>();
        if (gScript == null)
            Debug.Log("Player Controller script could not get a reference to the game controller.");

        // instantiate the player distance halo
        // fucking unity instantiation. 
        //  new Vector3(-5, -5, -5), Quaternion.identity
        halo = (GameObject)Instantiate(distHalo);
        dragWarn = (GameObject)Instantiate(dragonWarning);
        lArr = (GameObject)Instantiate(leftArrow);
        rArr = (GameObject)Instantiate(rightArrow);

        isDragonDead = true;
    }

    // Use this for initialization
    void Start() {
        // load information from saved data, which is updated whenever a new character is bought
        for (int i = 0; i < numChars; i++)
        {
            CharInfo c = models[i].GetComponent<CharInfo>();
            if (c != null)
            {
                c.Purchased = GameSaver.gSaver.charBought[i];
                c.level = GameSaver.gSaver.charLevel[i];
            }
        }
        

        cam = Camera.main;
		plane = new Plane(Vector3.up, player.transform.position); //creates a plane at the players base for player rotation

        //init the distance halo... not really for dragon warning
        HaloScript dScript = dragWarn.GetComponent<HaloScript>();
        if (dScript == null)
            Debug.Log("InitPlayer() couldnt get a reference to the halo script for dragon warning");
        else
        {
            dScript.InitHalo(1);
            dragWarn.SetActive(false);
        }

        // init the arrows
        HaloScript laScript = lArr.GetComponent<HaloScript>();
        if (laScript == null)
            Debug.Log("InitPlayer() couldnt get a reference to the halo script for left arrow");
        else
        {
            laScript.InitHalo(1);
            lArr.SetActive(false);
        }

        HaloScript raScript = rArr.GetComponent<HaloScript>();
        if (raScript == null)
            Debug.Log("InitPlayer() couldnt get a reference to the halo script for right arrow");
        else
        {
            raScript.InitHalo(1);
            rArr.SetActive(false);
        }

    }

    void FixedUpdate() {
		// spin the avatar
		if (avatar != null) {
			avatar.transform.RotateAround (avatar.transform.position, Vector3.up, 25 * Time.deltaTime);
		}

        //if the end of level trophy was collected, then give player 50 coins
        if (gScript.GetRoundWon() && endLevelCoinCounter < maxEndRoundCoinCount)
        {
            if (endLevelCoinCounter % 3 == 0)
                CollectCoin(-1);
            endLevelCoinCounter++;
        }

        //check if dragon has been spawned and enable drag warning
        if(isDragonDead && dragWarnShot == false)
        {
            dragWarn.SetActive(false);
            lArr.SetActive(false);
            rArr.SetActive(false);
            dragWarnShot = true;
        }

        if (!isDragonDead && dragWarnShot == true)
        {
            dragWarn.SetActive(true);

            if(gScript.GetDragonToggle() == false) // coming from the right
            {
                rArr.SetActive(true);
            }
            else
            {
                lArr.SetActive(true);
            }

            dragWarnShot = false;
        }
    }

	// Update is called once per frame
	void Update () {
        
        // this is the player movement code and it is some spaghetti shit
        if (isSpawned && !gScript.IsGamePaused() && !gScript.GetRoundWon() && !gScript.IsRoundNewTime()) { // Update() will still try to reference player character after being killed off
            //reset rotation to zero
            if (player.transform.position.y >= -2.5f)
                player.transform.Rotate(new Vector3(0,0,0));

            //capture and save the last map y value
            lastY = player.transform.position.z;

            //set the plane if grounded
            if (charControl.isGrounded && setPlaneOnce == true) {
				plane = new Plane(Vector3.up, charControl.ClosestPoint (Vector3.zero));
				setPlaneOnce = false;
			}

            // if a click or tap has occurred, test for coordinates being in the ui or in the game, then process the mouse or tap input
			if(gScript.IsMouseOrTouchOn())
            {
                Vector2 pos = gScript.GetInputInScreen();
                if (IsClickInGame(pos))
                {
                    //rotate player to look at mouse position
                    ray = cam.ScreenPointToRay(pos);
                    plane.Raycast(ray, out rayDist);
                    lookDir = ray.GetPoint(rayDist);
                    lookDir.y = player.transform.position.y;
                    player.transform.LookAt(lookDir);
                    //move player forward
                    if (jumpCtr < jumpTimes && player.transform.position.y > -2.0f && setPlaneOnce == false)
                    { //setPlaneOnce prevents action from being made so the player lands true on spawn
                      //anim.Play() always immediately plays the animation
                        anim.SetInteger(animId, 2);
                        anim.Play("Move");

                        //play the jump audio clip
                        audioOut.PlayOneShot(acJump, 0.6f);

                        // save this direction
                        moveDirClamp = player.transform.forward;
                        lookDirClamp = LimitVector(player.transform.position, lookDir);

                        //save the initial distance from the player to click point.
                        saveDist = GetDistance(lookDirClamp, player.transform.position);

                        jumpCtr++;
                        landOnce = true;
                    }
                }
            }

			if (jumpCtr > 0) {
				//grab direction that player is facing
				moveDirection = moveDirClamp;
				// use dist as velocity for jump movement
				jumpDist = GetDistance(lookDirClamp, player.transform.position);
				// get velocity of movement based on distance from player to click point in world coordinates
				moveVel = jumpDist * runMult;
				moveVel = Mathf.Clamp (moveVel, 0.0f, velClamp);

				//half way through the jump, the y axis needs to revert velocity
				jumpVel = (jumpDist - saveDist) + (saveDist * 0.5f);
				jumpVel *= jumpMult;
				jumpVel = Mathf.Clamp (jumpVel, (-1 * jumpClamp), jumpClamp);

				moveDirection.x *= moveVel;
				moveDirection.z *= moveVel;
				moveDirection.y = jumpAmp;
				moveDirection.y *= jumpVel;
			}

            // gravity
            moveDirection.y -= gravity * Time.deltaTime;

            //apply movement
            charControl.Move(moveDirection * Time.deltaTime);
		}
	}

	void LateUpdate() {
		if (isSpawned && !gScript.IsGamePaused() && !gScript.GetRoundWon()) {
			if (charControl.isGrounded && jumpCtr > 0) { //transitioning from mid air to ground
				//stop player movement
				moveDirection.x = 0;
                moveDirection.y = 0;
				moveDirection.z = 0;
                //apply movement
                charControl.Move(moveDirection * Time.deltaTime);
                
                jumpCtr = 0;
			}

			if (jumpCtr < 1 && landOnce == true) { // fully grounded
                if (charControl.isGrounded)
                    CapturePlayerY();

                //reset animation
                if (anim.GetInteger (animId) != 0 || anim.GetInteger (animId) != 1) {
					anim.SetInteger (animId, 1);
					anim.Play ("Idle");

					//reset the player y coordinate after landing on a block. lame ass character controller cant detect collisions unless controller is moving that direction
					// which is weird because the mouse click button will move the character up. 
					if (yStandLatch == false && bScript != null) {
						if(bScript.IsBlockBackUp())
                        {
                            player.transform.position = new Vector3(player.transform.position.x, yStandCapture, player.transform.position.z);
                            landOnce = false; // reset so that gravity can become active again
                        }
					}
				}

				//reset stuck detection one shot
				playerStuckOnce = false;
			}

			// player falls off of map
			if (player.transform.position.y < -2.0f) {
				jumpCtr = 0;
                //anim.SetInteger (animId, 5); //death animation
                //anim.Play("Die");
                //instead of death animation, which causes the character controls to go screwy, rotate the player around
                player.transform.Rotate(Vector2.up, 30);

				if (anim.GetCurrentAnimatorStateInfo (0).normalizedTime > 0.95f && anim.GetInteger(animId) == 5) { // if death animation has played once then disable
					anim.enabled = false;
					moveDirection.y -= gravity * Time.deltaTime;
					jumpAmp = 1.0f;
				}
                //going to apply extra gravity here as sometimes the player goes crazy and sinks slooooowly
                moveDirection.x = 0;
                moveDirection.y -= gravity * Time.deltaTime;
                moveDirection.z = 0;
                //apply movement
                charControl.Move(moveDirection * Time.deltaTime);
            }

			// set the scale of the model to get a bit bigger when jumping for effect
			if (!charControl.isGrounded) {
				player.transform.localScale = new Vector3(Mathf.Clamp(player.transform.position.y, scaleFactor, 1.5f), Mathf.Clamp(player.transform.position.y, scaleFactor, 1.5f), Mathf.Clamp(player.transform.position.y, scaleFactor, 1.5f));
			}
        }

        // these next two playerkill functions must each determine if the player is still alive separately, before they are called. if not, one function will kill the player before the other function, and the other function will still attempt to dereference it
        if(isSpawned)
        {
            // determine if player is stuck. For some reason the dragon will push the player character off a block and then it sometimes hangs
            if (player.transform.position.y < -2.6f)
            {
                InitPlayerStuckTime();
                if (IsPlayerStuck())
                    KillPlayer(gScript.GetNumCheckPoints());
            }
        }

        if (isSpawned)
        {
            // make sure to keep this as the last method called in lateupdate
            if (player.transform.position.y < -20)
            {
                KillPlayer(gScript.GetNumCheckPoints());
            }
        }

        // if the player is dead or is about to die (sometimes the player can die without going below the -5 y threshold
        if ((!isSpawned || player.transform.position.y < -5.0f) && playerDieSoundOnce == false)
        {
            audioOut.PlayOneShot(acPlayerDie, 1f);
            playerDieSoundOnce = true;
        }
    }

	public void SpawnPlayer(int index, float x, float y) {
		if(!gScript.IsGamePaused() || !gScript.GetRoundWon()) // wont spawn if the game is paused
        {
            int which = 0;

            if (isSpawned)
            {
                return;
            }

            if (index < 0 || index >= models.Length)
            {
                which = 0;
            }
            else
            {
                which = index;
            }

            //player spawn
            player = (GameObject)Instantiate(models[which], new Vector3(x, 12f, y), Quaternion.identity);
            InitPlayer(which);

            // avatar spawning
            if (avatar == null)
            {
                avatar = (GameObject)Instantiate(models[which], new Vector3(-10f, -2.05f, -14.55f), Quaternion.identity);
                if (DoAvatarScaling == true)
                {
                    avatar.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
                    //avatar.layer = 8; // 8 is the Above UI layer (this also isnt necessary anymore)
                }
            }
            else if (modelIndex != which)
            { // only spawn a new avatar if the character model changes
                GameObject.Destroy(this.avatar);
                avatar = null;
                avatar = (GameObject)Instantiate(models[which], new Vector3(-10f, -2.05f, -14.55f), Quaternion.identity);
                if (DoAvatarScaling == true)
                {
                    avatar.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
                    //avatar.layer = 8;
                }
            }

            isSpawned = true;
            modelIndex = which;
        }
	}

	public void InitPlayer(int index) {
		//adjust the Character Controller to fit the Box Collider that SHOULD be included with the suriyan models
		box = player.GetComponent<BoxCollider>();
		Vector3 vec = new Vector3(1, 1, 1);
		Vector3 vec2 = new Vector3 (1, 1, 1);
		if (box == null) {
			Debug.Log ("Could not Get Box Collider component.");
		} else {
			vec = box.size;
			vec2 = box.center;
		}
		charControl = player.AddComponent<CharacterController> () as CharacterController;
		if (charControl != null) {
			charControl.radius = (vec.x + vec.y + vec.z) * 0.33f * 0.33f; // average all three box collider dimensions, then take a third of that
			charControl.center = new Vector3 (0, (vec2.y), 0);
			charControl.height = vec2.y + 0.3f;
			//set the y value for when player block comes back up
			yStandCapture = box.center.y;
			yStandLatch = false;
		}

		anim = player.GetComponent<Animator>();
		animId = Animator.StringToHash ("animation");

		moveDirClamp = player.transform.position;
		lookDirClamp = player.transform.position;
		moveDirection = Vector3.zero;
		jumpCtr = 0; // may be redundant. im paranoid at this point
		jumpDist = 0.0f;
		//setPlaneOnce = true;
		landOnce = true;
		player.tag = "Player";
		jumpAmp = 2.0f;
		playerStuckOnce = false;
        playerDieSoundOnce = false;

		//character info stuff
		CharInfo c = player.GetComponentInChildren<CharInfo>();
		if (c == null)
			Debug.Log ("No Character Info found");

		cName = c.Name;
		scaleFactor = c.ScaleFactor;
		colorA = c.colorA;
		colorB = c.colorB;
		DoAvatarScaling = c.DoAvatarScaling;
        musicLoop = c.MusicLoop;
        maxDistance = c.Distance;
        jumpTimes = c.JumpTimes;
        frozen = c.worldBottomColor;
        c.level = GameSaver.gSaver.charLevel[index];
        charLevel = c.level;

        //calculate end level coin counter
        endLevelCoinCounter = 0;
        maxEndRoundCoinCount = 3 * 5 * charLevel;

        skyBox = GameObject.FindGameObjectWithTag("SkyBoxController").GetComponent<SkyboxController>();
        if (skyBox == null)
            Debug.Log("Player Controller couldnt get a reference to the skybox controller.");
        else
        {
            skyBox.TopColor = c.worldTopColor;
            skyBox.BottomColor = c.worldBottomColor;
        }

        

        //init the distance halo. Need to call InitHalo() below the character info
        HaloScript hScript = halo.GetComponent<HaloScript>();
        if (hScript == null)
            Debug.Log("InitPlayer() couldnt get a reference to the halo script");
        else
        {
            hScript.InitHalo(maxDistance);
            halo.SetActive(true);
        }

        

        //update the jump x side bar ui text
        Text jumpX = GameObject.FindGameObjectWithTag("uiJumpTimes").GetComponent<Text>();
        if(jumpX != null)
        {
            string str = "Jump X" + jumpTimes.ToString();
            jumpX.text = str;
        }

        //god im not proud of this. shitty design work
        //update the sprite for the progress bar
        Image bar = GameObject.FindGameObjectWithTag("uiProgressIndicator").GetComponent<Image>();
        MenuButton foo = GameObject.FindGameObjectWithTag("MenuButton").GetComponent<MenuButton>();
        CharacterButton b;
        GameObject[] butts = GameObject.FindGameObjectsWithTag("uiCharButton");
        if(butts != null)
        {
            foreach (GameObject g in butts)
            {
                b = g.GetComponent<CharacterButton>();
                if (b != null)
                {
                    if (b.charIndex == gScript.GetPlayerIndex())
                    {
                        if (bar != null)
                            bar.sprite = b.GetSprite();
                        if (foo != null)
                            foo.SetCharSelected(gScript.GetPlayerIndex(), b.GetSprite());
                    }
                }
            }
        }

        SetCharacterName(cName);
        //SetNumLivesUI();
    }

    public void KillPlayer(int numCheckPointsLeft) {

        if(numCheckPointsLeft != -5)
            gScript.MakeSmallExplosion(player.transform.position);

        isSpawned = false;
        //player.SetActive (false);
        GameObject.Destroy(player);
        player = null;

        halo.SetActive(false);
        dragWarn.SetActive(false);
        
        if(numCheckPointsLeft > -5 && numCheckPointsLeft < 0) // if a negative value was sent to KillPlayer, and its not -5, then it was set by the dragon. This helps prevent immediate respawn after dragon death
        {
            numCheckPointsLeft = Mathf.Abs(numCheckPointsLeft); //important to reset this to positive if not -5
        }

        if (numCheckPointsLeft < 1 && numCheckPointsLeft != -5) // this next bit is a total hack. but i dont want to call setroundovertime() again, if the menu is up
        {
            gScript.SetRoundOverTime(false); // once the round overui element is displayed, it will then call the menu button latched version
        }
    }

	public void CollectCoin(int value) {
        //negative one indicates an end of round or new coin day
        if(value != -1)
        {
            gScript.IncreaseGameSpeed();
            int mod = 5 - GetLevel();
            if (Random.Range(0, (3 + mod) - value) == 0 && !gScript.IsRoundOver() && GetLevel() <= 5)
            {
                gScript.SpawnDragon();
            }
        }
        else
        {
            value = 1;
        }
        //update values
        gScript.AddToBigCoinBank(value);
		gScript.SetCoinCount ();

        // play sound
        if (value == 1)
        {
            audioOut.PlayOneShot(acCoin, 0.8f);
        }
        else
        {
            audioOut.PlayOneShot(acCoinBig, 0.8f);
        }
	}

	float GetDistance(Vector3 a, Vector3 b) {
		return Vector2.Distance (GetVector2(a), GetVector2(b));
	}

	Vector3 LimitVector(Vector3 initial, Vector3 limited) {
		float save = limited.y;
		Vector2 allowed = GetVector2 (limited) - GetVector2 (initial);
		allowed = Vector2.ClampMagnitude (allowed, maxDistance);
		return initial + new Vector3(allowed.x, save, allowed.y);
	}

	float Clamp(float val, float clamp) {
		float outNum = val;
		if(val > clamp) {
			outNum = clamp;
		}
		return outNum;
	}

	Vector2 GetVector2(Vector3 vec) {
		return new Vector2 (vec.x, vec.z);
	}

	public Vector2 GetPlayerPosition() {
		if (!isSpawned) {
			return new Vector2 (0,0);
		}
		return new Vector2 (player.transform.position.x, player.transform.position.z);
	}

    public float GetPlayerYVal()
    {
        if (!isSpawned)
            return 0f;
        return player.transform.position.y;
    }

    public float GetPlayerMapYSaved()
    {
        return lastY;
    }

	public bool HasLanded() {
		bool result = false;
		if (jumpCtr < 1) {
			result = true;
		}
		return result;
	}

	public bool HasReachedJumpClickLimit() {
		bool result = true;
		if (isSpawned) {
			if(jumpCtr < jumpTimes) {
				result = false;
			}
		}
		return result;
	}

	public void GivePlayerBlockScript(BlockController bScript) {
		this.bScript = bScript;
	}

    public void CapturePlayerY()
    {
        yStandCapture = player.transform.position.y;
    }

	public bool IsAlive() {
		return isSpawned;
	}

	public Color GetCharColorA() {
		return colorA;
	}

	public Color GetCharColorB() {
		return colorB;
	}

    public Color GetFrozenAndTrophyColors()
    {
        return frozen;
    }

	public void InitPlayerStuckTime() {
		if (playerStuckOnce == false) {
			System.DateTime epochStart = new System.DateTime(1986, 3, 25, 0, 0, 0, System.DateTimeKind.Utc);
			playerStuckTime = (long)(System.DateTime.UtcNow - epochStart).TotalSeconds;
			playerStuckOnce = true;
		}
	}

	public bool IsPlayerStuck() {
		bool result = false;
		System.DateTime epochStart = new System.DateTime(1986, 3, 25, 0, 0, 0, System.DateTimeKind.Utc);
		long curTime = (long)(System.DateTime.UtcNow - epochStart).TotalSeconds;

		if (curTime > playerStuckTime + 5) {
			result = true;
			playerStuckOnce = false;
		}

		return result;
	}

	public void SetCharacterName(string name) {
        uiName = GameObject.FindGameObjectWithTag("UIName");
        charNameText = uiName.GetComponent<Text>();

        if (charNameText != null) {
			charNameText.text = name;
		}
	}

    public void SetCharLevel(int index, int num)
    {
        CharInfo c = models[index].GetComponent<CharInfo>();
        if(c != null)
        {
            c.level = num;

            if (c.level > 1)
                Debug.Log(index + " " + c.level);
        }
        else
        {
            Debug.Log("SetCharLevel couldnt get a reference to the models CharInfo for index: " + index);
        }
    }

    public void SetLerpVal(float lerp)
    {
        lerpVal = lerp;
    }

    public float GetLerpVal()
    {
        return lerpVal;
    }

    public bool IsClickInGame(Vector2 click)
    {
        bool result = true;
        // test within this coord that accounts for the side ui menu
        float xMin = Screen.width * 0.14f;

        if (click.x < xMin)
            result = false;

        // test within these coords that account for the menu button in the upper right corner
        // these are hard coded to the anchors set in the ui
        xMin = Screen.width * 0.88f;
        float xMax = Screen.width * 0.95f;
        float yMin = Screen.height * 0.9486f;
        float yMax = Screen.height;

        if (click.x >= xMin && click.x <= xMax && click.y >= yMin && click.y <= yMax)
            result = false;

        return result;
    }

    public string GetCharName(int index)
    {
        CharInfo c = models[index].GetComponent<CharInfo>();
        return c.Name;
    }

    public int GetCharCost(int index)
    {
        CharInfo c = models[index].GetComponent<CharInfo>();
        return c.Cost;
    }

    public float GetCharDistance(int index)
    {
        CharInfo c = models[index].GetComponent<CharInfo>();
        return c.Distance;
    }

    public int GetCharJumpTimes(int index)
    {
        CharInfo c = models[index].GetComponent<CharInfo>();
        return c.JumpTimes;
    }

    public bool GetCharPurchased(int index)
    {
        CharInfo c = models[index].GetComponent<CharInfo>();
        return c.Purchased;
    }

    public int GetCharLevel(int index)
    {
        int val = 0;
        CharInfo c = models[index].GetComponent<CharInfo>();
        if (c != null)
            val = c.level;
        else
            Debug.Log("Could not fetch CharInfo Component in GetCharLevel()");

        return val; //returns 0 on failure
    }

    public int GetNumModels()
    {
        return models.Length;
    }

    public void SetCharacterBought(int index)
    {
        models[index].GetComponent<CharInfo>().Purchased = true;
    }

    public bool WithinPlayField()
    {
        bool result = false;
        Vector2 pPos = GetPlayerPosition();
        Vector2Int blocks = gScript.GetNumBlocks();

        if (pPos.x > -4 && pPos.y > -2 && pPos.x < blocks.x + 2 && pPos.y < blocks.y + 2 && GetPlayerYVal() <= 13 && isSpawned)
            result = true;

        return result;
    }

    public int GetCurrentCharIndex()
    {
        return gScript.GetPlayerIndex();
    }

    public bool IsReadyForSpawn()
    {
        bool result = true;

        if (!isDragonDead && !gScript.IsMorphFroze())
            result = false;

        return result;
    }

    public void SetDragonIsDead(bool dead)
    {
        isDragonDead = dead;
    }

    public bool IsCoinCountingDone()
    {
        bool result = false;
        if (gScript.GetRoundWon())
        {
            if (endLevelCoinCounter >= maxEndRoundCoinCount)
                result = true;
        }
        else
            result = true;

        return result; 
    }

    public Vector3 GetVector3()
    {
        return transform.position;
    }

    public int GetLevel()
    {
        return charLevel;
    }

    public void SetLevel(int x)
    {
        if (x > 0)
        {
            charLevel = x;
            CharInfo c = models[GetCurrentCharIndex()].GetComponent<CharInfo>();
            if (c != null)
                c.level = charLevel;
            else
                Debug.Log("Could not fetch CharInfo Component when updating player level");
        }
    }

    public bool IsDragonDead()
    {
        return isDragonDead;
    }
}
