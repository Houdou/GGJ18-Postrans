using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum GameScene {
    Null = 0,
    Level = 2
}

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
    
    [SerializeField]
    public static readonly float MinimumHomeDistance = 3.0f;
    [SerializeField]
    public static readonly float RoadMaxLength = 7.0f;

    public int[] RoadCapacityPerLevel;
    public float[] MailRangePerLevel;

    public bool useScreenLogger = true;

    public Text ScreenLogger { get; private set; }

    // Use this for initialization
    void Awake() {
        ScreenLogger = GameObject.Find("ScreenLogger").GetComponent<Text>();
        GameScene = GameScene.Level;
    }

    // Update is called once per frame
    void Update() {

    }

    public GameScene GameScene = GameScene.Null;

    public string ScreenLoggerHeader = "DEBUG:\n";

    public void Log(object any)
    {
        if(useScreenLogger) {
            ScreenLogger.text = ScreenLoggerHeader + any.ToString();
        } else {
            Debug.Log(any);
        }
    }

    public LevelController GetLevelController() {
        if(GameScene == GameScene.Level) {
            return GameObject.Find("LevelController").GetComponent<LevelController>();
        } else {
            return null;
        }
    }
}