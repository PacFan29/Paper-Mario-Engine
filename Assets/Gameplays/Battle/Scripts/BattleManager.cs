using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    [Header("データ")]
    public BattleData battleData;
    public EventData events;
    [Header("プレイヤー")]
    public PlayerBattleMovements player;
    [Header("仲間")]
    public BattleMovement partner;
    [Header("敵")]
    public EnemyDataList enemyList;
    public Transform enemyGroup;

    [Header("その他UI")]
    public GameObject actionSelector;
    public GameObject abilitySelector;
    public GameObject selector;
    public GameObject levelUp;
    public Text howManyEXP;

    public static bool ready = true;
    public static Transform[] enMoves = new Transform[5];

    public static int actionId = 0;
    public static int selectedEnIndex = 0;
    public static int earnedEXP = 0;
    public static int earnedCoins = 0;
    public static int turnEnIndex = 0;

    private bool inputMemory = false;
    private int firstEnemy = 0; //冒頭の敵
    private int lastEnemy = 4;  //末尾の敵
    [HideInInspector] public int result = -2;     //勝敗判定

    private bool firstStrike = false;

    [HideInInspector] public int abilityId;
    [HideInInspector] public int abActionId;
    [HideInInspector] public int FPUsed;
    [HideInInspector] public bool selected;
    // Start is called before the first frame update
    void Start()
    {
        int enemies = battleData.enemyIds.Count;

        int move = 5 - enemies;
        enemyGroup.position += new Vector3(move * 2, 0, move * -1f);

        Transform[] enPoses = enemyGroup.GetComponentsInChildren<Transform>();
        enMoves = new Transform[enemies];

        for (int i = 1; i <= enemies; i++) {
            EnemyDataList.EnemyData getData = enemyList.enemyDatas[battleData.enemyIds[i-1]];

            GameObject enemy = Instantiate(getData.Prefab, enPoses[i].transform.position, Quaternion.identity);
            enMoves[i-1] = enemy.transform;
            
            EnemyStatus enStatus = enemy.GetComponent<EnemyStatus>();
            enStatus.Level = getData.Level;
            enStatus.MaxHP = getData.MaxHP;
            enStatus.HP = enStatus.MaxHP;
            enStatus.Power = getData.Power;
            enStatus.Defense = getData.Defense;
            enStatus.baseEXP = getData.baseEXP;
            enStatus.Coins = UnityEngine.Random.Range(getData.minCoins, getData.maxCoins+1);

            for (int j = 0; j < enStatus.abnormalRates.Length; j++) {
                enStatus.abnormalRates[j] = getData.abnormalRates[j];
            }
            for (int j = 0; j < enStatus.selfAbnormalRates.Length; j++) {
                enStatus.selfAbnormalRates[j] = getData.abnormalRates[j+8];
            }
        }

        foreach (Transform obj in enemyGroup.transform) {
            Destroy(obj.gameObject);
        }

        firstEnemy = 0;
        lastEnemy = battleData.enemyIds.Count-1;

        //先制攻撃の判定
        result = -2;
        StartCoroutine(BattleBegin(battleData.beginning));
    }
    IEnumerator BattleBegin(BattleData.Beginning beginning) {
        yield return new WaitForSeconds(0.5f);

        result = 0;

        if (beginning == BattleData.Beginning.PlayerFirstStrike) {
            selectedEnIndex = 0;
            player.targetEnemyFirst = enMoves[0];
            player.targetEnemy = enMoves[0];
            
            player.ActionSetUp(battleData.abilityId, battleData.actionId);

            actionId = 6;
            ready = false;

            firstStrike = true;
        } else {
            actionId = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        変数名：actionId
        00 = マリオのターン
        01 = マリオのターン（技の選択）
        02 = 敵の選択（マリオ）
        03 = 仲間のターン
        04 = 仲間のターン（技の選択）
        05 = 敵の選択（仲間）
        06 = 待機
        07 = 敵のターン
        */
        bool allDead = true;
        for (int i = 0; i < enMoves.Length; i++) {
            if (!isEnemyDead(i)) {
                allDead = false;
                break;
            }
        }

        if (!allDead) {
            while (isEnemyDead(firstEnemy) && firstEnemy < 4) {
                firstEnemy++;
            }
            while (isEnemyDead(lastEnemy) && firstEnemy > 0) {
                lastEnemy--;
            }
        }

        actionSelector.SetActive(
            ((actionId == 0 || actionId == 1) ||  
            (actionId == 3 || actionId == 4))
            && result == 0
        );
        actionSelector.GetComponent<ActionSelect>().canSelect = ((actionId == 0 || actionId == 3) && result == 0);
        abilitySelector.SetActive((actionId == 1 || actionId == 4) && result == 0);
        selector.SetActive(actionId == 2 || actionId == 5);

        if (result == 0) {
            switch(actionId) {
                case 0:
                case 3:
                if (selected) {
                    actionId++;
                    selected = false;
                }
                break;

                case 1:
                case 4:
                if (selected) {
                    selectedEnIndex = 0;
                    while (isEnemyDead(selectedEnIndex)) {
                        selectedEnIndex++;
                        if (allDead) break;
                    }
                    actionId++;

                    selected = false;
                }
                if (Input.GetButtonDown("B")) {
                    actionId--;
                }
                break;

                case 2:
                case 5:
                if (abilityId == 0) {
                    if (Input.GetAxis("Horizontal") > 0){
                        if (!inputMemory){
                            inputMemory = true;
                            if (selectedEnIndex < lastEnemy) selectedEnIndex++;
                            while (isEnemyDead(selectedEnIndex) && selectedEnIndex < lastEnemy) {
                                selectedEnIndex++;
                            }
                        }
                    } else if (Input.GetAxis("Horizontal") < 0){
                        if (!inputMemory){
                            inputMemory = true;
                            if (selectedEnIndex > firstEnemy) selectedEnIndex--;
                            while (isEnemyDead(selectedEnIndex) && selectedEnIndex > firstEnemy) {
                                selectedEnIndex--;
                            }
                        }
                    } else {
                        inputMemory = false;
                    }
                } else {
                    selectedEnIndex = firstEnemy;
                }
                selector.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, enMoves[selectedEnIndex].position);

                if (Input.GetButtonDown("A")) {
                    if (actionId == 5) {
                        ;
                    } else {
                        int firstIndex = 0;
                        while (isEnemyDead(firstIndex) && firstIndex < 5) {
                            firstIndex++;
                        }
                        player.targetEnemyFirst = enMoves[firstIndex];
                        player.targetEnemy = enMoves[selectedEnIndex];
                        player.ActionSetUp(abilityId, abActionId);
                    }

                    player.gameObject.GetComponent<MarioStatus>().FP -= FPUsed;

                    actionId = 6;
                    ready = false;
                }
                if (Input.GetButtonDown("B")) {
                    actionId--;
                }
                break;

                case 6:
                if (!player.turn && ready) {
                    if (firstStrike) {
                        actionId = 0;
                        firstStrike = false;
                    } else {
                        actionId = 7;
                    }
                    turnEnIndex = 0;
                    WinLoseCheck();
                }
                break;

                case 7:
                if (turnEnIndex > lastEnemy || (turnEnIndex > 0 && firstStrike)) {
                    firstStrike = false;
                    WinLoseCheck();

                    if (player.status.HP > 0) {
                        actionId = 0;
                    }
                } else {
                    while (isEnemyDead(turnEnIndex)) {
                        turnEnIndex++;
                        if (turnEnIndex > lastEnemy) {
                            WinLoseCheck();
                            return;
                        }
                    }
                    
                    if (!enMoves[turnEnIndex].GetComponent<BattleMovement>().turn) {
                        WinLoseCheck();

                        if (player.status.HP > 0) {
                            enMoves[turnEnIndex].GetComponent<BattleMovement>().turn = true;
                        }
                    }
                }
                break;
            }
        }
    }

    public bool isEnemyDead(int index) {
        index = Mathf.Clamp(index, 0, 4);
        if (enMoves[index] == null) {
            return true;
        } else {
            return enMoves[index].GetComponent<BattleMovement>().isDead;
        }
    }
    public void WinLoseCheck() {
        /*
        -1 = 敗北
         0 = 継続
         1 = 勝利
        */
        if (player.status.HP <= 0) {
            player.Die();
            StartCoroutine("Lose");
        }
        for (int i = 0; i < enMoves.Length; i++) {
            if (!isEnemyDead(i)) {
                return;
            }
        }
        StartCoroutine("Victory");
    }

    IEnumerator Lose() {
        GameManager.Death = true;
        result = -1;
        yield return new WaitForSeconds(2.75f);

        //フェードアウト(0.2秒)
        CommonHUDManager.fadeSwitch = true;
        yield return new WaitForSeconds(1.75f);

        GameManager.Death = false;
        CommonHUDManager.fadeSwitch = false;
        SceneManager.LoadScene("GameOver");
    }
    IEnumerator Victory() {
        player.Victory();
        MarioStatus mario = player.gameObject.GetComponent<MarioStatus>();
        actionId = 0;

        BattleMusicManager music = GameObject.Find("共通HUD").GetComponent<BattleMusicManager>();
        music.VictoryMusic();

        if (earnedEXP <= 0 && !mario.isMaxLevel()) {
            earnedEXP = 1;
        }

        events.isVictory = true;
        events.earnedCoins = earnedCoins;
        earnedCoins = 0;

        result = 1;

        if (earnedEXP > 0) {
            howManyEXP.text = earnedEXP.ToString() + "スターポイントゲット！";
        } else {
            howManyEXP.text = "";
        }

        yield return new WaitForSeconds(0.5f);
        BattleHUDManager.EXPPos = 0;
        mario.StarPoints += earnedEXP;
        earnedEXP = 0;
        
        yield return new WaitForSeconds(3f);

        if (mario.StarPoints >= 100) {
            result = 2;
            BattleHUDManager.EXPPos = 1;
            music.LevelUpMusic();
            levelUp.SetActive(true);
        } else {
            StartCoroutine("BackToField");
        }
    }

    public IEnumerator BackToField() {
        GameObject CommonHUD = GameObject.Find("共通HUD");
        CommonHUDManager.fadeSwitch = true;
        yield return new WaitForSeconds(0.4f);

        CommonHUDManager.fadeSwitch = false;
        CommonHUD.GetComponent<MusicManager>().BackToFieldMusic();
        SceneManager.LoadScene(events.areaIndex);
    }
}
