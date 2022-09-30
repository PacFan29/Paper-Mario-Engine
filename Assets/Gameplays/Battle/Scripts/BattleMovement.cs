using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleMovement : MonoBehaviour
{
    [Header("ステータス")]
    public Status status;
    protected Rigidbody rb;
    protected Vector3 velocity;
    protected float gravity = 2.25f;
    protected bool Grounded = true;
    [Header("リジッドボディ")]
    public LayerMask GroundLayer;
    protected RaycastHit hit;
    [HideInInspector] public Transform target;
    public bool turn;
    public bool flying;
    protected bool useGravity = true;
    protected Vector3 basePos;
    protected int actionId = 0;
    protected List<int> actions = new List<int>();

    protected float waitTime = 0.25f;

    public int power;
    [HideInInspector] public bool ignoreDefense;

    [HideInInspector] public bool isDead = false;

    [Header("アニメーション")]
    public Animator anim;
    protected float damageTime = 0f;
    protected bool isGuarding = false;

    [Header("共通効果音")]
    public AudioClip jumpSound;
    public AudioClip notEffectiveSound;
    public AudioClip damageSound;
    public AudioClip guardSound;
    public AudioClip dieSound;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        basePos = this.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Physics.Raycast(transform.position, -Vector3.up, out hit, 2.4f, GroundLayer);
        if (hit.distance <= 0) {
            Grounded = false;
        }

        if (!Grounded) {
            AirMovement();
        } else {
            velocity.y = 0;
        }

        if (rb != null) rb.velocity = velocity;

        if (!turn) {
            Vector3 pos = new Vector3(basePos.x, this.transform.position.y, basePos.z);
            this.transform.position = pos;
        }
    }

    void AirMovement() {
        if (!flying && useGravity) velocity.y -= gravity;

        Physics.Raycast(transform.position, -Vector3.up, out hit, 2.4f, GroundLayer);

        if (velocity.y < 0 && hit.distance > 0) {
            transform.position -= Vector3.up * (2f - hit.distance);
            Grounded = true;
        }
    }

    public void DamageOrHeal(int power, bool isEnemy, int damagePattern, bool isOver, bool ignoreD) {
        DamageManager damageCanvas = GameObject.Find("ポップアップ用キャンバス").GetComponent<DamageManager>();
        Vector3 plusPos = Vector3.zero;
        if (power > 0) {
            plusPos = new Vector3(0, 4, 0);
        } else {
            if (isEnemy) {
                plusPos = new Vector3(2.5f, 2.5f, 0);
            } else {
                plusPos = new Vector3(-2.5f, 2.5f, 0);
            }

            if (!ignoreD) {
                power = -status.DamageCalc(-power, status.Defense, ignoreD);
            }

            //ダメージ倍数の確認
            MarioStatus mario = null;
            if (this.GetComponent<MarioStatus>() != null) {
                mario = this.GetComponent<MarioStatus>();

                float multiple = (float)mario.statusSaver.GetForces(BadgeData.Badges.Parameter.Mario_Damage, mario.Pinch, mario.Danger);

                power = (int)(Math.Floor((float)power * Math.Pow(2, multiple)));
            }

            if (power >= 0) {
                SoundPlay(notEffectiveSound);
            } else {
                if (isGuarding) {
                    SoundPlay(guardSound);
                    power += 1;

                    if (mario != null) {
                        power += mario.statusSaver.GetForces(BadgeData.Badges.Parameter.Mario_Guard, mario.Pinch, mario.Danger);

                        if (power > 0) {
                            power = 0;
                        }
                    }
                } else {
                    SoundPlay(damageSound);
                }
            }
        }
        damageCanvas.MakePopup(this.transform.position + plusPos, power, false);

        status.HP += power;

        StartCoroutine(DamageAnimation(damagePattern, isOver));
    }
    public virtual IEnumerator DamageAnimation(int damagePattern, bool isOver) {
        yield return null;
    }

    public void SoundPlay(AudioClip sound) {
        if (sound != null) {
            this.GetComponent<AudioSource>().PlayOneShot(sound);
        }
    }

    public virtual void ConsumeTurn() {
        actions.RemoveAt(0);
    }
}
