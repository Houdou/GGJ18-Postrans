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

        IsConnected = valid;

        return valid;
    }

    public bool Disconnect() {
        return true;
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
