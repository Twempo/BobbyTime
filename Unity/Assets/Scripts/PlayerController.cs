using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float speed;
    public float jumpHeight;

	public float mouseSensX;
	public float mouseSensY;

	float jumpMulti;
    Rigidbody rb;

    public GameObject cam;
	public GameObject camTarget;
    public GameObject camTargetHolder;
    public GameObject tripod;
    Rigidbody camRb;

    void Start() {
        rb = GetComponentInChildren<Rigidbody>();
        camRb = cam.GetComponent<Rigidbody>();
		jumpMulti = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate() {
        Vector3 movement = speed*(transform.forward*Input.GetAxis("Vertical")+transform.right*Input.GetAxis("Horizontal"));
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
       
		if (Input.GetAxis("Jump")>.9f&&jumpMulti>0)
        {
            rb.velocity = movement + new Vector3(0,jumpHeight*jumpMulti,0);
            jumpMulti -= Time.fixedDeltaTime * 2;
        }
		transform.RotateAround(transform.position,transform.up,Input.GetAxis("Mouse X")*mouseSensX*Time.fixedDeltaTime*10);

        Vector3 camNewPos = Vector3.Lerp(cam.transform.position, tripod.transform.position, .45f);
        camRb.MovePosition(camNewPos);
        camTarget.transform.position = Vector3.Lerp(camTarget.transform.position, camTargetHolder.transform.position, .45f);
        tripod.transform.LookAt(camTarget.transform);
        camRb.rotation = Quaternion.Euler(camRb.rotation.x,tripod.transform.rotation.y,camRb.rotation.z);
    }

    void OnTriggerEnter(Collider c) {
        if (c.CompareTag("Env"))
            jumpMulti = 1;
    }
}
