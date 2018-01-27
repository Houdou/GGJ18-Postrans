using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    #region Singleton
    private static InputManager _instance;
    private static object _lock = new object();

    public static InputManager Instance {
        get {
            if(applicationIsQuitting) {
                Debug.LogWarning("[Singleton] Instance '" + typeof(InputManager) +
                                 "' already destroyed on application quit. Returning null.");
                return null;
            }

            lock(_lock) {
                if(_instance == null) {
                    _instance = (InputManager)FindObjectOfType(typeof(InputManager));

                    if(_instance == null) {
                        GameObject singleton = new GameObject("GameMaster");
                        _instance = singleton.AddComponent<InputManager>();

                        DontDestroyOnLoad(singleton);

                        Debug.Log("[Singleton] Instance '" + typeof(InputManager) +
                                  "' is created.");
                    }
                }

                return _instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;

    public void OnDestroy() {
        applicationIsQuitting = true;
    }
    #endregion

    #region Events
    public event Action<Vector2> OnClick;
    public event Action<Vector2> OnPointerDown;
    public event Action<Vector2> OnPointerUp;
    #endregion

    private void Awake() {
        
    }
    
    public bool IsPointerDown { get; private set; }
    private float PointerDownTimestamp;
    public Vector2 PointerDownPos;

    public readonly float ClickDeltaTimeThreshold = 0.4f;
    public readonly float ClickDeltaPosThreshold = 0.5f;

    private void Update() {
        //TODO: Platform dependent pointer

        if(Input.GetMouseButtonDown(0) && Input.GetMouseButton(0)) {
            if(OnPointerDown != null) {
                OnPointerDown(MousePos);
            }
            IsPointerDown = true;
            PointerDownTimestamp = Time.time;
            PointerDownPos = MousePos;
        }
        if(Input.GetMouseButtonUp(0) && IsPointerDown) {
            if(OnPointerUp != null) {
                OnPointerUp(MousePos);
            }
            if(Time.time - PointerDownTimestamp <= ClickDeltaTimeThreshold
                && Vector2.Distance(PointerDownPos, MousePos) <= ClickDeltaPosThreshold) {
                if(OnClick != null) {
                    OnClick(MousePos);
                }
            }
            IsPointerDown = false;
        }
    }    

    public Vector2 MousePos {
        get {
            return Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }
    }
}
