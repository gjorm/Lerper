using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyMobile;

public class ShareButton : MonoBehaviour {

    private Button button;
    private MenuButton menuB;
    private PlayerController pScript;

    // Use this for initialization
    void Start()
    {
        button = GetComponent<Button>();
        if (button == null)
            Debug.Log("Share button couldnt get a reference to its button component");
        else
            button.onClick.AddListener(Share);

        menuB = GameObject.FindGameObjectWithTag("MenuButton").GetComponent<MenuButton>();
        if (menuB == null)
            Debug.Log("Share Button couldnt get reference to the menu button");

        pScript = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        if (pScript == null)
            Debug.Log("Share Button script could not get a reference to the player controller.");
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

    public void Share()
    {
        IEnumerator co = ScreenShot();
        StartCoroutine(co);
    }

    IEnumerator ScreenShot()
    {

        yield return new WaitForEndOfFrame();

        Texture2D pic = MobileNativeShare.CaptureScreenshot();

        if (pic == null)
            Debug.Log("Texture from screenshot not successful.");

        MobileNativeShare.ShareTexture2D(pic, "LerperScreen", "", "");
    }
}
