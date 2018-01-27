using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road {
    public static int IDCounter = -1;

    private GameObject road;
    public readonly int ID;

    public List<Station> stations;

    public Road(GameObject obj) {
        road = obj;
        ID = ++IDCounter;

        stations = new List<Station>();
    }

    public bool AddStation(Station station) {
        return true;
    }

    public bool RemoveStation(Station station) {
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

    public void Initialize() {
        isInitialized = true;
        model = new Road(gameObject);
    }

    void Start() {

    }
    
    void Update() {
        if(!isInitialized) { return; }


    }
}
