using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialOkButton : MonoBehaviour {

    public AudioClip acClickOn;

    public string[] tutorials;
    public Sprite[] tutImages;
    public string[] buttonQuotes;

    private Button button;
    private AudioSource audioOut;
    private PlayerController pScript;
    private LerpGameController gScript;
    private int progress;
    private WinPanel tutPanel;
    private BuyPanelScript bPanel;

    private Image tutImage;
    private Text tuts;
    private Text bbText;

    private void Awake()
    {
        progress = 0;

        tutImage = GameObject.FindGameObjectWithTag("uiTutPic").GetComponent<Image>();
        if (tutImage == null)
            Debug.Log("Tutorial Ok button couldnt get a reference to the Tutorial Sprite Image");
            

        tuts = GameObject.FindGameObjectWithTag("uiTutText").GetComponent<Text>();
        if (tuts == null)
            Debug.Log("Tutorial OK button couldnt get a reference to the Tutorial Text Element");
            
    }

    // Use this for initialization
    void Start()
    {
        button = GetComponent<Button>();
        if (button == null)
            Debug.Log("Tutorial OK Button script could not get a reference to the button sript.");
        else
        {
            button.onClick.AddListener(NextTut);

            bbText = GetComponentInChildren<Text>();
            if (bbText == null)
                Debug.Log("Tutorial Ok button couldnt get reference to its own Text");
        }

        audioOut = GetComponent<AudioSource>();
        if (audioOut == null)
            Debug.Log("Tutorial OK Button couldnt get its Audio Source component.");

        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<LerpGameController>();
        if (gScript == null)
            Debug.Log("Tutorial Ok Button script could not get a reference to the game controller.");

        pScript = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        if (pScript == null)
            Debug.Log("Tutorial OK Button script could not get a reference to the Player controller.");

        tutPanel = GetComponentInParent<WinPanel>();
        if (tutPanel == null)
            Debug.Log("Tutorial Ok Button couldnt get a reference to its parent Win Panel Script");

        bPanel = GameObject.FindGameObjectWithTag("uiPanel").GetComponent<BuyPanelScript>();
        if (bPanel == null)
            Debug.Log("Open tutorial Button Couldnt Get reference to the ui Buy Panel");

        progress = 0;
        tutImage.sprite = tutImages[progress];
        tuts.text = tutorials[progress];
        bbText.text = buttonQuotes[progress];
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

    public void NextTut()
    {
        audioOut.PlayOneShot(acClickOn, 0.8f);

        progress++;

        if (progress == 8)
        {
            progress = 0;
            tutPanel.SetMovePanelOut();
            gScript.SetTutorialDone();

            //reset the tutorial for the next time it is played
            tutImage.sprite = tutImages[0];
            tuts.text = tutorials[0];
            bbText.text = buttonQuotes[0];
        }
        else
        {
            tutImage.sprite = tutImages[progress];
            tuts.text = tutorials[progress];
            bbText.text = buttonQuotes[progress];
        }

        
    }


}
