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
                    

                    break;
            }
            operationMode = value;

            GameMaster.Instance.Log("Current operation mode: " + operationMode);
        }
    }

    private void Awake() {
        
    }

    // Input handling
    public void BuildStationOn(Vector2 pos) {
        Debug.Log("Build station on" + pos);
    }

    // Object management
    public List<Road> RoadList;
    public List<Station> StationList;

    private void Update () {
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
}
