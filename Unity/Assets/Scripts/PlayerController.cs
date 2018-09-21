using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

//need to give separate ammo to each gun
//need to implement swap weapons with wait time
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
    public Gun primary;
    public Gun secondary;
    public int maxHealth;
    public Slider healthBar;
    public Text health;

    private Gun currGun;
    private bool zoom = false;
    private bool canShoot = true;
    private bool canReload = true;
    private bool doneReload = false;
    private bool manRel = false;
    private bool swapping = false;
    private bool swapped;

    private IEnumerator reloadingTime;
    private IEnumerator shot;
    
    private int currAmmo;
    private int totalAmmo;
    private int currHealth;

    private int primaryTotalAmmo;
    private int primaryCurrAmmo;

    private int secondaryTotalAmmo;
    private int secondaryCurrAmmo;

    public UnityEvent reload;
    public UnityEvent shoot;

	void Start() {
        currGun = primary;
        rb = GetComponentInChildren<Rigidbody>();
		camRb = cam.GetComponent<Rigidbody>();
		jumpMulti = 1;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
        swapped = false;

        primaryCurrAmmo = primary.clipSize;
        primaryTotalAmmo = primary.maxAmmo;
        secondaryCurrAmmo = secondary.clipSize;
        secondaryTotalAmmo = secondary.maxAmmo;
        currAmmo = currGun.ammo;
        totalAmmo = currGun.maxAmmo;
        currHealth = maxHealth;
        ammoCount.text = "\t" + currGun.name + "\n" + "Ammo: " + currAmmo.ToString() + "/" + totalAmmo.ToString();
        reloading.value = 0;
        health.text = currHealth.ToString();
        healthBar.value = currHealth;
        
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

        if (Input.GetMouseButtonDown(0) && currGun.fireType == Type.Semi && canShoot && !manRel && currAmmo!=0)
        {
            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 8;
     
            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;

            RaycastHit hit;
            Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, layerMask);

                if (canShoot)
                {
                Debug.DrawRay(cam.transform.position, cam.transform.forward * (!(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, layerMask)) ? 0 : hit.distance), Color.yellow);
                currAmmo--;
                updateAmmo();
                shoot.Invoke();
                if (canShoot)
                {
                    StartCoroutine(waitSeconds(1f / currGun.fireRate));
                }
            }
        }

        if (Input.GetMouseButton(0)&&currGun.fireType!=Type.Semi)
        {
            if ((totalAmmo >= 0 || currAmmo > 0) && !manRel)
            {
                if (currAmmo > 0)
                {
                    if (currAmmo < currGun.clipSize - 1)
                    {
                        // Bit shift the index of the layer (8) to get a bit mask
                        int layerMask = 1 << 8;

                        // This would cast rays only against colliders in layer 8.
                        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
                        layerMask = ~layerMask;

                        RaycastHit hit;
                        

                        if (canShoot)
                        {
                            Debug.DrawRay(cam.transform.position, cam.transform.forward * (!(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, layerMask))?0: hit.distance), Color.yellow);
                            currAmmo--;
                            updateAmmo();
                            shoot.Invoke();
                            if (canShoot)
                            {
                                StartCoroutine(waitSeconds(1f / currGun.fireRate));
                            }
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

                        if (canShoot)
                        {
                            Debug.DrawRay(cam.transform.position, cam.transform.forward * (!(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, layerMask)) ? 0 : hit.distance), Color.yellow);
                            currAmmo--;
                            updateAmmo();
                            shoot.Invoke();
                            if (canShoot)
                            {
                                StartCoroutine(waitSeconds(1f / currGun.fireRate));
                            }
                        }
                    }
                }
            }
        }

        if (doneReload)
        {
            if (swapped)
            {
                swapped = false;
                doneReload = false;
                canReload = true;
                manRel = false;
            }
        }

        if (Input.GetKey("r") && currAmmo != currGun.clipSize && totalAmmo>0)
        {
            manRel = true;
            canReload = true;
        }
        if (((currAmmo <= 0||manRel||doneReload)&&totalAmmo>0)&&!swapping)
        {
            if (doneReload&&!swapped&&!swapping)
            {
                int place = currAmmo;
                currAmmo += currGun.clipSize - currAmmo>totalAmmo?totalAmmo:currGun.clipSize-currAmmo;
                totalAmmo -= currGun.clipSize - currAmmo<totalAmmo&& totalAmmo-(currGun.clipSize-place)>=0?currGun.clipSize-place:totalAmmo;
                updateAmmo();
                reload.Invoke();
                doneReload = false;
                canReload = true;
                reloading.value = 0f;
                manRel = false;
            }
            //reloading
            if (canReload && (currAmmo <= 0||manRel)&& !swapping)
            {
                reloadingTime = reloadTime();
                StartCoroutine(reloadTime());
                Debug.Log("Start!");
            }
            reloading.value += .0167f/currGun.reloadSpeed;
        }

        if (Input.GetKeyDown("q"))
        {
            swapWeapon();
            StartCoroutine(swappo(currGun.reloadSpeed/2));
        }
    }

    void OnTriggerEnter(Collider c) {
        if (c.CompareTag("Env"))
            jumpMulti = 1;
        if(c.CompareTag("Ammo Pickup"))
        {
            c.gameObject.SetActive(false);
            totalAmmo = currGun.maxAmmo;
            currAmmo = currGun.clipSize;
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

    IEnumerator swappo(float time)
    {
        swapping = true;
        StopCoroutine(reloadingTime);
        Debug.Log("Stop!");
        yield return new WaitForSeconds(time);
        swapping = false;
        manRel = false;
        doneReload = false;
        canReload = true;
        swapped = true;
    }

    IEnumerator reloadTime()
    {
        canReload = false;
        yield return new WaitForSeconds(currGun.reloadSpeed);
        doneReload = true;
        manRel = false;
    }

    void updateAmmo()
    {
        ammoCount.text = "\t" + currGun.name + "\n" + "Ammo: " + currAmmo.ToString() + "/" + totalAmmo.ToString();
    }

    void setCurrHealth(int change)
    {
        currHealth = change;
        health.text = currHealth.ToString();
        healthBar.value = currHealth;
    }

    int getCurrHealth()
    {
        return currHealth;
    }

    void swapWeapon()
    {
        reloading.value = 0;
        if (currGun == secondary)
        {
            secondaryCurrAmmo = currAmmo;
            secondaryTotalAmmo = totalAmmo;

            currGun = primary;
            currAmmo = primaryCurrAmmo;
            totalAmmo = primaryTotalAmmo;
            updateAmmo();
        }
        else if(currGun == primary)
        {
            primaryCurrAmmo = currAmmo;
            primaryTotalAmmo = totalAmmo;

            currGun = secondary;
            currAmmo = secondaryCurrAmmo;
            totalAmmo = secondaryTotalAmmo;
            updateAmmo();
        }
    }
}
