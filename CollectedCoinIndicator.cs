using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectedCoinIndicator : MonoBehaviour {
    private SpriteRenderer rend;
    private Vector3 pos;
    private Color col;
	// Use this for initialization
	void Start () {
        
        rend = GetComponent<SpriteRenderer>();
        if (rend == null)
            Debug.Log("Collected Coin Indicator script couldnt get a reference to its sprite renderer");
        
	}
	
	// Update is called once per frame
	void Update () {
        pos = transform.position;
        pos.y += Time.deltaTime;
        transform.position = pos;

        col = rend.color;
        col.a -= Time.deltaTime;
        rend.color = col;

        if (col.a <= 0.0f)
            GameObject.Destroy(gameObject);
    }
}
