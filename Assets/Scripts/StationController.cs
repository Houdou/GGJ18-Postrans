using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Station {
    private GameObject station;

    public readonly bool IsHome;

    public List<Road> roads;

    public Station(GameObject obj) {
        station = obj;

        roads = new List<Road>();
    }

    public bool AddRoad(Road road) {
        return true;
    }

    public bool RemoveRoad(Road road) {
        return true;
    }


}

public class StationController : MonoBehaviour {

    public SpriteRenderer sprite;

    public bool isInitialized;

    private void Awake() {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Initialize() {
        isInitialized = true;
    }

    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(!isInitialized) { return; }


    }
}