using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class PlayerInfo : MonoBehaviour
{
    [Header("スキン")]
    public Transform skin;
    public Animator skinAnim;
    [HideInInspector] public int actionId;

    private bool isBack; //後ろ向きの場合はtrue
    private bool isFlip; //右の場合はtrue
    private float waitingTime = 0f;
    private float damageTime = 0f;
    private float damageAnim = 0f;
    private Vector3 direction;
    private bool oppositeDirection = false;
    private bool isDead = false;

    [Header("ダメージ")]
    public DamageManager damageCanvas;

    private PlayerController controller;
    private MarioStatus status;

    [Header("効果音")]
    public AudioClip dieSound;

    // Start is called before the first frame update
    void Start()
    {
        controller = this.GetComponent<PlayerController>();
        controller.canInput = true;

        status = this.GetComponent<MarioStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Y")) {
            DamageOrHeal(5);
        }

        SkinManager();

        if (
            status.HP <= 0 && this.GetComponent<PlayerPhysics>().velocity.magnitude <= 0 
            && !isDead && GetComponent<PlayerPhysics>().Grounded
        ) {
            controller.canInput = false;
            isDead = true;
            controller.velocitySetUp = Vector3.zero;
            StartCoroutine("Die");
        }

        if (damageTime > 0) {
            damageTime -= Time.deltaTime;
            if (status.HP > 0) {
                if (damageTime <= 2f) {
                    BlinkingSkin((damageTime % 0.1f) / 0.05f < 1);
                } else {
                    BlinkingSkin((damageTime % 0.2f) / 0.1f < 1);
                }
            }
        } else {
            BlinkingSkin(true);
        }
        if (damageAnim > 0) {
            if (GetComponent<PlayerPhysics>().Grounded) damageAnim -= Time.deltaTime;
            if (damageAnim <= 0f && status.HP > 0) {
                controller.canInput = true;
                oppositeDirection = false;
                actionId = 0;
            }

            float friction = 1.125f;
            Vector3 VelNormalized = this.GetComponent<PlayerPhysics>().velocity.normalized;

            float VelX = controller.velocitySetUp.x;
            float VelZ = controller.velocitySetUp.z;
            controller.velocitySetUp.x -= Math.Min(Math.Abs(VelX), friction * Math.Abs(VelNormalized.x)) * Math.Sign(VelX);
            controller.velocitySetUp.z -= Math.Min(Math.Abs(VelZ), friction * Math.Abs(VelNormalized.z)) * Math.Sign(VelZ);
        }
    }

    void SkinManager() {
        /*
        bool isBack; //後ろ向きの場合はtrue
        bool isFlip; //右の場合はtrue
        float xScale = (isFront ^ isFlip) ? -1 : 1;
        */
        Vector3 velocity = this.GetComponent<PlayerPhysics>().velocity;
        Vector3 XZvel = oppositeDirection ? new Vector3(-velocity.x, 0f, -velocity.z) : new Vector3(velocity.x, 0f, velocity.z);
        if (XZvel.magnitude > 0f) {
            direction = XZvel;
        }

        isFlip = direction.x > 0;
        isBack = direction.z > 0;
        FlipRotation(isFlip, isBack);

        /*
        変数名：actionId

        ＜フィールド＞

        00 = 通常
        01 = ハンマー（60°, 120°, -120°, -60°）
        02 = ハンマー（途中、60°, 120°, -120°, -60°）
        03 = ハンマー（叩きつけ、60°, 120°, -120°, -60°）
        04 = ハンマー（空振り、60°, 120°, -120°, -60°）
        05 = ハンマー（0°, 180°）
        06 = ハンマー（途中、0°, 180°）
        07 = ハンマー（叩きつけ、0°, 180°）
        08 = ハンマー（空振り、0°, 180°）
        09 = ダメージ1
        10 = ダメージ2
        11 = ダメージ3（刺された）
        12 = ダメージ4（やけど）
        13 = 倒れた
        14 = 起き上がる
        15 = やられた

        16 = クルリンジャンプ（回転）
        17 = クルリンジャンプ（落下）
        18 = 回転ハンマー振り出し（1段階）
        19 = 回転ハンマー振り出し（2段階）
        20 = 回転ハンマー
        21 = 回転ハンマー（高速）


        ＜バトル＞

        22 = ジャンプ踏みつけ
        23 = ガツーンジャンプ踏みつけ
        24 = ハンマー振り出し開始
        25 = ハンマー振り出し
        26 = ハンマー振り出し（強烈）
        27 = ハンマー叩きつけ
        28 = ハンマー叩きつけ（強烈）
        29 = ガード
        30 = スーパーガード

        31 = 勝利１
        32 = 勝利２
        33 = レベルアップ！
        */
        skinAnim.SetInteger("Action ID", actionId);
        skinAnim.SetFloat("Speed", XZvel.magnitude);
        skinAnim.SetFloat("Y Velocity", velocity.y);
        skinAnim.SetBool("Grounded", GetComponent<PlayerPhysics>().Grounded);
        skinAnim.SetBool("Pinch", status.Pinch || status.Danger || status.HP <= 0);
        skinAnim.SetFloat("Waiting Time", waitingTime);
        skinAnim.SetBool("Is Battle", false);

        if (velocity == Vector3.zero) {
            waitingTime += Time.deltaTime;
        } else {
            waitingTime = 0f;
        }

        void FlipRotation(bool isFlip, bool isBack) {
            float xScale = (isBack ^ isFlip) ? -1 : 1;
            float dirToGo = (Math.Abs(direction.z) >= 0.5 && controller.canInput) ? (25 * -xScale) : 0f;

            skin.localScale = new Vector3(xScale, 1, 1);
            skin.rotation = Quaternion.Euler(0, (isBack ? 180 : 0) + dirToGo, 0);
            //skin.rotation = Quaternion.Lerp(skin.rotation, Quaternion.Euler(0, (isBack ? 180 : 0) + dirToGo, 0), Time.deltaTime*20f);

            /*
            !isFlip && !isBack : 上90、右-90、右上-90
            isFlip && !isBack : 左90、上-90、左上-90
            !isFlip && isBack : 上?、右?、右上?
            isFlip && isBack : 左?、上?、左上?
            */
        }
    }
    void BlinkingSkin(bool enabled) {
        Renderer[] childrenRenderer = skin.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < childrenRenderer.Length; i++){
			childrenRenderer[i].enabled = enabled;
		}
    }

    public void DamageOrHeal(int value) {
        damageCanvas.MakePopup(this.transform.position, value, false);
        status.HP += value;
    }

    public bool isDamage() {
        return damageTime > 0;
    }
    public void TakeDamage(Vector3 otherPosition, HazardType damageType) {
        oppositeDirection = true;
        controller.canInput = false;
        damageAnim = 0.75f;
        switch (damageType) {
            case HazardType.Spike:
            actionId = 11;
            break;

            case HazardType.Fire:
            controller.SoundPlay(controller.burnedSound);
            actionId = 12;
            break;
            
            default:
            controller.SoundPlay(controller.damageSound);
            actionId = 9;
            break;
        }
        DamageOrHeal(-1);
        damageTime = 5f;

        Vector3 thisPos = this.transform.position;
        (thisPos.y, otherPosition.y) = (0, 0);
        Vector3 direction = this.transform.position - otherPosition;

        controller.velocitySetUp = direction * 10f;
    }
    public IEnumerator Die() {
        yield return new WaitForSeconds(1f);
        GameManager.Death = true;
        controller.SoundPlay(dieSound);
        actionId = 15;

        yield return new WaitForSeconds(4.5f);
        GameManager.Death = false;
        SceneManager.LoadScene("GameOver");
    }
}
