using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float speed;
    public float jumpForce;
    public float maxJumpTime;
    Rigidbody rb;
    float jumpTime = 0;

    public GameObject cam;
    Rigidbody camRb;
    Vector3 camOffset;

    void Start() {
        rb = GetComponentInChildren<Rigidbody>();
        camRb = cam.GetComponent<Rigidbody>();
        camOffset = cam.transform.position - transform.position;
    }

    void FixedUpdate() {
        Vector3 movement = speed*(transform.forward*Input.GetAxis("Vertical")+transform.right*Input.GetAxis("Horizontal"));
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
        camRb.MovePosition(transform.position + camOffset);
        if (Input.GetAxis("Jump") >= .2f && jumpTime <= maxJumpTime)
        {
            Debug.Log("Jumping");
            rb.AddForce(Input.GetAxis("Jump") * transform.up * jumpForce);
            jumpTime += Time.fixedDeltaTime;
        }
        else
            jumpTime = 0;
    }
}
