using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HammerInterval : MonoBehaviour
{
    public ActionCommands ac;
    [Header("点灯")]
    public Transform otherLight;
    public Sprite[] lightImages = new Sprite[4];
    [Header("点灯（星）")]
    public Transform starLight;
    public Sprite[] starLightImages = new Sprite[2];

    private int step = 0;
    private int maxStep = 4;
    private int latestStep = 0;
    private Image[] lights;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //maxStep = Mathf.Clamp(maxStep, 4, 10);
        //step = Mathf.Clamp(step, 0, maxStep);
        maxStep = ac.hMaxStep;
        step = ac.hStep;
        /*
        intervalが4の場合：余白20px(標準)
        intervalが5の場合：余白16px
        intervalが5の場合：余白8px
        intervalが9以上の場合：余白0px
        余白の計算式：Max(0, (9 - interval)) * 4px
        →変数spaceとする

        intervalが4の場合：260px(標準)
        intervalが5の場合：304px
        intervalが7の場合：368px
        intervalが10の場合：440px
        widthの計算式：80 + ((40 + space) * (interval - 1))
        */
        /*
        標準：maxStep = 4、speed = 0.5f
        回転ハンマー：maxStep = 7、speed = 0.2f
        ウルトラハンマー：maxStep = 10、speed = 0.15f
        ガツーンナグーリ：maxStep = 4、speed = 0.7f
        ハンマーナゲール：maxStep = 5、speed = 0.7f
        アイスナグーリ：maxStep = 4、speed = 0.7f
        */

        if (latestStep != maxStep) {
            float space = Math.Max(0, (9 - maxStep)) * 4f;
            float width = 80f + ((40f + space) * (maxStep - 1));

            otherLight.localPosition = new Vector2(340 + (20f - space), 0f);

            foreach (Transform obj in this.transform) {
                if ( 0 <= obj.gameObject.name.LastIndexOf("Clone") ) {
                    Destroy(obj.gameObject);
                }
            }

            //星の直前の点灯
            lights = new Image[(maxStep - 1)];
            lights[0] = otherLight.gameObject.GetComponent<Image>();

            for ( int i = 1 ; i < (maxStep - 1) ; i++ ) {
                //複製と同時に配列に格納
                RectTransform scoreimage = (RectTransform)Instantiate(otherLight);
                scoreimage.SetParent(this.transform , false);
                
                scoreimage.localPosition = new Vector2(
                    otherLight.localPosition.x - (scoreimage.sizeDelta.x + space) * i ,
                    otherLight.localPosition.y);

                lights[i] = scoreimage.gameObject.GetComponent<Image>();
            }

            this.transform.localPosition = new Vector2(-580 + width, -60f);

            latestStep = maxStep;
        }

        for (int i = 0; i < lights.Length; i++) {
            //その他点灯
            if (step >= (lights.Length - i)) {
                int index = Math.Min(3, (lights.Length - i));
                lights[i].sprite = lightImages[index];
            } else {
                lights[i].sprite = lightImages[0];
            }
        }

        //星の点灯
        int star = (step >= maxStep) ? 1 : 0;
        starLight.gameObject.GetComponent<Image>().sprite = starLightImages[star];
    }
}
