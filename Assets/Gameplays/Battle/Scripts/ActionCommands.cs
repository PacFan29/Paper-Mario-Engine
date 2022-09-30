using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCommands : BattleMovement
{
    //ジャンプ
    private float time = 0f;
    protected bool succeed = false;

    //ハンマー
    protected float hInterval = 0.5f;
    [HideInInspector] public int hStep = 0;
    [HideInInspector] public int hMaxStep = 4;
    protected int hammerTrigger = 0;
    
    protected void initialize() {
        time = 0f;
        succeed = false;
        
        hInterval = 0.5f;
        hStep = 0;
        hMaxStep = 4;
        hammerTrigger = 0;
    }
    protected void pressA()
    {
        time -= Time.deltaTime;
        if (Input.GetButtonDown("A")) {
            time = 0.1f;
        }
        succeed = (time > 0);
    }

    protected void hammerInterval() {
        time -= Time.deltaTime;

        if (time <= 0) {
            if (hStep == hMaxStep) {
                hammerTrigger = 2;
            } else {
                hStep++;

                if (hStep == hMaxStep) {
                    time = 0.5f;
                } else {
                    time = hInterval;
                }
            }
        }
        if (Input.GetAxis("Horizontal") >= 0) {
            hammerTrigger = 2;
        }

        succeed = (hStep == hMaxStep && Input.GetAxis("Horizontal") >= 0);
    }
}
