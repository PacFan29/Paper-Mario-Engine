using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActListItem : MonoBehaviour
{
    [HideInInspector] public int index = 0;
    [HideInInspector] public bool isPartner = false;
    [Header("データ")]
    public StatusSaver status;
    public ActPatterns actPatterns;
    [Header("出力")]
    public Image Icon;
    public Text NameText;
    public GameObject FPDisplay;
    public Text FPNeeded;
    [Header("選択")]
    public GameObject arrow;

    [HideInInspector] public bool selected = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ActPatterns.Patterns pattern = actPatterns.patterns[index];
        Icon.sprite = pattern.icon;
        NameText.text = pattern.name;

        FPDisplay.SetActive(pattern.FPNeeded > 0);
        FPNeeded.text = pattern.FPNeeded.ToString() + " FP";

        arrow.SetActive(selected);
    }
}
