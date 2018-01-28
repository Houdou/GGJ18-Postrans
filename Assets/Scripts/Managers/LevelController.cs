using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public enum OperationMode {
    [Description("Null")] Null = 0,
    [Description("Build Station")] BuildStation = 1,
    [Description("Build Road")] BuildRoad = 2,
    [Description("Upgrade Station")] UpgradeStation = 3,

}

public class LevelController : MonoBehaviour {
    [SerializeField]
    private OperationMode operationMode;
    public OperationMode OperationMode {
        get {
            return operationMode;
        }
        set {
            // Clear prevs binding
            switch(OperationMode) {
                default:
                case OperationMode.Null:
                    InputManager.Instance.OnClick -= StationSendMail;
                    break;

                case OperationMode.BuildStation:
                    InputManager.Instance.OnClick -= BuildStationOn;
                    break;

                case OperationMode.BuildRoad:
                    InputManager.Instance.OnPointerDown -= FindRoadStartNear;
                    InputManager.Instance.OnPointerUp -= FindRoadEndNear;
                    break;

                case OperationMode.UpgradeStation:
                    InputManager.Instance.OnClick -= StationUpgrade;
                    break;
            }
            // Update input binding
            switch(value) {
                default:
                case OperationMode.Null:
                    InputManager.Instance.OnClick += StationSendMail;
                    break;

                case OperationMode.BuildStation:
                    InputManager.Instance.OnClick += BuildStationOn;
                    break;

                case OperationMode.BuildRoad:
                    InputManager.Instance.OnPointerDown += FindRoadStartNear;
                    InputManager.Instance.OnPointerUp += FindRoadEndNear;
                    break;

                case OperationMode.UpgradeStation:
                    InputManager.Instance.OnClick += StationUpgrade;
                    break;
            }
            operationMode = value;

            GameMaster.Instance.Log("Current operation mode: " + operationMode);
        }
    }

    public GameObject RoadPrefab;
    public GameObject StationPrefab;
    public GameObject HomePrefab;
    public GameObject CloudPrefab;

    public Sprite[] Markers;
    public Sprite[] StationSprites;


    private GameObject StationGroup;
    private GameObject RoadGroup;
    private GameObject HomeGroup;

    // Input handling
    [SerializeField]
    private readonly float StationFindingThreshold = 1.0f;
    private Station startStation = null;

    public void FindRoadStartNear(Vector2 pos) {
        Station s = FindStationNear(pos);
        if(s != null) {
            startStation = s;
        }
    }
    public void FindRoadEndNear(Vector2 pos) {
        if(startStation == null) { return; }
        Station endStation = FindStationNear(pos);
        if(endStation != null) {
            // Build road between start and end.
            BuildRoadBetween(startStation, endStation);
        }
    }

    public void StationSendMail(Vector2 pos) {
        Station s = FindStationNear(pos);
        if(s != null) {
            s.ProcessMails();
        }
    }

    public void StationUpgrade(Vector2 pos) {
        Station s = FindStationNear(pos);
        if(s != null && !s.IsHome) {
            if(s.UpgradeStation()) {
                ParticleSystem Smoke = Instantiate(SmokePS, pos, Quaternion.identity);
                Smoke.Play();
            }

            UpdateIdleMailTargetStation();
        }
    }

    // Helper function
    public Station FindStationNear(Vector2 pos) {
        float dist = 99999.9f;
        Station s = null;
        foreach(var station in StationList) {
            float d = Vector2.Distance(pos, station.Pos);
            if(d < dist && d <= StationFindingThreshold) {
                s = station;
                dist = d;
            }
        }
        return s;
    }

    private void Awake() {
        StationGroup = new GameObject("StationGroup");
        RoadGroup = new GameObject("RoadGroup");
        HomeGroup = new GameObject("HomeGroup");

        RoadList = new List<Road>();
        StationList = new List<Station>();
        HomeList = new List<Station>();
        MailList = new List<Mail>();
    }

    // Object management
    public List<Road> RoadList;
    public List<Station> StationList;
    public List<Station> HomeList;

    public List<Mail> MailList;

