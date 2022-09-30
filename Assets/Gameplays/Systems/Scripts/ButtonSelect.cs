using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSelect : MonoBehaviour
{
    [Header("ボタン")]
    public ButtonManager[] buttons;
    [HideInInspector] public int selectIndex = 0;
    [HideInInspector] public bool accepted = false;
    private bool inputMemory = false;

    // Start is called before the first frame update
    void Start()
    {
        selectIndex = 0;
        accepted = false;
    }

    // Update is called once per frame
    void Update()
    {
        Control();

        for (int i = 0; i < buttons.Length; i++) {
            buttons[i].selected = (i == selectIndex);
            buttons[i].accepted = accepted;
        }
    }

    void Control() {
        if (!accepted) {
            if (Input.GetAxis("Vertical") < 0){
                if (!inputMemory){
                    inputMemory = true;
                    if (selectIndex < (buttons.Length - 1)) selectIndex++;
                }
            } else if (Input.GetAxis("Vertical") > 0){
                if (!inputMemory){
                    inputMemory = true;
                    if (selectIndex > 0) selectIndex--;
                }
            } else {
                inputMemory = false;
            }

            if (Input.GetButtonDown("A")) {
                accepted = true;
            }
        }
    }
}
