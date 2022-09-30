using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public GameObject popUp;

    public void MakePopup(Vector3 position, int value, bool isFP) {
        DamagePopUp damage = Instantiate(popUp, Vector3.zero, Quaternion.identity, this.transform).GetComponent<DamagePopUp>();
        damage.targetPosition = position;

        value = Mathf.Clamp(value, -999, 999);
        damage.value = value;
        
        damage.isFP = isFP;
    }
}
