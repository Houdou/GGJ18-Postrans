using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageIndicator : MonoBehaviour {

    public GameObject DotPrefab;
    public GameObject DotGroup;

    public DotController[] Dots;

    public void Awake() {
        DotGroup = new GameObject("DotGroup");
        DotGroup.transform.parent = transform;
        DotGroup.transform.localPosition = Vector3.zero;
    }

    public void ResetCapacity(int capacity, Vector3 OffsetVec) {
        if(DotGroup != null)
            Destroy(DotGroup);
        DotGroup = new GameObject("DotGroup");
        DotGroup.transform.parent = transform;

        Dots = new DotController[capacity];

        for(int i = 0; i < capacity; i++) {
            GameObject newDot = Instantiate(DotPrefab, new Vector3(0.125f * (i % 5), (i/5) * -0.1f, 0.0f), Quaternion.identity, DotGroup.transform);
            Dots[i] = newDot.GetComponent<DotController>();
            Dots[i].setColor(-1);
        }

        DotGroup.transform.localPosition = OffsetVec;
    }

    public void UpdateColor(Queue<Mail> mails) {
        Mail[] mailArray = new Mail[Dots.Length];
        int i = 0;
        foreach(var mail in mails) {
            mailArray[i++] = mail;
        }
        for(int j = 0; j < Dots.Length; j++) {
            Dots[j].setColor(mailArray[j] != null ? mailArray[j].TargetHome.ID : -1);
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
