using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotController : MonoBehaviour {

    public Color[] HomeColors;

    public SpriteRenderer sr;
    public Color TargetColor;

    public void Awake() {
        sr = GetComponent<SpriteRenderer>();
    }

    public void setColor(int homeID) {
        switch(homeID) {
            case 0:
                TargetColor = HomeColors[0];
                break;
            case 1:
                TargetColor = HomeColors[1];
                break;
            case 2:
                TargetColor = HomeColors[2];
                break;
            default:
                TargetColor = HomeColors[3];
                break;
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(sr.color != TargetColor) {
            sr.color = Color.Lerp(sr.color, TargetColor, Time.deltaTime * 3.0f);
        }
	}
}
