using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B00Template : EnemyBattleMovements
{
    void OnEnable() {
        Initialize();
    }

    public override void Turn() {
        if (actions.Count > 0) {
            power = status.Power;

            switch (actions[0]) {
                case 0:
                //移動
                Walk(true);
                break;

                case 1:
                velocity = Vector3.zero;
                waitTime -= Time.deltaTime;
                if (waitTime <= 0) {
                    ConsumeTurn();
                }
                break;

                case 2:
                if (this.transform.position.x < target.position.x) {
                    Vector3 pos = this.transform.position;
                    this.transform.position = new Vector3(target.position.x, pos.y, pos.z);
                    velocity.x = 0;
                }
                if (velocity.z > 0 && this.transform.position.z > target.position.z) {
                    Vector3 pos = this.transform.position;
                    this.transform.position = new Vector3(pos.x, pos.y, target.position.z);
                    velocity.z = 0;
                }
                if (velocity.z < 0 && this.transform.position.z < target.position.z) {
                    Vector3 pos = this.transform.position;
                    this.transform.position = new Vector3(pos.x, pos.y, target.position.z);
                    velocity.z = 0;
                }

                if (Grounded) {
                    if (jumpCount > 0) BattleManager.ready = true;
                    actionId = 0;
                    ConsumeTurn();
                }

                if (jumpCount <= 0) velocity.x = 10f;
                break;
            }
        } else {
            if (this.transform.position.x != basePos.x) {
                Walk(false);
            } else {
                velocity = Vector3.zero;
                TurnEnd();
            }
        }
    }
    public override void ConsumeTurn() {
        actions.RemoveAt(0);
        if (actions.Count > 0) {
            if (actions[0] == 2) Jump();
        }
    }

    public override void Initialize() {
        actionId = UnityEngine.Random.Range(0, 2);
        waitTime = 0.25f;
        SetTarget();

        switch (actionId) {
            case 0:
            //頭突き
            jumpCount = 1;
            ignoreDefense = false;
            actions.Add(0);
            actions.Add(1);
            actions.Add(2);
            break;
            
            case 1:
            //頭突き3回
            jumpCount = 3;
            ignoreDefense = false;
            actions.Add(0);
            actions.Add(1);
            actions.Add(2);
            break;
        }
    }
}
