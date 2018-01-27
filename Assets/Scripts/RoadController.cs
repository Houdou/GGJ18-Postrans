﻿using System;
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
        bool valid = true;

        valid &= a.AddRoad(this);
        valid &= b.AddRoad(this);

        Length = Vector2.Distance(a.Pos, b.Pos);
        valid &= (Length <= GameMaster.RoadMaxLength);
        
        Vector2 diffVec = b.Pos - a.Pos;
        DiffVec = diffVec;

        if(diffVec.magnitude > 1.0f) { diffVec -= 0.5f * diffVec.normalized; }
        Height = Mathf.Min(2.4f, Mathf.Max(Mathf.CeilToInt(diffVec.magnitude / 2.4f), 1));
        

        IsConnected = valid;

        return valid;
    }

    public bool Disconnect() {
        return true;
    }

    public Vector3 GetCurvePos(Vector2 pos, float nextStep = 0.0f) {
        float progress = Mathf.Clamp01(Vector2.Dot(pos - StationList[0].Pos, DiffVec) / DiffVec.magnitude);
        return GetCurvePosByProgress(progress + nextStep);
    }

    public Vector3 GetCurvePosByProgress(float progress) {
        return new Vector3(0.0f, 0.0f, -Height * Mathf.Sin(Mathf.Clamp01(progress) * Mathf.PI)) + (Vector3)StationList[0].Pos;
    }
}

public class RoadController : MonoBehaviour {
    public Road model;
    public SpriteRenderer sprite;

    public bool isInitialized;

    private void Awake() {
        sprite = GetComponent<SpriteRenderer>();
    }

    public Road Initialize() {
        isInitialized = true;
        model = new Road(gameObject);

        return model;
    }

    void Start() {

    }
    
    void Update() {
        if(!isInitialized) { return; }


    }
}
