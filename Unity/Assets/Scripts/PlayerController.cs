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
		//	Motion
		Vector3 movement = speed * (transform.forward * Input.GetAxis( "Vertical" ) + transform.right * Input.GetAxis( "Horizontal" ));
		rb.velocity = new Vector3( movement.x, rb.velocity.y, movement.z );

		//	Jumping
		if(Input.GetAxis( "Jump" ) > .9f && jumpMulti > 0) {
			rb.velocity = movement + new Vector3( 0, jumpHeight * jumpMulti, 0 );
			jumpMulti -= Time.fixedDeltaTime * 2;
		}

		//	Camera + Model Rotation
		transform.RotateAround( transform.position, transform.up, Input.GetAxis( "Mouse X" ) * mouseSensX * Time.fixedDeltaTime * 10 );
		Vector3 camNewPos = Vector3.Lerp( cam.transform.position, tripod.transform.position, .45f );
		camRb.MovePosition( camNewPos );
		camTarget.transform.position = Vector3.Lerp( camTarget.transform.position, camTargetHolder.transform.position, .45f );
		tripod.transform.LookAt( camTarget.transform );
		float newX = camRb.rotation.eulerAngles.x + (-Input.GetAxis( "Mouse Y" ) * mouseSensY * Time.fixedDeltaTime * 5);
		if(newX < 180)
			newX = Mathf.Clamp( newX, 0, 30 );
		else
			newX = Mathf.Clamp( newX, 330, 359 );
		camRb.transform.rotation = Quaternion.Euler( newX, tripod.transform.rotation.eulerAngles.y, tripod.transform.rotation.eulerAngles.z );
		Debug.Log( tripod.transform.rotation.eulerAngles.y + ", " + camRb.transform.rotation.eulerAngles.y );
	}

	void OnTriggerEnter( Collider c ) {
		if(c.CompareTag( "Env" ))
			jumpMulti = 1;
	}
}
