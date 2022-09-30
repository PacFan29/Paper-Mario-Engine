using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemCommon : MonoBehaviour
{
    public int itemIndex;
    
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<MarioStatus>() != null) {
            GotIt(other.gameObject.GetComponent<MarioStatus>().statusSaver);
        }
    }

    public virtual void GotIt(StatusSaver status) {
        Destroy(gameObject);
    }
}
