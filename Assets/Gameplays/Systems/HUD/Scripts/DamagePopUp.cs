using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamagePopUp : MonoBehaviour
{
    private Animator animator;
    [HideInInspector] public int value;
    [HideInInspector] public bool isFP;
    [HideInInspector] public Vector3 targetPosition;
    [Header("オブジェクト")]
    public Text valueText;
    public GameObject[] HPGroup = new GameObject[2];
    public GameObject[] FPGroup = new GameObject[2];
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        valueText.text = Math.Abs(value).ToString();

        GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main, targetPosition);
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("IsHeal", (value > 0));

        for (int i = 0; i < HPGroup.Length; i++) {
            HPGroup[i].SetActive(!isFP);
            FPGroup[i].SetActive(isFP);
        }

        GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main, targetPosition);
    }

    public void DestroyObj() {
        Destroy(gameObject);
    }
}
