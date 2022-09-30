using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BadgeInfos : MonoBehaviour
{
    public MarioStatus mario;
    [Header("攻撃力")]
    public Text powerText;
    [Header("ジャンプの攻撃力")]
    public Image jIcon;
    public Sprite[] jSprites = new Sprite[5];
    public Text jPowerText;
    [Header("ハンマーの攻撃力")]
    public GameObject hammerDisplay;
    public Image hIcon;
    public Sprite[] hSprites = new Sprite[5];
    public Text hPowerText;
    [Header("防御力")]
    public Text defenseText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mario == null) {
            GameObject[] playerObjs = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject obj in playerObjs) {
                if (obj.GetComponent<MarioStatus>() != null) {
                    mario = obj.GetComponent<MarioStatus>();
                }
            }
        }
        
        //攻撃力
        powerText.text = mario.Power.ToString();
        //ジャンプの攻撃力
        jIcon.sprite = jSprites[(mario.BootsRank - 1)];
        jPowerText.text = (mario.Power + mario.BootsRank - 1).ToString();
        //ハンマーの攻撃力
        hIcon.sprite = hSprites[(mario.HammerRank - 1)];
        hPowerText.text = (mario.Power + mario.HammerRank - 1).ToString();
        //ハンマーの所持状況次第
        hammerDisplay.SetActive(mario.HammerRank > 0);
        //防御力
        defenseText.text = mario.Defense.ToString();
        if (mario.Defense < 0) {
            defenseText.color = Color.red;
        } else {
            defenseText.color = Color.black;
        }
    }
}
