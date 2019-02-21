﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextButton : MonoBehaviour
{
    public AudioClip acClickOn;

    private Button button;
    private GameObject playPanel;
    private FinalPlayPanel fpPanel;
    private AudioSource audioOut;
    private PlayerController pScript;
    private Text[] bbText;

    void Awake()
    {
        playPanel = GameObject.FindGameObjectWithTag("uiFinalPlayPanel");
        if (playPanel == null)
            Debug.Log("Next Button couldnt get a reference to the FinalPlayPanel Object");
        else
        {
            fpPanel = playPanel.GetComponent<FinalPlayPanel>();
            if (fpPanel == null)
                Debug.Log("Next button couldnt get reference to the Final Play Panel Script");
        }
    }

    // Use this for initialization
    void Start()
    {
        

        bbText = new Text[2]; // should be only two...
        button = GetComponent<Button>();
        if (button == null)
            Debug.Log("Next Button script could not get a reference to the button sript.");
        else
        {
            button.onClick.AddListener(NextScreen);

            Text[] t = button.gameObject.GetComponentsInChildren<Text>();
            if (t != null)
                for (int i = 0; i < t.Length; i++)
                {
                    bbText[i] = t[i];
                    if (bbText[i] == null)
                        Debug.Log("Next Button couldnt get a reference to one of its child Text component.");
                }
        }

        audioOut = GetComponent<AudioSource>();
        if (audioOut == null)
            Debug.Log("Next Button couldnt get its Audio Source component.");

        pScript = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        if (pScript == null)
            Debug.Log("Next Button script could not get a reference to the Player controller.");

        //playPanel.SetActive(false);
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

    public void NextScreen()
    {
        audioOut.PlayOneShot(acClickOn, 0.8f);
        playPanel.SetActive(true);
        fpPanel.OpenFinalPanel();
    }
}
