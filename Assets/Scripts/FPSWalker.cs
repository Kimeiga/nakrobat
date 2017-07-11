using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
public class FPSWalker: MonoBehaviour {
	
	public float runSpeed = 6.0f;
	
	public float walkSpeed = 2.0f;

    //hakan
    public bool walkActivated = false;

    //for weaponsway script's animations
    public bool running = true;
    public bool walking = false;
	
	// If checked, the run key toggles between running and walking. Otherwise player runs if the key is held down and walks otherwise
	// There must be a button set up in the Input Manager called "Run"
	public bool toggleWalk = false;
	
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;
	
	// Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
	public float fallingDamageThreshold = 10.0f;
	
	// If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
	public bool slideWhenOverSlopeLimit = false;
	
	// If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
	public bool slideOnTaggedObjects = false;
	
	public float slideSpeed = 12.0f;
	
	// If checked, then the player can change direction while in the air
	public bool airControl = false;
	
	// Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
	public float antiBumpFactor = .75f;
	
	// Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping
	public int antiBunnyHopFactor = 1;
	
	public Vector3 moveDirection = Vector3.zero;
	public bool grounded = false;
	private CharacterController controller;
	public float speed;
    private float desiredSpeed;
	private RaycastHit hit;
	private float fallStartLevel;
	private bool falling;
	private float slideLimit;
	private float rayDistance;
	private Vector3 contactPoint;
	private bool playerControl = false;
	private int jumpTimer;


    //Hakan Added These

    //Crouching Control
    private float height;
    //public Transform headTrans;
    public bool crouching;
    public float crouchSpeed = 6;

    Vector3 headPosition;
    public Vector3 lastHeadPosition = Vector3.zero;
    public float measuredHeadSpeed;
    //public GameObject head;

    public bool justJumped = false;
    public Vector3 tempMoveDirection = Vector3.zero;
    public bool jumped = false;

    void Start() {

		controller = GetComponent<CharacterController>();
		desiredSpeed = runSpeed;
		rayDistance = controller.height * .5f + controller.radius;
		slideLimit = controller.slopeLimit - .1f;
		jumpTimer = antiBunnyHopFactor;

        height = controller.height;
    }
    


