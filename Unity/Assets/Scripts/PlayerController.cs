﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

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
    public Text ammoCount;
    public Slider reloading;
    public Gun gun;

    private bool zoom = false;
    private bool canShoot = true;
    private bool canReload = true;
    private bool doneReload = false;
    private bool manRel = false;

    private int currAmmo;
    private int totalAmmo;

    public UnityEvent reload;
    public UnityEvent shoot;

	void Start() {
		rb = GetComponentInChildren<Rigidbody>();
		camRb = cam.GetComponent<Rigidbody>();
		jumpMulti = 1;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
        currAmmo = gun.ammo;
        totalAmmo = gun.maxAmmo;
        ammoCount.text = "Ammo: " + currAmmo.ToString() + "/" + totalAmmo.ToString();
        reloading.value = 0;
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
        if (Input.GetMouseButtonDown(0) && gun.fireType == Type.Semi && canShoot && !manRel && currAmmo!=0)
        {
            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 8;
     
            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;

            RaycastHit hit;
            Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, layerMask);

                if (/*Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, layerMask) && */canShoot)
                {
                Debug.DrawRay(cam.transform.position, cam.transform.forward * (!(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, layerMask)) ? 0 : hit.distance), Color.yellow);
                Debug.Log("Did Shoot");
                currAmmo--;
                updateAmmo();
                shoot.Invoke();
                if (canShoot)
                {
                    StartCoroutine(waitSeconds(1f / gun.fireRate));
                }
            }
            else
            {
                Debug.Log("Did not Shoot");
            }
        }
        if (Input.GetMouseButton(0)&&gun.fireType!=Type.Semi)
        {
            if ((totalAmmo >= 0 || currAmmo > 0) && !manRel)
            {
                if (currAmmo > 0)
                {
                    if (currAmmo < gun.clipSize - 1)
                    {
                        // Bit shift the index of the layer (8) to get a bit mask
                        int layerMask = 1 << 8;

                        // This would cast rays only against colliders in layer 8.
                        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
                        layerMask = ~layerMask;

                        RaycastHit hit;
                        //Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, layerMask);
                        

                        if (/*Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, layerMask) && */canShoot)
                        {
                            Debug.DrawRay(cam.transform.position, cam.transform.forward * (!(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, layerMask))?0: hit.distance), Color.yellow);
                            Debug.Log("Did Shoot");
                            currAmmo--;
                            updateAmmo();
                            shoot.Invoke();
                            if (canShoot)
                            {
                                StartCoroutine(waitSeconds(1f / gun.fireRate));
                            }
                        }
                        else
                        {
                            Debug.Log("Did not Shoot");
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
                        Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, layerMask);

                        if (/*Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, layerMask) && */canShoot)
                        {
                            Debug.DrawRay(cam.transform.position, cam.transform.forward * (!(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, layerMask)) ? 0 : hit.distance), Color.yellow);
                            Debug.Log("Did Shoot");
                            currAmmo--;
                            updateAmmo();
                            shoot.Invoke();
                            if (canShoot)
                            {
                                StartCoroutine(waitSeconds(1f / gun.fireRate));
                            }
                        }
                        else
                        {
                            Debug.Log("Did not Shoot");
                        }
                    }
                }
            }
        }
        if (Input.GetKey("r") && currAmmo != gun.clipSize && totalAmmo>0)
        {
            manRel = true;
            canReload = true;
        }
        if ((currAmmo <= 0||manRel||doneReload)&&totalAmmo>0)
        {
            if (doneReload)
            {
                int place = currAmmo;
                currAmmo += gun.clipSize - currAmmo>totalAmmo?totalAmmo:gun.clipSize-currAmmo;
                totalAmmo -= gun.clipSize - currAmmo<totalAmmo&& totalAmmo-(gun.clipSize-place)>=0?gun.clipSize-place:totalAmmo;
                updateAmmo();
                reload.Invoke();
                doneReload = false;
                canReload = true;
                reloading.value = 0f;
                manRel = false;
            }
            //reloading
            if (canReload && (currAmmo <= 0||manRel))
            {
                StartCoroutine(reloadTime());
            }
            reloading.value += .0167f/gun.reloadSpeed;
        }
    }

    void OnTriggerEnter(Collider c) {
        if (c.CompareTag("Env"))
            jumpMulti = 1;
        if(c.CompareTag("Ammo Pickup"))
        {
            c.gameObject.SetActive(false);
            totalAmmo = gun.maxAmmo;
            currAmmo = gun.clipSize;
            updateAmmo();
        }
    }

    //fire rate implemented
    IEnumerator waitSeconds(float time)
    {
        canShoot = false;
        yield return new WaitForSeconds(time);
        canShoot = true;
    }

    void updateAmmo()
    {
        ammoCount.text = "Ammo: " + currAmmo.ToString() + "/" + totalAmmo.ToString();
    }

    IEnumerator reloadTime()
    {
        canReload = false;
        yield return new WaitForSeconds(gun.reloadSpeed);
        doneReload = true;
        manRel = false;
    }
}
