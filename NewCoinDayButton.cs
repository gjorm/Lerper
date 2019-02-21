using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewCoinDayButton : MonoBehaviour {

    public AudioClip acClickOn;

    private Button button;
    private AudioSource audioOut;
    private PlayerController pScript;
    private LerpGameController gScript;
    private GameObject newCoinPanel;

    private void Awake()
    {
        newCoinPanel = GameObject.FindGameObjectWithTag("uiNewCoinDayPanel");
        if (newCoinPanel == null)
            Debug.Log("New Coin Day Button couldnt get reference to its parent panel object.");
    }

    // Use this for initialization
    void Start()
    {
        button = GetComponent<Button>();
        if (button == null)
            Debug.Log("Tutorial OK Button script could not get a reference to the button sript.");
        else
        {
            button.onClick.AddListener(UmOk);
        }

        audioOut = GetComponent<AudioSource>();
        if (audioOut == null)
            Debug.Log("New Coin OK Button couldnt get its Audio Source component.");

        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<LerpGameController>();
        if (gScript == null)
            Debug.Log("New Coin Ok script could not get a reference to the game controller.");

        pScript = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        if (pScript == null)
            Debug.Log("New Coin OK Button script could not get a reference to the Player controller.");
    }

    // Update is called once per frame
    void Update()
    {
        Image img = GetComponent<Image>();
        Color cA = pScript.GetCharColorA();
        Color cB = pScript.GetCharColorB();
        Color foo = Color.Lerp(cB, cA, pScript.GetLerpVal());
        foo.a = 255f;
        img.color = foo;
    }

    public void UmOk()
    {
        audioOut.PlayOneShot(acClickOn, 0.8f);

        gScript.SetNewCoinDayCollect();
        gScript.PauseGame(false);
        newCoinPanel.SetActive(false);
    }
}
