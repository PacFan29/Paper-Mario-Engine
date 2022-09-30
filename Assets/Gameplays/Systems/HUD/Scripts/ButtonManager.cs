using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    private Animator selectFrame;
    private int actId;
    public Sprite[] buttonSprites = new Sprite[2];
    public bool selected;
    public bool accepted;
    // Start is called before the first frame update
    void OnEnable()
    {
        actId = 0;
        selectFrame = GetComponent<Animator>();

        StartCoroutine("ButtonStart");
    }

    // Update is called once per frame
    void Update()
    {
        selectFrame.SetInteger("ActID", actId);
        selectFrame.SetBool("Selected", selected);

        if (accepted) {
            actId = 2;
        } else {
            if (actId > 1) actId = 1;
        }

        this.GetComponent<Image>().sprite = selected ? buttonSprites[0] : buttonSprites[1];
    }

    IEnumerator ButtonStart() {
        actId = 0;
        yield return new WaitForSeconds(0.17f);
        actId = 1;
    }
}
