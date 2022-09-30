using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BPLeft : MonoBehaviour
{
    public StatusSaver status;
    public Text maxBP;
    public RectTransform segmentGroup;
    public GameObject oneSegment;
    public Text leftBP;
    private int BPPrevious = -1;

    private float originY;
    // Start is called before the first frame update
    void Start()
    {
        originY = this.GetComponent<RectTransform>().anchoredPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        maxBP.text = "BP " + status.MaxBP.ToString();
        leftBP.text = "のこり " + status.BP.ToString();

        Vector2 segSize = oneSegment.GetComponent<RectTransform>().sizeDelta;

        int height = (int)Math.Ceiling((float)status.MaxBP / 10f);
        this.GetComponent<RectTransform>().sizeDelta = new Vector2((segSize.y * 10f) + 30f, 75f + (segSize.y * height));

        Vector3 ancPos = this.GetComponent<RectTransform>().anchoredPosition;
        this.GetComponent<RectTransform>().anchoredPosition = new Vector3(ancPos.x, originY + ((segSize.y / 2f) * (height - 1)), ancPos.z);

        if (BPPrevious != status.BP) {
            foreach (Transform obj in segmentGroup.transform) {
                if ( 0 <= obj.gameObject.name.LastIndexOf("Clone") ) {
                    Destroy(obj.gameObject);
                }
            }

            Color light;
            if (status.BP > 0) {
                light = Color.white;
            } else {
                light = new Color(0.3f, 0.3f, 0.3f, 1f);
            }
            oneSegment.GetComponent<Image>().color = light;

            for ( int i = 1 ; i < status.MaxBP ; i++ ) {
                RectTransform segment = (RectTransform)Instantiate(oneSegment).transform;
                segment.SetParent(segmentGroup.transform , false);
                segment.localPosition = new Vector2(
                    segment.localPosition.x + segment.sizeDelta.x * (i % 10) ,
                    segment.localPosition.y - segment.sizeDelta.y * (i / 10));
                
                if (status.BP > i) {
                    light = Color.white;
                } else {
                    light = new Color(0.3f, 0.3f, 0.3f, 1f);
                }

                segment.gameObject.GetComponent<Image>().color = light;
            }

            BPPrevious = status.BP;
        }
    }
}
