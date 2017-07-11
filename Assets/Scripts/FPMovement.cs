using UnityEngine;
using System.Collections;

public class FPMovement : MonoBehaviour {

    public CharacterController characterController;

    public bool canMove = true;

    //speeds
    public float runSpeed = 7;
    public float walkSpeed = 4;
    public float crouchSpeed = 3;
    private float speed;
    public bool playerControl = true;


    // If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
    public bool slideWhenOverSlopeLimit = false;
    // If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
    public bool slideOnTaggedObjects = false;
    public float slideSpeed = 6f;
    private float slideLimit;
    public bool sliding = false;
    public bool sliding2 = false;
    public int slideCounter = 0;


    public enum moveStates
    {
        running = 0,
        walking = 1,
        crouching = 2
    }

    private moveStates moveState;

    public bool grounded = false;
    private bool crouchingGrounded;
    private CapsuleCollider sidesCollider;

    private float inputX;
    private float inputY;
    private Vector3 xzDirection;

    public Vector3 moveDirection;

    public float jumpSpeed = 8;
    public bool falling = false;

    public bool wasFalling = false;
    public float slideDirectionMod = 1;

    public bool jumping = false;
    public bool hitCeiling = false;
    public float gravity = 20;
    public float antiBumpFactor = .75f;
    public bool fellOffLedge = false;
    public bool fellOffLedgePrimer = false;

    private float lastHeight;
    private float baseHeight;
    private float targetHeight;
    public float crouchHeight = 1.3f;
    public float springHeight = 1.8f;
    public float crouchSpringHeight = 1;
    public float crouchTime = 0.5f;
    private float tempCrouchTime;
    private float tempHeightStart;
    public float springTimeModifier = 0.25f;
    public float landBumpModifier = 0.5f;
    public float springOffset = 0.4f;

    public Transform meshTransform;
    private Vector3 meshTransformOriginalScale;

    public Transform lookTransform;

    public int crouchDownUp = 0;

    public enum crouchStates
    {
        idle = 0,

        standToCrouch = 1,
        crouchToStand = 2,

        standToCrouchMidAir = 3,
        crouchToStandMidAir = 4,

        standToJumpSpring = 5,

        standToJumpRecoil = 6,
        jumpRecoilToStand = 7,

        crouchToJumpSpring = 8,

        crouchToJumpRecoil = 9,
        jumpRecoilToCrouch = 10
    }

    private RaycastHit hit;
    private Vector3 contactPoint;
    private float rayDistance;
    public float rayDistance2;

    private float currentLerpTime;
    private float perc;

    private bool crouchDownButton;
    private bool crouchUpButton;
    private bool jumpDownButton;
    private bool jumpUpButton;
    private bool walkButton;
    private bool crouchButton;

    public bool wallHang = false;
    private float stepOffsetInitial;
    public bool onSlope;

    public bool dontBounce = false;
    public bool dontBounceAssist = false;


	// Use this for initialization
	void Start () {

        stepOffsetInitial = characterController.stepOffset;

        slideLimit = characterController.slopeLimit - .1f;

        sidesCollider = GetComponent<CapsuleCollider>();

        baseHeight = characterController.height;
        targetHeight = baseHeight;

        springHeight = baseHeight - springOffset;
        crouchSpringHeight = crouchHeight - springOffset;

        moveState = moveStates.running;
        characterController = GetComponent<CharacterController>();
        

        xzDirection = Vector3.zero;

        
        
        currentLerpTime = 0;

        meshTransformOriginalScale = meshTransform.localScale;
	}
    
    void Update()
    {
        rayDistance = characterController.height * 0.5f + characterController.radius;

        if (Input.GetButtonDown("Crouch"))
        {
            crouchDownButton = true;
        }

        if (Input.GetButtonUp("Crouch"))
        {

            crouchUpButton = Input.GetButtonUp("Crouch");
        }
        if (Input.GetButtonDown("Jump"))
        {

            jumpDownButton = Input.GetButtonDown("Jump");
        }
        if (Input.GetButtonUp("Jump"))
        {

            jumpUpButton = Input.GetButtonUp("Jump");
        }
        walkButton = Input.GetButton("Walk");
        crouchButton = Input.GetButton("Crouch");

        
    }

