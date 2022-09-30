using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEffects : MonoBehaviour
{
    public GameObject footstep;
    public GameObject fainted;
    // Start is called before the first frame update
    void FootstepEffect() {
        MakeEffect(footstep);
    }
    void FaintedEffect() {
        MakeEffect(fainted);
    }

    void MakeEffect(GameObject effect) {
        Instantiate(effect, transform.position - (Vector3.up * 2.25f), Quaternion.identity);
    }
}
