using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    public LerpGameController gScript;
    public AudioClip[] music;
    public float camY = 9; //this is camera height
    public float camUiBuffer = 0.10f; // this is the number, in percent, of the width of the UI shit. the camera will cast rays from this point. And the UI will set its size to this percent of screen width

    private float rayDist, dX, dZ, dS, dD, x, z, s, newX, newZ;
    private float minZ = 0.9f;
    private float maxZ = 4.35f;
    private float minX = -0.6f;
    private float maxX = 12.0f;

    private int screenHeightCapture;
    private float screenWCounter = 0.0f;

    private int mapBlocksX = 12; // default values will be overridden, hopefully
    private int mapBlocksY = 6;

    private float xVelocity = 0.0f;
    private float yVelocity = 0.0f;
    //private float shakeVel = 0.0f;
    private float smoothDamp = 0.3f;
    //private float shakeDamp = 0.49f;
    private float epsilon = 0.0005f;
    private Ray ray, rayTop, rayBottom, rayRight, rayLeft;
    private Vector3 test, testTop, testBottom, testRight, testLeft;
    private Vector2 pos;

    private PlayerController pScript;
    private AudioSource audioOut;

    Plane plane;
    //private float screenShake = 0;
    private int negate = -1;
    private long stopShakeTime = 0;
    private bool isCameraShaking = false;
    //private bool shakeDouble = false;
    private float xLatch;
    private float shakeTimer;
    private bool latchOnce = true;
    private float maxShake;


    private void Awake()
    {
        audioOut = GetComponent<AudioSource>();
        if (audioOut == null)
            Debug.Log("Camera script couldnt get a reference to the camera Audio Source");

        //audioOut.PlayOneShot(music[1], 0.9f);

        screenHeightCapture = Screen.height;
    }

    void Start()
    {
        plane = new Plane(Vector3.up, new Vector3(0, -0.5f, 0));
        

        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<LerpGameController>();
        if (gScript == null)
            Debug.Log("Camera script could not get a reference to the game controller.");


        // the next two lines fix the draw order
        Camera.main.transparencySortMode = TransparencySortMode.CustomAxis;
        Camera.main.transparencySortAxis = new Vector3(0.0f, 0.0f, 1.0f);
        // force left hand horizontal orientation
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;

        isCameraShaking = false;
        negate = 1;
        shakeTimer = 0.0f;
        latchOnce = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (pScript.IsAlive())
        {
            MoveCamera();
        }
    }

    void OnPostRender()
    {
        if(isCameraShaking == false && pScript.IsAlive())
        {
            // adjust camera size to keep a buffer to the left for the UI
            dD = Mathf.Abs(screenHeightCapture - Screen.height);
            if (dD == 0)
            { // if the screen height is being resized, do not alter the size of the camera! Unity does this itself during screen size changes
                s = Camera.main.orthographicSize;
                dS = minX - testLeft.x;
                s -= dS;
                Mathf.Clamp(s, 3, 6);
                Camera.main.orthographicSize = s;
            }

            // this resets the capture after a period of time after the screen has been resized
            if (dD > 0 || dD < 0)
            {
                screenWCounter += Time.deltaTime;
            }
            if (screenWCounter > 5.0f)
            {
                screenHeightCapture = Screen.height;
                screenWCounter = 0.0f;
            }


        }
    }

    public void InitCamera(GameObject player, int mapX, int mapY)
    {
        pScript = player.GetComponent<PlayerController>();
        //Debug.Log (pScript.IsAlive ());

        mapBlocksX = mapX;
        mapBlocksY = mapY;

        transform.position = new Vector3(transform.position.z, camY, 60); // 60 is the starting number of numBlocksY
    }

    public void MoveCamera()
    {
       

        // im moving the min max definitions to update here on every frame now to counteract the error after pausing the game (note: this doesnt work)
        minZ = -0.6f;
        maxZ = mapBlocksY + 0.6f;
        minX = -0.6f;
        maxX = mapBlocksX - 0.4f;

        z = transform.position.z;
        x = transform.position.x;

        if (pScript.IsAlive())
        {
            pos = pScript.GetPlayerPosition(); // get player position for vertical movement
        }

        // always test and do vertical movement
        ray = Camera.main.ScreenPointToRay(new Vector3((Screen.width / 2), (Screen.height / 2), 0)); // cast ray at center of screen to find player
        plane.Raycast(ray, out rayDist);
        test = ray.GetPoint(rayDist);

        rayTop = Camera.main.ScreenPointToRay(new Vector3((Screen.width / 2), Screen.height, 0)); // cast ray at top of screen to find top limit
        plane.Raycast(rayTop, out rayDist);
        testTop = rayTop.GetPoint(rayDist);

        rayBottom = Camera.main.ScreenPointToRay(new Vector3((Screen.width / 2), 0, 0)); // etc for down
        plane.Raycast(rayBottom, out rayDist);
        testBottom = rayBottom.GetPoint(rayDist);

        // calculating vertical cam movement
        dZ = (pos.y + 0.5f) - test.z;
        if (dZ >= 0)
        {
            if (testTop.z < maxZ)
            {
                z += dZ;
            }
        }
        else
        {
            if (testBottom.z > minZ)
            {
                z += dZ;
            }
        }

        if (isCameraShaking == false)
        {
            shakeTimer = 0.0f;

            rayRight = Camera.main.ScreenPointToRay(new Vector3(Screen.width, (Screen.height / 2), 0)); // far right
            plane.Raycast(rayRight, out rayDist);
            testRight = rayRight.GetPoint(rayDist);

            rayLeft = Camera.main.ScreenPointToRay(new Vector3((Screen.width * camUiBuffer), (Screen.height / 2), 0)); // far Left
            plane.Raycast(rayLeft, out rayDist);
            testLeft = rayLeft.GetPoint(rayDist);



            /* maybe i can get this working, but its... kind of not necessary
            if (testBottom.z < minZ) {
                altDZ = Mathf.Abs (testBottom.z) - Mathf.Abs (minZ);
                z += altDZ;
            } 
            */

            // and horizontal movement
            dX = maxX - testRight.x;
            x += dX;

            // smooth out the transition
            newX = Mathf.SmoothDamp(transform.position.x, x, ref xVelocity, smoothDamp);

            newX = Mathf.Clamp(newX, 3, 5);

        } else
        {
            // screen shake!!
            x = xLatch + shakeTimer;
            shakeTimer += (5 * negate) * (Time.deltaTime * 2);

            if(shakeTimer >= maxShake)
            {
                shakeTimer -= 5 * Time.deltaTime;
                negate *= -1;
                maxShake -= 0.032f;
            }

            if(shakeTimer <= (maxShake * -1))
            {
                shakeTimer += 5 * Time.deltaTime;
                negate *= -1;
                maxShake -= 0.032f;
            }

            // disable after 1 second
            System.DateTime epochStart = new System.DateTime(1986, 3, 25, 0, 0, 0, System.DateTimeKind.Utc);
            long curTime = (long)(System.DateTime.UtcNow - epochStart).TotalSeconds;

            if (curTime >= stopShakeTime || maxShake < 0.00f)
            {
                isCameraShaking = false;
                shakeTimer = 0.0f;
                x = xLatch;
            }

            newX = x;
        }

        newZ = Mathf.SmoothDamp(transform.position.z, z, ref yVelocity, smoothDamp);

        Mathf.Clamp(newX, minX, mapBlocksX);
        Mathf.Clamp(newZ, minZ, mapBlocksY + 2.1f);

        transform.position = new Vector3(newX, camY, newZ);
    }

    // x coord is bottom block row and y coord is upper block coord
    public Vector2 GetViewBlockLimits()
    {
        return new Vector2(testBottom.z, testTop.z);
    }

    public bool EvalFloatAsZero(float val)
    {
        bool result = false;

        if (Mathf.Abs(val) < epsilon)
            result = true;

        return result;
    }

    public void SetScreenShake()
    {
        // set the amount
        shakeTimer = 0.0f;
        maxShake = 0.28f;
        // set the timer
        System.DateTime epochStart = new System.DateTime(1986, 3, 25, 0, 0, 0, System.DateTimeKind.Utc);
        long curTime = (long)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        stopShakeTime = curTime + 1;

        if(isCameraShaking == false && latchOnce == true)
        {
            xLatch = transform.position.x; // save the initial x position found here, but only once on the very first call to screen shake
            latchOnce = false;
        }

        isCameraShaking = true;
        //shakeDouble = true;
    }

    public void StopMusic()
    {
        audioOut.Stop();
    }

    public void SetWinMusic()
    {
        audioOut.Play();
    }

    public void SetLoseMusic()
    {
        audioOut.PlayOneShot(music[1], 0.7f);
    }
}

