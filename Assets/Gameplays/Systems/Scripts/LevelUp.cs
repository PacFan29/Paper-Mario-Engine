using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUp : MonoBehaviour
{
    [Header("ステータス")]
    public MarioStatus mario;
    public Options options;
    public PlayerBattleMovements player;
    [Header("マネージャー")]
    public BattleManager manager;

    [Header("スポットライト")]
    public Transform spotLight;
    private int selectIndex = 1;

    [Header("ＨＰ")]
    public Text currentHP;
    public Text afterHP;
    public Text updatedHP;
    public GameObject[] HPDisplay = new GameObject[2];

    [Header("ＦＰ")]
    public Text currentFP;
    public Text afterFP;
    public Text updatedFP;
    public GameObject[] FPDisplay = new GameObject[2];

    [Header("ＢＰ")]
    public Text currentBP;
    public Text afterBP;
    public Text updatedBP;
    public GameObject[] BPDisplay = new GameObject[2];

    private bool[] increasables = new bool[3];

    [Header("詳細")]
    public Text description;
    private string[] helpTexts = {
        "ＨＰの さいだいちが 『５』ふえるぞ！\nバトルに じしんがない人に オススメ",
        "ＦＰの さいだいちが 『５』ふえるぞ！\nワザを たくさん使いたい人に オススメ",
        "ＢＰの さいだいちが 『３』ふえるぞ！\nバッジを たくさんつけたい人に オススメ"
    };

    private bool inputMemory;
    private bool canInput = true;
    // Start is called before the first frame update
    void OnEnable()
    {
        mario.Level++;
        mario.StarPoints -= 100;
    }

    // Update is called once per frame
    void Update()
    {
        //カンスト判定
        increasables[0] = mario.MaxHP < options.maximumHP;
        increasables[1] = mario.MaxFP < options.maximumFP;
        increasables[2] = mario.MaxBP < options.maximumBP;

        //出力の設定
        for (int i = 0; i < increasables.Length; i++) {
            SetUp(i, (increasables[i] && (i != selectIndex || canInput && i == selectIndex)));
        }
        
        //スポットライト
        Quaternion spotRot = Quaternion.Euler(0, 0, 22.5f * (selectIndex-1));
        spotLight.rotation = Quaternion.Lerp(spotLight.rotation, spotRot, 10 * Time.deltaTime);

        //詳細欄
        string[] parameters = {"Ｈ", "Ｆ", "Ｂ"};
        description.text = increasables[selectIndex] ? 
            helpTexts[selectIndex] : 
            parameters[selectIndex]+"Ｐは さいだいだ\nもう これいじょう ふやせないぞ！";

        if (canInput && spotLight.gameObject.activeSelf) {
            //操作
            if (Input.GetAxis("Horizontal") < 0){
                if (!inputMemory && selectIndex > 0){
                    inputMemory = true;
                    selectIndex--;
                }
            } else if (Input.GetAxis("Horizontal") > 0){
                if (!inputMemory && selectIndex < 2){
                    inputMemory = true;
                    selectIndex++;
                }
            } else {
                inputMemory = false;
            }
            if (Input.GetButtonDown("A")) {
                if (increasables[selectIndex]) {
                    canInput = false;
                    StartCoroutine(IncreaseParameter(selectIndex));
                } else {
                    //〇Ｐは さいだいだ
                    //もう これいじょう ふやせないぞ！
                }
            }
        }
    }

    void SetUp(int index, bool increasable) {
        switch (index) {
            case 0:
            currentHP.text = mario.MaxHP.ToString();
            afterHP.text = (mario.MaxHP + 5).ToString();
            updatedHP.text = mario.MaxHP.ToString();

            HPDisplay[0].SetActive(increasable);
            HPDisplay[1].SetActive(!increasable);
            break;
            
            case 1:
            currentFP.text = mario.MaxFP.ToString();
            afterFP.text = (mario.MaxFP + 5).ToString();
            updatedFP.text = mario.MaxFP.ToString();

            FPDisplay[0].SetActive(increasable);
            FPDisplay[1].SetActive(!increasable);
            break;

            case 2:
            currentBP.text = mario.MaxBP.ToString();
            afterBP.text = (mario.MaxBP + 3).ToString();
            updatedBP.text = mario.MaxBP.ToString();

            BPDisplay[0].SetActive(increasable);
            BPDisplay[1].SetActive(!increasable);
            break;
        }
    }

    IEnumerator IncreaseParameter(int index) {
        if (mario.StarPoints < 100) {
            player.ParamIncreased();
        }
        
        switch (index) {
            case 0:
            mario.MaxHP += 5;
            break;
                        
            case 1:
            mario.MaxFP += 5;
            break;

            case 2:
            mario.MaxBP += 3;
            break;
        }
        mario.HP += 999;
        mario.FP += 999;

        yield return new WaitForSeconds(1f);

        if (mario.StarPoints >= 100 && mario.Level < options.MaxLevel()) {
            //スターポイントが100を満たさなくなるまで、繰り返す。
            mario.Level++;

            if (mario.Level == options.MaxLevel()) {
                mario.StarPoints = 0;
            } else {
                mario.StarPoints -= 100;
            }
            canInput = true;
        } else {
            //フィールドへ戻す
            StartCoroutine(manager.BackToField());
        }
    }
}
