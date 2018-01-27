using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mail : MonoBehaviour {
    public float type; // type of the mail
    public float value; // normal return value
    public float max_value; // max return value
    public float rate; // rate to generate
    public float moveSpeed = 1f;

    private float posX; // x 
    private float posY; // y 
    private float time = 0.0f; // time
    private float exsit_time = 30.0f; // exist time
    
    public float StartTime;
    public float LifeTime {
        get {
            return Time.time - StartTime;
        }
    }

    private LevelController lc;

    public Station TargetStation;

    private void Awake() {
        lc = GameMaster.Instance.GetLevelController();
    }

    private void Start() {
        UpdateTargetStation();
    }

    void Update() {
        if(TargetStation != null) {
            MoveMail(TargetStation.Pos);
        }
    }

    //
    //  1. Get x/y from area
    //  2. Get random type and related value
    //  3. Determine the time left
    //
    public void MailInit(Vector2 init_pos) {
        Vector3 pos = new Vector3(init_pos.x, init_pos.y, 0);
        transform.position = pos;

        posX = init_pos.x;
        posY = init_pos.y;

    }

    //
    //  Mail Destroy
    //
    void DestroyMail() {
        Destroy(this);
    }

    //
    //  Move mail to the closest postrans
    //
    void MoveMail(Vector3 targetPosition) {
        transform.position = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * moveSpeed);
    }

    public Station UpdateTargetStation() {
        float shortestDistance = 9999.9f;
        Station targetStation = null;

        foreach (var station in lc.StationList) {
            float distance = Vector2.Distance(station.Pos, transform.position);

            Debug.Log(distance);

            if(distance < station.MailRange && distance < shortestDistance) {
                shortestDistance = distance;
                targetStation = station;
            }
        }
        if(targetStation != null) {
            TargetStation = targetStation;
        }


        return targetStation;
    }
}

