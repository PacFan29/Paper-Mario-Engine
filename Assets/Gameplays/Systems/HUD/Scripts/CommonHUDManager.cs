using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CommonHUDManager : MonoBehaviour
{
    [Header("ステータス")]
    public Options options;
    public StatusSaver statusSaver;
    public ItemData itemData;
    [Header("値グループ")]
    public RectTransform HPValueGroup;
    public RectTransform FPValueGroup;
    public RectTransform PartnerHPValueGroup;
    [Header("各値の出力")]
    public Text HPValue;
    public Text MaxHPValue;
    public Text FPValue;
    public Text MaxFPValue;
    public Text PartnerHPValue;
    public Text PartnerMaxHPValue;
    public Text StarPointsValue;
    public Text CoinsValue;
    [Header("緊急キノコ")]
    public GameObject lifeShroomIcon;
    public Text lifeShroomAmounts;
    [Header("アニメーション")]
    public Animator StarPtsAnimator;
    public Animator CoinsAnimator;
    public PlayableDirector PinchScreen;
    public Image fade;
    private float fadeOpacity = 1f;
    public static bool fadeSwitch;

    private int[] currentValues = new int[5];
    private int[] maxValues = new int[3];
    private float[] basePos = new float[3];

    [Header("バトル開始演出")]
    public RawImage battleImg;
    public VideoPlayer battleVideo;
    // Start is called before the first frame update
    void Start()
    {
        basePos[0] = HPValueGroup.anchoredPosition.x;
        basePos[1] = FPValueGroup.anchoredPosition.x;
        basePos[2] = PartnerHPValueGroup.anchoredPosition.x;

        battleImg.color = new Color(1, 1, 1, 0);
        battleVideo.Prepare();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GameObject findPlayer = GameObject.FindGameObjectWithTag("Player");
        MarioStatus mario = null;
        if (findPlayer != null && findPlayer.GetComponent<MarioStatus>() != null) {
            mario = findPlayer.GetComponent<MarioStatus>();
        }

        if (mario != null) {
            //ＨＰ
            HPFPDisplaySet(HPValueGroup, HPValue, MaxHPValue, 0);
            maxValues[0] = mario.FinalMaxHP;
            ValueController(statusSaver.HP, 0);
            //ＦＰ
            HPFPDisplaySet(FPValueGroup, FPValue, MaxFPValue, 1);
            maxValues[1] = mario.FinalMaxFP;
            ValueController(statusSaver.FP, 1);
        }
        //仲間ＨＰ
        HPFPDisplaySet(PartnerHPValueGroup, PartnerHPValue, PartnerMaxHPValue, 2);
        maxValues[2] = 10;
        ValueController(10, 2);

        //スターポイント（経験値）
        if (statusSaver.Level == options.MaxLevel()) {
            StarPointsValue.text = "-";
        } else {
            StarPointsValue.text = Math.Min(statusSaver.StarPoints, 100).ToString();
        }

        //コイン
        CoinsValue.text = statusSaver.Coins.ToString();

        //ピンチ！
        if (mario != null) {
            bool pinch = false;

            if (
                (mario.HP <= 0 || mario.Pinch || mario.Danger) && (mario.HP != mario.MaxHP) && 
                (!GameManager.Death || SceneManager.GetActiveScene().name == "BattleScene")
            ) {
                PinchScreen.Play();
                pinch = SceneManager.GetActiveScene().name == "BattleScene";
            } else {
                PinchScreen.time = 0;
                PinchScreen.Evaluate();
                PinchScreen.Stop();

                pinch = false;
            }

            if (GetComponent<AudioLowPassFilter>() != null) {
                GetComponent<AudioLowPassFilter>().cutoffFrequency = pinch ? 2000f : 22000f;
            }
        }

        //現在持っている緊急キノコの数
        int lifeShroomCount = 0;
        foreach (int itemIndex in statusSaver.items) {
            foreach (ItemData.Effects effect in itemData.data[itemIndex].effects) {
                if (effect.parameter == ItemData.Parameter.Dead) {
                    lifeShroomCount++;
                }
            }
        }
        lifeShroomIcon.SetActive(lifeShroomCount > 0);
        if (lifeShroomCount > 1) {
            lifeShroomAmounts.text = lifeShroomCount.ToString();
        } else {
            lifeShroomAmounts.text = "";
        }

        //フェード
        if (fadeSwitch) {
            fadeOpacity += 5 * Time.deltaTime;
        } else {
            fadeOpacity -= 5 * Time.deltaTime;
        }
        fadeOpacity = Mathf.Clamp(fadeOpacity, 0f, 1f);

        fade.color = new Color(0f, 0f, 0f, fadeOpacity);
    }

    void ValueController(int currentValue, int index) {
        if (currentValues[index] != currentValue) {
            int add = Math.Max(1, (int)(Math.Abs(currentValue - currentValues[index]) * 0.1));
            int addSign = Math.Sign(currentValue - currentValues[index]);

            currentValues[index] += (add * addSign);
        }
    }
    void HPFPDisplaySet(RectTransform valueGroup, Text value, Text maxValue, int index) {
        value.text = (currentValues[index]).ToString();
        maxValue.text = "/" + maxValues[index].ToString();

        valueGroup.anchoredPosition = HPFPPosSet(
            valueGroup.anchoredPosition, 
            basePos[index], 
            maxValues[index]
        );
    }
    void StarPtsCoinAnimations(Animator animator, bool coins) {
        if (coins) {
            ;
        } else {
            ;
        }
    }

    //最大HP・FPに応じて位置を変えるメソッド
    Vector3 HPFPPosSet (Vector3 position, float main, int value){
        //位置の初期化
        position = new Vector3 (main, position.y, position.z);

        //桁ごとにずらす
        position.x -= 26.5f * (3 - value.ToString().Length);

        //指定した位置を返す
        return position;
    }

    public void ReturnToField() {
        //SceneManager.LoadScene(0);
        Destroy(gameObject);
    }
    public void BattleProduction() {
        if (battleVideo.isPrepared) {
            battleImg.color = new Color(1, 1, 1, 1);
            battleImg.texture = battleVideo.texture;

            battleVideo.time = 0f;
            battleVideo.Play();
        }
    }
}
