using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station {
    public static int IDCounter = -1;
    
    private GameObject station;
    public readonly int ID;

    public Vector2 Pos {
        get {
            return station.transform.position;
        }
    }

    public readonly bool IsHome;
    public int RoadLimits {
        get {
            int limitedLevel = Mathf.Min(GameMaster.Instance.RoadCapacityPerLevel.Length - 1, level);
            return GameMaster.Instance.RoadCapacityPerLevel[limitedLevel];
        }
    }
    public float MailRange {
        get {
            int limitedLevel = Mathf.Min(GameMaster.Instance.MailRangePerLevel.Length - 1, level);
            return GameMaster.Instance.MailRangePerLevel[limitedLevel];
        }
    }


    public List<Road> RoadList;
    public Dictionary<Station, Road> NeighbourStationList;

    public int level { get; private set; }

    public Station(GameObject obj, bool isHome) {
        station = obj;        
        ID = ++IDCounter;
        IsHome = isHome;

        level = 0;
        
        RoadList = new List<Road>();
        NeighbourStationList = new Dictionary<Station, Road>();
        NavigationDict = new Dictionary<Station, Road>();
        DistanceDict = new Dictionary<Station, float>();
        PrevStationDict = new Dictionary<Station, Station>();
    }

    public bool AddRoad(Road road) {
        if(RoadList.Count < RoadLimits) {
            RoadList.Add(road);
            NeighbourStationList.Add(road.Next(this), road);
            return true;
        } else {
            return false;
        }
    }

    public bool RemoveRoad(Road road) {
        return true;
    }

    public float DistanceTo(Station station) {
        if(NeighbourStationList.ContainsKey(station)) {
            return NeighbourStationList[station].Length;
        } else {
            return Mathf.Infinity;
        }
    }

    // Navigation
    public Dictionary<Station, Road> NavigationDict;
    public Dictionary<Station, float> DistanceDict;
    public Dictionary<Station, Station> PrevStationDict;
    public bool searchMark = false;

    public void ResetNavigationDict(List<Station> homeList) {
        // Reset distance
        foreach(var home in homeList) {
            DistanceDict[home] = Mathf.Infinity;
            NavigationDict[home] = null;
        }
    }

    public void UpdateNavigationDict(List<Station> stationList) {
        // Reset mark
        PrevStationDict.Clear();
        foreach(var station in stationList) {
            PrevStationDict[station] = null;

            station.searchMark = false;
        }

        // Mark self
        PrevStationDict[this] = this;
        NavigationDict[this] = null;
        DistanceDict[this] = 0.0f;
        searchMark = true;

        Queue<Station> searchList = new Queue<Station>();
        foreach(var station in NeighbourStationList.Keys) {
            searchList.Enqueue(station);
            PrevStationDict[station] = this;
        }

        while(searchList.Count > 0) {
            var s = searchList.Dequeue();
            if(PrevStationDict[s].DistanceTo(s) + PrevStationDict[s].DistanceDict[this] < s.DistanceDict[this]) {
                s.NavigationDict[this] = s.NeighbourStationList[PrevStationDict[s]];
                s.DistanceDict[this] = PrevStationDict[s].DistanceTo(s) + PrevStationDict[s].DistanceDict[this];
            }
            s.searchMark = true;

            foreach(var station in s.NeighbourStationList.Keys) {
                if(!station.searchMark) {
                    searchList.Enqueue(station);
                    PrevStationDict[station] = s;
                }
            }
        }
    }

    public Road GetNavigation(Station station) {
        if(!station.IsHome) { return null; }

        if(NavigationDict.ContainsKey(station)) {
            return NavigationDict[station];
        } else {
            return null;
        }
    }
}

public class StationController : MonoBehaviour {
    public Station model;
    public SpriteRenderer sprite;

    public bool isInitialized;
    public int ID;
    
    private void Awake() {
        sprite = GetComponent<SpriteRenderer>();
    }

    public Station Initialize(bool isHome) {
        isInitialized = true;
        model = new Station(gameObject, isHome);
        ID = model.ID;

        return model;
    }

    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(!isInitialized) { return; }


    }
}