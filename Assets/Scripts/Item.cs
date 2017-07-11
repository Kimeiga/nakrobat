using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{

    [Header("Hold Variables")]

    public bool adjustingLocalHoldPosRot = false;

    public Vector3 localHoldPosition;
    public Vector3 localHoldRotation;
    

    public Transform leftHandHoldTransform;
    public Transform rightHandHoldTransform;

    public Collider col;

    [Header("Function Variables")]

    public bool active = false;


    void Start()
    {
        if (adjustingLocalHoldPosRot)
        {
            localHoldPosition = transform.localPosition;
            localHoldRotation = transform.localEulerAngles;
        }


        //if we don't specifically set a collider, just use the one on root
        if(transform.GetComponent<Collider>() != null && col == null)
        {
            col = transform.GetComponent<Collider>();
        }

        if(col == null)
        {
            print(transform.name + " doesn't have a collider stored");
        }
        
    }

    void Update()
    {

        if (adjustingLocalHoldPosRot)
        {
            transform.localPosition = localHoldPosition;
            transform.localRotation = Quaternion.Euler(localHoldRotation);
        }

    }

}