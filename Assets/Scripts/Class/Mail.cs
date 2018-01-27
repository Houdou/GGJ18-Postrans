using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mail : MonoBehaviour {
    public float type; // type of the mail
    public float value; // normal return value
    public float max_value; // max return value
    public float rate; // rate to generate

    private float pos_x; // x 
    private float pos_y; // y 
    private float time = 0.0f; // time
    private float exsit_time = 30.0f; // exist time

    // Use this for initialization
    void Start() {
        MailManager MailManager = GameObject.Find("GameMaster").GetComponent<MailManager>();
    }

    // Update is called once per frame
    void Update() {
        time += Time.deltaTime;
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
    void MoveMail(Vector2 moveDir) {

    }


}
