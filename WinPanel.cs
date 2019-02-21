using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{

    public float speed = 0.4f;

    private PlayerController pScript;
    private LerpGameController gScript;
    private RectTransform rect;
    private float minX, maxX, dX;
    private float vel, vel2;
    private int compAct, compSpec;
    bool movePanelIn;
    bool movePanelOut;

    // Use this for initialization
    void Start()
    {
        minX = -Screen.width - 5;
        maxX = 0;

        pScript = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        if (pScript == null)
            Debug.Log("Win panel script could not get a reference to the player controller.");

        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<LerpGameController>();
        if (gScript == null)
            Debug.Log("Win Panel Script could not get a reference to the Game Controller.");

        rect = GetComponent<RectTransform>();
        if (rect == null)
        {
            Debug.Log("Win Panel Script couldnt get reference to its own rect transform");
        }
        else
        {
            rect.localPosition = new Vector3(minX, rect.localPosition.y, rect.localPosition.z);
        }

        movePanelIn = false;
        movePanelOut = false;
    }

    // Update is called once per frame
    void Update()
    {
        Image img = GetComponent<Image>();
        Color cA = pScript.GetCharColorA();
        Color cB = pScript.GetCharColorB();
        Color foo = Color.Lerp(cA, cB, pScript.GetLerpVal());
        foo.a = 255f;
        img.color = foo;

        minX = -Screen.width - 5; // not sure why, but some resolutions get screwy, so I have to over compensate

        if (movePanelIn == true)
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

        if (movePanelOut == true)
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
    }

    public void SetMovePanelOut()
    {
        movePanelIn = false;
        movePanelOut = true;

        /*
        rect = GetComponent<RectTransform>();
        if (rect == null)
        {
            Debug.Log("Win Panel Script couldnt get reference to its own rect transform");
        }
        else
        {
            rect.localPosition = new Vector3(minX, rect.localPosition.y, rect.localPosition.z);
        }
        */
    }

    public bool IsPanelMovedOut()
    {
        compAct = Mathf.RoundToInt(rect.localPosition.x);
        compSpec = Mathf.RoundToInt(minX);
        return (movePanelOut == false && compAct <= compSpec);
    }
}