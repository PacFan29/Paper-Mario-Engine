using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListCommon : MonoBehaviour
{
    [HideInInspector] public int index = 0;
    public Image Icon;
    public Text NameText;
    public GameObject select;
    public Text descriptionTx;
    [HideInInspector] public bool selected = false;
    [Header("データ")]
    public StatusSaver status;
    
    public void ValueSetUp(Sprite spr, string name, string description) {
        Icon.sprite = spr;
        NameText.text = name;

        select.SetActive(selected);

        if (selected) descriptionTx.text = description;
    }
}