    void FixedUpdate() {
        
        
        
        

        if (crouchButton)
        {
            moveState = moveStates.crouching;
        }
        else if (walkButton)
        {
            moveState = moveStates.walking;
        }
        else
        {
            moveState = moveStates.running;
        }
        
        if (moveState == moveStates.running)
        {
            speed = runSpeed;
        }
        if(moveState == moveStates.walking)
        {
            speed = walkSpeed;

        }
        if(moveState == moveStates.crouching && grounded)
        {
            speed = crouchSpeed;
        }

        if (grounded)
        {
            sliding = false;
            sliding2 = false;
            // See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
            // because that interferes with step climbing amongst other annoyances
            if (Physics.Raycast(transform.position, -Vector3.up, out hit, rayDistance))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                {

                    //print((Vector3.Angle(hit.normal, Vector3.up)).ToString());

                    sliding = true;
                    sliding2 = true;
                    grounded = true;
                }

                if (Vector3.Angle(hit.normal, Vector3.up) > 0.1f)
                {
                    onSlope = true;
                }
                else
                {
                    onSlope = false;
                }

            }
            // However, just raycasting straight down from the center can fail when on steep slopes
            // So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
            else
            {
                Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
                
                


                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                {
                    sliding = true;
                    sliding2 = true;
                    grounded = true;
                }
            }
        }
        else
        {
            sliding2 = false;
        }

        if (sliding)
        {
            slideCounter++;

        }
        else
        {
            slideCounter = 0;
        }

        if (canMove)
        {

            JumpMechanism();
            CrouchMechanism();


            //scale mesh of character
            Vector3 temp = new Vector3(meshTransformOriginalScale.x, meshTransformOriginalScale.y * characterController.height / baseHeight, meshTransformOriginalScale.z);
            if (!float.IsNaN(meshTransformOriginalScale.y * characterController.height / baseHeight))
            {

                meshTransform.localScale = temp;
            }

            //move head with crouching
            Vector3 temp2 = lookTransform.position;
            temp2.y += (characterController.height - lastHeight) * 0.5f;
            lookTransform.position = temp2;


            MoveMechanism();

        }


        crouchDownButton = false;
        crouchUpButton = false;
        jumpDownButton = false;
        jumpUpButton = false;

