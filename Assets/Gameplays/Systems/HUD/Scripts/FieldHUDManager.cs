using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class FieldHUDManager : MonoBehaviour
{
    [Header("エリア名表示")]
    public PlayableDirector areaName;
    public CanvasGroup areaNameGroup;
    public Text areaNameText;
    private bool areaNameShow = true;
    private float areaNameAlpha = 1f;
    private float areaNameTime = 0f;
    private bool areaNameEnable = false;
    [Header("ワイプ")]
    public RectTransform wipeTransform;
    public Animator wipeAnim;
    private Transform player;
    private int animNo = 0;
    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine("AreaNameDisplay");
        areaNameEnable = false;
    }

    // Update is called once per frame
    void Update()
    {
        //エリア名
        if (areaNameShow) {
            areaNameAlpha = 1f;
        } else {
            areaNameAlpha -= 2 * Time.deltaTime;
        }
        areaNameGroup.alpha = areaNameAlpha;
        areaNameText.text = SceneManager.GetActiveScene().name;

        if (areaNameEnable) {
            if (areaNameTime <= 0) {
                if (!areaNameShow) areaName.Play();
                areaNameShow = true;
            } else {
                areaNameShow = false;
            }

            if (Input.anyKey || Math.Abs(Input.GetAxis("Horizontal")) > 0 || Math.Abs(Input.GetAxis("Vertical")) > 0) {
                areaNameTime = 10f;
            } else if (areaNameTime > 0) {
                areaNameTime -= Time.deltaTime;
            }
        }

        //ワイプ
        if (GameManager.Death) {
            animNo = -1;
        }
        if (player != null) {
            wipeTransform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, player.position);
            wipeAnim.SetInteger("wipeNo", animNo);
        } else {
            GameObject findPlayer = GameObject.FindGameObjectWithTag("Player");
            if (findPlayer.GetComponent<MarioStatus>() != null) {
                player = findPlayer.transform;
            }
        }
    }

    IEnumerator AreaNameDisplay() {
        areaNameShow = true;
        yield return new WaitForSeconds(4f);
        areaNameEnable = true;
    }

    void WipeOutEnd() {
        animNo = 0;
    }
}
