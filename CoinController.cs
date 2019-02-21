using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public GameObject coinLight;
    public GameObject collInd1;
    [System.NonSerialized]
    private GameObject _coinLight;
    public int coinValue;
    private PlayerController peep;
    private LerpGameController gScript;
    private bool collectOneShot; // this now appears to be useless

    // Use this for initialization
    void Start()
    {
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<LerpGameController>();
        if (gScript == null)
            Debug.Log("A coin controller couldnt get a reference to the game controller");
        else
        {
            gScript.AddPowerUpToMasterList(gameObject);
            gScript.RegisterCoin(1);
        }
            

        //move the light up by one to shine more on the coin itself
        _coinLight = (GameObject)Instantiate(coinLight, new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), Quaternion.identity);

        // get the reference to the script held by the parent/main player object. This is done because the player object the coin touches doesnt have the player controller script, unless calling GetComponentInParent
        peep = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        if (peep == null)
            Debug.Log("A Coin Controller couldnt get the player reference");

        // this is a half assed fix for the assets fuck up. a value of one indicates a silver coin
        if (coinValue == 1)
        {
            transform.Rotate(new Vector3(0, 0, Random.Range(0,300)));
        }
        else
        {
            transform.Rotate(new Vector3(0, Random.Range(0, 300), 0));
        }

        collectOneShot = false;
    }

    // Update is called once per frame
    void Update()
    {
        // this is a half assed fix for the assets fuck up. a value of one indicates a silver coin
        if (coinValue == 1)
        {
            transform.Rotate(new Vector3(0, 0, 60) * Time.deltaTime);
        }
        else
        {
            transform.Rotate(new Vector3(0, 180, 0) * Time.deltaTime);
        }
    }

    public void KillThisCoin()
    {
        //_coinLight.SetActive(false);
        //gameObject.SetActive(false);

        GameObject.Destroy(_coinLight);
        GameObject.Destroy(gameObject);
    }

    // Collision Shit
    // Using the coins for Collision logic as my idiot way of handling player object references disables my ability to easily override the OnTriggerEnter method
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(collectOneShot == false)
            {
                collectOneShot = true;

                Instantiate(collInd1, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), Quaternion.Euler(90, 0, 0));

                peep.CollectCoin(coinValue);

                gScript.RemovePowerUpFromMasterList(gameObject.GetInstanceID());
            }
        }
    }

    public int GetCoinValue()
    {
        return coinValue;
    }

}

