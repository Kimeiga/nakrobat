using UnityEngine;
using System.Collections;

public class CrouchTest : MonoBehaviour {

    public CharacterController controller;
    private float standingHeight = 2;
    private float crouchHeight = 1.4f;
    private float crouchSpeed = 7f;

    private Vector3 moveDirection;
    private float gravity = 20;
    private bool grounded = true;
    private float jumpSpeed = 8;

	// Use this for initialization
	void Start () {

        controller = GetComponent<CharacterController>();

       

    }
	
	// Update is called once per frame
	void Update () {


        moveDirection.y -= gravity * Time.deltaTime;
        moveDirection = transform.TransformDirection(moveDirection);

        grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;

        if (Input.GetButtonUp("Jump"))
        {
            moveDirection.y = jumpSpeed;
        }

        //continual variables (FloatUpdate)


        if (Input.GetButton("Crouch"))
        {
            controller.height = iTween.FloatUpdate(controller.height, crouchHeight, crouchSpeed);
        }
        else
        {
            controller.height = iTween.FloatUpdate(controller.height, standingHeight, crouchSpeed);
            
        }
        

        /*
        if (Input.GetButtonDown("Crouch"))
        {
            float startHeight = controller.height;


            iTween.ValueTo(gameObject, iTween.Hash(
                "from", startHeight,
                "to", crouchHeight,
                "time", 1f,
                "onupdatetarget", gameObject,
                "onupdate", "tweenOnUpdateCallBack",
                "easetype", iTween.EaseType.easeOutExpo)
                );
        }

        if (Input.GetButtonUp("Crouch"))
        {
            float startHeight = controller.height;


            iTween.ValueTo(gameObject, iTween.Hash(
                "from", startHeight,
                "to", standingHeight,
                "time", 1f,
                "onupdatetarget", gameObject,
                "onupdate", "tweenOnUpdateCallBack",
                "easetype", iTween.EaseType.easeOutExpo)
                );
        }

        */

        print(controller.height);

	}

    void tweenOnUpdateCallBack(float newValue)
    {
        controller.height = newValue;
    }
}
