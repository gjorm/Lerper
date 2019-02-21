using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaloScript : MonoBehaviour {

    private PlayerController pScript;
    private Vector3 position;
    private Vector2 pos2d;
    public float yVal = 0.58f;
    public bool isDragWarn = false;
    public float scale = 0.75f;
    public bool isRightArrow = false;
    public bool isLeftArrow = false;

    private float whoa;

	// Use this for initialization
	void Start () {
        pScript = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        if (pScript == null)
            Debug.Log("Halo Script couldnt get reference to the Player Controller");
        position = new Vector3(0, yVal, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if(pScript.IsAlive() || pScript.WithinPlayField())
        {
            //translate accoroding to player position
            pos2d = pScript.GetPlayerPosition();
            position.x = pos2d.x;
            position.y = yVal; // maybe unnecessary
            position.z = pos2d.y;

            //
            if(isDragWarn == true || isLeftArrow == true || isRightArrow == true)
            {
                position.y = pScript.GetPlayerYVal() + yVal + 1.5f;
            }
            

            if(isDragWarn == true)
            {
                whoa = scale + (Mathf.Sin(Time.realtimeSinceStartup * 15) * 0.08f);
                gameObject.transform.localScale = new Vector3(whoa, whoa, whoa);
            }

            if(isLeftArrow == true)
            {
                position.x -= 0.95f;
            }

            if(isRightArrow == true)
            {
                position.x += 0.95f;
            }

            gameObject.transform.position = position;
        }
	}

    public void InitHalo(float distance)
    {
        //scale according to player character distance property
        float radius = distance;
        radius *= 2f;

        if(isDragWarn == true || isRightArrow == true || isLeftArrow == true)
        {
            gameObject.transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            gameObject.transform.localScale = new Vector3(radius, radius, radius);
        }
    }
}
