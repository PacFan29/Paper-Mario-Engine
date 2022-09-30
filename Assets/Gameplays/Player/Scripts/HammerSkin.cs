using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerSkin : MonoBehaviour
{
    public GameObject[] skins;
    public MarioStatus mario;

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject skin in skins) {
            skin.SetActive(false);
        }
        skins[mario.HammerRank - 1].SetActive(true);
    }
}
