using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : Status
{
    [Header("敵ステータス限定")]
    [HideInInspector] public int Coins = 0;
    public int baseEXP = 0;
    public int[] selfAbnormalRates = {
        100, //逃げる
        100, //吹き飛ばし
        100, //KO
    };
    // Start is called before the first frame update
    void Start()
    {
        HP = MaxHP;
    }

    // Update is called once per frame
    void Update()
    {
        //レベル
        Level = Mathf.Clamp(Level, 1, 100);
        //ＨＰ（ハート）
        MaxHP = Mathf.Clamp(MaxHP, 1, 32767);
        HP = Mathf.Clamp(HP, 0, MaxHP);
    }

    //例：逃げる
    //AbnormalState(selfAbnormalRates, 0);

    public int EXPCalc(int enemyCount) {
        //マリオのレベルを取得
        int marioLevel = 0;
        bool maxLevel = false;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            if (player.GetComponent<MarioStatus>() != null) {
                marioLevel = player.GetComponent<MarioStatus>().Level;
                maxLevel = player.GetComponent<MarioStatus>().isMaxLevel();
            }
        }

        float[] battleMultis = {
            0.5f,
            0.5f,
            0.55f,
            0.65f,
            0.75f,
        };
        enemyCount--; //0~4に統一する

        //スターポイントの計算
        int exp = (int)Math.Floor((Level - marioLevel) * battleMultis[enemyCount]) + baseEXP;
        if (exp < 0 || maxLevel) exp = 0;

        return exp;
    }
}
