using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleMovements : ActionCommands
{
    [HideInInspector] public Transform targetEnemyFirst;
    [HideInInspector] public Transform targetEnemy;

    private int jumpCount = 2;
    private int groundPoundToggle = 0;
    private int actionIndex;
    [HideInInspector] public bool isOver = false;
    private float guardTime = 0f;
    private bool superGuarding = false;

    // Update is called once per frame
    void Update()
    {
        if (turn) {
            if (actions.Count > 0) {
                switch (actions[0]) {
                    case 0:
                    //ジャンプ前の移動
                    Walk(true, true);
                    break;

                    case 1:
                    //ジャンプ
                    pressA();

                    if (this.transform.position.x > targetEnemy.position.x) {
                        Vector3 pos = this.transform.position;
                        this.transform.position = new Vector3(targetEnemy.position.x, pos.y, pos.z);
                        velocity.x = 0;
                    }
                    if (velocity.z > 0 && this.transform.position.z > targetEnemy.position.z) {
                        Vector3 pos = this.transform.position;
                        this.transform.position = new Vector3(pos.x, pos.y, targetEnemy.position.z);
                        velocity.z = 0;
                    }
                    if (velocity.z < 0 && this.transform.position.z < targetEnemy.position.z) {
                        Vector3 pos = this.transform.position;
                        this.transform.position = new Vector3(pos.x, pos.y, targetEnemy.position.z);
                        velocity.z = 0;
                    }

                    //クルリンジャンプ発動
                    if (jumpCount == 1 && velocity.y <= 0 && groundPoundToggle == 1) {
                        groundPoundToggle = 2;
                        actionId = 16;
                        StartCoroutine("GroundPound");
                    }

                    //踏みつけ前のアニメーション
                    if (jumpCount > 0 && velocity.y <= 0 && actionId == 0) {
                        if (actionIndex == 4) {
                            actionId = 23;
                        } else {
                            actionId = 22;
                        }
                    }

                    if (Grounded) {
                        if (jumpCount > 0) BattleManager.ready = true;
                        actionId = 0;
                        ConsumeTurn();
                    }

                    if (jumpCount <= 0) velocity.x = -10f;
                    break;

                    case 2:
                    //ガツーンジャンプ前の待機
                    velocity = Vector3.zero;
                    waitTime -= Time.deltaTime;
                    if (waitTime <= 0) {
                        ConsumeTurn();
                    }
                    break;

                    case 3:
                    //竜巻の発動
                    break;

                    case 4:
                    //ハンマー前の移動
                    Walk(true, false);
                    waitTime = 0.14f;
                    break;

                    case 5:
                    velocity = Vector3.zero;

                    if (waitTime <= 0) {
                        actionId = 25;

                        if (hammerTrigger == 0 && Input.GetAxis("Horizontal") < 0) {
                            hammerTrigger = 1;
                        } else if (hammerTrigger == 1) {
                            hammerInterval();
                        }
                    } else {
                        waitTime -= Time.deltaTime;
                        actionId = 24;
                    }
                    if (hammerTrigger == 2) {
                        ConsumeTurn();
                        waitTime = 1f;

                        if (succeed) {
                            power += GetComponent<MarioStatus>().HammerRank + GetComponent<MarioStatus>().Power - 1;
                            BattleHUDManager.rankValue = 0;
                        }

                        DamageTheEnemy(BattleManager.selectedEnIndex, 1, true);
                    }
                    break;

                    case 6:
                    case 7:
                    actionId = 21 + actions[0];
                    waitTime -= Time.deltaTime;
                    if (waitTime <= 0) {
                        actionId = 0;
                        ConsumeTurn();
                    }
                    break;
                }
            } else {
                if (this.transform.position.x != basePos.x) {
                    Walk(false, false);
                } else {
                    isOver = false;
                    velocity = Vector3.zero;
                    turn = false;
                }
            }
        } else {
            initialize();
            waitTime = 0.25f;
            BattleHUDManager.rankValue = -1;

            if (status.HP > 0) {
                if (BattleManager.actionId == 7) {
                    if (guardTime > 0) {
                        guardTime -= Time.deltaTime;

                        if (guardTime <= 0) {
                            superGuarding = false;
                        }
                    }

                    if (Input.GetButtonDown("A") && guardTime <= 0) {
                        //ガード
                        guardTime = 0.3f;
                        actionId = 29;
                        superGuarding = false;
                    }
                    if (Input.GetButtonDown("B") && guardTime <= 0) {
                        //スーパーガード
                        guardTime = 0.1f;
                        superGuarding = true;
                    }
                } else {
                    guardTime = 0f;
                }

                if (guardTime <= 0 && actionId == 29) {
                    actionId = 0;
                }

                isGuarding = (actionId == 29);
            } else {
                isGuarding = false;
            }
        }

        SkinAnimation();
    }

    public void SkinAnimation() {
        Vector3 XZvel = new Vector3(velocity.x, 0f, velocity.z);
        MarioStatus status = this.GetComponent<MarioStatus>();

        anim.SetInteger("Action ID", actionId);
        anim.SetFloat("Speed", XZvel.magnitude);
        anim.SetFloat("Y Velocity", velocity.y);
        anim.SetBool("Grounded", Grounded);
        anim.SetBool("Pinch", status.Pinch || status.Danger || status.HP <= 0);
        anim.SetFloat("Waiting Time", 0);
        anim.SetBool("Is Battle", true);

        if (damageTime > 0) damageTime -= Time.deltaTime;
        
        if(damageTime <= 0f && (actionId >= 9 && actionId <= 12) && status.HP > 0){
            actionId = 0;
        }
    }

    public void Walk(bool forwardMove, bool isJump) {
        Vector3 pos = this.transform.position;

        Vector3 distance = Vector3.left * (isJump ? 6 : 3);

        Vector3 endPos = targetEnemyFirst.transform.position + distance;
        endPos.y = basePos.y;

        if (forwardMove) {
            float EnZ = BattleManager.enMoves[BattleManager.selectedEnIndex].position.z;
            velocity.x = 20f;

            if (EnZ > pos.z) {
                velocity.z = 12f;
                if (pos.z > EnZ) this.transform.position = new Vector3(pos.x, pos.y, EnZ);
            } else {
                velocity.z = -12f;
                if (pos.z < EnZ) this.transform.position = new Vector3(pos.x, pos.y, EnZ);
            }

            if (pos.x > endPos.x) {
                this.transform.position = new Vector3(endPos.x, pos.y, pos.z);
                ConsumeTurn();
            }
        } else {
            velocity.x = -40f;

            if (pos.x < targetEnemyFirst.transform.position.x) {
                if (basePos.z > pos.z) {
                    velocity.z = 24f;
                    if (pos.z > basePos.z) this.transform.position = new Vector3(pos.x, pos.y, basePos.z);
                } else {
                    velocity.z = -24f;
                    if (pos.z < basePos.z) this.transform.position = new Vector3(pos.x, pos.y, basePos.z);
                }
            }

            if (pos.x < basePos.x) {
                this.transform.position = new Vector3(basePos.x, pos.y, pos.z);
            }
        }
    }
    public void Jump() {
        Transform targetEnemy = BattleManager.enMoves[BattleManager.selectedEnIndex];
        float height = targetEnemy.gameObject.GetComponent<CapsuleCollider>().height;
        float scale = targetEnemy.localScale.y;

        SoundPlay(jumpSound);
        velocity.y = (60f * scale) + ((targetEnemy.position.y - transform.position.y) * gravity) + (4f * BattleManager.selectedEnIndex);
        Grounded = false;

        float jumpFrame = (velocity.y - (height * scale)) / gravity;
        float xDistance = targetEnemy.position.x - this.transform.position.x;
        float zDistance = targetEnemy.position.z - this.transform.position.z;

        velocity.x = (xDistance * 27.8f) / jumpFrame;
        velocity.z = (zDistance * 27.8f) / jumpFrame;
    }

    public override void ConsumeTurn() {
        actions.RemoveAt(0);
        if (actions.Count > 0) {
            if (actions[0] == 1) Jump();
        }
    }

    public void Stomped() {
        int latestBootsRank = GetComponent<MarioStatus>().BootsRank;
        int damageIndex = BattleManager.selectedEnIndex;

        velocity.x = 0f;
        if (jumpCount < 100) {
            jumpCount--;
        } else {
            if (power > 1) power--;
        }

        //クルリンジャンプ成功
        if (groundPoundToggle == 2) {
            actionId = 0;
            if (succeed) {
                power += latestBootsRank;
                BattleHUDManager.rankValue++;
            }
            groundPoundToggle = 0;
        }

        if (succeed) {
            //アクションコマンド成功
            if (jumpCount <= 0) {
                isOver = true;
                velocity.x = -10f;
                velocity.y = 42f;

                if (actionIndex == 4) {
                    BattleHUDManager.rankValue++;
                    power += latestBootsRank + GetComponent<MarioStatus>().Power - 1;
                } else if (actionIndex == 5) {
                    BattleHUDManager.rankValue++;
                }
            } else {
                BattleHUDManager.rankValue++;
                velocity.y = 36f;

                if (actionIndex == 5) {
                    //ツギツギジャンプ
                    isOver = true;
                    
                    if (BattleManager.selectedEnIndex + 1 < BattleManager.enMoves.Length) {
                        while (BattleManager.enMoves[BattleManager.selectedEnIndex + 1].GetComponent<BattleMovement>().isDead) {
                            BattleManager.selectedEnIndex++;
                        }
                        targetEnemy = BattleManager.enMoves[BattleManager.selectedEnIndex + 1];
                    } else {
                        jumpCount = 1;
                        BattleHUDManager.rankValue--;
                        Stomped();
                        return;
                    }
                    
                    float height = targetEnemy.gameObject.GetComponent<CapsuleCollider>().height;
                    float scale = targetEnemy.localScale.y;

                    velocity.y = 36f + ((targetEnemy.position.y - basePos.y) * gravity);

                    float jumpFrame = (velocity.y - (height * scale)) / gravity;
                    float xDistance = targetEnemy.position.x - this.transform.position.x;
                    float zDistance = targetEnemy.position.z - this.transform.position.z;

                    velocity.x = (xDistance * 27.8f) / jumpFrame;
                    velocity.z = (zDistance * 27.8f) / jumpFrame;
                }
            }
        } else {
            //アクションコマンド失敗
            isOver = true;
            velocity.x = -10f;
            velocity.y = 27f;
            jumpCount = 0;
        }
        actionId = 0;
        DamageTheEnemy(damageIndex, 1, isOver);
    }

    public void ActionSetUp(int action, int index) {
        actionIndex = index;
        ignoreDefense = false;
        groundPoundToggle = 0;

        switch (action) {
            case 0:
            JumpActionSetUp(actionIndex);
            break;

            case 1:
            HammerActionSetUp(actionIndex);
            break;
        }

        turn = true;
    }
    void JumpActionSetUp(int action) {
        MarioStatus mario = GetComponent<MarioStatus>();
        power = mario.Power - 1 + mario.BootsRank;

        switch (action) {
            case 0: //ジャンプ
            case 6: //ミニミニフミィ
            case 7: //ネムラセフミィ
            case 8: //フニャフニャフミィ
            actions.Add(0);
            actions.Add(1);
            jumpCount = 2;
            break;
            
            case 1: //クルリンジャンプ
            groundPoundToggle = 1;
            actions.Add(0);
            actions.Add(1);
            jumpCount = 2;
            break;

            case 4: //ガツーンジャンプ
            actions.Add(0);
            actions.Add(2);
            actions.Add(1);
            jumpCount = 1;
            power += 2;
            break;

            case 5: //ツギツギジャンプ
            actions.Add(0);
            actions.Add(1);
            jumpCount = BattleManager.enMoves.Length;
            foreach (Transform enemy in BattleManager.enMoves) {
                if (enemy.GetComponent<BattleMovement>().isDead) {
                    jumpCount--;
                }
            }
            break;

            case 9: //タツマキジャンプ
            actions.Add(0);
            actions.Add(1);
            jumpCount = 2;
            break;

            case 10: //レンゾクジャンプ
            actions.Add(0);
            actions.Add(1);
            jumpCount = 100;
            power += 1;
            break;
        }
    }
    void HammerActionSetUp(int action) {
        MarioStatus mario = GetComponent<MarioStatus>();
        power = mario.Power - 1 + mario.HammerRank;

        switch (action) {
            case 0: //ハンマー
            case 6: //コンランナグーリ
            case 7: //アイスナグーリ
            if (action == 7) {
                hInterval = 0.7f;
            }
            actions.Add(4);
            actions.Add(5);
            actions.Add(6);
            break;

            case 1: //回転ハンマー
            case 2: //伝説のハンマー
            if (action == 2) {
                hInterval = 0.15f;
                hMaxStep = 10;
            } else {
                hInterval = 0.25f;
                hMaxStep = 7;
            }
            actions.Add(4);
            actions.Add(8);
            actions.Add(9);
            break;

            case 3: //ガツーンナグーリ
            hInterval = 0.7f;
            power += 2;
            actions.Add(4);
            actions.Add(5);
            actions.Add(7);
            break;

            case 4: //ツラヌキハンマー
            ignoreDefense = true;
            actions.Add(4);
            actions.Add(5);
            actions.Add(7);
            break;

            case 5: //ハンマーナゲール
            hInterval = 0.7f;
            hMaxStep = 5;
            actions.Add(5);
            actions.Add(10);
            break;
        }
    }

    void DamageTheEnemy(int enIndex, int damagePattern, bool isOver) {
        EnemyBattleMovements enemy = BattleManager.enMoves[enIndex].GetComponent<EnemyBattleMovements>();

        if (!enemy.isDead) {
            CheckBadge();

            enemy.DamageOrHeal(-power, true, damagePattern, isOver, ignoreDefense);
        }

        if (actionIndex == 5 && BattleManager.selectedEnIndex < 4) {
            BattleManager.selectedEnIndex++;
        }
    }

    public IEnumerator GroundPound() {
        useGravity = false;
        velocity.y = 0f;

        yield return new WaitForSeconds(0.5f);

        actionId = 17;
        useGravity = true;
        velocity.y = -40f;
    }

    public void Die() {
        SoundPlay(dieSound);
        actionId = 15;
    }
    public void Victory() {
        if (true) {
            actionId = 32;
        } else {
            actionId = 31;
        }
    }
    public void ParamIncreased() {
        actionId = 33;
    }

    public void CheckBadge() {
        MarioStatus mario = GetComponent<MarioStatus>();

        if (mario.statusSaver.GetCount(
            BadgeData.Badges.Parameter.Battle_ActionCommand, 
            mario.Pinch, 
            mario.Danger) > 0) {
            //イチカバチーカの確認
            if (!succeed) {
                //アクションコマンド失敗
                power *= 0;
            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.GetComponent<EnemyBattleMovements>() != null && !turn) {
            //踏まれた時の処理
            EnemyBattleMovements enemy = other.gameObject.GetComponent<EnemyBattleMovements>();

            if (superGuarding) {
                if (!enemy.isDead) {
                    enemy.SpikeDamage();
                    enemy.DamageOrHeal(-1, true, 0, true, true);
                }
            } else {
                enemy.Stomped();
            }
        }
    }

    public override IEnumerator DamageAnimation(int damagePattern, bool isOver) {
        if (!isGuarding) {
            string animName = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            if (actionId != 0) anim.Play(animName, 0, 0);

            actionId = damagePattern + 8;
            actionId = Mathf.Clamp(actionId, 9, 12);
        } else {
            if (status.HP <= 0) {
                actionId = 9;
                yield return null;
            }
        }

        switch (damagePattern) {
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

        yield return null;
    }
}
