using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameMaster : MonoBehaviour {

    #region Singleton
    private static GameMaster _instance;
    private static object _lock = new object();

    public static GameMaster Instance
    {
        get
        {
            if(applicationIsQuitting) {
                Debug.LogWarning("[Singleton] Instance '" + typeof(GameMaster) +
                                 "' already destroyed on application quit. Returning null.");
                return null;
            }

            lock(_lock) {
                if(_instance == null) {
                    _instance = (GameMaster)FindObjectOfType(typeof(GameMaster));

                    if(_instance == null) {
                        GameObject singleton = new GameObject("GameMaster");
                        _instance = singleton.AddComponent<GameMaster>();

                        DontDestroyOnLoad(singleton);

                        Debug.Log("[Singleton] Instance '" + typeof(GameMaster) +
                                  "' is created.");
                    }
                }

                return _instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;

    public void OnDestroy()
    {
        applicationIsQuitting = true;
    }
    #endregion

    public static readonly float RoadMaxLength = 7.0f;
    public int[] RoadCapacityPerLevel;

    public bool useScreenLogger = true;

    public Text ScreenLogger { get; private set; }

    // Use this for initialization
    void Awake() {
        ScreenLogger = GameObject.Find("ScreenLogger").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update() {

    }

    public string ScreenLoggerHeader = "DEBUG:\n";

    public void Log(object any)
    {
        if(useScreenLogger) {
            ScreenLogger.text = ScreenLoggerHeader + any.ToString();
        } else {
            Debug.Log(any);
        }
    }
}