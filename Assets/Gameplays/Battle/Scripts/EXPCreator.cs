using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXPCreator : MonoBehaviour
{
    public GameObject starPtModel;
    [HideInInspector] public int amounts = 100;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (amounts > 0) {
            amounts--;
            StarPointManager starPt = Instantiate(starPtModel, transform.position, Quaternion.identity).GetComponent<StarPointManager>();

            Vector3 direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            direction = direction.normalized;

            starPt.velocity = direction * Random.Range(0f, 12f);
            starPt.velocity.y = Random.Range(30f, 50f);
        } else {
            Destroy(gameObject);
        }
    }
}
