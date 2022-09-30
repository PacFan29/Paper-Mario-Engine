using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattleMovements : BattleMovement
{
    [Header("エフェクト")]
    public GameObject smallEffect;
    public GameObject bigEffect;
    public GameObject starPoints;
    [Header("データ")]
    public BattleData battleData;
    [Header("ゲージ")]
    public GameObject HPBar;

    protected int jumpCount = 0;

    // Update is called once per frame
    void Update()
    {
        if (turn) {
            Turn();
        }

        SkinAnimation();
    }

    public void SkinAnimation() {
        Vector3 XZvel = new Vector3(velocity.x, 0f, velocity.z);

        /*
        変数名：actionId
        00 = 通常
        01 = ダメージを受けた
        02 = 踏まれた
        03 = ?
        04 = やられた
        */
        anim.SetInteger("Action ID", actionId);
        anim.SetFloat("Speed", XZvel.magnitude);
        anim.SetFloat("Y Velocity", velocity.y);
        anim.SetBool("Grounded", Grounded);
        anim.SetBool("Pinch", false);

        if (damageTime > 0) {
            damageTime -= Time.deltaTime;

            if(damageTime <= 0f && actionId != 0 && status.HP > 0){
                actionId = 0;
                BattleManager.ready = true;
            }
        }
    }

    public virtual void Turn() {
        ;
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.GetComponent<PlayerBattleMovements>() != null && !turn) {
            //踏まれた時の処理
            PlayerBattleMovements mario = other.gameObject.GetComponent<PlayerBattleMovements>();
            mario.Stomped();
        }
    }

    public override IEnumerator DamageAnimation(int damagePattern, bool isOver) {
        string animName = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        if (actionId != 0) anim.Play(animName, 0, 0);
        actionId = damagePattern + 1;

        switch (actionId) {
            case 0:
            damageTime = 1f;
            break;

            case 1:
            damageTime = 0.84f;
            break;
            
            case 2:
            damageTime = 1f;
            break;
        }

        yield return new WaitForSeconds(0.1f);

        if (status.HP <= 0 && isOver) {
            isDead = true;
            actionId = 4;
            yield return new WaitForSeconds(1.34f);

            Defeated();
        }
    }

    public void Defeated() {
        float height = this.GetComponent<CapsuleCollider>().height / 2f;
        height *= this.transform.localScale.y;

        Instantiate(smallEffect, transform.position + Vector3.down * height, Quaternion.identity);

        this.GetComponent<CapsuleCollider>().isTrigger = true;
        isDead = true;

        int exp = this.GetComponent<EnemyStatus>().EXPCalc(battleData.enemyIds.Count);
        
        BattleManager.earnedEXP += exp;
        if (exp <= 0) BattleManager.ready = true;

        EXPCreator creator = Instantiate(starPoints, transform.position, Quaternion.identity).GetComponent<EXPCreator>();
        creator.amounts = exp;

        BattleManager.earnedCoins += this.GetComponent<EnemyStatus>().Coins;
        
        anim.gameObject.SetActive(false);
        HPBar.SetActive(false);
    }

    public Transform GetPlayer() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players) {
            if (player.GetComponent<PlayerBattleMovements>() != null) {
                return player.transform;
            }
        }

        return null;
    }
    public Transform GetPartner() {
        return null;
    }
    public void SetTarget() {
        int select = UnityEngine.Random.Range(0, 2);
        
        switch (select) {
            case 0:
            target = GetPlayer();
            break;

            case 1:
            if (GetPartner() != null) {
                target = GetPartner();
            } else {
                target = GetPlayer();
            }
            break;
        }
    }

    public void Walk(bool forwardMove) {
        Vector3 pos = this.transform.position;

        Vector3 distance = Vector3.right * 6;

        Vector3 endPos = GetPlayer().position + distance;
        endPos.y = basePos.y;

        Transform firstEnemy = GetPlayer().gameObject.GetComponent<PlayerBattleMovements>().targetEnemyFirst;
        bool isSelf = firstEnemy == this.transform;

        if (forwardMove) {
            float EnZ = target.position.z;
            velocity.x = -30f;

            if (pos.x < firstEnemy.position.x || isSelf) {
                if (EnZ > pos.z) {
                    velocity.z = 24f;
                    if (pos.z > EnZ) this.transform.position = new Vector3(pos.x, pos.y, EnZ);
                } else {
                    velocity.z = -24f;
                    if (pos.z < EnZ) this.transform.position = new Vector3(pos.x, pos.y, EnZ);
                }
            } else {
                velocity.z = 0;
            }

            if (pos.x < endPos.x) {
                this.transform.position = new Vector3(endPos.x, pos.y, pos.z);
                ConsumeTurn();
            }
        } else {
            velocity.x = 40f;

            if (pos.x < firstEnemy.position.x || isSelf) {
                if (basePos.z > pos.z) {
                    velocity.z = 24f;
                    if (pos.z > basePos.z) this.transform.position = new Vector3(pos.x, pos.y, basePos.z);
                } else {
                    velocity.z = -24f;
                    if (pos.z < basePos.z) this.transform.position = new Vector3(pos.x, pos.y, basePos.z);
                }
            } else {
                velocity.z = 0;
            }

            if (pos.x > basePos.x) {
                this.transform.position = new Vector3(basePos.x, pos.y, pos.z);
                TurnEnd();
            }
        }
    }
    public void Jump() {
        Transform targetPlayer = target;
        float height = targetPlayer.gameObject.GetComponent<CapsuleCollider>().height;
        float scale = targetPlayer.localScale.y;

        SoundPlay(jumpSound);
        velocity.y = (48f * scale) + ((targetPlayer.position.y - transform.position.y) * gravity);
        Grounded = false;

        float jumpFrame = (velocity.y - (height * scale)) / gravity;
        float xDistance = targetPlayer.position.x - this.transform.position.x;
        float zDistance = targetPlayer.position.z - this.transform.position.z;

        velocity.x = (xDistance * 27.8f) / jumpFrame;
        velocity.z = (zDistance * 27.8f) / jumpFrame;
    }
    public void Stomped() {
        jumpCount--;

        if (jumpCount > 0) {
            velocity.y = 30f;
        } else {
            velocity.x = 10f;
            velocity.y = 27f;
        }
        DamageThePlayer(1);
    }
    void DamageThePlayer(int damagePattern) {
        if (target.GetComponent<PlayerBattleMovements>() != null) {
            PlayerBattleMovements player = target.GetComponent<PlayerBattleMovements>();
            player.DamageOrHeal(-power, false, damagePattern, true, ignoreDefense);
        } else if (false) {
            ;
        }
    }

    public void TurnEnd() {
        BattleManager.turnEnIndex++;
        turn = false;
        velocity = Vector3.zero;

        Initialize();
    }

    public virtual void Initialize() {
        actionId = 0;
    }

    public void SpikeDamage() {
        actions.Clear();
        velocity.x = 10f;
        velocity.y = 20f;
    }
}
