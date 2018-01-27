using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum OperationMode {
    [Description("Null")] Null = 0,
    [Description("Build Station")] BuildStation = 1,
    [Description("Build Road")] BuildRoad = 2,

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
                    break;

                case OperationMode.BuildStation:
                    InputManager.Instance.OnClick -= BuildStationOn;
                    break;

                case OperationMode.BuildRoad:
                    InputManager.Instance.OnPointerDown -= FindRoadStartNear;
                    InputManager.Instance.OnPointerUp -= FindRoadEndNear;
                    break;
            }
            // Update input binding
            switch(value) {
                default:
                case OperationMode.Null:
                    break;

                case OperationMode.BuildStation:
                    InputManager.Instance.OnClick += BuildStationOn;
                    break;

                case OperationMode.BuildRoad:
                    InputManager.Instance.OnPointerDown += FindRoadStartNear;
                    InputManager.Instance.OnPointerUp += FindRoadEndNear;
                    break;
            }
            operationMode = value;

            GameMaster.Instance.Log("Current operation mode: " + operationMode);
        }
    }

    public GameObject RoadPrefab;
    public GameObject StationPrefab;
    public GameObject HomePrefab;

    private GameObject StationGroup;
    private GameObject RoadGroup;
    private GameObject HomeGroup;

    [SerializeField]
    private readonly float MinimumHomeDistance = 5.0f;

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
    }

    // Object management
    public List<Road> RoadList;
    public List<Station> StationList;
    // Build
    // Station
    public void BuildStationOn(Vector2 pos) {
        bool validPos = true; //TODO: Add unbuildable area.
        validPos &= Mathf.Abs(pos.x) <= ViewRange.x / 2.0f;
        validPos &= Mathf.Abs(pos.y) <= ViewRange.y / 2.0f;
        if(validPos) {
            Debug.Log("Build station on" + pos);
            GameObject newStation = Instantiate(StationPrefab, pos, Quaternion.identity, StationGroup.transform);
            StationController controller = newStation.GetComponent<StationController>();
            Station station = controller.Initialize(false);

            StationList.Add(station);

            //TODO: Manage animator
        } else {
            //TODO: Build error
        }
    }

    // Road
    public void BuildRoadBetween(Station a, Station b) {
        float dist = Vector2.Distance(a.Pos, b.Pos);
        if(dist <= GameMaster.RoadMaxLength) {
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
            int segments = Mathf.Max(Mathf.CeilToInt(diffVec.magnitude / 0.2f), 12);
            float height = Mathf.Min(2.4f, Mathf.Ceil(segments / 12.0f));
            lr.positionCount = segments + 1;
            for(int i = 0; i <= segments; i++) {
                Vector3 newPointPos = new Vector3(0.0f, 0.0f, -height * Mathf.Sin(i * Mathf.PI / segments))
                    + i * (Vector3)diffVec / segments + (Vector3)a.Pos + 0.25f * (Vector3)diffVec.normalized;
                lr.SetPosition(i, newPointPos);
            }


            //TODO: Manage animator
        } else {
            //TODO: Build error
            Debug.LogWarning("Road is too long");
        }

    }

    private void Start() {
        InitializeLevel();
    }

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
        //DEBUG
    }

    public Vector2 ViewRange;

    public void InitializeLevel() {
        // Limit the game view range
        ViewRange = new Vector2(19.2f, 10.8f);

        // Create several initial Home station
        int InitHomeCount = 3;

        for(int i = 0; i < InitHomeCount; i++) {
            Vector2 pos = new Vector2(Random.Range(0.2f, 0.8f) * ViewRange.x, Random.Range(0.2f, 0.8f) * ViewRange.y);
            pos -= ViewRange / 2.0f;

            //TODO: Set a minimum distance between different home.
            foreach(var station in StationList) {
                while(Vector2.Distance(pos, station.Pos) < MinimumHomeDistance) {
                    pos = new Vector2(Random.Range(0.2f, 0.8f) * ViewRange.x, Random.Range(0.2f, 0.8f) * ViewRange.y);
                    pos -= ViewRange / 2.0f;
                }
            }

            GameObject newHome = Instantiate(HomePrefab, pos, Quaternion.identity, HomeGroup.transform);
            StationController controller = newHome.GetComponent<StationController>();
            controller.Initialize(true);

            StationList.Add(controller.model);
        }
    }
}
