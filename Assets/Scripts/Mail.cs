using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mail : MonoBehaviour {

    public float prior; // type of the mail
    public float value; // normal return value
    public float bonus; // max return value
    
    public Station TargetHome;
    public bool IsAccepted;
    public bool IsTravelling;

    //public float rate; // rate to generate
    public float moveSpeed = 1f;
    
    public float StartTime;
    public float LifeTime {
        get {
            return Time.time - StartTime;
        }
    }

    private LevelController lc;

    public Station TargetCollectStation;
    public Road TravellingRoad;

    private void Awake() {
        lc = GameMaster.Instance.GetLevelController();
    }

    private void Start() {
        UpdateTargetStation();
    }

    private void Update() {
        if(TargetCollectStation != null && !IsAccepted) {
            MoveMail(TargetCollectStation.Pos);
            CheckAcceptance(TargetCollectStation);
        }

        if(IsTravelling) { // Collected, on path
            if(TargetCollectStation != null && IsAccepted) { // Not in station
                // TODO: Move along the road
                MoveMail(TargetCollectStation.Pos);
                CheckAcceptance(TargetCollectStation);
            }
        }
    }

    //
    //  1. Get x/y from area
    //  2. Get random type and related value
    //  3. Determine the time left
    //
    public void Initialize(Vector2 init_pos, int targetHome) {
        Vector3 pos = new Vector3(init_pos.x, init_pos.y, 0);
        transform.position = pos;

        TargetHome = lc.HomeList[targetHome];

        transform.Find("Marker").GetComponent<SpriteRenderer>().sprite = lc.Markers[targetHome];

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
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
    }

    public void MoveTowards(Station from, Road road) {
        Popup();

        // TODO: Show up
        IsTravelling = true;
        TravellingRoad = road;
        TargetCollectStation = road.Next(from);

        Debug.Log("Mail move towards " + TargetCollectStation.ID);
    }

    public Station UpdateTargetStation() {
        if(TargetCollectStation == null) {
            float shortestDistance = 9999.9f;
            Station targetStation = null;

            foreach(var station in lc.StationList) {
                float distance = Vector2.Distance(station.Pos, transform.position);

                if(distance < station.MailRange && distance < shortestDistance) {
                    shortestDistance = distance;
                    targetStation = station;
                }
            }

            if(targetStation != null) {
                TargetCollectStation = targetStation;
            }
        }
        return TargetCollectStation;
    }

    public void CheckAcceptance(Station station) {
        if(Vector2.Distance(transform.position, station.Pos) < 0.3f) {
            FadeOut();

            if(station.ID == TargetHome.ID) {
                // Arrival
            } else {
                bool arriveStation = station.AddMail(this);

                if(arriveStation) {
                    // TODO: Fade out


                    IsTravelling = false;

                    if(!IsAccepted) {
                        IsAccepted = true;
                    }


                } else {
                    // Waiting outside
                }
            }
        }
    }

    public void FadeOut() {
        GetComponent<Animator>().SetBool("Arrive", true);
        transform.Find("Marker").GetComponent<Animator>().SetBool("Arrive", true);
    }

    public void Popup() {
        GetComponent<Animator>().SetBool("Popup", true);
        transform.Find("Marker").GetComponent<Animator>().SetBool("Popup", true);
    }
}