	void FixedUpdate() {

        speed = Mathf.Lerp(speed, desiredSpeed, 0.5f);

        //measuredHeadSpeed = (head.transform.position - lastHeadPosition).magnitude;
        //lastHeadPosition = head.transform.position;

        float inputX = Input.GetAxis("Horizontal");
		float inputY = Input.GetAxis("Vertical");


		if (grounded) {
			bool sliding = false;
			// See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
			// because that interferes with step climbing amongst other annoyances
			if (Physics.Raycast(transform.position, -Vector3.up, out hit, rayDistance)) {
				if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
					sliding = true;
			}
			// However, just raycasting straight down from the center can fail when on steep slopes
			// So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
			else {
				Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
				if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
					sliding = true;
			}
			
			// If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
			if (falling) {
				falling = false;
				if (transform.position.y < fallStartLevel - fallingDamageThreshold)
					FallingDamageAlert (fallStartLevel - transform.position.y);
			}
			
			// If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
			if ( (sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide") ) {
				Vector3 hitNormal = hit.normal;
				moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
				Vector3.OrthoNormalize (ref hitNormal, ref moveDirection);
				moveDirection *= slideSpeed;
				playerControl = false;
			}
			// Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
			else {

                
                Vector3 tempMoveDirection = new Vector3(inputX, 0, inputY);
                float moveMagnitude = Mathf.Clamp(tempMoveDirection.magnitude, 0, 1);
                moveDirection = new Vector3(0, -antiBumpFactor, 0) + (tempMoveDirection.normalized * moveMagnitude);
                moveDirection = transform.TransformDirection(moveDirection) * speed;
				playerControl = true;
			}
			
			// Jump! But only if the jump button has been released and player has been grounded for a given number of frames
			if (!Input.GetButton("Jump"))
				jumpTimer++;
			else if (jumpTimer >= antiBunnyHopFactor) {
				moveDirection.y = jumpSpeed;
                justJumped = true;
                jumped = true;
				jumpTimer = 0;
			}
            

        }
		else {

			// If we stepped over a cliff or something, set the height at which we started falling
			if (!falling) {
				falling = true;
				fallStartLevel = transform.position.y;
			}
			
			// If air control is allowed, check movement but don't touch the y component
			if (airControl && playerControl) {

                
                tempMoveDirection = new Vector3(inputX, 0, inputY);
                float tempMagnitude = Mathf.Clamp(tempMoveDirection.magnitude, 0, 1f);
                tempMoveDirection = tempMoveDirection.normalized * tempMagnitude;
                tempMoveDirection = transform.TransformDirection(tempMoveDirection) * speed;
                tempMoveDirection.y = moveDirection.y;
                moveDirection = tempMoveDirection;
                
			}
		}
        

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;
		
		// Move the controller, and set grounded true or false depending on whether we're standing on something
		grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;

        //Crouching Control
        float h = 0;
        Vector3 pos = transform.position;
        //headPosition = headTrans.position;

        if (Input.GetButton("Crouch"))
        {
            h = 1;

            crouching = true;
        }
        else
        {
            crouching = false;
            h = height;

        }

        if (justJumped == true && grounded == true || falling == true && grounded == true) {

            float landHeight = controller.height - 0.4f;

            pos.y += (landHeight - controller.height) * 0.5f;
            headPosition.y += (landHeight - controller.height);

            controller.height = landHeight;

            h = height;
            justJumped = false;
        }

        float lastHeight = controller.height;
        controller.height = Mathf.Lerp(controller.height, h, crouchSpeed * Time.deltaTime);
        pos.y += (controller.height - lastHeight) * 0.5f;
        headPosition.y += (controller.height - lastHeight);
        transform.position = pos;
        //headTrans.position = headPosition;




        //prevents sticking to the ceiling
        if ((controller.collisionFlags & CollisionFlags.Above) != 0)
        {
            moveDirection.y = -1f;
        }


    }

    void Update()
    {

        

        
        //this switches between walkSpeed and runSpeed if there is no walk toggle
        if (!toggleWalk) {
            if (Input.GetButton("Walk") || crouching == true)
            {
                desiredSpeed = walkSpeed;
                walking = true;
                running = false;
            }
            else {

                desiredSpeed = runSpeed;
                walking = false;
                running = true;
            }
        }

        //if there is walk toggle, this switches between walkActivated mode and non walkActivated modes
        else {
            if (Input.GetButtonDown("Walk"))
                {
                    walkActivated = !walkActivated;
                }

            //in walkActivated mode, you are always at walk speed; in non walk activated mode, you are at normal speed unless crouching
            if (walkActivated)
            {
                desiredSpeed = walkSpeed;
                walking = true;
                running = false;
            }
            else
            {
                walking = false;
                running = true;
                if (crouching)
                {
                    desiredSpeed = walkSpeed;
                }
                else
                {
                    desiredSpeed = runSpeed;
                }
            }


        }

        

        
    }
    

    
    // Store point that we're in contact with for use in FixedUpdate if needed
    void OnControllerColliderHit (ControllerColliderHit hit) {
		contactPoint = hit.point;
	}
	
	// If falling damage occured, this is the place to do something about it. You can make the player
	// have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
	void FallingDamageAlert (float fallDistance) {
		print ("Ouch! Fell " + fallDistance + " units!");   
	}



    /// Do not abbreviate any variables from now on; you want to be able to read this code in the future
    /// and figure out exactly what it means. Therefore, you should not abbreviate variables.
    

    ///Check out the crouching mechanic and clean it up a bit. Most other things look ok right now.





}