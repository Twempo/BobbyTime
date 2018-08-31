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
    public Gun gun;
    Rigidbody camRb;
    private bool zoom = false;
    private float fireCount = 0;
    private bool canShoot = true;

    void Start() {
        rb = GetComponentInChildren<Rigidbody>();
        camRb = cam.GetComponent<Rigidbody>();
		jumpMulti = 1;
    }

    private void Update()
    {
        if(Input.GetMouseButton(1))
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 25, .6f);
            zoom = true;
        }
        else
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60, .6f);
            zoom = false;
        }
        if (Input.GetMouseButton(0))
        {
            fireCount+=1;
            /*if(gun.getComponent("ammo")>0){
                if(zoom){
                    
                } else {

                }
            }*/

            //recoil accounted for
            if (fireCount > 1)
            {
                // Bit shift the index of the layer (8) to get a bit mask
                int layerMask = 1 << 8;

                // This would cast rays only against colliders in layer 8.
                // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
                layerMask = ~layerMask;

                RaycastHit hit;

                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, layerMask)&&canShoot)
                {
                    Debug.DrawRay(cam.transform.position, cam.transform.forward * hit.distance, Color.yellow);
                    Debug.Log("Did Hit");
                    if (canShoot)
                    {
                        StartCoroutine(waitSeconds(1f / gun.fireRate));
                    }
                }
                else
                {
                    Debug.Log("Did not Hit");
                }
            }

            //first shot
            else
            {
                // Bit shift the index of the layer (8) to get a bit mask
                int layerMask = 1 << 8;

                // This would cast rays only against colliders in layer 8.
                // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
                layerMask = ~layerMask;

                RaycastHit hit;

                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, layerMask)&&canShoot)
                {
                    Debug.DrawRay(cam.transform.position, cam.transform.forward * hit.distance, Color.yellow);
                    Debug.Log("Did Hit");
                    if (canShoot)
                    {
                        StartCoroutine(waitSeconds(1f / gun.fireRate));
                    }
                }
                else
                {
                    Debug.Log("Did not Hit");
                }
            }
        }
        else
        {
            fireCount = 0;
        }
    }
    void FixedUpdate() {
        Vector3 movement = speed*(transform.forward*Input.GetAxis("Vertical")+transform.right*Input.GetAxis("Horizontal"));
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
       
		if (Input.GetAxis("Jump")>.9f&&jumpMulti>0)
        {
            rb.velocity = movement + new Vector3(0,jumpHeight*jumpMulti,0);
            jumpMulti -= Time.fixedDeltaTime * 2;
        }
		transform.RotateAround(transform.position,transform.up,Input.GetAxis("Mouse X")*mouseSensX*Time.fixedDeltaTime);

        Vector3 camNewPos = Vector3.Lerp(cam.transform.position, tripod.transform.position, .45f);
        camRb.MovePosition(camNewPos);
        camTarget.transform.position = Vector3.Lerp(camTarget.transform.position, camTargetHolder.transform.position, .45f);
        camRb.transform.LookAt(camTarget.transform.position);
    }

    void OnTriggerEnter(Collider c) {
        if (c.CompareTag("Env"))
            jumpMulti = 1;
    }

    IEnumerator waitSeconds(float time)
    {
        canShoot = false;
        yield return new WaitForSeconds(time);
        canShoot = true;
    }
}
