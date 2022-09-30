using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListBadge : ListCommon
{
    public BadgeData data;
    [Header("必要量")]
    public Text neededNumber;
    public GameObject neededSegments;
    public GameObject segment;

    // Update is called once per frame
    void Update()
    {
        if (status.badges.Count > index) {
            int badgeIndex = status.badges[index].BadgeIndex;
            BadgeData.Badges badgeData = data.data[badgeIndex];
            ValueSetUp(badgeData.Image, badgeData.Name, badgeData.Description);

            neededNumber.text = badgeData.BPNeeded.ToString();

            //初期化
            foreach (Transform obj in neededSegments.transform) {
                if ( 0 <= obj.gameObject.name.LastIndexOf("Clone") ) {
                    Destroy(obj.gameObject);
                }
            }

            //値が0の場合は、隠す。
            segment.SetActive(badgeData.BPNeeded > 0);

            Color light;
            Color attachColor;
            if (status.badges[index].isAttaching) {
                light = Color.white;
                attachColor = new Color(0.6f, 1f, 0.6f, 1f);
            } else {
                light = new Color(0.3f, 0.3f, 0.3f, 1f);
                attachColor = Color.white;
            }
            segment.GetComponent<Image>().color = light;
            this.GetComponent<Image>().color = attachColor;

            for ( int i = 1 ; i < badgeData.BPNeeded ; i++ ) {
                RectTransform oneSegment = (RectTransform)Instantiate(segment).transform;
                oneSegment.SetParent(neededSegments.transform , false);
                oneSegment.localPosition = new Vector2(
                    oneSegment.localPosition.x - oneSegment.sizeDelta.x * i ,
                    oneSegment.localPosition.y);
                
                oneSegment.gameObject.GetComponent<Image>().color = light;
            }

            //バッジの付け外し
            if (selected) {
                if (Input.GetButtonDown("A")) {
                    if (badgeData.BPNeeded <= status.BP || status.badges[index].isAttaching) {
                        status.badges[index].isAttaching = !status.badges[index].isAttaching;
                    }
                }
            }
        }
    }
}
