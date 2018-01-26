using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailArea {

    private float pos_x; // x of center
    private float pos_y; // y of center
    private float radius; // radius of the circle
    private float generate_speed; // control the speed of generation, controled by sin@
    private int max_mail; // maximum hold of mails in area
    private int total_mail; // history record of total mail generated
        
    //
    //  1. Detect other areas 
    //  2. Generate random x/y/r
    //
    public void CreateNewArea(MailArea[] mailAreaArr) {
            
    }

    //
    //  Star the mail generation in a changing speed (sin)
    //
    public void StartGenerateMail() {

    }

    //
    //  Generate one mail in the area
    //  @mail_x
    //  @mail_y
    //  @mail_type
    //
    private void GenerateOneMail() {

    }

    //
    //  Stop the mail gerneration
    //
    public void StopGenerateMail() {

    }
   
}
