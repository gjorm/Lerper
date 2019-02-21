using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressIndicator : MonoBehaviour {

    private LerpGameController gScript;
    private PlayerController pScript;
    private RectTransform sideBar;
    private RectTransform progR;
    private CanvasScaler canScale;
    private Image img;
    private float maxY, pAtY, percY, finalY, scale;
    private Vector3 pos;

	// Use this for initialization
	void Start () {

        gScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<LerpGameController>();
        if (gScript == null)
            Debug.Log("Progress Indicator couldnt get reference to the Game Controller");

        pScript = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        if (pScript == null)
            Debug.Log("Progress indicator couldnt get a reference to the Player controller");

        img = GetComponent<Image>();
        if (img == null)
            Debug.Log("Progress indicator couldnt get a reference to its own Image.");

        sideBar = GameObject.FindGameObjectWithTag("uiSideCanvas").GetComponent<RectTransform>();
        if (sideBar == null)
            Debug.Log("Progress indicator couldnt get a reference to the side canvas rect");

        canScale = GameObject.FindGameObjectWithTag("uiSideCanvas").GetComponent<CanvasScaler>();
        if (canScale == null)
            Debug.Log("Progress indicator couldnt get a reference to the side canvas scaler");

        progR = GetComponent<RectTransform>();
        if (progR == null)
            Debug.Log("Progress indicator couldnt get a reference to its own rect transform.");

    }
	
	// Update is called once per frame
	void Update () {
        maxY = gScript.GetNumBlocks().y - 4; // 4 is how far below the trophy is actually placed.
        pAtY = pScript.GetPlayerPosition().y;
        scale = sideBar.rect.height - (Screen.height * 0.05f);

        percY = pAtY / maxY;

        finalY = (percY * scale) - (scale / 2.0f);

        pos = progR.localPosition;
        pos.y = finalY;
        progR.localPosition = pos;
	}

    public void UpdateImage(Sprite s)
    {
        img.sprite = s;
    }
}
