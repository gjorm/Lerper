using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelColorizer : MonoBehaviour {
    public bool InverseColors = true;
    private PlayerController pScript;

	// Use this for initialization
	void Start () {

        pScript = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        if (pScript == null)
            Debug.Log("Win panel script could not get a reference to the player controller.");

        
    }
	
	// Update is called once per frame
	void Update () {
        Image img = GetComponent<Image>();
        Color cA = pScript.GetCharColorA();
        Color cB = pScript.GetCharColorB();
        Color foo;
        if(InverseColors)
            foo = Color.Lerp(cB, cA, pScript.GetLerpVal());
        else
            foo = Color.Lerp(cA, cB, pScript.GetLerpVal());
        foo.a = 255f;
        img.color = foo;
    }
}
