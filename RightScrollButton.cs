using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightScrollButton : MonoBehaviour {

    public AudioClip acClickOn;
    private PlayerController pScript;
    private GameObject scrollView;
    private Button button;
    private AudioSource audioOut;
    private ScrollRect scrollRect;
    private Text charCost;
    private BuyButton buyButton;
    private MenuButton menuButton;

    // Use this for initialization
    void Start()
    {
        scrollView = GameObject.FindGameObjectWithTag("ScrollView");
        if (scrollView == null)
            Debug.Log("Right Scroll button could not get the scrollview reference.");
        else
            scrollRect = scrollView.GetComponent<ScrollRect>();

        if (scrollRect == null)
        {
            Debug.Log("Right Scroll button could not get the scroll rect reference.");
        }

        button = GetComponent<Button>();
        if (button == null)
            Debug.Log("Right Scroll button script could not get a reference to the button sript.");
        else
            button.onClick.AddListener(ClickRight);

        audioOut = GetComponent<AudioSource>();
        if (audioOut == null)
            Debug.Log("Right Scroll button couldnt get its Audio Source component.");

        charCost = GameObject.FindGameObjectWithTag("uiCharCostField").GetComponent<Text>();
        if (charCost == null)
            Debug.Log("Right Scroll button script could not get a reference to the Character Cost Text component.");

        pScript = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        if (pScript == null)
            Debug.Log("Right Scroll script could not get a reference to the player controller.");

        buyButton = GameObject.FindGameObjectWithTag("uiBuyButton").GetComponent<BuyButton>();
        if (buyButton == null)
        {
            Debug.Log("Right Scroll couldnt get a reference to the Buy Button script.");
        }

        menuButton = GameObject.FindGameObjectWithTag("MenuButton").GetComponent<MenuButton>();
        if (menuButton == null)
        {
            Debug.Log("Reft Scroll button couldnt get a reference to the menu Button script.");
        }
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

    public void ClickRight()
    {
        float move = scrollRect.horizontalNormalizedPosition;
        if (move < 1)
        {
            audioOut.PlayOneShot(acClickOn, 0.8f);
            scrollRect.inertia = true;

            scrollRect.velocity = new Vector2(0.3f, 0);

            //charCost.text = "";
        }

        Button btn = buyButton.GetComponentInParent<Button>();
        btn.enabled = false;
        buyButton.SetWiggle(false);
        buyButton.SetCharIndex(0, null);

        //menuButton.ClearCharFields();
    }

    public void StopScrolling()
    {
        scrollRect.velocity = new Vector2(0, 0);
    }
}

