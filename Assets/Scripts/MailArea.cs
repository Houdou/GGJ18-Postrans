using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailArea : MonoBehaviour {
    public int MailCount = 0;
    public GameObject mail_prefab1;
    public GameObject mail_prefab2;
    public GameObject mail_prefab3;
    public AudioClip mailGenerateAC;

    public float PosX; // x of center
    public float PosY; // y of center
    public float radius; // radius of the circle
    private float generateSpeed; // control the speed of generation, controled by sin@
    private int maxMail; // maximum hold of mails in area
    private int totalMail; // history record of total mail generated

    private float time = 0.0f; // time count since start
    private float releaseHold = 5f;
    private bool gernerateMail = true;

    private LevelController lc;

    private void Awake() {
        lc = GameMaster.Instance.GetLevelController();
    }

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
        float randomX = Random.Range(-1f, 1f) + init_pos.x;
        float randomY = Random.Range(-1f, 1f) + init_pos.y;

        randomX = (randomX > bound.x - radius) ? (randomX -= radius) : randomX;
        randomX = (randomX < (-1)*bound.x + radius) ? (randomX += radius) : randomX;
        randomY = (randomY > bound.y - radius) ? (randomY -= radius) : randomY;
        randomY = (randomY < (-1)*bound.y + radius) ? (randomY += radius) : randomY;

        releaseHold = 5f + Random.Range(-1f, 1f) * 2;
    
        PosX = randomX;
        PosY = randomY;
        transform.position = new Vector3(PosX, PosY, 0);


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
        if(MailCount == count) {
            StopGenerateMail();
        }
        else if(MailCount < count) {
            StartGenerateMail();
        }

        if (gernerateMail) {
            float mail_x = Random.Range(PosX - radius, PosX + radius);
            float mail_y = Random.Range(PosY - radius, PosY + radius);
            int mail_type = 1;

            float random_type = Random.Range(0f, 1f);
            if(random_type < 0.05f) {
                mail_type = 3;
                CreateNewMail(new Vector2(mail_x, mail_y), mail_prefab3);
            } else if(random_type < 0.4f) {
                mail_type = 2;
                CreateNewMail(new Vector2(mail_x, mail_y), mail_prefab2);
            } else {
                CreateNewMail(new Vector2(mail_x, mail_y), mail_prefab1);
            }

            MailCount++;
            AudioSource.PlayClipAtPoint(mailGenerateAC, new Vector2(mail_x, mail_y));
            //Debug.Log("mail_x: " + mail_x + " // mail_y:" + mail_y);
        }

    }

    //
    //  Stop the mail gerneration
    //
    void StopGenerateMail() {
        gernerateMail = false;
    }

    void CreateNewMail(Vector2 initPos, GameObject mailPrefab) {
        GameObject newMail = Instantiate(mailPrefab);
        Mail mail = newMail.GetComponent<Mail>();
        
        mail.Initialize(initPos, Random.Range(0, lc.HomeList.Count));
        
        //mailList.Add(newMail);
        GameMaster.Instance.GetLevelController().MailList.Add(mail);
    }
}
