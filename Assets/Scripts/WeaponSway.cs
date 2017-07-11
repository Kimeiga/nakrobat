using UnityEngine;
using System.Collections;

public class WeaponSway : MonoBehaviour {

    public bool aim;
    public float aimMod = 0.2f;

	public float moveAmount = 1;
    private float targetMoveAmount = 1;
	public float moveSpeed = 2;
    //set in inventory v
    //public Gun gun;
	private float moveOnX;
	private float moveOnY;

    //set in inventory v
    private Vector3 defaultPosition;
	private Vector3 newPosition;

    public FPSWalker FPSWalkerScript;
    public SpeedCalculator speedCalculatorScript;

    public float crouchOffset = -0.05f;
    public float jumpOffset = -0.05f;
    public float moveMultiplier = -0.2f;

    private float eks;

    //set in inventory v
    public Transform animationTransform;
    private Vector3 animationTransformOffset;
    private Vector3 animationTransformPositionTarget;

    private Vector3 XYMoveDirection;
    private float moveThreshold = 0.001f;

	// Use this for initialization
	void Start () {

        /*
        if (gun) {
            defaultPosition = gun.idlePosition;
        }
        */
        //animationTransform = gun.transform;

	}
	

	// Update is called once per frame
	void Update () {

        if (animationTransform)
        {

            XYMoveDirection = new Vector3(speedCalculatorScript.measured3DSpeed.x, 0, speedCalculatorScript.measured3DSpeed.z);



            animationTransformPositionTarget.z = moveMultiplier * XYMoveDirection.magnitude;

            if (FPSWalkerScript.crouching)
            {


                eks = crouchOffset;

            }
            else
            {


                eks = 0;

            }





            if (!FPSWalkerScript.grounded)
            {

                animationTransformPositionTarget.y = jumpOffset;

            }
            else
            {


                animationTransformPositionTarget.y = Mathf.Lerp(animationTransformPositionTarget.y, 0, FPSWalkerScript.crouchSpeed * Time.deltaTime);
            }


            //animationTransformPositionTarget.z = Mathf.Lerp(animationTransformPositionTarget.x, eks, FPSWalkerScript.crouchSpeed * Time.deltaTime);


            if (aim)
            {
                //animationTransform.localPosition = Vector3.Lerp(animationTransform.localPosition, gun.animationTransformAimPosition, 0.1f);
                animationTransform.localPosition += (animationTransformPositionTarget * aimMod);
                targetMoveAmount = 0.1f;
            }
            else
            {
                animationTransform.localPosition = Vector3.Lerp(animationTransform.localPosition, Vector3.zero, 0.1f);
                animationTransform.localPosition += animationTransformPositionTarget;
                targetMoveAmount = 2;
            }


            moveAmount = targetMoveAmount;

        }

        /*
        if (gun != null)
        {

            moveOnX = Input.GetAxis("Mouse X") * Time.deltaTime * moveAmount;
            moveOnY = Input.GetAxis("Mouse Y") * Time.deltaTime * moveAmount;

            newPosition = new Vector3(defaultPosition.x + moveOnX, defaultPosition.y + moveOnY, defaultPosition.z);
            //gun.transform.localPosition = Vector3.Lerp(gun.transform.localPosition, newPosition, Time.deltaTime * moveSpeed);


        }
        */

	}
}
