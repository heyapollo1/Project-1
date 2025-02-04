using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
//using System;

public class Shooting : MonoBehaviour
{
    public GameObject bullet;
    public float fireForce = 20f;
    public bool canFire;
    private float timer;
    public float timeBetweenFiring;

    public Transform aimTransform; // Reference to the Aim parent
    public Transform gunTransform;
    public Transform player;
    public SpriteRenderer gunSpriteRenderer; // SpriteRenderer for the gun
    public Sprite[] gunSprites; // Array of sprites for each direction
    public Transform[] firePoints;

    private int currentDirectionIndex;
    private Vector3 mousePosition;

    void Update()
    {
        HandleAiming();

        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring)
            {
                canFire = true;
                timer = 0;
            }
        }

        if (Input.GetMouseButton(0) && canFire)
        {
            Fire(currentDirectionIndex);
            canFire = false;
        }
    }

    void HandleAiming()
    {
        // Get the mouse position in world coordinates
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure the z-axis is 0 for 2D

        // Calculate the direction from the player to the mouse
        Vector3 direction = mousePosition - player.position;

        // Calculate the angle between the player and the mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //Normalize angle to be between 0 and 360 degrees
        if (angle < 0)
        {
            angle += 360;
        }
        
        aimTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));


        // Update the rotation of the gun
        switch (currentDirectionIndex)
        {
            case 0: // Right
                gunTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0)); // No offset
                break;
            case 1: // Up-Right
                gunTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, -45)); // Example offset
                break;
            case 2: // Up
                gunTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90)); // Example offset
                break;
            case 3: // Up-Left
                gunTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, -135)); // Example offset
                break;
            case 4: // Left
                gunTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, -180)); // Example offset
                break;
            case 5: // Down-Left
                gunTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, -225)); // Example offset
                break;
            case 6: // Down
                gunTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, -270)); // Example offset
                break;
            case 7: // Down-Right
                gunTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, -315)); // Example offset
                break;
        }

        // Calculate the current direction index (0-7) for the 8 directions
        currentDirectionIndex = Mathf.RoundToInt((angle + 180) / 45f) % 8;

        currentDirectionIndex = (currentDirectionIndex + 4) % 8;

        // Update the gun sprite based on the current direction
        gunSpriteRenderer.sprite = gunSprites[currentDirectionIndex];
    }

