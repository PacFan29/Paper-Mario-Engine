using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUDManager : MonoBehaviour
{
    [Header("マネージャー")]
    public BattleManager manager;
    private bool isVictory;
    [Header("アクション選択")]
    public Sprite[] jumpHammerIcons = new Sprite[6];
    public Sprite[] otherIcons = new Sprite[4];
    [Header("ピンチ・危険")]
    public GameObject pinchDisplay;
    [Header("スターポイント表示")]
    public GameObject EXPGroup;
    public Sprite[] EXPSprites = new Sprite[4];
    public GameObject smallSpace;
    public GameObject smallPts;
    public GameObject bigPts;
    public RectTransform howManyEXP;
    private int latestAmount;
    public static int EXPPos = -1;

    private Color[] rankColor = new Color[5]{
        new Color (0f, 0.5f, 1f, 1f), //青（ナイス）
        new Color (0f, 0.8f, 0f, 1f), //緑（グッド）
        new Color (1f, 0.8f, 0f, 1f), //黄（グレイト）
        new Color (1f, 0f, 0f, 1f), //赤（ワンダフル）
        new Color (1f, 0f, 1f, 1f) //桃（エクセレント）
    };
    private string[] rankText = new string[5]{
        "ナイス",
        "グッド",
        "グレイト",
        "ワンダフル",
        "エクセレント"
    };
    [Header("ナイス、グッド、グレイト・・・")]
    public GameObject rank;
    private int rankMemory = -1;
    public static int rankValue = -1;
    private bool rankAppear;

    private Color[] guardColor = new Color[2]{
        new Color (0f, 0.65f, 0.84f, 1f), //青（ガード）
        new Color (1f, 0.8f, 0f, 1f) //黄（スーパーガード）
    };
    private string[] guardText = new string[2]{
        "ガード",
        "スーパーガード"
    };
    [Header("ガード")]
    public GameObject guard;
    private int guardMemory = -1;
    public static int guardValue = -1;
    private bool guardAppear;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("EXPAnimation");

        EXPPos = -1;
    }

    void FixedUpdate() {
        if (BattleManager.earnedEXP > latestAmount) {
            latestAmount++;
            if (BattleManager.earnedEXP == latestAmount) BattleManager.ready = true;
        } else if (BattleManager.earnedEXP < latestAmount) {
            latestAmount--;
            if (BattleManager.earnedEXP == latestAmount) BattleManager.ready = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        isVictory = manager.result == 1;

        bool isThinking = (
            (BattleManager.actionId == 0) ||
            (BattleManager.actionId == 1) ||
            (BattleManager.actionId == 3) ||
            (BattleManager.actionId == 4)
        ) && manager.result == 0;
        pinchDisplay.SetActive(isThinking);

        EXPDisplay();
        RankDisplay();
        GuardDisplay();
    }

    void EXPDisplay() {
        //初期化
        foreach (Transform obj in EXPGroup.transform) {
            if ( 0 <= obj.gameObject.name.LastIndexOf("Clone") ) {
                Destroy(obj.gameObject);
            }
        }

        int small = latestAmount % 10;
        int big = latestAmount / 10;

        //値が0の場合は、隠す。
        smallPts.SetActive(small > 0);
        bigPts.SetActive(big > 0);

        smallSpace.SetActive(isVictory && latestAmount > 0);

        RectTransform group = (RectTransform)EXPGroup.transform;
        RectTransform bigTrans = (RectTransform)bigPts.transform;
        if (isVictory) {
            group.anchoredPosition = new Vector3(0, 30, 0);
            group.anchorMin = new Vector2(0.5f, 0.5f);
            group.anchorMax = new Vector2(0.5f, 0.5f);

            bigTrans.localPosition = new Vector3((big-1) * (bigTrans.sizeDelta.x / 2f), 19f, 0);
        } else {
            group.anchoredPosition = new Vector3(-156, 52, 0);
            group.anchorMin = new Vector2(1, 0);
            group.anchorMax = new Vector2(1, 0);

            bigTrans.localPosition = new Vector3(112.5f, 19f, 0);
        }

        for ( int i = 1 ; i < small ; i++ ) {
            //1の位の複製
            RectTransform scoreimage = (RectTransform)Instantiate(smallPts).transform;
            scoreimage.SetParent(EXPGroup.transform , false);
            scoreimage.localPosition = new Vector2(
                scoreimage.localPosition.x - scoreimage.sizeDelta.x * i ,
                scoreimage.localPosition.y);
        }
        for ( int i = 1 ; i < big ; i++ ) {
            //10の位の複製
            RectTransform scoreimage = (RectTransform)Instantiate(bigPts).transform;
            scoreimage.SetParent(EXPGroup.transform , false);
            scoreimage.localPosition = new Vector2(
                scoreimage.localPosition.x - scoreimage.sizeDelta.x * i ,
                scoreimage.localPosition.y);
        }

        howManyEXP.localPosition = Vector3.Lerp(
            howManyEXP.localPosition, 
            new Vector3(EXPPos * 1500f, howManyEXP.localPosition.y, howManyEXP.localPosition.z), 
            7 * Time.deltaTime
        );
    }

    IEnumerator EXPAnimation() {
        int index = 0;

        while (true) {
            yield return new WaitForSeconds(0.1f);

            index++;
            index %= 4;
            foreach (Transform obj in EXPGroup.transform) {
                if (obj.gameObject.GetComponent<Image>().type == Image.Type.Simple) {
                    obj.gameObject.GetComponent<Image>().sprite = EXPSprites[index];
                }
            }
        }
    }

    void RankDisplay() {
        Animator rankAnim = rank.GetComponent<Animator>();
        Text rankTx = rank.transform.GetChild(0).GetComponent<Text>();

        if (rankValue != rankMemory) {
            rankAppear = (rankValue > rankMemory);
            if (rankAppear) rankAnim.Play("RankAppear", 0, 0);

            rankValue = Mathf.Clamp(rankValue, -1, 4);
            rankMemory = rankValue;
        }

        if (rankValue >= 0) {
            rankTx.text = rankText[rankValue];
            rankTx.color = rankColor[rankValue];
        }

        rankAnim.SetBool("Appear", rankAppear);
    }
    void GuardDisplay() {
        Animator guardAnim = guard.GetComponent<Animator>();
        Text guardTx = guard.transform.GetChild(0).GetComponent<Text>();

        if (guardValue != guardMemory) {
            guardAppear = (guardValue > guardMemory);
            if (guardAppear) guardAnim.Play("GuardAppear", 0, 0);

            guardValue = Mathf.Clamp(guardValue, -1, 1);
            guardMemory = guardValue;
        }

        if (guardValue >= 0) {
            guardTx.text = guardText[guardValue];
            guardTx.color = guardColor[guardValue];
        }

        guardAnim.SetBool("Appear", guardAppear);
    }
}
