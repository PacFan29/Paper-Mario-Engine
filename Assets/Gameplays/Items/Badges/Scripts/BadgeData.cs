using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BadgeData", menuName = "ScriptableObjects/Badge Data")]
public class BadgeData : ScriptableObject
{
    public List<Badges> data = new List<Badges>();

    [Serializable]
    public class Badges : ItemData.Data {
        [Header("必要量")]
        public int BPNeeded;
        [Header("その他ショップ")]
        public int PiantaAmounts;
        public int StarPieceAmounts;

        public enum Parameter {
            Battle_Jump,
            Battle_Hammer,
            Battle_Items,
            Battle_Tactics,
            Battle_ActionCommand,
            Mario_Guard,
            MarioHP,
            MarioPower,
            MarioDefense,
            FP,
            PartnerHP,
            PartnerPower,
            PartnerDefense,
            Coins,
            Items,
            Mario_Damage,
            Mario_Lucky,
            Partner_Lucky,
            Mario_HPHeal,
            Partner_HPHeal,
            FPHeal,
            Mario_UsingFP,
            Partner_UsingFP,
            Field_Heart,
            Field_Flower,
            ReturnCoinsFromItem,
            Battle_M_Appeal,
            Battle_P_Appeal,
            EnemyHP,
            PartnerSwitch,
            Battle_Acrobat,
            Partner_Damage,
            Random,
            None,
            Partner_Guard,
        }
        public enum Situation {
            None,
            Only,
            Pinch,
            Danger,
            Mario_Abnormal,
            Partner_Abnormal,
        }
        public enum Ability {
            None,
            Sleep,
            Dizzy,
            Confuse,
            Tiny,
            Stop,
            Soft,
            Burn,
            Freeze,
            IcePower,
            ProtectSpikes,
            Electricity,
            ReturnDamage,
            SoundA,
            SoundB,
            SoundC,
            SoundD,
            SoundE
        }
        public enum FieldSituation {
            None,
            GetAttacked,
            AttackEnemy,
            TouchedEnemy,
            Speed
        }
        [Serializable]
        public class Effects {
            public Parameter parameter;
            public Situation situation;
            public Ability ability;
            public FieldSituation fieldSituation;
            public int force;
        }
        [Header("効果")]
        public List<Effects> effects = new List<Effects>();
    }
}
