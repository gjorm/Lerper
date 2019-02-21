using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour {
    public AudioClip acClickOn;

    private Button button;
    private MenuButton menuButton;
    private LerpGameController gScript;
    private AudioSource audioOut;
    private PlayerController pScript;
    private int charIndex;

    // Use this for initialization
    void Start()
    {
        //player index defaults to 0
        charIndex = 0;

        menuButton = GameObject.FindGameObjectWithTag("MenuButton").GetComponent<MenuButton>();
        if (menuButton == null)
            Debug.Log("Play Button couldnt get a reference to the Menu Button");

        button = GetComponent<Button>();
        if (button == null)
            Debug.Log("Play Button script could not get a reference to the button sript.");
        else
            button.onClick.AddListener(PlayNewRound);

        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<LerpGameController>();
        if (gScript == null)
            Debug.Log("Play Button script could not get a reference to the game controller.");

        audioOut = GetComponent<AudioSource>();
        if (audioOut == null)
            Debug.Log("Play Button couldnt get its Audio Source component.");

        pScript = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        if (pScript == null)
            Debug.Log("Buy Button script could not get a reference to the player controller.");

    }
    // Update is called once per frame
    void Update () {
        Image img = GetComponent<Image>();
        Color cA = pScript.GetCharColorA();
        Color cB = pScript.GetCharColorB();
        Color foo = Color.Lerp(cB, cA, pScript.GetLerpVal());
        foo.a = 255f;
        img.color = foo;


    }

    public void SetCharIndex(int i)
    {
        charIndex = i;
        
    }

    public void PlayNewRound()
    {
        audioOut.PlayOneShot(acClickOn, 1f);

        gScript.NewRound(charIndex);

        menuButton.ResetRoundVar();

        menuButton.CloseMenu();
    }
}
