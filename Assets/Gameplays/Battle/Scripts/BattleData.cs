using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleData", menuName = "ScriptableObjects/Battle Data")]
public class BattleData : ScriptableObject
{
    public enum Beginning {
        Normal,
        PlayerFirstStrike,
        EnemyFirstStrike
    }
    public Beginning beginning;
    public int abilityId;
    public int actionId;
    public List<int> enemyIds = new List<int>();
}
