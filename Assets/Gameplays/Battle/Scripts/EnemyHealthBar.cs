using System;
using UnityEngine;
using UnityEngine.UI;

//RGBの共通部分をクラスにする
class ColorController {
    //変数の宣言
    private float red;
    private float green;
    private float blue;

    //インスタンス化
    public ColorController(){}
    public ColorController (float r, float g, float b){
        red = r;
        green = g;
        blue = b;
    }

    //セッター
    public void setRed(float r){
        red = r;
    }
    public void setGreen(float g){
        green = g;
    }
    public void setBlue(float b){
        blue = b;
    }

    //ゲッター
    public float getRed(){
        return red;
    }
    public float getGreen(){
        return green;
    }
    public float getBlue(){
        return blue;
    }
}

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField]
    private Transform targetTfm;
 
    private RectTransform myRectTfm;

    public Image HealthAmount;
    public Image DamageAmount;
    public Text HealthValue;
    private float color = 0.0f;
    private float c_switch = 1.0f;

    private int en_hp;
    private int max_en_hp;

    private float amountRatio;
    private float hp_am;
    private float dm_am;

    private float time;
    private int amountMemory;

    private ColorController[] colors = new ColorController[3];
 
    void Start() {
        myRectTfm = GetComponent<RectTransform>();

        max_en_hp = 32767;
        hp_am = 1.0f;
        dm_am = 1.0f;
        time = 0f;
        amountMemory = en_hp;

        colors[0] = new ColorController(HealthAmount.color.r, HealthAmount.color.g, HealthAmount.color.b);
        colors[1] = new ColorController(DamageAmount.color.r, DamageAmount.color.g, DamageAmount.color.b);
        colors[2] = new ColorController(HealthValue.color.r, HealthValue.color.g, HealthValue.color.b);
    }
 
    void Update() {
        Vector3 offset = new Vector3(0, -2f, 0);

        myRectTfm.position 
            = RectTransformUtility.WorldToScreenPoint(Camera.main, targetTfm.position + offset);

        color += 3f * c_switch * Time.deltaTime;
        if (color >= 0.35f && c_switch > 0 || color <= 0.0f && c_switch < 0){
            c_switch *= -1;
        }
        colors[1].setRed(1f);
        colors[1].setGreen(color + 0.35f);
        colors[1].setBlue(0.93f);

        EnemyStatus status = targetTfm.gameObject.GetComponent<EnemyStatus>();
        en_hp = status.HP;
        max_en_hp = status.MaxHP;

        if (en_hp != amountMemory){
            time = 0.8f;
            amountMemory = en_hp;
        }

        amountRatio = (float)en_hp / max_en_hp;
        if (hp_am > amountRatio) {
            hp_am -= (hp_am - amountRatio) / 10.0f;
        } else if (hp_am < amountRatio) {
            hp_am += (amountRatio - hp_am) / 10.0f;
        }
        if (time <= 0){
            if (dm_am > hp_am) {
                dm_am -= (dm_am - hp_am) / 10.0f;
            } else if (dm_am < hp_am) {
                dm_am += (hp_am - dm_am) / 10.0f;
            }
        } else {
            time -= Time.deltaTime;
        }

        if (en_hp > max_en_hp){
            en_hp = max_en_hp;
        } else if (en_hp < 0){
            en_hp = 0;
        }

        HealthAmount.fillAmount = hp_am;
        DamageAmount.fillAmount = dm_am;
        HealthValue.text = en_hp.ToString();

        HealthAmount.color = new Color(colors[0].getRed(), colors[0].getGreen(), colors[0].getBlue(), 1f);
        DamageAmount.color = new Color(colors[1].getRed(), colors[1].getGreen(), colors[1].getBlue(), 1f);
        HealthValue.color = new Color(colors[2].getRed(), colors[2].getGreen(), colors[2].getBlue(), 1f);
    }
}
