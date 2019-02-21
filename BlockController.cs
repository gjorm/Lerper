using UnityEngine;
using System.Collections;

public class BlockController : MonoBehaviour
{

    public Color colorA;
    public Color colorB;
    public Color frozen;
    public Color trophy;
    public float totalLandTime = 0.3f;
    public float pLandVel = 3.0f;

    private GameObject gameController;
    private Renderer blockRend;
    private float blockFade;
    private float blockLerp;
    private bool pLandMoveRequested = false; // enables the entire player block move process
    private bool pLandMoveLatch = true; // allows only for one iteration of the move process
    private bool bLandMoveLatch = false; // allows for reset of timer at the beginning
    private float pLandTimer = 0.0f;
    private float pLandTimer2 = 0.0f;
    private bool mouseMove = false;
    private float mTimer = 0.0f;
    private float mTimer2 = 0.0f;
    private float totalMouseTime = 0.3f;
    private bool isFrozen = false;
    private bool isTrophyArea = false;
    private float yVal;
    private float blockHeightScale = 0.7f;
    private bool isVerticalPath = false;

    // Use this for initialization
    void Start()
    {
        blockRend = GetComponent<Renderer>();
        blockRend.enabled = true;
        blockRend.material.EnableKeyword("_ALPHABLEND_ON");
        blockFade = 0.0f;

        // get an instance of the gamecontroller on startup. cant seem to drag and drop things from the scene unfortunately
        gameController = GameObject.FindGameObjectWithTag("GameController");
        if (gameController == null)
            Debug.Log("A block couldnt get a copy of the game controller.");
    }

    // Update is called once per frame
    void Update()
    {
        // adjust the height of the block according to fade as well
        yVal = 0 - (blockHeightScale - (blockFade * blockHeightScale));
        transform.position = new Vector3(transform.position.x, yVal, transform.position.z);

        

        Color blockColor = blockRend.material.color;
        if (!isFrozen && !isTrophyArea)
        {
            if(isVerticalPath)
            {
                blockColor = Color.Lerp(colorB, colorA, blockLerp);
                blockColor.a = Mathf.Clamp(blockFade * 1.3f, 0, 1); //manually come in and adjust alpha
            }
            else
            {
                blockColor = Color.Lerp(colorA, colorB, blockLerp);
                blockColor.a = Mathf.Clamp(blockFade * 1.3f, 0, 1); //manually come in and adjust alpha
            }
        }
        else
        {
            if(isFrozen)
            {
                blockColor = frozen;
                blockColor.a = 1.0f;
            } else
            {
                blockColor = trophy;
                blockColor.a = 1.0f;
            }
        }
        blockRend.material.SetColor("_Color", blockColor);

        // Player Character Lands on Block Section ////////////////////////////////////////////
        //when player lands on block
        if (pLandMoveRequested == true && pLandMoveLatch == true)
        {
            //set the timer
            if (bLandMoveLatch == false)
            {
                pLandTimer = 0.0f;
                pLandTimer2 = 0.0f;
                bLandMoveLatch = true; // set this to prevent constant force and to stop constantly resetting timer
            }

            if (bLandMoveLatch == true)
            {
                if (pLandTimer >= -totalLandTime)
                {
                    transform.position = new Vector3(transform.position.x, yVal + (pLandTimer * pLandVel), transform.position.z);
                    pLandTimer -= Time.deltaTime;// * 1.5f;
                    pLandTimer2 -= Time.deltaTime;// * 1.5f;
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, yVal + (pLandTimer2 * pLandVel), transform.position.z);
                    pLandTimer2 += Time.deltaTime;
                }
            }

            // if cycle is done timing then reset transform back to zero
            if (bLandMoveLatch == true && pLandTimer <= -totalLandTime && pLandTimer2 >= 0.0f)
            {
                pLandMoveLatch = false;
                transform.position = new Vector3(transform.position.x, yVal, transform.position.z);
            }
        }

        //catch if player jumped off from block before a full cycle is allowed and reset block
        if (pLandMoveRequested == false && pLandTimer < 0.0f)
        {
            transform.position = new Vector3(transform.position.x, yVal, transform.position.z);
        }
        //force a reset of the latches when player leaves
        if (pLandMoveRequested == false)
        {
            pLandMoveLatch = true;
            bLandMoveLatch = false;
        }



        // Player Clicks on Block Section /////////////////////////////////////////////////
        if (mouseMove == true)
        {
            if (mTimer <= totalMouseTime)
            {
                transform.position = new Vector3(transform.position.x, yVal - mTimer, transform.position.z);
                mTimer += Time.deltaTime * 4f;
                mTimer2 += Time.deltaTime * 4f;
            }
            else
            {
                transform.position = new Vector3(transform.position.x, yVal - mTimer2, transform.position.z);
                mTimer2 -= Time.deltaTime * 4f;
            }

            //once done timing reset everything
            if (mTimer >= totalMouseTime && mTimer2 <= 0.0f)
            {
                mouseMove = false;
                mTimer = 0.0f;
                mTimer2 = 0.0f;
                transform.position = new Vector3(transform.position.x, yVal, transform.position.z);
            }
        }
    }

    // collision detection
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dragon"))
        { // if a dragon hits the block, disable the block
            if (gameController != null)
            {
                LerpGameController gScript = gameController.GetComponent<LerpGameController>();
                Vector2 foo = new Vector2(transform.position.x, transform.position.z);

                if (gScript)
                {
                    gScript.DisableBlock(foo);

                    CameraController camC = Camera.main.GetComponent<CameraController>();
                    camC.SetScreenShake();

                }
                else
                {
                    Debug.Log("Block at " + foo + " could not get a valid instance of the game controller script.");
                }

            }
            else
            {
                Debug.Log("Couldn't acquire a gamecontroller");
            }
        }
    }

    public void setBlockFade(float fade)
    {
        //scale and clamp the fade value so that the blocks are more opaque towards the top and move further up and down
        blockFade = Mathf.Clamp(fade * 1.2f, 0.0f, 1.0f);
    }

    public void PlayerLandMove(bool enabled, GameObject player)
    {
        if (enabled == false)
        {
            pLandMoveRequested = false;
            pLandMoveLatch = true;
            bLandMoveLatch = false;
        }
        else
        {
            pLandMoveRequested = true;
        }
    }

    public void setBlockLerp(float lerp)
    {
        blockLerp = lerp;
    }

    public bool IsBlockBackUp()
    {
        bool result = false;
        if (bLandMoveLatch == true && pLandTimer <= -totalLandTime && pLandTimer2 >= 0.0f)
        {
            result = true;
        }

        return result;
    }

    public void ClickBlockMove()
    {
        if (mouseMove == false)
        {
            mouseMove = true;
            mTimer = 0.0f;
        }
    }

    public void SetColorA(Color a)
    {
        colorA = a;
    }

    public void SetColorB(Color b)
    {
        colorB = b;
    }

    public void SetFrozenAndTrophyColors(Color bleep)
    {
        frozen = bleep;
        trophy = bleep;
    }

    public void SetFrozen(bool f)
    {
        if(!isTrophyArea)
            isFrozen = f;
    }

    public void SetTrophyArea(bool f)
    {
        isTrophyArea = f;
    }

    public void SetVerticalPath(bool f)
    {
        isVerticalPath = f;
    }
}
