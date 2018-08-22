using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float speed;
    public float maxJumpForce;
	float jumpForce;
    Rigidbody rb;

    public GameObject cam;
    Rigidbody camRb;
    Vector3 camOffset;

    void Start() {
        rb = GetComponentInChildren<Rigidbody>();
        camRb = cam.GetComponent<Rigidbody>();
        camOffset = cam.transform.position - transform.position;
		jumpForce = maxJumpForce;
    }

    void FixedUpdate() {
        Vector3 movement = speed*(transform.forward*Input.GetAxis("Vertical")+transform.right*Input.GetAxis("Horizontal"));
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
        camRb.MovePosition(transform.position + camOffset);
		if (Input.GetAxis("Jump")>.9f&&jumpForce>0)
        {
            rb.AddForce(Input.GetAxis("Jump") * transform.up * jumpForce);
			jumpForce -= Time.fixedDeltaTime*80;
        }
		if(Physics.Raycast( transform.position - new Vector3( 0, 1, 0 ), Vector3.down, 0.01f)) {
			jumpForce = maxJumpForce;
			Debug.Log( "Grounded"+transform.position );
		}
    }
}
