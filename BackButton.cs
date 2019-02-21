using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour {
    public AudioClip acClickOn;

    private Button button;
    private MenuButton menuButton;
    private AudioSource audioOut;
    private PlayerController pScript;
    private Text[] bbText;

    private float alpha;

    // Use this for initialization
    void Start () {
        menuButton = GameObject.FindGameObjectWithTag("MenuButton").GetComponent<MenuButton>();
        if (menuButton == null)
            Debug.Log("Back Button couldnt get a reference to the Menu Button");

        bbText = new Text[2]; // should be only two...
        button = GetComponent<Button>();
        if (button == null)
            Debug.Log("Back Button script could not get a reference to the button sript.");
        else
        {
            button.onClick.AddListener(GoBack);
            Text[] t = button.gameObject.GetComponentsInChildren<Text>();
            if (t != null)
                for(int i = 0; i < t.Length; i++)
                {
                    bbText[i] = t[i];
                    if(bbText[i] == null)
                        Debug.Log("Back Button couldnt get a reference to one of its child Text component.");
                }
        }

        audioOut = GetComponent<AudioSource>();
        if (audioOut == null)
            Debug.Log("Back Button couldnt get its Audio Source component.");

        pScript = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        if (pScript == null)
            Debug.Log("Buy Button script could not get a reference to the Player controller.");

        alpha = 255f;
    }
	
	// Update is called once per frame
	void Update () {
        Image img = GetComponent<Image>();
        Color cA = pScript.GetCharColorA();
        Color cB = pScript.GetCharColorB();
        Color foo = Color.Lerp(cB, cA, pScript.GetLerpVal());
        foo.a = alpha;
        img.color = foo;

        foo = bbText[0].color;
        foo.a = alpha;
        bbText[0].color = foo;
        foo = bbText[1].color;
        foo.a = alpha;
        bbText[1].color = foo;
    }

    public void GoBack()
    {
        audioOut.PlayOneShot(acClickOn, 0.8f);
        menuButton.CloseMenu();
    }

    public void SetAlpha(float a)
    {
        alpha = a;
    }
}