    // Build
    // Station
    public void BuildStationOn(Vector2 pos) {
        bool validPos = true; //TODO: Add unbuildable area.
        validPos &= Mathf.Abs(pos.x) <= ViewRange.x / 2.0f;
        validPos &= Mathf.Abs(pos.y) <= ViewRange.y / 2.0f;

        // Check overlap
        foreach(var station in StationList) {
            validPos &= Vector2.Distance(pos, station.Pos) > 1.0f;
        }

        if(validPos) {
            Debug.Log("Build station on" + pos);
            ParticleSystem Smoke = Instantiate(SmokePS, pos, Quaternion.identity);
            Smoke.Play();

            GameObject newStation = Instantiate(StationPrefab, pos, Quaternion.identity, StationGroup.transform);
            StationController controller = newStation.GetComponent<StationController>();
            Station station = controller.Initialize(false);

            StationList.Add(station);

            //TODO: Manage animator
            UpdateIdleMailTargetStation();
        } else {
            //TODO: Build error
        }
    }

    // Road
    public void BuildRoadBetween(Station a, Station b) {
        bool valid =
            a != b
            && a.RoadList.Count < a.RoadLimits
            && b.RoadList.Count < b.RoadLimits
            && Vector2.Distance(a.Pos, b.Pos) < GameMaster.RoadMaxLength;

        if(!valid) { return; }

        // Check overlapping
        foreach(var road in a.RoadList) { valid &= road.Next(a) != b; }
        foreach(var road in b.RoadList) { valid &= road.Next(b) != a; }

        if(valid) {
            Vector2 pos = (a.Pos + b.Pos) / 2.0f;
            Debug.Log("Build road between " + a.ID + ", " + b.ID);

            GameObject newRoad = Instantiate(RoadPrefab, pos, Quaternion.identity, RoadGroup.transform);
            RoadController controller = newRoad.GetComponent<RoadController>();
            Road road = controller.Initialize();
            road.Connect(a, b);

            RoadList.Add(road);

            // Build road line renderer
            LineRenderer lr = newRoad.GetComponent<LineRenderer>();
            lr.useWorldSpace = true;

            Vector2 diffVec = b.Pos - a.Pos;
            if(diffVec.magnitude > 1.0f) {
                diffVec -= 0.5f * diffVec.normalized;
            }
            int segments = Mathf.Max(Mathf.CeilToInt(diffVec.magnitude / 0.15f), 18);
            lr.positionCount = segments + 1;
            for(int i = 0; i <= segments; i++) {
                Vector3 newPointPos = new Vector3(0.0f, 0.0f, -road.Height * Mathf.Sin(i * Mathf.PI / segments))
                    + i * (Vector3)diffVec / segments + (Vector3)a.Pos + 0.25f * (Vector3)diffVec.normalized;
                lr.SetPosition(i, newPointPos);
            }

            //TODO: Manage animator

            UpdateNavigation();
        } else {
            //TODO: Build error
            Debug.LogWarning("Road is invalid");
        }

    }

    // Cloud
    private void GenerateCloud() {
        Instantiate(CloudPrefab);
    }

    public ParticleSystem SmokePS;
    public ParticleSystem FirePS;


    // Game Loop
    private void Start() {
        InitializeLevel();

        Bound = new Vector2(MapObject.GetComponent<Renderer>().bounds.size.x / 2, MapObject.GetComponent<Renderer>().bounds.size.y / 2);

        //DEBUG
        CreateMailArea(new Vector2(1f, 1f), Bound);
        CreateMailArea(new Vector2(-3f, -3f), Bound);
        CreateMailArea(new Vector2(6f, -4f), Bound);

        GenerateCloud();
    }

    private float time = 0.0f;
    public float CloudGenerateTime;

    private void Update() {
        //DEBUG
        if(Input.GetKeyDown(KeyCode.Q)) {
            OperationMode = OperationMode.Null;
        }
        if(Input.GetKeyDown(KeyCode.W)) {
            OperationMode = OperationMode.BuildStation;
        }
        if(Input.GetKeyDown(KeyCode.E)) {
            OperationMode = OperationMode.BuildRoad;
        }
        if(Input.GetKeyDown(KeyCode.R)) {
            OperationMode = OperationMode.UpgradeStation;
        }
        //DEBUG

        time += Time.deltaTime;

        if (time >= CloudGenerateTime) {
            time -= CloudGenerateTime;
            GenerateCloud();
        }

        TimeCounting(Time.time);
        MoneyCounting(0);

    }

