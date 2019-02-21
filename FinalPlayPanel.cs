using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalPlayPanel : MonoBehaviour {

    private GameObject actPanelObject;
    private WinPanel actPanel;
    private PlayButton playButton;
    private MenuButton menuB;
    private Image charSelectFinal;
    private Text charNameFinal;

    private void Awake()
    {
        //actPanelObject is the Child Game Object underneat the FinalPlayPanel
        actPanelObject = GameObject.FindGameObjectWithTag("uiFinalActualPanel");
        if (actPanelObject == null)
            Debug.Log("Final Play Panel script couldnt get reference to the Final Actual Panel Game Object");
        else
            actPanelObject.SetActive(true);

        actPanel = gameObject.GetComponentInChildren<WinPanel>();
        if (actPanel == null)
            Debug.Log("Final Play Panel script couldnt get reference to the Win Panel script for the Actual Panel");
        else
            actPanel.SetMovePanelOut();

        playButton = gameObject.GetComponentInChildren<PlayButton>();
        if (playButton == null)
            Debug.Log("FinalPlay Panel Script could not get reference to Play Button script");

        menuB = GameObject.FindGameObjectWithTag("MenuButton").GetComponent<MenuButton>();
        if (menuB == null)
            Debug.Log("FinalPLay Panel Script could not get reference to the Menu Button script.");

        charSelectFinal = GameObject.FindGameObjectWithTag("uiCharSelectedFinal").GetComponent<Image>();
        if (charSelectFinal == null)
            Debug.Log("Final Play script couldnt get reference to the ui element for Character Selected");

        charNameFinal = GameObject.FindGameObjectWithTag("uiCurrentNameFinal").GetComponent<Text>();
        if (charNameFinal == null)
            Debug.Log("Final Play button script could not get a reference to the Buy Panel Character Name Text component.");

    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        //wait for the Actual Final Panel to be moved out before disabling itself (because it holds the background fade color)
        if (actPanel.IsPanelMovedOut() == true)
            gameObject.SetActive(false);
	}

    public void OpenFinalPanel()
    {
        actPanelObject.SetActive(true);

        playButton.SetCharIndex(menuB.GetCharIndex());
        charSelectFinal.sprite = menuB.GetCharSprite();
        charNameFinal.text = menuB.GetCharName();

        actPanel.SetMovePanelIn();
    }

    public void CloseFinalPanel()
    {
        actPanel.SetMovePanelOut();
    }
}
