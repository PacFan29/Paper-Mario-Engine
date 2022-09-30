using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventData", menuName = "ScriptableObjects/Event Data")]
public class EventData : ScriptableObject
{
    [Header("イベントありか否か")]
    public bool isEvent;
    [Header("エリア遷移")]
    public bool isTransition;
    public int areaIndex;
    public int warpIndex;
    [Header("バトル遷移")]
    public bool isVictory;
    public bool isFlee;
    public int earnedCoins;
}
