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
    
    private void Awake() {
        StationGroup = new GameObject("StationGroup");
        RoadGroup = new GameObject("RoadGroup");
        HomeGroup = new GameObject("HomeGroup");

        RoadList = new List<Road>();
        StationList = new List<Station>();
    }

    // Input handling
    public void BuildStationOn(Vector2 pos) {
        Debug.Log("Build station on" + pos);
        Instantiate(StationPrefab, pos, Quaternion.identity, StationGroup.transform);
    }

    public readonly float StationFindingThreshold = 1.0f;
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

    public void BuildRoadBetween(Station a, Station b) {
        Vector2 pos = (a.Pos + b.Pos) / 2.0f;
        Debug.Log("Build road between " + a.ID + ", " + b.ID);
    }

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

    // Object management
    public List<Road> RoadList;
    public List<Station> StationList;
    
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

            GameObject newHome = Instantiate(HomePrefab, pos, Quaternion.identity, HomeGroup.transform);
            StationController controller = newHome.GetComponent<StationController>();
            controller.Initialize(true);

            StationList.Add(controller.model);
        }
    }
}
