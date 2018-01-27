using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCameraController : MonoBehaviour {

    public float DeltaZoom = 0.0f;

    public float StartTime;

    public float ZoomingTime = 0.0f;
    private float SmoothFactor = 1.0f;

    public Vector3 CameraZoomPos {
        get {
            DeltaZoom = Mathf.Clamp01(DeltaZoom);
            return new Vector3(0.0f, -1.8f + DeltaZoom * -1.0f, -13f + DeltaZoom * -5f);
        }
    }


	// Use this for initialization
	void Start () {
        ResetZoom();
	}
	
	// Update is called once per frame
	void Update () {
        DeltaZoom = (Time.time - StartTime - ZoomingTime) / 240.0f;
        transform.position = Vector3.Lerp(transform.position, CameraZoomPos, Time.deltaTime * SmoothFactor);

    }

    public void ResetZoom() {
        StartTime = Time.time;
    }
}
