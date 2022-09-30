using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [HideInInspector] public bool split = false;
    private Rigidbody rb;
    [HideInInspector] public Vector3 velocity;
    private float jumpVel = 50f;
    // Start is called before the first frame update
    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();

        velocity = new Vector3(
            0f, 
            50f, 
            0f
        );
        jumpVel = 50f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.gameObject.transform.GetChild(0).Rotate(0f, -180f * Time.deltaTime, 0f);

        if (split) {
            velocity.y -= 2.75f;

            rb.velocity = velocity;
        } else {
            rb.velocity = Vector3.zero;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<MarioStatus>() != null) {
            MarioStatus mario = other.gameObject.GetComponent<MarioStatus>();

            mario.Coins++;
            Destroy(gameObject);
        } else if (LayerMask.LayerToName(other.gameObject.layer) == "Default" && velocity.y < 0 && split) {
            RaycastHit hit;
            Physics.Raycast(transform.position, -Vector3.up, out hit, 4f);

            float diff = 1f - hit.distance;
            this.transform.position += Vector3.up * diff;

            jumpVel -= 25f;
            if (jumpVel > 0) {
                velocity.x /= 2f;
                velocity.y = jumpVel;
                velocity.z /= 2f;
            } else {
                split = false;
            }
        }
    }
}