void Fire(int directionIndex)
{
    if (directionIndex >= 0 && directionIndex < firePoints.Length)
    {
        // Get the correct fire point based on the direction index
        Transform selectedFirePoint = firePoints[directionIndex];

        // Instantiate the projectile at the correct fire point
        GameObject projectile = Instantiate(bullet, selectedFirePoint.position, selectedFirePoint.rotation);

        // Set the projectile's direction and apply force
        Vector2 fireDirection = (mousePosition - player.position).normalized;
        projectile.GetComponent<Rigidbody2D>().velocity = fireDirection * fireForce;

        Debug.Log($"Fire Direction: {fireDirection}");
        }
        else
        {
            Debug.LogError("Direction index out of range for fire points.");
        }
    }
}
/*public class Shooting : MonoBehaviour
{
    //private Camera sceneCamera;
    public GameObject bullet;
    public float fireForce = 20f;
    public bool canFire;
    private float timer;
    public float timeBetweenFiring;

    public Transform aimTransform; // Reference to the Aim parent
    public Transform gunTransform;
    public Transform player;
    public Transform[] firePoints;
    public float angleThreshold = 22.5f;
    public Animator weaponAnimator;

    private int currentDirectionIndex;
    Vector3 mousePosition;

    void Update()
    {
        HandleAiming();

        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring)
            {
                canFire = true;
                timer = 0;
            }
        }

        if (Input.GetMouseButton(0) && canFire)
        {
            Fire(currentDirectionIndex);
            canFire = false;
        }
    }

    void HandleAiming()
    {
        // Get the mouse position in world coordinates
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure the z-axis is 0 for 2D

        // Calculate the direction from the player to the mouse
        Vector3 direction = mousePosition - player.position;

        // Calculate the angle between the player and the mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Normalize angle to be between 0 and 360 degrees
        if (angle < 0)
        {
            angle += 360;
        }

        // Update the rotation of the Aim parent
        aimTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        //float spriteOffset = 90f; // Adjust this value as needed
        //aimTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + spriteOffset));

        // Calculate the current direction index (0-7) for the 8 directions
        currentDirectionIndex = Mathf.RoundToInt((angle + 180) / 45f) % 8;

        // Update the "Direction" parameter in the Animator
        weaponAnimator.SetFloat("Direction", angle);

    }

    void Fire(int directionIndex)
    {
        // Ensure directionIndex is valid
        if (directionIndex >= 0 && directionIndex < firePoints.Length)
        {
            // Get the correct fire point based on the direction index
            Transform selectedFirePoint = firePoints[directionIndex];

            // Instantiate the projectile at the correct fire point
            GameObject projectile = Instantiate(bullet, selectedFirePoint.position, selectedFirePoint.rotation);

            // Set the projectile's direction and apply force
            Vector2 fireDirection = (mousePosition - selectedFirePoint.position).normalized;
            projectile.GetComponent<Rigidbody2D>().velocity = fireDirection * fireForce;

            Debug.Log($"Fire Direction: {fireDirection}");
        }
        else
        {
            Debug.LogError("Invalid direction index.");
        }
    }
}
/*public class Shooting : MonoBehaviour

{

    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs : EventArgs
{
        public Vector3 gunEndPointPosition;
        public Vector3 shootPosition;
}

    private Camera sceneCamera;
    //private Transform aimTransform;
    //private Animator aimAnimator;
    //private Transform firePoint;
    //private Transform firePoint;
    public GameObject bullet;
    public float fireForce = 20f;
    public bool canFire;
    private float timer;
    public float timeBetweenFiring;

    //ChatGPT code
    public Transform gunTransform;   // Assign the gun transform in the inspector
    //public Sprite gunSpriteRenderer; // SpriteRenderer to change the gun's sprite
    public Transform player;
    //public Sprite[] gunIdleSprites;  // 8 sprites for idle states
    public GameObject[] firePoints; // Array of FirePoints for each direction
    //public Sprite[] gunShootSprites; // 8 sprites for shoot states
    public float angleThreshold = 22.5f; // Angle threshold to determine direction
    public Animator weaponAnimator;

    private int currentDirectionIndex;
    //private bool isShooting = false;

    Vector3 mousePosition;

    // Start is called before the first frame update
    void Awake()
    {
        //aimTransform = transform.Find("Aim");
        //aimAnimator = aimTransform.GetComponent<Animator>();
        //firePoint = aimTransform.Find("FirePoint");
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        HandleAiming();
        //UpdateGunDirection();
        //HandleShooting();

        if (!canFire)
        {
            timer += Time.deltaTime;//means go up with time of the game.
            if (timer > timeBetweenFiring)
            {
                canFire = true;
                timer = 0;
            }
        }

        if (Input.GetMouseButton(0) && canFire)
        {
            Fire(currentDirectionIndex);
            canFire = false;
        }
    }

    void FixedUpdate()
    {
        //mousePosition = sceneCamera.ScreenToWorldPoint(Input.mousePosition);//assign mouse position on scene camera
    }

    void HandleAiming()
    {

        //mousePosition = sceneCamera.ScreenToWorldPoint(Input.mousePosition);//assign mouse position on scene camera
        //mousePosition.z = 0;

        //Vector2 direction = (mousePosition - gunTransform.position).normalized;
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();

        // Calculate the angle between the player and the mouse
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Calculate the current direction index (0-7) for the 8 directions
        int currentDirectionIndex = Mathf.RoundToInt((angle + 180) / 45f) % 8;


        Debug.Log(angle);

        // Convert angle to 0° - 360° range
        if (angle < 0)
        {
            angle += 360;
        }

        weaponAnimator.SetFloat("Direction", angle);
    }

    void Fire(int directionIndex)
    {
        // Get the correct fire point based on the direction index
        Transform selectedFirePoint = firePoints[directionIndex].transform;

        // Instantiate the projectile at the correct fire point
        GameObject projectile = Instantiate(bullet, selectedFirePoint.position, selectedFirePoint.rotation);

        // Set the projectile's direction and apply force
        Vector2 fireDirection = (selectedFirePoint.position - player.position).normalized;
        projectile.GetComponent<Rigidbody2D>().velocity = fireDirection * fireForce;

    //Vector2 firingDirection = (mousePosition - firePoint.position).normalized;
    //GameObject projectile = Instantiate(bullet, firePoint.position, Quaternion.identity);
    //projectile.GetComponent<Rigidbody2D>().AddForce(firePoint.right * fireForce, ForceMode2D.Impulse);
    //Bullet projectileScript = projectile.GetComponent<Bullet>();
    //projectileScript.Initialize(firingDirection);

    //GameObject projectile = Instantiate(bullet, firePoint.position, firePoint.rotation);
        //projectile.GetComponent<Rigidbody2D>().AddForce(firePoint.right * fireForce, ForceMode2D.Impulse);
    }
}/*


/*{
    public Animator anim;
    public float deadZone;

    enum Facing {N,S,E,W,NE,SE,SW,NW};
    private Camera sceneCamera;
    private Transform aimTransform;
    private Facing myDirection;
    public SpriteRenderer rend;
    private Sprite weaponN, weaponS, weaponE, weaponW, weaponNE, weaponSE, weaponSW, weaponNW;

    Vector3 playerPosition;
    Vector3 mousePosition;

    // Start is called before the first frame update
    void Awake()
    {
        aimTransform = transform.Find("Weapon");
    }

    void Start()
    {
        sceneCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        weaponN = Resources.Load<Sprite>("gun pointer_1");
        weaponS = Resources.Load<Sprite>("gun pointer_5");
        weaponE = Resources.Load<Sprite>("gun pointer_3");
        weaponW = Resources.Load<Sprite>("gun pointer_7");
        weaponNE = Resources.Load<Sprite>("gun pointer_2");
        weaponSE = Resources.Load<Sprite>("gun pointer_6");
        weaponSW = Resources.Load<Sprite>("gun pointer_4");
        weaponNW = Resources.Load<Sprite>("gun pointer_0");

    }

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
        Animate();
        Debug.Log(myDirection);
    }

    void FixedUpdate()
    {
        playerPosition = sceneCamera.WorldToScreenPoint(transform.position);
        mousePosition = Input.mousePosition;//assign mouse position on scene camera
    }

    /*void ProcessInputs()
    {
        Vector3 aimDirection = mousePosition - transform.position;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, aimAngle);
        Debug.Log(aimAngle);
    }*/