    public Vector2 ViewRange;
    private float StartLevelTime;

    public void InitializeLevel() {
        // Limit the game view range
        ViewRange = new Vector2(19.2f, 10.8f);

        // Create several initial Home station
        int InitHomeCount = 3;

        for(int i = 0; i < InitHomeCount; i++) {
            Vector2 pos = new Vector2(Random.Range(0.2f, 0.8f) * ViewRange.x, Random.Range(0.2f, 0.8f) * ViewRange.y);
            pos -= ViewRange / 2.0f;

            //TODO: Set a minimum distance between different home.
            int count = 0;
            float minDist = 9999.9f;
            do {
                foreach(var station in StationList) {
                    minDist = Mathf.Min(minDist, Vector2.Distance(pos, station.Pos));
                }
                pos = new Vector2(Random.Range(0.2f, 0.8f) * ViewRange.x, Random.Range(0.2f, 0.8f) * ViewRange.y);
                pos -= ViewRange / 2.0f;
            } while(minDist < GameMaster.MinimumHomeDistance && count++ < 100);

            GameObject newHome = Instantiate(HomePrefab, pos, Quaternion.identity, HomeGroup.transform);
            newHome.transform.Find("Marker").GetComponent<SpriteRenderer>().sprite = Markers[i];

            StationController controller = newHome.GetComponent<StationController>();
            controller.Initialize(true);

            StationList.Add(controller.model);
            HomeList.Add(controller.model);
        }

        StartLevelTime = Time.time;

    }

    public void UpdateNavigation() {
        int HomeCount = HomeList.Count;

        foreach(var station in StationList) {
            station.ResetNavigationDict(HomeList);
        }

        foreach(var home in HomeList) {
            home.UpdateNavigationDict(StationList);
        }
        
        //DEBUG
        //foreach(var station in StationList) {
        //    Vector3 startPos = station.Pos;
        //    foreach(var s in station.NavigationDict) {
        //        //Debug.Log("Station " + station.ID + "'s nav to " + s.Key.ID + " is ");
        //        Road possibleRoad = station.NavigationDict[s.Key];
        //        if(possibleRoad != null) {
        //            //Debug.Log();
        //            Vector3 endPos = possibleRoad.Next(station).Pos;
        //            Debug.DrawLine(startPos, (endPos - startPos).normalized + startPos, Color.red);
        //        }
        //    }
        //}
    }

    public GameObject MapObject;
    public GameObject MailArea;
    public List<GameObject> MailAreas = new List<GameObject>();

    public Vector2 Bound;

    public void CreateMailArea(Vector2 initPos, Vector2 bound) {
        GameObject area = Instantiate(MailArea);
        area.GetComponent<MailArea>().AreaInit(initPos, bound);

        MailAreas.Add(area);
    }

    public void UpdateIdleMailTargetStation() {
        foreach(var mail in MailList) {
            mail.UpdateTargetStation();
        }
    }

    // Game Functions
    public void BuildStationOp() {
        OperationMode = OperationMode.BuildStation;
    }
    public void BuildRoadOp() {
        OperationMode = OperationMode.BuildRoad;
    }
    public void UpgradeStationOp() {
        OperationMode = OperationMode.UpgradeStation;
    }
    public void SendMailOp() {
        OperationMode = OperationMode.Null;
    }

    public Text TimeLeftCounting;
    public float GameTotalTime;

    // Time Counting
    public void TimeCounting(float curTime) {
        float timePast = curTime - StartLevelTime;
        float timeLeft = GameTotalTime - timePast;

        if (timeLeft <= 0) {
            return;
        }

        TimeLeftCounting.text = TimeFormating(timeLeft);
    }

    private string TimeFormating(float time) {
        int minute = (int)(time / 60);
        int second = (int)(time % 60);

        string second_str = (second < 10) ? "0" + second : second.ToString();

        return (minute + ":" + second_str);
    }


    public Text MoneyLeftCounting;
    // Money Counting
    public void MoneyCounting(int sum) {
        MoneyLeftCounting.text = sum.ToString();
    }

}
