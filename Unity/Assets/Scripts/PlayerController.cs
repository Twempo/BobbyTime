using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float speed;
    public GameObject grounder;
    public GameObject[] env;
	float jumpMulti;
    Rigidbody rb;

    public GameObject cam;
    Rigidbody camRb;
    Vector3 camOffset;

    void Start() {
        rb = GetComponentInChildren<Rigidbody>();
        camRb = cam.GetComponent<Rigidbody>();
        camOffset = cam.transform.position - transform.position;
		jumpMulti = 1;
    }

    void FixedUpdate() {
        Vector3 movement = speed*(transform.forward*Input.GetAxis("Vertical")+transform.right*Input.GetAxis("Horizontal"));
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
        camRb.MovePosition(transform.position + camOffset);
		if (Input.GetAxis("Jump")>.9f&&jumpMulti>0)
        {
            rb.velocity = movement + new Vector3(0,5*jumpMulti,0);
            jumpMulti -= Time.fixedDeltaTime * 2;
        }
        foreach(GameObject go in env)
        {

        }
    }
}