/*void ProcessInputs()
{
    Vector3 aimDirection = mousePosition - transform.position;
    float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
    aimTransform.eulerAngles = new Vector3(0, 0, aimAngle);

    float xDelta = mousePosition.x - playerPosition.x;
    float yDelta = mousePosition.y - playerPosition.y;

    if (yDelta > 0 && Mathf.Abs(xDelta) < deadZone) myDirection = Facing.N;
    if (yDelta < 0 && Mathf.Abs(xDelta) < deadZone) myDirection = Facing.S;

    if (xDelta > 0 && Mathf.Abs(yDelta) < deadZone) myDirection = Facing.E;
    if (xDelta < 0 && Mathf.Abs(yDelta) < deadZone) myDirection = Facing.W;

    if (Mathf.Abs(xDelta - yDelta) < deadZone)
    {
        if (xDelta > 0 && yDelta > 0) myDirection = Facing.NE;
        if (xDelta > 0 && yDelta < 0) myDirection = Facing.SE;
        if (xDelta < 0 && yDelta > 0) myDirection = Facing.NW;
        if (xDelta < 0 && yDelta < 0) myDirection = Facing.SW;
    }


}

void Animate()
{
    if (myDirection == Facing.N)
    {
        rend.sprite = weaponN;
    }
    else if (myDirection == Facing.S)
    {
        rend.sprite = weaponS;
    }
    else if (myDirection == Facing.E)
    {
        rend.sprite = weaponE;
    }
    else if (myDirection == Facing.W)
    {
        rend.sprite = weaponW;
    }
    else if (myDirection == Facing.S)
    {
        rend.sprite = weaponS;
    }
    else if (myDirection == Facing.NE)
    {
        rend.sprite = weaponNE;
    }
    else if (myDirection == Facing.SE)
    {
        rend.sprite = weaponSE;
    }
    else if (myDirection == Facing.SW)
    {
        rend.sprite = weaponSW;
    }
    else if (myDirection == Facing.NW)
    {
        rend.sprite = weaponNW;
    }

}*/
/*void Animate()
    {
        anim.SetFloat("AnimMoveX", myDirection.Facing);
        anim.SetFloat("AnimMoveY", myDirection.y);
        anim.SetFloat("AnimMoveMagnitude", moveDirection.magnitude);
        anim.SetFloat("AnimLastMoveX", lastMoveDirection.x);
        anim.SetFloat("AnimLastMoveY", lastMoveDirection.y);
    }
}*/


/*
{

    private Camera sceneCamera;
    private Transform aimTransform;
    public float deadZone;

    enum Facing { N, S, E, W, NE, SE, SW, NW };
    private Facing myDirection = Facing.S;

    Vector3 mousePosition;

    // Start is called before the first frame update
    void Awake()
    {
        aimTransform = transform.Find("Weapon");
    }

    void Start()
    {
        sceneCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
    }

    void FixedUpdate()
    {
        mousePosition = sceneCamera.ScreenToWorldPoint(Input.mousePosition);//assign mouse position on scene camera
    }

    void ProcessInputs()
    {
        Vector3 aimDirection = mousePosition - transform.position;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, aimAngle);
        Debug.Log(aimAngle);
    }
}

*/