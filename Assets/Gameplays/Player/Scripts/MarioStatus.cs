using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioStatus : Status
{
    [Header("各攻撃力")]
    [HideInInspector] public int FP;
    [HideInInspector] public int MaxFP;
    [HideInInspector] public int BP;
    [HideInInspector] public int MaxBP;

    [HideInInspector] public int FinalMaxHP;
    [HideInInspector] public int FinalMaxFP;

    public int StarPoints = 0;
    public int Coins = 0;

    [Header("各攻撃力")]
    public int BootsRank = 1;
    public int HammerRank = 0;

    [Header("データ")]
    public BadgeData badgeData;
    public StatusSaver statusSaver;
    public Options options;

    private Dictionary<string, int> badgeEffects = new Dictionary<string, int>() {
        {"HPPlus", 0},
        {"FPPlus", 0}
    };
    [Header("状態")]

    public bool Pinch;
    public bool Danger;
    // Start is called before the first frame update
    void OnEnable()
    {
        Level = statusSaver.Level;

        MaxHP = statusSaver.MaxHP;
        HP = statusSaver.HP;
        if (HP <= 0) HP = 1;

        MaxFP = statusSaver.MaxFP;
        FP = statusSaver.FP;
        MaxBP = statusSaver.MaxBP;
        BP = MaxBP;

        StarPoints = statusSaver.StarPoints;
        Coins = statusSaver.Coins;

        BootsRank = statusSaver.BootsRank;
        HammerRank = statusSaver.HammerRank;
    }

    void Update()
    {
        //バッジの効果
        badgeEffects["HPPlus"] = 0;
        badgeEffects["FPPlus"] = 0;
        Power = 1;
        Defense = 0;

        badgeEffects["HPPlus"] = statusSaver.GetForces(BadgeData.Badges.Parameter.MarioHP, Pinch, Danger);
        badgeEffects["FPPlus"] = statusSaver.GetForces(BadgeData.Badges.Parameter.FP, Pinch, Danger);
        Power += statusSaver.GetForces(BadgeData.Badges.Parameter.MarioPower, Pinch, Danger);
        Defense += statusSaver.GetForces(BadgeData.Badges.Parameter.MarioDefense, Pinch, Danger);

        //レベル
        Level = Mathf.Clamp(Level, 1, options.MaxLevel());
        statusSaver.Level = Level;
        //ＨＰ（ハート）
        MaxHP = Mathf.Clamp(MaxHP, 5, options.maximumHP);
        FinalMaxHP = MaxHP + badgeEffects["HPPlus"];
        statusSaver.MaxHP = MaxHP;
        HP = Mathf.Clamp(HP, 0, FinalMaxHP);
        statusSaver.HP = HP;
        //ＦＰ（フラワー）
        MaxFP = Mathf.Clamp(MaxFP, 5, options.maximumFP);
        FinalMaxFP = MaxFP + badgeEffects["FPPlus"];
        statusSaver.MaxFP = MaxFP;
        FP = Mathf.Clamp(FP, 0, FinalMaxFP);
        statusSaver.FP = FP;
        //ＢＰ（バッジ）
        MaxBP = Mathf.Clamp(MaxBP, 3, options.maximumBP);
        statusSaver.MaxBP = MaxBP;
        BP = MaxBP;
        foreach (StatusSaver.BadgeStatus badge in statusSaver.badges) {
            if (badge.isAttaching) {
                BP -= badgeData.data[badge.BadgeIndex].BPNeeded;
            }
        }
        statusSaver.BP = BP;
        //スターポイント（経験値）
        StarPoints = Mathf.Clamp(StarPoints, 0, 999);
        statusSaver.StarPoints = StarPoints;
        //コイン
        Coins = Mathf.Clamp(Coins, 0, 999);
        statusSaver.Coins = Coins;
        //各攻撃力
        BootsRank = Mathf.Clamp(BootsRank, 1, 5);
        statusSaver.BootsRank = BootsRank;
        HammerRank = Mathf.Clamp(HammerRank, 0, 5);
        statusSaver.HammerRank = HammerRank;

        if (Power < 0) Power = 0;

        Pinch = isPinch(HP, FinalMaxHP);
        Danger = isDanger(HP, FinalMaxHP);
    }

    public bool isMaxLevel() {
        return Level == options.MaxLevel();
    }
}
