using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailArea : MonoBehaviour {
    public List<GameObject> mails = new List<GameObject>();
    public GameObject mail_prefab1;
    public GameObject mail_prefab2;
    public GameObject mail_prefab3;

    public float pos_x; // x of center
    public float pos_y; // y of center
    public float radius; // radius of the circle
    private float generate_speed; // control the speed of generation, controled by sin@
    private int max_mail; // maximum hold of mails in area
    private int total_mail; // history record of total mail generated

    private float time = 0.0f; // time count since start
    private float releaseHold = 5f;
    private bool gernerateMail = true;

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        time += Time.deltaTime;

        GenerateOneMail(time);
    }

    //
    //  1. Detect other areas 
    //  2. Generate random x/y/r
    //  
    public void AreaInit(Vector2 init_pos, Vector2 bound) {
        radius = Random.Range(1.5f, 2.5f);
        float random_x = Random.Range(-1f, 1f) + init_pos.x;
        float random_y = Random.Range(-1f, 1f) + init_pos.y;

        random_x = (random_x > bound.x - radius) ? (random_x -= radius) : random_x;
        random_x = (random_x < (-1)*bound.x + radius) ? (random_x += radius) : random_x;
        random_y = (random_y > bound.y - radius) ? (random_y -= radius) : random_y;
        random_y = (random_y < (-1)*bound.y + radius) ? (random_y += radius) : random_y;

        releaseHold = 5f + Random.Range(-1f, 1f) * 2;
    
        pos_x = random_x;
        pos_y = random_y;
        this.transform.position = new Vector3(pos_x, pos_y, 0);


        //Debug.Log("x:" + random_x + " // y:" + random_y + "// r:" + radius);
    }


    //
    //  Create Object
    //
    void CreateNewAreaObj() {
        
    }

    //
    //  Star the mail generation in a changing speed (sin)
    //
    void StartGenerateMail() {
        gernerateMail = true;
    }

    //
    //  Generate one mail in the area
    //  @mail_x
    //  @mail_y
    //  @mail_type
    //
    public void GenerateOneMail(float time) {
        //Debug.Log("time: " + time + " // hold:" + releaseHold);
        int count = (int)time / (int)releaseHold;
        if(mails.Count == count) {
            StopGenerateMail();
        }
        else if(mails.Count < count) {
            StartGenerateMail();
        }

        if (gernerateMail) {
            float mail_x = Random.Range(pos_x - radius, pos_x + radius);
            float mail_y = Random.Range(pos_y - radius, pos_y + radius);
            int mail_type = 1;

            float random_type = Random.Range(0f, 1f);
            if(random_type >= 0.05f && random_type < 0.4f) {
                mail_type = 2;
                CreateNewMail(new Vector2(mail_x, mail_y), mail_prefab2);
            }
            else if (random_type < 0.05f) {
                mail_type = 3;
                CreateNewMail(new Vector2(mail_x, mail_y), mail_prefab3);
            }
            else {
                CreateNewMail(new Vector2(mail_x, mail_y), mail_prefab1);
            }

            //Debug.Log("mail_x: " + mail_x + " // mail_y:" + mail_y);
        }

    }

    //
    //  Stop the mail gerneration
    //
    void StopGenerateMail() {
        gernerateMail = false;
    }

    void CreateNewMail(Vector2 init_pos, GameObject mail_prefab) {
        GameObject mail = Object.Instantiate(mail_prefab);
        mail.GetComponent<Mail>().MailInit(init_pos);

        mails.Add(mail);
    }
}
