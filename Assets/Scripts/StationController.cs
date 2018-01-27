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

    public bool IsAvailable;
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
    public int MailStorageLimit {
        get {
            int limitedLevel = Mathf.Min(GameMaster.Instance.MailStoragePerLevel.Length - 1, level);
            return GameMaster.Instance.MailStoragePerLevel[limitedLevel];
        }
    }
    public float MailSpeed {
        get {
            int limitedLevel = Mathf.Min(GameMaster.Instance.MailSpeedPerLevel.Length - 1, level);
            return GameMaster.Instance.MailSpeedPerLevel[limitedLevel];
        }
    }

    public int level { get; private set; }
    
    public bool UpgradeStation() {
        if(level < GameMaster.MaxStationLevel) {
            level += 1;
            station.GetComponent<StationController>().UpgradeStation(level);
            return true;
        } else {
            return false;
        }
    }

    public Station(GameObject obj, bool isHome) {
        station = obj;        
        ID = ++IDCounter;
        IsHome = isHome;

        if(IsHome) {
            IsAvailable = true;
        }

        level = 0;
        
        RoadList = new List<Road>();
        NeighbourStationList = new Dictionary<Station, Road>();
        NavigationDict = new Dictionary<Station, Road>();
        DistanceDict = new Dictionary<Station, float>();
        PrevStationDict = new Dictionary<Station, Station>();

        SendingQueue = new Queue<Mail>();
        MailStorage = new Queue<Mail>();
        OverflowMailQueue = new Queue<Mail>();
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
        return RoadList.Remove(road);
    }

    public float DistanceTo(Station station) {
        if(NeighbourStationList.ContainsKey(station)) {
            return NeighbourStationList[station].Length;
        } else {
            return Mathf.Infinity;
        }
    }

    // Navigation
    public List<Road> RoadList;
    public Dictionary<Station, Road> NeighbourStationList;
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
            if(!station.searchMark) {
                searchList.Enqueue(station);
                PrevStationDict[station] = this;
            }
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

    // Mail Storage
    public Queue<Mail> SendingQueue;
    public Queue<Mail> MailStorage;
    public Queue<Mail> OverflowMailQueue;

    public bool AddMail(Mail mail) {
        if(MailStorage.Contains(mail) || OverflowMailQueue.Contains(mail)) { return false; }

        if(MailStorage.Count < MailStorageLimit) {
            MailStorage.Enqueue(mail);

            // TODO: Mail animation
            return true;
        } else {
            OverflowMailQueue.Enqueue(mail);
            return false;
            // TODO: Mail queueing;
        }
    }

    public void ProcessMails() {
        // Send all mail
        while(MailStorage.Count > 0) {
            Mail mail = MailStorage.Dequeue();

            if(NavigationDict[mail.TargetHome] == null) {
                // Target not accessible
                OverflowMailQueue.Enqueue(mail);
            } else {
                SendingQueue.Enqueue(mail);
            }
        }

        // Collect outsider
        while(OverflowMailQueue.Count > 0 && MailStorage.Count < MailStorageLimit) {
            Mail mail = OverflowMailQueue.Dequeue();
            mail.FadeOut();
            MailStorage.Enqueue(mail);
        }
    }

    public void SendMail() {
        if(SendingQueue.Count > 0) {
            var mail = SendingQueue.Dequeue();
            mail.MoveTowards(this, NavigationDict[mail.TargetHome], MailSpeed);
        }
    }
}

public class StationController : MonoBehaviour {
    public Station model;
    public SpriteRenderer sprite;

    public Color TargetColor;

    public bool isInitialized;
    public int ID;
    
    private void Awake() {
        sprite = GetComponent<SpriteRenderer>();
    }

    public Station Initialize(bool isHome) {
        isInitialized = true;
        model = new Station(gameObject, isHome);
        ID = model.ID;

        if(!isHome) {
            TargetColor = sprite.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            Invoke("SetFadeIn", 1.0f);
        } else {
            TargetColor = Color.white;
        }

        return model;
    }

    public float SendingCounter = 0.0f;
    public float SendingInterval = 0.12f;

    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(!isInitialized) { return; }

        if(sprite.color != TargetColor) {
            sprite.color = Color.Lerp(sprite.color, TargetColor, Time.deltaTime * 2.0f);
        }

        if(model.SendingQueue.Count > 0) {
            SendingCounter += Time.deltaTime;
            if(SendingCounter > SendingInterval) {
                SendingCounter -= SendingInterval;
                model.SendMail();
                if(model.SendingQueue.Count == 0) {
                    SendingCounter = 0.0f;
                }
            }
        }
    }

    public void SetFadeIn() {
        TargetColor = Color.white;
        model.IsAvailable = true;
        GameMaster.Instance.GetLevelController().UpdateIdleMailTargetStation();
    }

    public void UpgradeStation(int level) {
        // TODO: Animate effects;
        GetComponent<SpriteRenderer>().sprite = GameMaster.Instance.GetLevelController().StationSprites[level];
    }
}