using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PowerUp { Nothing, HourGlass, Diamond, CheckPoint, Bomb, Trophy, Crystal };

public class PowerUpController : MonoBehaviour
{

    public GameObject hgLight;
    public PowerUp PowerUpType;

    private GameObject _hgLight;
    private AudioSource audioOut;
    private LerpGameController gScript;
    private Renderer rend;
    private Color cpColor;
    private bool bombOnce = false;
    private long bombTime;
    private int bombSpin;
    private bool gamePauseBombFix;
    private bool collectOneShot; // (this now seems to be useless) physics system can trigger multiple  on collision enter functions, which messages with SetPowerUpType() in game controller

    public void Awake()
    {
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<LerpGameController>();
        if (gScript == null)
            Debug.Log("Power Up Controller couldnt get reference to the Game Controller script");
        else
            gScript.AddPowerUpToMasterList(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        

        _hgLight = (GameObject)Instantiate(hgLight, new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), Quaternion.identity);

        audioOut = GetComponent<AudioSource>();

        // this next section is mostly for the checkpoint power up but it may come in handy later for the other powerups
        rend = gameObject.GetComponentInChildren<Renderer>();
        rend.enabled = true;
        rend.material.EnableKeyword("_ALPHABLEND_ON");

        bombOnce = false;
        gamePauseBombFix = false;
        bombTime = 0;
        bombSpin = 0;

        collectOneShot = false;
    }

    // Update is called once per frame
    void Update()
    {
       

        // call this next bit after the bomb fix part
        if (bombOnce == true)
        {
            BombExplosion(); // this function checks on the bombOnce oneshot so theres no need to call it inside, but fuck it

            GetComponentInChildren<Transform>().Rotate(new Vector3(0, bombSpin, 0));
            bombSpin += 37;
            bombSpin = Mathf.Clamp(bombSpin, 0, 300);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //for some FUCKING reason, coins and other powerups keep getting spawned exactly where the trophy is spawned at. im tired of trying to figuring out why
        if(other.CompareTag("PowerUp")) // this is the other powerups tag
        {
            if(PowerUpType == PowerUp.Trophy || PowerUpType == PowerUp.CheckPoint || PowerUpType == PowerUp.Crystal) // this is the current power ups tag, and has to be set to Trophy or Checkpoints or Crystals
            {
                GameObject go = other.gameObject;
                if(go != null)
                {
                    CoinController c = null;
                    PowerUpController p = null;
                    c = go.GetComponent<CoinController>();
                    p = go.GetComponent<PowerUpController>();

                    if (c != null)
                    {
                        gScript.SubtractLevelCoins(1);
                        c.KillThisCoin();
                    }

                    if (p != null)
                        p.KillThisPowerUp();
                }
                // there may be a weird issue with the light still hanging around. I think a previous calling function may be destroy the object, but perhaps not its light
                GameObject.Destroy(other.gameObject);
            }
        }
        

        // normal collision logic
        if (other.CompareTag("Player") || other.CompareTag("Dragon")) // allow the dragon to set off bombs but not collect power ups
        {
            if (gScript != null)
            {
                if(collectOneShot == false)
                {

                    if (PowerUpType == PowerUp.CheckPoint && !other.CompareTag("Dragon"))
                    {
                        gScript.SetCheckPoint(new Vector2(gameObject.transform.position.x, gameObject.transform.position.z));

                        cpColor = Color.white;
                        rend.material.color = cpColor;
                        rend.material.SetColor("_Color", cpColor);

                        collectOneShot = true;
                    }
                    else if (PowerUpType == PowerUp.Bomb)
                    {
                        SetBombTimer();

                        bombSpin = 37;

                        collectOneShot = true;
                    }
                    else if (PowerUpType == PowerUp.Trophy && !other.CompareTag("Dragon"))
                    {
                        gScript.SetRoundOverTime(true); // set the round as being over and won
                        gScript.RemovePowerUpFromMasterList(gameObject.GetInstanceID());
                        collectOneShot = true;
                    }
                    else if (PowerUpType == PowerUp.Crystal && !other.CompareTag("Dragon") && !gScript.IsCrystalActive())
                    {
                        gScript.ActivateCrystal(gameObject.transform.position.x);
                        gScript.RemovePowerUpFromMasterList(gameObject.GetInstanceID());
                        collectOneShot = true;
                    }
                    else if (!other.CompareTag("Dragon"))
                    {
                        gScript.SetPowerUpType(PowerUpType);
                        gScript.RemovePowerUpFromMasterList(gameObject.GetInstanceID());
                        collectOneShot = true;
                    }
                }
            }
            else
            {
                Debug.Log("Game Controller not acquired.");
            }
        }
    }

    public void KillThisPowerUp()
    {
        //_hgLight.SetActive(false);
        //gameObject.SetActive(false);

        GameObject.Destroy(_hgLight);
        GameObject.Destroy(gameObject);
    }

    public void SetBombTimer()
    {
        if (!gScript.IsGamePaused())
        {

            System.DateTime epochStart = new System.DateTime(1986, 3, 25, 0, 0, 0, System.DateTimeKind.Utc);
            bombTime = (long)(System.DateTime.UtcNow - epochStart).TotalSeconds;

            bombOnce = true;
            audioOut.loop = true;
            audioOut.Play();
        }
    }

    public void BombExplosion()
    {
        // this will cause the resetting of the timer for the bomb if the player goes to the menu while after hitting a bomb, to give them time after going back to the game
        if (gamePauseBombFix == false && gScript.IsGamePaused())
        {
            gamePauseBombFix = true;
        }

        if (gamePauseBombFix == true && !gScript.IsGamePaused())
        {
            gamePauseBombFix = false;
            if (bombOnce == true)
            {
                SetBombTimer(); // reset the bomb timer if the oneshot is true
            }
        }

        if (bombOnce == true && (!gScript.IsGamePaused() || gScript.IsRoundOver()))
        {
            System.DateTime epochStart = new System.DateTime(1986, 3, 25, 0, 0, 0, System.DateTimeKind.Utc);
            long curTime = (long)(System.DateTime.UtcNow - epochStart).TotalSeconds;

            if (curTime > bombTime + 1)
            {
                audioOut.Stop();

                CameraController camC = Camera.main.GetComponent<CameraController>();
                camC.SetScreenShake();

                gScript.BombExplosion(new Vector2(transform.position.x, transform.position.z));
                bombOnce = false;
                gScript.RemovePowerUpFromMasterList(gameObject.GetInstanceID());
            }
        }
    }
}

