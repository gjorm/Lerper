using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinOkButton : MonoBehaviour {

    private Button button;
    private MenuButton menuB;
    private PlayerController pScript;
    private bool okPerm;

	// Use this for initialization
	void Start () {
        button = GetComponent<Button>();
        if (button == null)
            Debug.Log("Win Ok button couldnt get a reference to its button component");
        else
            button.onClick.AddListener(OkButton);

        menuB = GameObject.FindGameObjectWithTag("MenuButton").GetComponent<MenuButton>();
        if (menuB == null)
            Debug.Log("Win Ok Button couldnt get reference to the menu button");

        pScript = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        if (pScript == null)
            Debug.Log("Win OK Button script could not get a reference to the player controller.");

        okPerm = false;
    }
	
	// Update is called once per frame
	void Update () {
        Image img = GetComponent<Image>();
        Color cA = pScript.GetCharColorA();
        Color cB = pScript.GetCharColorB();
        Color foo = Color.Lerp(cB, cA, pScript.GetLerpVal());
        foo.a = 255f;
        img.color = foo;

        // make sure to allow enough time for the coin counting to be done
        if (okPerm == false && pScript.IsCoinCountingDone() == true)
        {
            okPerm = true;
            button.enabled = true;
        }
        if (okPerm == true && pScript.IsCoinCountingDone() == false)
        {
            okPerm = false;
            button.enabled = false;
        }
    }

    public void OkButton()
    {
        if(okPerm == true)
        {
            menuB.enabled = true;
            okPerm = false;
            menuB.RoundOverMenu();
        }
    }
}
