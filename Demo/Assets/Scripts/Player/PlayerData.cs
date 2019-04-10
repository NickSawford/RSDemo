using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerData : MonoBehaviour
{
    // Declares the Players Rigidbody
    private Rigidbody playerRigidbody;

    // Declares the Spawn settings for the player
    [Header("Spawn Settings")]
    public Vector3 playerSpawn;
    public GameObject loadManager;

    // Declares Mouse and Controller Settings
    [Header("Mouse And Controller Settings")]
    public float mouseSensitivity;
    public float gamePadSensitivity;
    public float UpDownRange;
    float verticalRotation;

    // Declares Player Movement Settings
    [Header("Movement Settings")]
    public float speed;
    public float maxVelocityChange;
    public float jumpHeight;
    public float jumpSpeed;
    Vector3 velocity;
    Vector3 velocityChange;
    Vector3 targetVelocity;

    // Declares Gravity and Terminal Velocity
    [Header("Gravity and Terminal Velocity")]
    public float gravity;
    public float acceleration;
    public float topSpeed;

    // Declares debug for the Current Speed
    [Header("Current Speed")]
    public float metersPerSecond;

    // Declares bools for Collision Checks
    [Header("Collision Checks")]
    public bool forwardCollisionOn;
    public bool downCollisionOn;
    public bool upCollisionOn;

    // Declares Teleport Checks
    [Header("Teleport Checks")]
    public Vector3 currentPosition;
    public Vector3 markPosition;
    public int markCD;
    public bool markOn;
    public int teleCD;
    public bool teleOn;
    public int markCDAmount;
    public int teleCDAmount;
    public bool markAllowed;

    // Declares Crouch Settings
    private CapsuleCollider playerCollider;
    private GameObject playerCamera;
    private Vector3 cameraPos1;
    private Vector3 cameraPos2;
    private float cameraSpeed;
    private bool crouchOn;

    // Use this for initialization
    void Start()
    {
        // Gets the Players Rigidbody from the GameObjects component list
        playerRigidbody = GetComponent<Rigidbody>();

        // Sets the Mouse Vertical Rotation to 0 as default
        verticalRotation = 0;

        // Removes the cursor and prevents from clicking off the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Removes the use of rotation and gravity for the Rigidbody
        playerRigidbody.freezeRotation = true;
        playerRigidbody.useGravity = false;

        // gets the Capsule Collider, Camera and crouch positions for the camera
        playerCollider = GetComponent<CapsuleCollider>();
        playerCamera = GameObject.Find("Main Camera");
        cameraPos1 = new Vector3(0, 1.75f, 0);
        cameraPos2 = new Vector3(0, 1, 0);

        // Sets the bools and cooldowns for the Marking and Teleport
        markOn = true;
        teleOn = true;
        markCD = markCDAmount;
        teleCD = teleCDAmount;
        markAllowed = true;

        // Sets the players Spawn and a default Mark so the Player does not default teleport to 0, 0, 0 
        this.gameObject.transform.position = playerSpawn;
        markPosition = playerSpawn;

        // Gets the Load Managers script from its components
        loadManager.GetComponent<LoadManager>();
    }

    // A Move Method thats takes in X and Z inputs from Keyboard or Controller
    public virtual void Move(float x, float z)
    {   
        // If the Player is on the ground        
        if (downCollisionOn)
        {
            // Calculate how fast the Player should be moving, transform in that direction and increase by set Speed
            targetVelocity = new Vector3(x, 0, z);
            targetVelocity = transform.TransformDirection(targetVelocity);
            targetVelocity *= speed;

            // Apply a force that attempts to reach the Players target velocity by changing the Rigidbodies velocity to Add the Force of the Velocity Change using 
            // a clamp to see if the value is between the min and max value (only for X and Z)
            velocity = playerRigidbody.velocity;
            velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            playerRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);                     
        }
        else
        {
            // Calculate how fast the Player should be moving, transform in that direction and increase by set Speed (As above)
            targetVelocity = new Vector3(x, 0, z);
            targetVelocity = transform.TransformDirection(targetVelocity);
            targetVelocity *= jumpSpeed;

            // Apply a force that attempts to reach the Players target velocity by changing the Rigidbodies velocity to Add an acceleration of the Velocity Change using 
            // a clamp to see if the value is between the min and max value (only for X and Z, and the use of acceleration gives the ability to move while jumping)
            velocity = playerRigidbody.velocity;
            velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            playerRigidbody.AddForce(velocityChange, ForceMode.Acceleration);       
        }

        // Apply gravity manually to the rigidbody to try and simulate terminal velocity ( v = the square root of ((2*m*g)/(ρ*A*C)) )
        playerRigidbody.AddForce(new Vector3(velocityChange.x, (-gravity * playerRigidbody.mass) * acceleration - (playerRigidbody.drag * playerRigidbody.velocity.y) * Time.fixedDeltaTime, velocityChange.z));

        // Calculate the rate of falling for debug
        metersPerSecond = Mathf.Round(playerRigidbody.velocity.magnitude * 100f) / 100f;

        // Set the drag
        playerRigidbody.drag = (acceleration / topSpeed);
    }

    // Method for Rotating with the mouse and for the camera and Player to look in that direction, takes in a Rotation X and Y
    public virtual void RotateLookMouse(float rotX, float rotY)
    {
        // Increases the rotation by the sensitivity given, the Y axis starts at 0 (Centre of screen) 
        rotX = rotX * mouseSensitivity;
        verticalRotation -= rotY * mouseSensitivity;

        // Clamps the lower angle and the upper angle so the camera cant look all the way in 360 degrees and rotates the camera accordingly
        verticalRotation = Mathf.Clamp(verticalRotation, -UpDownRange, UpDownRange);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        transform.Rotate(0, rotX, 0);
    }

    // Method for Rotating with the controller and for the camera and Player to look in that direction, takes in a Rotation X and Y (Works as the Mouse Method above)
    public virtual void RotateLookGamePad(float rotX, float rotY)
    {
        // Increases the rotation by the sensitivity given, the Y axis starts at 0 (Centre of screen) 
        rotX = rotX * gamePadSensitivity;
        verticalRotation -= rotY * gamePadSensitivity;

        // Clamps the lower angle and the upper angle so the camera cant look all the way in 360 degrees and rotates the camera accordingly
        verticalRotation = Mathf.Clamp(verticalRotation, -UpDownRange, UpDownRange);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        transform.Rotate(0, rotX, 0);
    }

    // Method for jumping which takes in a Key input
    public virtual void Jump(bool jump)
    {
        // If the Player is on the ground and recieves input to Jump than add Velocity to Y while mainting the input for X and Z
        if (downCollisionOn && jump)
        {
            playerRigidbody.velocity = new Vector3(velocityChange.x, CalculateJumpVerticalSpeed(), velocityChange.z);           
        }      
    }

    // Method for crouching
    public virtual void Crouch(bool crouch)
    {
        // Sets the camera to Lerp between a standing position (Pos 1) and a crouched position (Pos 2)
        playerCamera.transform.localPosition = Vector3.Lerp(cameraPos1, cameraPos2, cameraSpeed);

        // If the method recieves input than set crouch to true and change the collider height to crouched and increase by camera speed to do so
        if (crouch)
        {
            crouchOn = true;
            playerCollider.height = 1.5f;
            playerCollider.center = new Vector3(0, 0.75f, 0);
            cameraSpeed += Time.deltaTime * 8;
            if (cameraSpeed >= 1)           
                cameraSpeed = 1;                     
        }

        // Else if there is not something above the player, set crouched to false change the collider height to standing and increase by camera speed to do so
        else if (!upCollisionOn)
        {
            crouchOn = false;
            playerCollider.height = 2;
            playerCollider.center = new Vector3(0, 1.01f, 0);
            cameraSpeed -= Time.deltaTime * 8;
            if (cameraSpeed <= 0)            
                cameraSpeed = 0;           
        }
    }

    // Teleport and Mark Method
    public virtual void Teleport(bool mark, bool tele)
    { 
        // If the player is allowed to Mark and if Mark has been inputted and is not on cooldown
        if (markAllowed)
        {
            if (mark && markOn)
            {
                // Set the mark position to equal the current position and set mark to false
                markPosition = currentPosition;
                markOn = false;
            }
        }

        // If teleport is inputted and is off cooldown
        if (tele && teleOn)
        {
            // Set the players position to equal the marked position and set teleport to false
            this.gameObject.transform.position = markPosition;
            teleOn = false;
        }
    }

    // Method for calculating the vertical speed of the Jump
    float CalculateJumpVerticalSpeed()
    {
        // returns the square root of 2 x the jump heigh set x the gravity
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    // Method for using Raycast Collision as opposed to Collider Collisions
    void RaycastCollision()
    {
        // Declares the raycast
        RaycastHit hit = new RaycastHit();

        // Declares and sets the forward rays and the positions of those rays in relation to the player
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 forwardPos1 = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        Vector3 forwardPos2 = new Vector3(transform.position.x, transform.position.y + 1.8f, transform.position.z);
        Vector3 forwardPos3 = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);

        // Declares and sets the downward rays and the positons of those ray in relation to the player
        Vector3 down = transform.TransformDirection(-Vector3.up);
        Vector3 downPos1 = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        Vector3 downPos2 = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, transform.position.z);
        Vector3 downPos3 = new Vector3(transform.position.x - 0.5f, transform.position.y + 0.5f, transform.position.z);
        Vector3 downPos4 = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z + 0.5f);
        Vector3 downPos5 = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z - 0.5f);

        // Declares the downward rays
        Vector3 up;
        Vector3 upPos1;
        Vector3 upPos3;
        Vector3 upPos2;
        Vector3 upPos4;
        Vector3 upPos5;

        // Sets the upward rays positions relative to the player if the player is crouched or standing
        if (crouchOn)
        {
            up = transform.TransformDirection(Vector3.up);
            upPos1 = new Vector3(transform.position.x, transform.position.y + 1.4f, transform.position.z);
            upPos3 = new Vector3(transform.position.x - 0.5f, transform.position.y + 1.4f, transform.position.z);
            upPos2 = new Vector3(transform.position.x + 0.5f, transform.position.y + 1.4f, transform.position.z);
            upPos4 = new Vector3(transform.position.x, transform.position.y + 1.4f, transform.position.z + 0.5f);
            upPos5 = new Vector3(transform.position.x, transform.position.y + 1.4f, transform.position.z - 0.5f);
        }
        else
        {
            up = transform.TransformDirection(Vector3.up);
            upPos1 = new Vector3(transform.position.x, transform.position.y + 1.8f, transform.position.z);
            upPos3 = new Vector3(transform.position.x - 0.5f, transform.position.y + 1.8f, transform.position.z);
            upPos2 = new Vector3(transform.position.x + 0.5f, transform.position.y + 1.8f, transform.position.z);
            upPos4 = new Vector3(transform.position.x, transform.position.y + 1.8f, transform.position.z + 0.5f);
            upPos5 = new Vector3(transform.position.x, transform.position.y + 1.8f, transform.position.z - 0.5f);
        }

        // If the Raycast that his facing forwards hit something (in a 0.6f distance), set forward collision true else false
        if (Physics.Raycast(forwardPos1, forward, 0.6f) || Physics.Raycast(forwardPos2, forward, 0.6f)
           || Physics.Raycast(forwardPos3, forward, 0.6f))
            forwardCollisionOn = true;           
        else
            forwardCollisionOn = false;

        // If the Raycast that his facing up hit something (in a 0.6f distance), set up collision true else false
        if (Physics.Raycast(upPos1, up, 0.6f) || Physics.Raycast(upPos2, up, 0.6f) || Physics.Raycast(upPos3, up, 0.6f)
            || Physics.Raycast(upPos4, up, 0.6f) || Physics.Raycast(upPos5, up, 0.6f))        
            upCollisionOn = true;       
        else
            upCollisionOn = false;

        // If the Raycast that his facing down hit something (in a 0.6f distance)
        if (Physics.Raycast(downPos1, down, out hit, 0.6f) || Physics.Raycast(downPos2, down, out hit, 0.6f) 
            || Physics.Raycast(downPos3, down, out hit, 0.6f) || Physics.Raycast(downPos4, down, out hit, 0.6f) 
            || Physics.Raycast(downPos5, down, out hit, 0.6f))
        {
            // Set collision down to true
            downCollisionOn = true;
            
            // If the Ray collides with a collider "Moving Platform"
            if (hit.collider.gameObject.tag == "Moving Platform")
            {
                // Set the player a child of the moving platform to prevent bouncing and instability
                transform.parent = hit.collider.gameObject.transform;
            }
            else
            {
                // otherwise set the players parent to null
                transform.parent = null;
            }

            // If the Raycast hits a collider with tag "Out of Bounds" then spawn the player back to the start spawn
            if (hit.collider.gameObject.tag == "Out of Bounds")
                this.gameObject.transform.position = playerSpawn;

            // If the Raycast hits a collider with tag "End Spawn" then set the Load Manager to load the main menu
            if (hit.collider.gameObject.tag == "End Spawn")
            {
                loadManager.GetComponent<LoadManager>().LoadSpecific(1);
            }

            // If the Raycast hits a collider with tag "Mark Allowed" then set mark allowed to true so that the ground is abled to be marked, else set it to false
            if (hit.collider.gameObject.tag == "Mark Allowed")
            {
                markAllowed = true;
            }
            else
                markAllowed = false;
        }
        else
            downCollisionOn = false;

        // Forward raycast debug 
        Debug.DrawRay(forwardPos1, forward, Color.blue);
        Debug.DrawRay(forwardPos2, forward, Color.blue);
        Debug.DrawRay(forwardPos3, forward, Color.blue);

        // Down raycast debug 
        Debug.DrawRay(downPos1, down, Color.green);
        Debug.DrawRay(downPos2, down, Color.green);
        Debug.DrawRay(downPos3, down, Color.green);
        Debug.DrawRay(downPos4, down, Color.green);
        Debug.DrawRay(downPos5, down, Color.green);

        // Up raycast debug 
        Debug.DrawRay(upPos1, up, Color.red);
        Debug.DrawRay(upPos2, up, Color.red);
        Debug.DrawRay(upPos3, up, Color.red);
        Debug.DrawRay(upPos4, up, Color.red);
        Debug.DrawRay(upPos5, up, Color.red);
    }

    // Update is called every fixed frame-rate frame for the raycasts to update
    void FixedUpdate()
    {
        RaycastCollision();
    }

    // Update is called once per frame
    void Update()
    {
        // sets the current position to be that of the players position
        currentPosition = this.gameObject.transform.position;

        // If mark is not set to true
        if (!markOn)
        {
            // Decrease the mark cooldown counter
            markCD--;

            // If the mark cooldown counter is less than or equal to 0 then set the cooldown the cooldown amount and set mark to true
            if (markCD <= 0)
            {
                markCD = markCDAmount;
                markOn = true;
            }
        }

        // If teleport is not set to true
        if (!teleOn)
        {
            // Decrease the teleport cooldown counter
            teleCD--;

            // If the teleport cooldown counter is less than or equal to 0 then set the cooldown the cooldown amount and set teleport to true
            if (teleCD <= 0)
            {
                teleCD = teleCDAmount;
                teleOn = true;
            }
        }
    }
}
