using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road {
    public static int IDCounter = -1;

    private GameObject road;
    public readonly int ID;

    public List<Station> StationList;

    public float Length = -1.0f;
    public float Height = -1.0f;
    public Vector2 DiffVec = Vector2.zero;
    public bool IsConnected = false;

    public Road(GameObject obj) {
        road = obj;
        ID = ++IDCounter;

        StationList = new List<Station>();
    }

    public bool Connect(Station a, Station b) {
        if(IsConnected) { return false; }

        StationList.Add(a);
        StationList.Add(b);
        IsConnected = true;

        a.AddRoad(this);
        b.AddRoad(this);
        
        Vector2 diffVec = b.Pos - a.Pos;
        DiffVec = diffVec;

        if(diffVec.magnitude > 1.0f) { diffVec -= 0.5f * diffVec.normalized; }
        Height = Mathf.Min(2.4f, Mathf.Max(Mathf.CeilToInt(diffVec.magnitude / 2.4f), 1));
        Length = diffVec.magnitude;

        IsConnected = true;
        return true;
    }

    public bool Disconnect() {
        IsConnected = false;
        StationList.Clear();
        return true;
    }

    public float GetRoadProgress(Vector2 pos) {
        return Mathf.Clamp01(Vector2.Dot((pos - StationList[0].Pos).normalized, DiffVec.normalized));
    }

    public Vector3 GetCurvePos(Station to, Vector2 pos, float nextStep = 0.0f) {
        if(!StationList.Contains(to)) { return (Vector3)pos;  }
        
        float progress = Mathf.Clamp01(Vector2.Dot((pos - StationList[0].Pos).normalized, DiffVec.normalized));
        float targetProgress = Mathf.Clamp01(Vector2.Dot((to.Pos - StationList[0].Pos).normalized, DiffVec.normalized));
        
        nextStep *= Mathf.Sign(targetProgress - progress);

        return GetCurvePosByProgress(progress + nextStep);
    }

    public Vector3 GetCurvePosByProgress(float progress) {
        return new Vector3(0.0f, 0.0f, -Height * Mathf.Sin(Mathf.Clamp01(progress) * Mathf.PI))
            + (Vector3)StationList[0].Pos + (Vector3)DiffVec.normalized * 0.25f
            + Mathf.Clamp01(progress) * (Vector3)DiffVec;
    }

    // Navigation
    public Station Next(Station from) {
        if(IsConnected) {
            if(from == StationList[0]) { return StationList[1]; }
            if(from == StationList[1]) { return StationList[0]; }
        }
        return null;
    }
}

public class RoadController : MonoBehaviour {
    public Road model;
    public SpriteRenderer sprite;

    public bool isInitialized;
    public int ID;

    private void Awake() {
        sprite = GetComponent<SpriteRenderer>();
    }

    public Road Initialize() {
        isInitialized = true;
        model = new Road(gameObject);
        ID = model.ID;

        return model;
    }

    void Start() {

    }
    
    void Update() {
        if(!isInitialized) { return; }


    }
}
