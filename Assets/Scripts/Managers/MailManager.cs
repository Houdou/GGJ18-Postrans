using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailManager : MonoBehaviour {
    public GameObject map_bg;
    public GameObject MailArea;
    public List<GameObject> MailAreas = new List<GameObject>();
    public List<GameObject> MailStation = new List<GameObject>();
    private float time = 0.0f; // time count since start


	// Use this for initialization
	void Start () {
        Vector2 bound = new Vector2(map_bg.GetComponent<Renderer>().bounds.size.x / 2, map_bg.GetComponent<Renderer>().bounds.size.y / 2);

        CreateNewArea(new Vector2(1f, 1f), bound);
        CreateNewArea(new Vector2(-3f, -3f), bound);
        CreateNewArea(new Vector2(6f, -4f), bound);

    }
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;
    }

    void CreateNewArea(Vector2 init_pos, Vector2 bound) {
        GameObject area = Object.Instantiate(MailArea);
        area.GetComponent<MailArea>().AreaInit(init_pos, bound);

        MailAreas.Add(area);
    }
}
