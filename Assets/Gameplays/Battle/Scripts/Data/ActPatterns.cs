using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "ActPatterns", menuName = "ScriptableObjects/Act Patterns")]
public class ActPatterns : ScriptableObject
{
    public enum Act {
        Jump,
        Hammer,
        Specials,
        CommonTactics,
        MarioTactics,
        PartnerAbilities,
        PartnerTactics
    }
    public enum Conditions {
        None,
        BootsRank,
        HammerRank,
        PartnerRank,
        Badge
    }
    [Serializable]
    public class Patterns {
        public Act act;
        [Header("アイコン・名称")]
        public Sprite icon;
        public string name;
        [TextArea] public string description;
        [Header("パターンのインデックス")]
        public int patternIndex = 0;
        [Header("条件(ランク類の場合は特定の値以上[≧]、バッジの場合は特定の値と一致している場合[＝])")]
        public Conditions condition;
        public int partnerId = 0;
        public int value = -1;
        [Header("FP必要量")]
        public int FPNeeded = 0;
    }

    public List<Patterns> patterns = new List<Patterns>();
}
