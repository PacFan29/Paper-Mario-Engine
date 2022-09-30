using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/Item Data")]
public class ItemData : ScriptableObject
{
    public List<Items> data = new List<Items>();

    [Serializable]
    public class Data {
        public Sprite Image;
        public string Name;
        [TextArea] public string Description;
        public int BuyingPrice;
        public int SellingPrice;
    }

    public enum Opponent {
        Self,
        Enemy,
        AllEnemiesButOnlyFloorOrCeiling,
        AllEnemiesCompletely
    }
    public enum Parameter {
        HP,
        FP,
        Power,
        Defense,
        Attack,
        Sleep,
        Dizzy,
        Confuse,
        Tiny,
        Stop,
        Soft,
        Burn,
        Freeze,
        Poison,
        Dead,
        SlowHP,
        SlowFP,
        Avoid,
        Electricity,
        Invisible,
        Cure,
        ReturnDamage,
        Bigger,
        PointSwap,
        Discard
    }
    [Serializable]
    public class Effects {
        public Opponent opponent;
        public Parameter parameter;
        public int AnimationIndex = -1;
        public int Force = 0;
    }
    [Serializable]
    public class Items : Data {
        public List<Effects> effects = new List<Effects>();
    }
}
