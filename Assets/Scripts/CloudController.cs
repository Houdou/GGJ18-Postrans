using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour {
    public GameObject MapObject;
    public Sprite[] Cloud;
    public float MoveSpeed = 1f;

    private Vector2 Bound;
    private Vector2 CloudSize;

    // Use this for initialization
    void Start () {
        GetComponent<SpriteRenderer>().sprite = Cloud[Random.Range(0, 4)];
        MoveSpeed = Random.Range(0.1f, 0.3f);

        float randomScale = Random.Range(0.3f, 0.6f);
        transform.localScale = new Vector3(randomScale, randomScale, 1);

        Bound = new Vector2(MapObject.GetComponent<Renderer>().bounds.size.x / 2, MapObject.GetComponent<Renderer>().bounds.size.y / 2);
        CloudSize = new Vector2(GetComponent<Renderer>().bounds.size.x / 2, GetComponent<Renderer>().bounds.size.y / 2);
        
        transform.position = new Vector3((-1)*(Bound.x + CloudSize.x), (Bound.y - CloudSize.y) * Random.Range(-1.0f, 1.0f), 1);
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(transform.position.x + Time.deltaTime * MoveSpeed, transform.position.y, transform.position.z);

        if(transform.position.x >= Bound.x + CloudSize.x) {
            Destroy(gameObject);
        }
    }
}
