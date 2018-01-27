using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Road {
    private GameObject road;

    public List<Road> stations;

    public Road(GameObject obj) {
        road = obj;

        stations = new List<Road>();
    }

    public bool AddStation(Road road) {
        return true;
    }

    public bool RemoveStation(Road road) {
        return true;
    }


}

public class RoadController : MonoBehaviour {

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
    
    void Update() {
        if(!isInitialized) { return; }


    }
}
