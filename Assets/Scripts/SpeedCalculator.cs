using UnityEngine;
using System.Collections;

public class SpeedCalculator : MonoBehaviour {
    

    //Crosshair and Accuracy Control
    public float measuredSpeed;

    public Vector3 measured3DSpeed;

    //speed test

    public Vector3 lastPosition = Vector3.zero;
    public float clamp;
    

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {


        measured3DSpeed = transform.position - lastPosition;

        //speed stuff
        measuredSpeed = (transform.position - lastPosition).magnitude;
        //measuredSpeed -= afpswScript.measuredHeadSpeed;
        lastPosition = transform.position;

        measuredSpeed = Mathf.Clamp(measuredSpeed, 0, 0.16f);

        //localCrouchingSpeed = transform.InverseTransformVector()



    }

    void Update() {
    }
    
}
