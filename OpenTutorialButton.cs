using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenTutorialButton : MonoBehaviour {

    private LerpGameController gScript;
    private Button button;
    private PlayerController pScript;
    

	// Use this for initialization
	void Start () {
        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<LerpGameController>();
        if (gScript == null)
            Debug.Log("OpenTutorialButton could not get a reference to the game controller.");

        button = GetComponent<Button>();
        if (button == null)
            Debug.Log("Open Tutorial Button script could not get a reference to the button sript.");
        else
            button.onClick.AddListener(OpenTutorial);

        pScript = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        if (pScript == null)
            Debug.Log("Open Tutorial Button script could not get a reference to the player controller.");

        

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

    public void OpenTutorial()
    {
        gScript.OpenTutorial();
    }
}