        wallHang = false;
        
	}

    void MoveMechanism()
    {
        if (grounded)
        {
            fellOffLedgePrimer = true;
            
        }
        if (falling && !sliding2)
        {
            wasFalling = true;
        }
        else
        {
            wasFalling = false;
           
        }

        slideDirectionMod = Mathf.Lerp(slideDirectionMod, 1, 0.1f);
        
            inputX = Input.GetAxis("Horizontal");
            inputY = Input.GetAxis("Vertical");

            xzDirection = new Vector3(inputX, 0, inputY);

            if (xzDirection.magnitude > 1)
            {
                xzDirection = xzDirection.normalized;
            }

            xzDirection *= speed;

            moveDirection.x = xzDirection.x;
            moveDirection.z = xzDirection.z;



            if (!wallHang)
            {

                moveDirection.y -= gravity * Time.deltaTime;
            characterController.stepOffset = stepOffsetInitial;
            }
            else
            {
                moveDirection.y = 0;
            characterController.stepOffset = 0.01f;
            }

            if (((characterController.collisionFlags & CollisionFlags.Above) != 0) && hitCeiling == false)
            {
                moveDirection.y = -0.4f;

                hitCeiling = true;
            }

        //playerControl = true;

        moveDirection = transform.TransformDirection(moveDirection);


        // If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
        if (grounded && (((slideCounter > 2) && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide")))
        {
            Vector3 slideMoveDirection = moveDirection;
            Vector3 hitNormal = hit.normal;
            slideMoveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
            Vector3.OrthoNormalize(ref hitNormal, ref slideMoveDirection);
            slideMoveDirection *= slideSpeed;
            //playerControl = false;

            //slideMoveDirection.y *= 4;

            moveDirection *= 0.2f;
            moveDirection += slideMoveDirection * 0.8f * slideDirectionMod ;

                if (jumpUpButton)
                {
                    moveDirection.y = jumpSpeed*0.7f;
                }

        }
        

        
        grounded = (characterController.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;

        if (grounded)
        {
            hitCeiling = false;
            moveDirection.y = -antiBumpFactor;
            if (wasFalling)
            {
                slideDirectionMod = 0.5f;
            }
        }
        else
        {
            falling = true;
            if (fellOffLedgePrimer && !jumping && !(slideCounter > 2))
            {
                fellOffLedgePrimer = false;
                fellOffLedge = true;
            }
        }

        if (fellOffLedge)
        {
            moveDirection.y = -1;
            fellOffLedge = false;
        }
        
    }

    void JumpMechanism()
    {
        if (grounded)
        {
            
            if (!crouchButton)
            {
                
                if (falling && !(slideCounter > 2) && !dontBounceAssist)
                {

                    

                    crouchDownUp = 7;
                    currentLerpTime = 0;
                    perc = 0;
                    tempCrouchTime = crouchTime * ((characterController.height - springHeight) / (baseHeight - springHeight));
                    tempHeightStart = characterController.height;
                    

                    falling = false;
                    jumping = false;


                    if (dontBounce)
                    {
                        dontBounceAssist = true;
                    }
                }

                //springcrouch

                if (jumpDownButton)
                {

                    crouchDownUp = 5;
                    currentLerpTime = 0;
                    perc = 0;
                    tempCrouchTime = crouchTime * ((characterController.height - springHeight) / (baseHeight - springHeight));
                    tempHeightStart = characterController.height;

                }


                if (jumpUpButton)
                {
                    moveDirection.y = jumpSpeed;
                    

                    crouchDownUp = 6;
                    currentLerpTime = 0;
                    perc = 0;
                    tempCrouchTime = crouchTime * ((baseHeight - characterController.height) / (baseHeight - springHeight));
                    tempHeightStart = characterController.height;


                    falling = true;
                    jumping = true;

                    dontBounceAssist = false;
                }
            }
            else
            {

                if (falling && !(slideCounter > 2) && !dontBounceAssist)
                {
                    crouchDownUp = 10;
                    currentLerpTime = 0;
                    perc = 0;
                    tempCrouchTime = crouchTime * ((characterController.height - crouchSpringHeight) / (crouchHeight - crouchSpringHeight));
                    tempHeightStart = characterController.height;

                    falling = false;
                    jumping = false;

                    //moveDirection.y = 0;


                    if (dontBounce)
                    {
                        dontBounceAssist = true;
                    }

                }

                if (jumpDownButton)
                {

                    crouchDownUp = 8;
                    currentLerpTime = 0;
                    perc = 0;
                    tempCrouchTime = crouchTime * ((characterController.height - crouchSpringHeight) / (crouchHeight - crouchSpringHeight));
                    tempHeightStart = characterController.height;
                }


                if (jumpUpButton)
                {
                    moveDirection.y = jumpSpeed;

                    falling = true;
                    jumping = true;

                    crouchDownUp = 9;
                    currentLerpTime = 0;
                    perc = 0;
                    tempCrouchTime = crouchTime * ((crouchHeight - characterController.height) / (crouchHeight - crouchSpringHeight));
                    tempHeightStart = characterController.height;

                    dontBounceAssist = false;
                }
            }

        }
    }
    
    void CrouchMechanism()
    {

        if(!grounded && !onSlope)
        {

            dontBounceAssist = false;
        }

        lastHeight = characterController.height;
        
        //jump release
        if (grounded)
        {

            if (crouchDownButton)
            {
                crouchDownUp = 1;
                currentLerpTime = 0;
                perc = 0;
                tempCrouchTime = crouchTime * ((characterController.height - crouchHeight) / (baseHeight - crouchHeight));
                tempHeightStart = characterController.height;
            }

            if (crouchUpButton)
            {
                crouchDownUp = 2;
                currentLerpTime = 0;
                tempCrouchTime = crouchTime * ((baseHeight - characterController.height) / (baseHeight - crouchHeight));
                perc = 0;
                tempHeightStart = characterController.height;
            }

            if (crouchDownUp == 3)
            {
                crouchDownUp = 1;
                currentLerpTime = 0;
                perc = 0;
                tempCrouchTime = crouchTime * ((characterController.height - crouchHeight) / (baseHeight - crouchHeight));
                tempHeightStart = characterController.height;
            }

            if (crouchDownUp == 4)
            {
                crouchDownUp = 2;
                currentLerpTime = 0;
                tempCrouchTime = crouchTime * ((baseHeight - characterController.height) / (baseHeight - crouchHeight));
                perc = 0;
                tempHeightStart = characterController.height;
            }

        }
        else
        {
            if (crouchDownButton)
            {
                crouchDownUp = 3;
                currentLerpTime = 0;
                perc = 0;
                tempCrouchTime = crouchTime * ((characterController.height - crouchHeight) / (baseHeight - crouchHeight));
                tempHeightStart = characterController.height;
            }
            if (crouchUpButton)
            {
                crouchDownUp = 4;
                currentLerpTime = 0;
                tempCrouchTime = crouchTime * ((baseHeight - characterController.height) / (baseHeight - crouchHeight));
                perc = 0;
                tempHeightStart = characterController.height;
            }
        }

        if (crouchDownUp == 1)
        {
            Crouch(tempHeightStart, crouchHeight, tempCrouchTime, true);
        }
        if (crouchDownUp == 2)
        {
            Crouch(tempHeightStart, baseHeight, tempCrouchTime, true);
        }
        if (crouchDownUp == 3)
        {
            Crouch(tempHeightStart, crouchHeight, tempCrouchTime, false);
        }
        if (crouchDownUp == 4)
        {
            Crouch(tempHeightStart, baseHeight, tempCrouchTime, false);
        }
        if (crouchDownUp == 5)
        {
            Crouch(tempHeightStart, springHeight, tempCrouchTime * springTimeModifier, true);
        }
        if (crouchDownUp == 6)
        {
            Crouch(tempHeightStart, baseHeight, tempCrouchTime * springTimeModifier, true);
        }
        if (crouchDownUp == 7)
        {
            float tempSpringTime = tempCrouchTime * springTimeModifier * landBumpModifier;
            
            if (dontBounce)
            {
                tempSpringTime *= 2;
            }
            

            Crouch(tempHeightStart, springHeight, tempSpringTime, true);

            if (crouchDownUp == 0)
            {

                moveDirection.y = 0;

                crouchDownUp = 6;
                currentLerpTime = 0;
                perc = 0;
                tempCrouchTime = crouchTime * ((baseHeight - characterController.height) / (baseHeight - springHeight));
                tempHeightStart = characterController.height;
            }

            if (dontBounce)
            {
                dontBounceAssist = true;
            }
            
        }
        if (crouchDownUp == 8)
        {
            Crouch(tempHeightStart, crouchSpringHeight, tempCrouchTime * springTimeModifier, true);
        }
        if (crouchDownUp == 9)
        {
            Crouch(tempHeightStart, crouchHeight, tempCrouchTime * springTimeModifier, true);
        }
        if (crouchDownUp == 10)
        {

            float tempCrouchSpringTime = tempCrouchTime * springTimeModifier * landBumpModifier;

            if (dontBounce)
            {
                tempCrouchSpringTime *= 2;
            }

            Crouch(tempHeightStart, crouchSpringHeight, tempCrouchTime * springTimeModifier * landBumpModifier, true);

            if (crouchDownUp == 0)
            {

                moveDirection.y = 0;

                crouchDownUp = 9;
                currentLerpTime = 0;
                perc = 0;
                tempCrouchTime = crouchTime * ((crouchHeight - characterController.height) / (crouchHeight - crouchSpringHeight));
                tempHeightStart = characterController.height;
            }
            

        }
        

    }

    void Crouch(float start,float end,float lerpTime, bool onGround)
    {

        

        if(perc < 1)
        {

            lerpTime = Mathf.Abs(lerpTime);

            float previousHeight = characterController.height;

            currentLerpTime += Time.deltaTime;

            //lerp!
            perc = currentLerpTime / lerpTime;

            perc = Mathf.Sin(perc * Mathf.PI * 0.5f);

            characterController.height = Mathf.Lerp(start, end, perc);
            

            Vector3 temp = transform.position;
            if (onGround)
            {

                temp.y += (characterController.height - previousHeight) * 0.5f;
            }
            else
            {

                temp.y -= (characterController.height - previousHeight) * 0.5f;


            }
            if (!float.IsNaN(temp.y))
            {
                transform.position = temp;
            }

            perc = currentLerpTime / lerpTime;
        }
        else
        {
            crouchDownUp = 0;
        }

    
    } 
    
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        contactPoint = hit.point;
        if (onSlope && xzDirection.magnitude > 0.05f)
        {
            dontBounce = true;

        }
        else
        {
            dontBounce = false;

        }
    }

    void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "Level" && !grounded)
        {

            if (Input.GetButton("Wall Hang"))
            {
                wallHang = true;

            }
        }
    }


}
