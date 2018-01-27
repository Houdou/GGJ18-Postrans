using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {
    private int totalMoney;
    public Text moneyUI;
	// Use this for initialization
	void Start () {
        totalMoney = 500;
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("Total money: "+totalMoney);
	}

    public void income(int a)
    {
        totalMoney += a;
    }
    public void expense(int a)
    {
        totalMoney -= a;
    }
    void SetText()
    {
        moneyUI.text = "Total: " + totalMoney.ToString();
    }
}
