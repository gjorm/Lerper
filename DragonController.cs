using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonController : MonoBehaviour
{
    public float speed;

    private BoxCollider box;
    private CharacterController charControl;
    private Vector3 movement;
    private LerpGameController gScript;
    private PlayerController pScript;
    private int dir;
    private bool dragonToggle;
    private bool playerKillOnce;
    private long startTime;
    private bool startOneShot;

    // Use this for initialization
    void Start()
    {
        box = gameObject.GetComponent<BoxCollider>();
        Vector3 vec = new Vector3(1, 1, 1);
        Vector3 vec2 = new Vector3(1, 1, 1);
        if (box == null)
        {
            Debug.Log("Could not Get Box Collider component.");
        }
        else
        {
            vec = box.size;
            vec2 = box.center;
        }
        charControl = gameObject.GetComponent<CharacterController>();
        if (!charControl)
        {
            Debug.Log("Couldnt fetch the character controller component in dragon");
        }

        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<LerpGameController>();
        if (gScript == null)
            Debug.Log("Dragon script couldnt get reference to game controller");
        else
            gScript.IncrementDragonsSpawned();

        pScript = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        if (pScript == null)
            Debug.Log("Dragon script couldnt get reference to the player controller");
        else
            pScript.SetDragonIsDead(false);

        dir = -1;
        dragonToggle = false;
        playerKillOnce = false;

        if (gScript != null)
        {
            dragonToggle = gScript.GetDragonToggle();
        }

        if (dragonToggle == true)
        {
            transform.RotateAround(transform.position, Vector3.up, -90);
            dir = -1;
            gScript.SetDragonToggle(false); // these should not affect the local dragonToggle variable
        }
        else
        {
            transform.RotateAround(transform.position, Vector3.up, 90);
            dir = 1;
            gScript.SetDragonToggle(true);
        }

        System.DateTime epochStart = new System.DateTime(1986, 3, 25, 0, 0, 0, System.DateTimeKind.Utc);
        startTime = (long)(System.DateTime.UtcNow - epochStart).TotalSeconds;

        startOneShot = false;
    }

    // Update is called once per frame
    void Update()
    {
        //iff the player pauses the game directly after a dragon is spawned, then keep updating the time while it is paused. This prevents the dragon from
        // coming in and obliterating the player after unpausing, when the player was expecting a delay
        if(gScript.IsGamePaused() || gScript.IsMorphFroze())
        {
            System.DateTime epochStart = new System.DateTime(1986, 3, 25, 0, 0, 0, System.DateTimeKind.Utc);
            startTime = (long)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        }

        if(startOneShot == false && !gScript.IsGamePaused() && !gScript.IsMorphFroze())
        {
            System.DateTime epochStart = new System.DateTime(1986, 3, 25, 0, 0, 0, System.DateTimeKind.Utc);
            long check = (long)(System.DateTime.UtcNow - epochStart).TotalSeconds;
            if(check >= startTime + 2)
            {
                startOneShot = true;
            }

            // okay, so even though the dragon is spawned in lerp game controller, and that code will stay, it is modified here to catch up with a fast moving player
            float startZ = pScript.GetPlayerMapYSaved() + Random.Range(0, 4);
            transform.position = new Vector3(transform.position.x, transform.position.y, startZ);
        }

        if(startOneShot == true)
        {
            if (!gScript.IsGamePaused() && !gScript.IsMorphFroze())
            {
                //movement = transform.forward;
                movement = new Vector3(dir, 0, 0);
                movement *= speed;
                charControl.Move(movement * Time.deltaTime);
                Vector3 trans = transform.position;
                // latch the height of the dragon to prevent the blocks from pushing up on it
                trans.y = 0.25f;
                transform.position = trans;
            }


            //if the dragon has moved beyond its needed coordinates, then kill it
            if (transform.position.x < -4.0 && dragonToggle == true)
            {
                KillDragon();
            }
            else if (transform.position.x > gScript.numBlocksX + 4 && dragonToggle == false)
            {
                KillDragon();
            }
        }
    }

    public void KillDragon()
    {
        pScript.SetDragonIsDead(true);

        gScript.ResetDragHolder();

        gScript.SetDragonTime();

        GameObject.Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerKillOnce == false)
            {
                playerKillOnce = true;

                if (pScript != null)
                {
                    pScript.KillPlayer(gScript.GetNumCheckPoints() * -1);
                    gScript.IncrementDragonAvoided();
                }
            }
        }
    }
}
