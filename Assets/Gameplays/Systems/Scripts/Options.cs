using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Options", menuName = "ScriptableObjects/Options")]
public class Options : ScriptableObject
{
    [Header("サウンド")]
    public float musicVolume;
    public float soundVolume;
    public float voiceVolume;
    [Header("パラメータの上限")]
    public int maximumHP = 200;
    public int maximumFP = 200;
    public int maximumBP = 99;

    public int MaxLevel() {
        int hSeg = maximumHP / 5;
        int fSeg = maximumFP / 5;
        int bSeg = maximumBP / 3;

        int maxLv = hSeg + fSeg + bSeg - 3;

        return Math.Min(99, maxLv);
    }
}
