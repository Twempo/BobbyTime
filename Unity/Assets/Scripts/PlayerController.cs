using System.Collections;
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
    public Text ammoCount;
    public Slider reloading;
    public Gun gun;
    Rigidbody camRb;

    private bool zoom = false;
    private bool canShoot = true;
    private bool canReload = true;
    private bool doneReload = false;

    private int currAmmo;
    private int totalAmmo;

    public UnityEvent reload;
    public UnityEvent shoot;

    void Start() {
        rb = GetComponentInChildren<Rigidbody>();
        camRb = cam.GetComponent<Rigidbody>();
        jumpMulti = 1;
        currAmmo = gun.ammo;
        totalAmmo = gun.maxAmmo;
        ammoCount.text = "Ammo: " + currAmmo.ToString() + "/" + totalAmmo.ToString();
        reloading.value = 0;
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
            if (totalAmmo > 0 || currAmmo > 0)
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

                else
                {
                    if (doneReload)
                    {
                        Debug.Log("done reloading");
                        totalAmmo -= gun.clipSize - currAmmo;
                        currAmmo += gun.clipSize;
                        //Debug.Log(currAmmo);
                        updateAmmo();
                        reload.Invoke();
                        doneReload = false;
                        canReload = true;
                    }
                    //reloading
                    if (canReload&&currAmmo<=0)
                    {
                        StartCoroutine(reloadTime());
                        //Debug.Log("reloading");
                    }
                }

            }
        }
        if (currAmmo <= 0)
        {
            if (doneReload)
            {
                totalAmmo -= gun.clipSize - currAmmo;
                currAmmo += gun.clipSize;
                updateAmmo();
                reload.Invoke();
                doneReload = false;
                canReload = true;
                reloading.value = 0f;
            }
            //reloading
            if (canReload && currAmmo <= 0)
            {
                StartCoroutine(reloadTime());
            }
            reloading.value += .0167f/gun.reloadSpeed;
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
    }
}
