using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarioInfos : MonoBehaviour
{
    public MarioStatus mario;
    public StatusSaver status;
    public Text levelText;
    [Header("ブーツ")]
    public Image bootsIcon;
    public Sprite[] bootsSprites = new Sprite[3];
    [Header("ハンマー")]
    public Image hammerIcon;
    public Sprite[] hammerSprites = new Sprite[3];
    [Header("値")]
    public Text[] valueTexts = new Text[8];
    public Image[] bars = new Image[2];

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
        
        levelText.text = "マリオ  レベル " + status.Level.ToString();

        bootsIcon.sprite = bootsSprites[status.BootsRank - 1];
        if (status.HammerRank > 0) {
            hammerIcon.gameObject.SetActive(true);
            hammerIcon.sprite = hammerSprites[status.HammerRank - 1];
        } else {
            hammerIcon.gameObject.SetActive(false);
        }

        valueTexts[0].text = status.HP.ToString() + "/" + mario.FinalMaxHP.ToString();
        valueTexts[1].text = status.FP.ToString() + "/" + mario.FinalMaxFP.ToString();
        valueTexts[2].text = status.MaxBP.ToString();

        valueTexts[3].text = (mario.isMaxLevel()) ? "-" : status.StarPoints.ToString();
        valueTexts[4].text = status.Coins.ToString();
        valueTexts[5].text = status.StarPieces.ToString();
        valueTexts[6].text = (0).ToString();
        valueTexts[7].text = (0).ToString("D2") + " : " + (0).ToString("D2");

        bars[0].fillAmount = (float)status.HP / (float)mario.FinalMaxHP;
        bars[1].fillAmount = (float)status.FP / (float)mario.FinalMaxFP;
    }
}
