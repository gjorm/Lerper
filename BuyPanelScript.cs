using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyPanelScript : MonoBehaviour {

    public float speed = 0.4f;

    private PlayerController pScript;
    private RectTransform rect;
    [System.NonSerialized]
    private float minX, maxX, dX, old;
    private float vel, vel2;
    private int compAct, compSpec;
    bool movePanelIn;
    bool movePanelOut;

    // Use this for initialization
    void Awake () {

        minX = -Screen.width - 5;
        old = minX;
        maxX = 0;

        pScript = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        if (pScript == null)
            Debug.Log("Buy panel script could not get a reference to the player controller.");

        rect = GetComponent<RectTransform>();
        if (rect == null)
        {
            Debug.Log("Buy Panel Script couldnt get reference to its own rect transform");
        }
        else
        {
            rect.localPosition = new Vector3(minX, rect.localPosition.y, rect.localPosition.z);
        }

        

        movePanelIn = false;
        movePanelOut = false;
    }
	
	// Update is called once per frame
	void Update () {
        Image img = GetComponent<Image>();
        Color cA = pScript.GetCharColorA();
        Color cB = pScript.GetCharColorB();
        Color foo = Color.Lerp(cA, cB, pScript.GetLerpVal());
        foo.a = 255f;
        img.color = foo;

        // calculate minX as it can change
        minX = -Screen.width - 5;
        // if a screen size change did happen, then reset the transform and update the old variable
        if(minX != old)
        {
            rect.localPosition = new Vector3(minX, rect.localPosition.y, rect.localPosition.z);
            old = minX;
        }

        if (movePanelIn == true && movePanelOut == false)
        {
            compAct = Mathf.RoundToInt(rect.localPosition.x);
            compSpec = Mathf.RoundToInt(maxX);
            if (compAct < compSpec) // this is not necessary for the move in, but im doing it anyways. Not sure why this worked and moveOut did not
            {
                dX = Mathf.SmoothDamp(rect.localPosition.x, maxX, ref vel, speed);
                rect.localPosition = new Vector3(dX, rect.localPosition.y, rect.localPosition.z);
            }
            else
            {
                rect.localPosition = new Vector3(maxX, rect.localPosition.y, rect.localPosition.z);
                movePanelIn = false;
            }
        }

        if (movePanelOut == true && movePanelIn == false)
        {
            compAct = Mathf.RoundToInt(rect.localPosition.x);
            compSpec = Mathf.RoundToInt(minX);
            if (compAct > compSpec) // this is asinine. Im not sure if its .NET or Unity but this is to account for floating point / rounding errors apparently
            {
                dX = Mathf.SmoothDamp(rect.localPosition.x, minX, ref vel2, speed);
                rect.localPosition = new Vector3(dX, rect.localPosition.y, rect.localPosition.z);
            }
            else
            {
                rect.localPosition = new Vector3(minX, rect.localPosition.y, rect.localPosition.z);
                movePanelOut = false;
            }
        }
    }

    public void SetMovePanelIn()
    {
        movePanelIn = true;
        movePanelOut = false;

        rect = GetComponent<RectTransform>();
        if (rect == null)
        {
            Debug.Log("Buy Panel Script couldnt get reference to its own rect transform");
        }
    }

    public void SetMovePanelOut()
    {
        movePanelIn = false;
        movePanelOut = true;

        rect = GetComponent<RectTransform>();
        if (rect == null)
        {
            Debug.Log("Buy Panel Script couldnt get reference to its own rect transform");
        }
    }

    public bool IsPanelMovedOut()
    {
        minX = Screen.width - 5;
        compAct = Mathf.RoundToInt(rect.localPosition.x);
        compSpec = Mathf.RoundToInt(minX);
        return (movePanelOut == false && compAct <= compSpec);
    }

    public bool IsPanelMovedIn()
    {
        maxX = 0;
        compAct = Mathf.RoundToInt(rect.localPosition.x);
        compSpec = Mathf.RoundToInt(maxX);
        return (movePanelOut == false && compAct >= compSpec);
    }
}
