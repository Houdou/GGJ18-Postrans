using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station {
    public static int IDCounter = -1;
    
    private GameObject station;
    private StationController controller;
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
    public float OrbitRadius {
        get {
            int limitedLevel = Mathf.Min(GameMaster.Instance.OrbitRadiusPerLevel.Length - 1, level);
            return GameMaster.Instance.OrbitRadiusPerLevel[limitedLevel];
        }
    }
    public int MailStorageLimit {
        get {
            int limitedLevel = Mathf.Min(GameMaster.Instance.MailStoragePerLevel.Length - 1, level);
            return GameMaster.Instance.MailStoragePerLevel[limitedLevel];
        }
    }
    public Vector3 StorageIndicatorOffset {
        get {
            int limitedLevel = Mathf.Min(GameMaster.Instance.SotrageIndicatorOffsetPerLevel.Length - 1, level);
            return GameMaster.Instance.SotrageIndicatorOffsetPerLevel[limitedLevel];
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
            controller.UpgradeStation(level);
            return true;
        } else {
            return false;
        }
    }

    public Station(GameObject obj, bool isHome) {
        station = obj;
        controller = station.GetComponent<StationController>();
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
        if(station == this) {
            return 0.0f;
        }

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
                station.searchMark = true;
                PrevStationDict[station] = this;
            }
        }

        while(searchList.Count > 0) {
            var s = searchList.Dequeue();
            float newDist = PrevStationDict[s].DistanceTo(s) + PrevStationDict[s].DistanceDict[this];
            if(newDist < s.DistanceDict[this]) {
                // Update pointer to this;
                s.NavigationDict[this] = s.NeighbourStationList[PrevStationDict[s]];
                s.DistanceDict[this] = newDist;
            }

            foreach(var station in s.NeighbourStationList.Keys) {
                if(!station.searchMark) {
                    searchList.Enqueue(station);
                    station.searchMark = true;
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
            controller.UpdateIndicator(MailStorage);
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
        controller.UpdateIndicator(MailStorage);
    }

    public void FillingMails() {
        // Collect outsider
        if(OverflowMailQueue.Count > 0 && MailStorage.Count < MailStorageLimit) {
            Mail mail = OverflowMailQueue.Dequeue();
            if(mail.IsTravelling)
                mail.FadeOut();
            MailStorage.Enqueue(mail);
        }
        controller.UpdateIndicator(MailStorage);
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
    public StorageIndicator si;

    public Color TargetColor;

    public bool isInitialized;
    public int ID;
    
    private void Awake() {
        sprite = GetComponent<SpriteRenderer>();
        si = GetComponent<StorageIndicator>();
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

        si.ResetCapacity(model.MailStorageLimit, isHome ? new Vector3(-0.75f, -1f, 0.0f) : model.StorageIndicatorOffset);

        return model;
    }

    public float FillingCounter = 0.0f;
    public float FillingInterval = 0.3f;
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

        if(model.OverflowMailQueue.Count > 0) {
            FillingCounter += Time.deltaTime;
            if(FillingCounter > FillingInterval) {
                FillingCounter -= FillingInterval;
                model.FillingMails();
            }
        }

        if(model.SendingQueue.Count > 0) {
            SendingCounter += Time.deltaTime;
            if(SendingCounter > SendingInterval) {
                SendingCounter -= SendingInterval;
                model.SendMail();
                if(model.SendingQueue.Count == 0) {
                    SendingCounter = 0.0f;
                    UpdateIndicator(model.MailStorage);
                }
            }
        }
    }

    public void SetFadeIn() {
        TargetColor = Color.white;
        model.IsAvailable = true;
        GameMaster.Instance.GetLevelController().UpdateIdleMailTargetStation();
    }

    public void UpdateIndicator(Queue<Mail> mails) {
        si.UpdateColor(mails);
    }

    public void UpgradeStation(int level) {
        // TODO: Animate effects;
        GetComponent<SpriteRenderer>().sprite = GameMaster.Instance.GetLevelController().StationSprites[level];

        si.ResetCapacity(model.MailStorageLimit, model.IsHome ? new Vector3(-0.75f, -1f, 0.0f) : model.StorageIndicatorOffset);
    }
}