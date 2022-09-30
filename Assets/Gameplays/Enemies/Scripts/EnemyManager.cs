using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public abstract class EnemyManager : MonoBehaviour
{
    protected int enemyId;
    
    //リジッドボディー
    private Rigidbody rb;
    protected Vector3 GroundNormal = Vector3.up;
    public LayerMask GroundLayer;
    private RaycastHit hit;
    protected Vector3 XZvel;
    private Vector3 velocity;
    protected bool Grounded = false;
    //プレイヤー
    protected Transform player;

    [Header("データ")]
    public int[] otherEnemyIds = new int[4];
    public EnemyDataList enemyList;
    public BattleData data;
    public EventData events;
    public BadgeData badgeData;
    [Header("状態")]
    public bool attacking;
    public bool spiked;
    [Header("エフェクト")]
    public GameObject defeatedEffect;
    [Header("コイン")]
    public GameObject coin;
    private int earnedCoins;
    [Header("バトルBGM")]
    public BattleMusicManager battleMusic;

    private bool touchable = true;
    private int situation = 0;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        GameObject[] findPlayer = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject pl in findPlayer) {
            if (pl.GetComponent<MarioStatus>() != null) {
                player = pl.GetComponent<Transform>();
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        (velocity.x, velocity.z) = (XZvel.x, XZvel.z);

        Physics.Raycast(transform.position, -Vector3.up, out hit, 2.2f, GroundLayer);
        if (hit.distance <= 0) {
            Grounded = false;
        }
        
        //着地していない場合
        if (Grounded) {
            velocity.y = 0f;
        } else {
            GroundNormal = Vector3.up;
            AirMovement();
        }

        Vector3 finalVelocity = Vector3.zero;
        finalVelocity += velocity.x * Vector3.right;
        finalVelocity += velocity.y * Vector3.up;
        finalVelocity += velocity.z * Vector3.forward;
        rb.velocity = finalVelocity;

        if (!touchable) {
            if (events.isVictory) {
                events.isVictory = false;
                earnedCoins = events.earnedCoins;
                situation = 1;
            }

            switch (situation) {
                case 0:
                //逃げられた
                break;

                case 1:
                //破られた
                StartCoroutine("Defeated");
                break;
            }
        }
    }
    void AirMovement() {
        float gravity = 2.25f;

        velocity.y -= gravity;

        Physics.Raycast(transform.position, -Vector3.up, out hit, 2.2f, GroundLayer);

        if (velocity.y < 0 && hit.distance > 0) {
            transform.position += Vector3.up * (2f - hit.distance);
            Grounded = true;
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Player" && touchable) {
            //スターポイントがもらえる状態か確認
            StatusSaver marioStatus = other.gameObject.GetComponent<MarioStatus>().statusSaver;
            bool isWeak = enemyList.enemyDatas[enemyId].Level <= marioStatus.Level;

            //トゲーヲガードの認識
            bool spikeProtected = IsProtected(marioStatus);
            //センセイサレナイの認識
            bool getAttacked = FieldSituation(marioStatus, BadgeData.Badges.FieldSituation.GetAttacked);
            //ヤッツケアタックの認識
            bool attackEnemy = FieldSituation(marioStatus, BadgeData.Badges.FieldSituation.AttackEnemy) && isWeak;
            //ヤッツケーレの認識
            bool touchedEnemy = FieldSituation(marioStatus, BadgeData.Badges.FieldSituation.TouchedEnemy) && isWeak;

            PlayerInfo marioInfo = other.gameObject.GetComponent<PlayerInfo>();

            int attackIndex = 0;
            if (Stomped(other.gameObject)) {
                data.abilityId = 0;

                if (spiked && !spikeProtected) {
                    attackIndex = 2;
                } else {
                    attackIndex = 1;
                }
                if (marioInfo.actionId == 16 || marioInfo.actionId == 17) {
                    //クルリンジャンプで先制攻撃（いいよなぁ、FPを使わずにクルリンジャンプができることを。）
                    data.actionId = 1;
                } else {
                    //ジャンプで先制攻撃
                    data.actionId = 0;
                }
                other.gameObject.GetComponent<PlayerController>().Stomp();

                data.beginning = BattleData.Beginning.PlayerFirstStrike;
            } else {
                attackIndex = attacking ? 2 : 0;

                if (attacking) {
                    data.beginning = BattleData.Beginning.EnemyFirstStrike;
                } else {
                    data.beginning = BattleData.Beginning.Normal;
                }
            }
            if (getAttacked && attackIndex == 2) attackIndex = 0;

            if (touchedEnemy || (attackEnemy && attackIndex == 1)) {
                situation = 1;
                touchable = false;
                return;
            }

            BattleStart(attackIndex);
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "PlayerHammer" && touchable) {
            //スターポイントがもらえる状態か確認
            StatusSaver marioStatus = GameObject.Find("マリオ").GetComponent<MarioStatus>().statusSaver;
            bool isWeak = enemyList.enemyDatas[enemyId].Level <= marioStatus.Level;

            //ヤッツケアタックの認識
            bool attackEnemy = FieldSituation(marioStatus, BadgeData.Badges.FieldSituation.AttackEnemy) && isWeak;
            //ヤッツケーレの認識
            bool touchedEnemy = FieldSituation(marioStatus, BadgeData.Badges.FieldSituation.TouchedEnemy) && isWeak;

            PlayerInfo marioInfo = other.gameObject.GetComponent<PlayerInfo>();

            data.beginning = BattleData.Beginning.PlayerFirstStrike;
            data.abilityId = 1;
            data.actionId = 0;

            if (attackEnemy || touchedEnemy) {
                situation = 1;
                touchable = false;
                return;
            }

            BattleStart(1);
        }
    }
    void BattleStart(int attackIndex) {
        //バトルシーンへ移動
        battleMusic.BattleMusic(attackIndex);

        data.enemyIds = new List<int>();
        data.enemyIds.Add(enemyId);

        for (int i = 1; i < otherEnemyIds.Length + 1; i++) {
            data.enemyIds.Add(otherEnemyIds[i-1]);
        }
        StartCoroutine("PrepareForBattle");

        touchable = false;
    }
    IEnumerator PrepareForBattle() {
        yield return new WaitForSeconds(0.25f);
        battleMusic.gameObject.GetComponent<CommonHUDManager>().BattleProduction();
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(3.5f);
        Time.timeScale = 1f;
        SceneManager.LoadScene("BattleScene");
    }

    void YourSituation() {
        ;
    }

    bool Stomped(GameObject col) {
        float halfHeight = GetComponent<CapsuleCollider>().height * 0.5f;

        float playerY = col.transform.position.y - (halfHeight * this.transform.localScale.y);
        float thisY = this.transform.position.y;

        return playerY >= thisY;
    }
    bool IsProtected(StatusSaver status) {
        //マリオがトゲーヲガードをつけているか確認
        return status.AbilityCheck(BadgeData.Badges.Ability.ProtectSpikes);
    }
    bool FieldSituation(StatusSaver status, BadgeData.Badges.FieldSituation situation) {
        return status.FieldSituation(situation);
    }

    IEnumerator Defeated() {
        yield return new WaitForSeconds(1f);

        int count = 0;
        float XZSpeed = 8f;
        earnedCoins = Mathf.Clamp(earnedCoins, 0, 64);

        while (earnedCoins > 0) {
            int coinLayer = Math.Min(16, earnedCoins);

            for (int i = 0; i < coinLayer; i++) {
                float angle = (360f / coinLayer) * i;

                CoinManager thisCoin = Instantiate(coin, this.transform.position, Quaternion.identity).GetComponent<CoinManager>();
                thisCoin.split = true;

                thisCoin.velocity.x = (float)Math.Sin((double)angle * (Math.PI / 180)) * XZSpeed;
                thisCoin.velocity.z = (float)Math.Cos((double)angle * (Math.PI / 180)) * XZSpeed;
            }
            earnedCoins -= 16;
            XZSpeed += (count % 2 == 0 ? -4f : 8f);
            count++;
        }

        float height = this.GetComponent<CapsuleCollider>().height / 2f;
        height *= this.transform.localScale.y;

        Instantiate(defeatedEffect, transform.position + Vector3.down * height, Quaternion.identity);

        Destroy(gameObject);
    }
}
