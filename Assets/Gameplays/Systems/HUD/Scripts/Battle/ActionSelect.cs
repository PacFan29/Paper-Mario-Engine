using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionSelect : MonoBehaviour
{
    [Header("マネージャー")]
    public BattleManager manager;
    [Header("勇者・仲間")]
    public GameObject player;
    public GameObject partner;
    private const float speed = 2f;
    private int num = 5;
    [Header("仲間か否か")]
    public bool isPartner = false;

    [Header("アイコン")]
    public Image imgBase;
    public Sprite[] JumpSprites = new Sprite[3];
    public Sprite[] HammerSprites = new Sprite[3];
    public Sprite ItemsSprite;
    public Sprite SpecialsSprite;
    public Sprite TacticsSprite;
    public Sprite PartnerAbilitiesSprite;

    private float r;       // 半径
 
    // 回転に利用する
    private List <RectTransform> transes;
    private List<float> startRads = new List<float>();
    
    // cache
    private Transform myTrans;

    private float rotate = 180;
    private float latestDirection = 180;
    private bool inputMemory = false;

    private float piece;
    private string[] names;
    [HideInInspector] public bool canSelect = true;

    [Header("項目名")]
    public Text nameText;
    // Start is called before the first frame update
    void OnEnable()
    {
        MarioStatus mario = player.GetComponent<MarioStatus>();
        foreach (Transform obj in this.transform) {
            if (0 <= obj.gameObject.name.LastIndexOf("Clone")) {
                Destroy(obj.gameObject);
            }
        }
        if (isPartner) {
            num = 3;
        } else {
            //ハンマーを持っているかを判定
            num = (mario.HammerRank > 0) ? 5 : 4;
        }

        transes = new List<RectTransform>();

        // キャッシュする
        myTrans = transform;

        // 半径を取得
        r = imgBase.rectTransform.localPosition.y;
    
        // ベースから生成
        GameObject objBase = imgBase.gameObject;
        for (int i = 0; i < num; i++) {
            GameObject obj = Instantiate(objBase);
            obj.transform.SetParent(myTrans, false);
        
            RectTransform trans = obj.GetComponent<RectTransform>();

            if (isPartner) {
                switch (i) {
                    case 0:
                    obj.GetComponent<Image>().sprite = PartnerAbilitiesSprite;
                    break;
                    
                    case 1:
                    obj.GetComponent<Image>().sprite = ItemsSprite;
                    break;

                    case 2:
                    obj.GetComponent<Image>().sprite = TacticsSprite;
                    break;
                }
                names = new string[3]{"ワザ", "さくせん", "アイテム"};
            } else {
                int j = (num == 4 && i > 0) ? i + 1 : i;
                switch (j) {
                    case 0:
                    obj.GetComponent<Image>().sprite = JumpSprites[(mario.BootsRank - 1)];
                    break;
                    
                    case 1:
                    obj.GetComponent<Image>().sprite = HammerSprites[(mario.HammerRank - 1)];
                    break;

                    case 2:
                    obj.GetComponent<Image>().sprite = ItemsSprite;
                    break;

                    case 3:
                    obj.GetComponent<Image>().sprite = SpecialsSprite;
                    break;

                    case 4:
                    obj.GetComponent<Image>().sprite = TacticsSprite;
                    break;
                }
                names = new string[5]{"ジャンプ", "さくせん", "スペシャル", "アイテム", "ハンマー"};
            }
        
            // 角度
            float angle = i * (360 / num) + 90f;
            // ラジアン
            float rad = angle * Mathf.Deg2Rad;
            // 座標変換
            float x = Mathf.Cos(rad) * r;
            float y = Mathf.Sin(rad) * r;
        
            // 初期位置
            trans.anchoredPosition = new Vector2(x, y);

            objBase.GetComponent<Image>().color = new Color(0,0,0,0);
        
            // update用に取得
            transes.Add(trans);
            startRads.Add(rad);
        }

        rotate = 180;
        latestDirection = rotate;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPartner) {
            GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main, partner.transform.position + (Vector3.up * 7f));
        } else {
            GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main, player.transform.position + (Vector3.up * 7f));
        }

        piece = (360 / num);
        int index = (int)((latestDirection + 180) / piece) % num;
        if (index < 0) index += num;

        if (canSelect){
            if (Input.GetAxis("Horizontal") < 0){
                if (!inputMemory){
                    inputMemory = true;
                    latestDirection += piece;
                }
            } else if (Input.GetAxis("Horizontal") > 0){
                if (!inputMemory){
                    inputMemory = true;
                    latestDirection -= piece;
                }
            } else {
                inputMemory = false;
            }
            if (Input.GetButtonDown("A")){
                if (isPartner) {
                    switch (index) {
                        case 0:
                        //技
                        break;
                        
                        case 1:
                        //作戦
                        break;
                        
                        case 2:
                        //アイテム
                        break;
                    }
                } else {
                    switch (index) {
                        case 0:
                        //ジャンプ
                        manager.abilityId = 0;
                        break;
                        
                        case 1:
                        //作戦
                        break;
                        
                        case 2:
                        //スペシャル
                        break;
                        
                        case 3:
                        //アイテム
                        break;

                        case 4:
                        //ハンマー
                        manager.abilityId = 1;
                        break;
                    }
                }

                manager.selected = true;
            }
        }

        // 回転処理
        for (int i = 0; i < transes.Count; i++) {
            float nowRad = (rotate * Mathf.Deg2Rad) + startRads[i];
            float x = Mathf.Cos(nowRad) * r;
            float y = Mathf.Sin(nowRad) * r / 3;
        
            transes[i].anchoredPosition = new Vector2(x, y);

            float percent = (y / -58f) + 0.5f;
            transes[i].GetComponent<Image>().color = new Color(percent,percent,percent,1);
            
            if (y < 0){
                // 最前面に移動
                transes[i].GetComponent<RectTransform>().SetAsLastSibling();
            } else {
                // 最背面に移動
                transes[i].GetComponent<RectTransform>().SetAsFirstSibling();
            }
        }
        if (rotate < latestDirection){
            rotate += 500 * Time.deltaTime;
            if (rotate >= latestDirection){
                rotate = latestDirection;
            }
        } else if (rotate > latestDirection){
            rotate -= 500 * Time.deltaTime;
            if (rotate <= latestDirection){
                rotate = latestDirection;
            }
        } else {
            rotate %= 360;
            latestDirection %= 360;
        }

        //項目名
        nameText.text = names[index];
        nameText.gameObject.transform.parent.GetComponent<RectTransform>().SetAsLastSibling();
    }
}
