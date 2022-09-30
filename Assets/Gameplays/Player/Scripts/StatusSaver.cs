using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusSaver", menuName = "ScriptableObjects/Status Saver")]
public class StatusSaver : ScriptableObject
{
    [Header("レベル")]
    public int Level;
    [Header("ハートポイント")]
    public int HP;
    public int MaxHP;
    [Header("フラワーポイント")]
    public int FP;
    public int MaxFP;
    [Header("バッジポイント")]
    public int BP;
    public int MaxBP;
    [Header("各攻撃力")]
    public int BootsRank;
    public int HammerRank;
    [Header("スターポイント")]
    public int StarPoints;
    [Header("コイン")]
    public int Coins;
    [Header("仲間")]
    public List<PartnerStatus> partners = new List<PartnerStatus>();
    
    [Serializable]
    public class PartnerStatus {
        public int Rank;
        public string Name;
        [HideInInspector] public int HP;
        [HideInInspector] public int MaxHP;
        public int InitialHP;
        public int HighHP;
        public int Defense;

        /*
        float increase = (float)(highHP - initialHP) / 10f;
        MaxHP = initialHP + ((int)Math.Floor(increase * Rank) * 5);
        */
    }

    [Header("アイテム(10個まで)")]
    public ItemData itemData;
    public List<int> items = new List<int>();
    [Header("バッジ(200個まで)")]
    public BadgeData badgeData;
    public List<BadgeStatus> badges = new List<BadgeStatus>();
    [Serializable]
    public class BadgeStatus {
        public int BadgeIndex;
        public bool isAttaching;

        public BadgeStatus(int BadgeIndex) {
            this.BadgeIndex = BadgeIndex;
            this.isAttaching = false;
        }
    }
    [Header("星の欠片")]
    public int StarPieces;

    public int GetForces(BadgeData.Badges.Parameter param, bool Pinch, bool Danger) {
        int forces = 0;

        foreach(StatusSaver.BadgeStatus status in badges) {
            if (status.isAttaching) {
                foreach(BadgeData.Badges.Effects effect in badgeData.data[status.BadgeIndex].effects) {
                    BadgeData.Badges.Situation situ = effect.situation;

                    if (
                        situ == BadgeData.Badges.Situation.None ||
                        (situ == BadgeData.Badges.Situation.Pinch && Pinch) ||
                        (situ == BadgeData.Badges.Situation.Danger && Danger)
                    ) {
                        if (effect.parameter == param) {
                            forces += effect.force;
                        }
                    }
                }
            }
        }
        return forces;
    }

    public int GetCount(BadgeData.Badges.Parameter param, bool Pinch, bool Danger) {
        int count = 0;

        foreach(StatusSaver.BadgeStatus status in badges) {
            if (status.isAttaching) {
                foreach(BadgeData.Badges.Effects effect in badgeData.data[status.BadgeIndex].effects) {
                    BadgeData.Badges.Situation situ = effect.situation;

                    if (
                        situ == BadgeData.Badges.Situation.None ||
                        (situ == BadgeData.Badges.Situation.Pinch && Pinch) ||
                        (situ == BadgeData.Badges.Situation.Danger && Danger)
                    ) {
                        if (effect.parameter == param) {
                            count++;
                        }
                    }
                }
            }
        }
        return count;
    }

    public bool AbilityCheck(BadgeData.Badges.Ability ability) {
        foreach (StatusSaver.BadgeStatus badge in badges) {
            foreach (BadgeData.Badges.Effects effect in badgeData.data[badge.BadgeIndex].effects) {
                if (badge.isAttaching && effect.ability == ability) {
                    //つけている様子
                    return true;
                }
            }
        }
        //つけていない様子
        return false;
    }

    public bool FieldSituation(BadgeData.Badges.FieldSituation situation) {
        foreach (StatusSaver.BadgeStatus badge in badges) {
            foreach (BadgeData.Badges.Effects effect in badgeData.data[badge.BadgeIndex].effects) {
                if (badge.isAttaching && effect.fieldSituation == situation) {
                    //つけている様子
                    return true;
                }
            }
        }
        //つけていない様子
        return false;
    }
}
