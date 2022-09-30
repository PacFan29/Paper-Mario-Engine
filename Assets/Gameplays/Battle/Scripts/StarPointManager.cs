using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarPointManager : MonoBehaviour
{
    public Vector3 velocity;
    public Transform rotatePart;
    private float lifeTime = 2f;

    // Update is called once per frame
    void FixedUpdate()
    {
        rotatePart.Rotate(0f, -7f, 0f);

        velocity.y -= 2.25f;
        this.GetComponent<Rigidbody>().velocity = velocity;

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0) Destroy(gameObject);
    }
}
