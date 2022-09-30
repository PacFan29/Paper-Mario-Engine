using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Status : MonoBehaviour
{
    [Header("共通ステータス")]
    public int Level = 1;

    [HideInInspector] public int HP;
    public int MaxHP = 10;
    public int Power = 1;
    public int Defense = 0;

    [HideInInspector] public int ChargePower = 0;

    [Header("状態異常一覧")]
    public int[] abnormalRates = {
        100, //眠り
        100, //グルグル
        100, //混乱
        100, //ミニミニ
        100, //ストップ
        100, //フニャフニャ
        100, //炎
        100  //氷
    };

    //例：眠り
    //AbnormalState(abnormalRates, 0);

    public bool AbnormalState(int[] abnormalList, int index) {
        return UnityEngine.Random.Range(0, 101) <= abnormalList[index];
    }
    public int DamageCalc(int power, int defense, bool ignoreDefense) {
        int damage = 0;
        if (ignoreDefense) {
            //防御無視
            damage = power;
        } else {
            //比較的簡単な計算式（攻撃力 - 防御力）
            damage = power - defense;
        }

        if (damage <= 0) {
            return 0;
        } else {
            return damage;
        }
    }

    public void TakeDamage(int damage) {
        HP -= damage;
    }

    public bool isPinch(int HP, int MaxHP) {
        return 
        ((HP <= Math.Max(0.3f, 0.5f - (0.2f - (float)(50 - MaxHP) / 200)) * MaxHP) || (HP <= 5))
        && HP > 0 && !isDanger(HP, MaxHP);
    }
    public bool isDanger(int HP, int MaxHP) {
        return (HP > 0 && HP <= MaxHP * 0.1f) || HP == 1;
    }
}
