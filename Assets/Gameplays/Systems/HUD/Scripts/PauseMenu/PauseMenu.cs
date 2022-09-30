using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private bool isPause = false;

    [Header("ヘッダー")]
    public Transform[] headers = new RectTransform[5];
    public Image headerColor;
    public Transform underLine;
    [Header("コンテンツ")]
    public GameObject[] contents = new GameObject[5];
    private int contentIndex = 0;
    [Header("フッター")]
    public Image footerColor;

    private Color[] colorList = {
        new Color (1f, 0f, 0f, 1f),
        new Color (1f, 0f, 1f, 1f),
        new Color (1f, 0.8f, 0f, 1f),
        new Color (0f, 0.8f, 0f, 1f),
        new Color (0f, 0.7f, 1f, 1f)
    };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CanvasGroup canvas = this.GetComponent<CanvasGroup>();
        if (Input.GetButtonDown("Start")) {
            contentIndex = 0;
            isPause = !isPause;

            if (isPause) {
                Time.timeScale = 0;
            } else {
                Time.timeScale = 1;
            }
        }
        if (isPause) {
            canvas.alpha = 1f;

            if (Input.GetButtonDown("RB") && contentIndex < (contents.Length - 1)) {
                contentIndex++;
            } else if (Input.GetButtonDown("LB") && contentIndex > 0) {
                contentIndex--;
            }
        } else {
            canvas.alpha = 0f;
        }

        //ヘッダー
        Vector3 linePos = underLine.position;
        underLine.position = new Vector3(headers[contentIndex].position.x, linePos.y, linePos.z);
        headerColor.color = colorList[contentIndex];

        //コンテンツ
        for (int i = 0; i < contents.Length; i++) {
            contents[i].SetActive(i == contentIndex);
        }

        //フッター
        footerColor.color = colorList[contentIndex];
    }
}
