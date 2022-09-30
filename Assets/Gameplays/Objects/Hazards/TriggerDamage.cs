using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HazardType {
    Normal,
    Spike,
    Fire
}
public class TriggerDamage : MonoBehaviour
{
    public HazardType type;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionStay(Collision other) {
        if (other.gameObject.GetComponent<PlayerInfo>() != null) {
            PlayerInfo player = other.gameObject.GetComponent<PlayerInfo>();
            if (!player.isDamage()) {
                player.TakeDamage(this.transform.position, type);
            }
        }
    }
}
