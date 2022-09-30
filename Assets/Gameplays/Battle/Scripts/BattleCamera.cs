using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    [Header("マネージャー")]
    public BattleManager manager;
    private Vector3 basePos;
    public PlayerBattleMovements playerM;

    [Header("プレイヤー")]
    public Transform player;

    private bool isOver;
    // Start is called before the first frame update
    void Start()
    {
        basePos = this.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 playerPos = player.position + new Vector3(0, 3.7f, -16.5f);
        //Vector3 enemyPos = BattleManager.enMoves[BattleManager.selectedEnIndex].position + new Vector3(0, 3.7f, -20f);

        if (manager.result == -1 || manager.result == 1) {
            if (manager.result == -1) {
                playerPos = player.position + new Vector3(0, 1.7f, -14.5f);
            }
            this.transform.position = Vector3.Lerp(this.transform.position, playerPos, 5.0f * Time.deltaTime);
        } else if (BattleManager.actionId == 6) {
            if (isOver) {
                this.transform.position = Vector3.Lerp(this.transform.position, basePos, 5.0f * Time.deltaTime);
            } else {
                //this.transform.position = Vector3.Lerp(this.transform.position, enemyPos, 5.0f * Time.deltaTime);
            }

            if (!isOver && playerM.isOver) {
                isOver = true;
            }
        } else {
            this.transform.position = Vector3.Lerp(this.transform.position, basePos, 5.0f * Time.deltaTime);
            isOver = false;
        }
    }
}
