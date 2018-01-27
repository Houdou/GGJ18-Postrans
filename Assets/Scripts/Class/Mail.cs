using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mail : MonoBehaviour {
    public float type; // type of the mail
    public float value; // normal return value
    public float max_value; // max return value
    public float rate; // rate to generate
    public float move_speed = 1f;

    private float pos_x; // x 
    private float pos_y; // y 
    private float time = 0.0f; // time
    private float exsit_time = 30.0f; // exist time

    public MailManager MailManagerObj;

    // Use this for initialization
    void Start() {
        MailManagerObj = GameObject.Find("GameMaster").GetComponent<MailManager>();
    }

    // Update is called once per frame
    void Update() {
        time += Time.deltaTime;

        Vector3 targetStation = FindTargetStation();
        if(targetStation != new Vector3(0, 0, 0)) {
            MoveMail(targetStation);
        }

    }

    //
    //  1. Get x/y from area
    //  2. Get random type and related value
    //  3. Determine the time left
    //
    public void MailInit(Vector2 init_pos) {
        Vector3 pos = new Vector3(init_pos.x, init_pos.y, 0);
        this.transform.position = pos;

        pos_x = init_pos.x;
        pos_y = init_pos.y;

    }

    //
    //  Mail Destroy
    //
    void DestroyMail() {
        Object.Destroy(this);
    }

    //
    //  Move mail to the closest postrans
    //
    void MoveMail(Vector3 targetPosition) {
        this.transform.position = Vector3.Lerp(this.transform.localPosition, targetPosition, Time.deltaTime * move_speed);
    }

    Vector3 FindTargetStation() {
        bool findTarget = false;
        float shortest_distance = 0;
        Vector3 targetStation = new Vector3(0, 0, 0);

        foreach (GameObject station in MailManagerObj.MailStation) {
            Vector3 position = station.transform.position;
            float distance = CalculateDistance(position, this.transform.position);

            if(distance < 5f) { // 3 should be the station range
                if (!findTarget) {
                    shortest_distance = distance;
                    targetStation = station.transform.position;
                    findTarget = true;
                }
                else {
                    if(distance < shortest_distance) {
                        shortest_distance = distance;
                        targetStation = station.transform.position;
                    }
                }
            }
        }

        return targetStation;
    }

    float CalculateDistance(Vector3 pos1, Vector3 pos2) {
        float a = pos1.x - pos2.x;
        float b = pos1.y - pos2.y;

        return Mathf.Sqrt(a*a + b*b);
    }
}
