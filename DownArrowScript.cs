using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownArrowScript : MonoBehaviour {

    private Image rend;
    private bool doFade;
    [System.NonSerialized]
    private float fade;
    private Color col;

	// Use this for initialization
	void Start () {
        rend = GetComponent<Image>();
        if (rend == null)
            Debug.Log("Down Arrow script couldnt get a reference to its own Image component");

        doFade = false;
        fade = 0.0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(doFade)
        {
            fade -= 0.01f;
        }
        else
        {
            fade = 0f;
        }

        if(fade <= 0 && doFade == true)
        {
            doFade = false;
            fade = 0f;
        }

        col = rend.color;
        col.a = fade;
        rend.color = col;
	}

    public void Pop()
    {
        doFade = true;
        fade = 1.2f; // overshoot a little so it stays opaque for a short bit
    }

    public void UnPop()
    {
        doFade = false;
        fade = 0f;
    }
}
