using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mail {

    private float pos_x; // x 
    private float pos_y; // y 
    private float type; // type of the mail
    private float value; // normal return value
    private float max_value; // max return value
    private float time; // exist time


    //
    //  1. Get x/y from area
    //  2. Get random type and related value
    //  3. Determine the time left
    //
    public void CreateNewMail(float x, float y) {
        pos_x = x;
        pos_y = y;
        type = 1;
        value = 10;
        max_value = 20;
        time = 60 * 1000;
    }

    //
    //  Mail Destroy
    //
    public void DestroyMail() {

    }

    //
    //  Move mail to the closest postrans
    //
    public void MoveMail(MailArea[] mailAreaArr, Vector2 moveDir) {

    }


}
