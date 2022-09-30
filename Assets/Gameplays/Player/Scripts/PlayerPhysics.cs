using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerPhysics : MonoBehaviour
{
    //リジッドボディー
    private Rigidbody rb;
    protected Vector3 GroundNormal = Vector3.up;
    public LayerMask GroundLayer;
    private RaycastHit hit;
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public Vector3 velocitySetUp;
    //操作
    protected Vector3 input;
    [HideInInspector] public bool canInput = true;
    //各状態
    [HideInInspector] public bool Grounded = false;
    protected bool useGravity = true;

    [Header("エフェクト")]
    public GameObject groundedEffect;
    public GameObject groundPoundEffect;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (canInput) {
            float speed = 20f;
            Vector3 XZvel = input * speed;

            (velocity.x, velocity.z) = (XZvel.x, XZvel.z);
        } else {
            (velocity.x, velocity.z) = (velocitySetUp.x, velocitySetUp.z);
        }

        Physics.Raycast(transform.position, -Vector3.up, out hit, 2.5f, GroundLayer);
        if (hit.distance <= 0) {
            Grounded = false;
        } else {
            GroundNormal = hit.normal;
        }
        
        //着地していない場合
        if (Grounded) {
            velocity.y = 0f;
        } else {
            GroundNormal = Vector3.up;
            AirMovement();
        }

        transform.rotation = Quaternion.FromToRotation(Vector3.up, GroundNormal);

        Vector3 finalVelocity = Vector3.zero;
        finalVelocity += velocity.x * transform.right;
        finalVelocity += velocity.y * transform.up;
        finalVelocity += velocity.z * transform.forward;
        rb.velocity = finalVelocity;

        transform.rotation = Quaternion.FromToRotation(Vector3.up, Vector3.up);
    }
    void AirMovement() {
        float gravity = 2.25f;

        if (useGravity) velocity.y -= gravity;

        Physics.Raycast(transform.position, -Vector3.up, out hit, 2.5f, GroundLayer);

        if (velocity.y < 0 && hit.distance > 0) {
            transform.position += Vector3.up * (2f - hit.distance);
            Grounded = true;

            if (this.GetComponent<PlayerInfo>().actionId == 17) {
                Instantiate(groundPoundEffect, transform.position - (Vector3.up * 2.25f), Quaternion.identity);
            } else {
                Instantiate(groundedEffect, transform.position - (Vector3.up * 2.25f), Quaternion.identity);
            }
        }
    }

    public void Jump() {
        velocity.y = 40f;
        Grounded = false;
    }
}
