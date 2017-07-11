using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerStatus : MonoBehaviour {

    public float health;

    public bool grabbing = false;
    public bool grabbed = false;
    public bool canSwitch = true;
    public GameObject[] hands;
    public Transform defaultHandTransform;
    public Transform handsTransform;
    public GameObject[] weapons;
    public Transform camTransform;
    public float grabRange = 3;
    private RaycastHit hit;
    public LayerMask grabLayerMask;

    public GameObject activeWeapon;
    public Gun activeGunScript;

    public float throwForce = 10;
    
    private int selectedSlot;

    private bool reaching = false;
    public GameObject reachObject;
    private bool pulling = false;
    public float reachSpeed = 2;

    private Gun gunScriptReach;


    //weapon sway
    private float moveOnX;
    private float moveOnY;
    public float moveAmount = 1;
    public float moveSpeed = 2;
    private Vector3 newPosition;
    private Vector3 defaultPosition;
    private Vector3 newPositionProxy;
    public Vector3 shotOffset = Vector3.zero;

    public FPMovement fpMovementScript;

	// Use this for initialization
	void Start () {

        health = 100;

        
        weapons = new GameObject[3];

        selectedSlot = 0;
        hands[0].SetActive(true);

        defaultPosition = hands[0].transform.localPosition;

        grabbing = false;
        grabbed = false;
	}
	
    void FixedUpdate()
    {

    }

	// Update is called once per frame
	void Update () {


        if (weapons[selectedSlot] != null)
        {
            activeWeapon = weapons[selectedSlot];
            activeGunScript = activeWeapon.GetComponent<Gun>();
        }
        else
        {
            activeGunScript = null;
            activeWeapon = null;
        }


        if (!grabbing)
        {

            ItemSway(hands[selectedSlot].transform);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0.02f || Input.GetButtonDown("Slot 0"))
        {
            if(selectedSlot != 0)
            {
                
                SwitchItems(0);
            }

        }
        if(Input.GetButtonDown("Slot 1"))
        {
            if(selectedSlot != 1)
            {
                SwitchItems(1);
            }
        }
        if(Input.GetAxis("Mouse ScrollWheel") < -0.02f || Input.GetButtonDown("Slot 2"))
        {
            if(selectedSlot != 2)
            {
                SwitchItems(2);
            }

        }

        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, grabRange, grabLayerMask))
        {

            if (Input.GetButton("Grab") && weapons[selectedSlot] == null)
            {
                
                if (hit.transform.tag == "Item" && !grabbing)
                {
                    reaching = true;
                    reachObject = hit.transform.gameObject;
                    canSwitch = false;
                    gunScriptReach = hit.transform.GetComponent<Gun>();
                    //hands[selectedSlot].transform.parent = null;
                    //ReachForItem(reachObject);

                    //hands[selectedSlot].SendMessage("ReachItem",reachObject);

                    grabbed = false;
                    grabbing = true;

                    

                    //EquipItem(hit.transform.gameObject,selectedSlot);


                }
            }
        }

        
        if (grabbing)
        {
            hands[selectedSlot].transform.parent = null;
            if (!grabbed)
            {
                ReachForItem(reachObject);
                if (Vector3.Distance(hands[selectedSlot].transform.position, reachObject.transform.position) < 0.001f)
                {
                    grabbed = true;
                   
                }
            }
            

            if (grabbed)
            {
                weapons[selectedSlot] = reachObject;
                weapons[selectedSlot].transform.parent = hands[selectedSlot].transform;
                Gun gunScript = weapons[selectedSlot].GetComponent<Gun>();

                gunScript.active = true;
                Rigidbody rigid = weapons[selectedSlot].GetComponent<Rigidbody>();
                rigid.isKinematic = true;
                rigid.useGravity = false;
                Collider col = weapons[selectedSlot].GetComponent<Collider>();
                col.isTrigger = true;

                //weapons[selectedSlot].transform.localPosition = gunScript.idlePosition;
                weapons[selectedSlot].transform.localRotation = Quaternion.identity;

                weapons[selectedSlot].layer = LayerMask.NameToLayer("Gun");
                foreach (Transform child in weapons[selectedSlot].transform)
                {
                    child.gameObject.layer = LayerMask.NameToLayer("Gun");
                }

                //hands[selectedSlot].SendMessage("PullItem", defaultHandTransform);

                hands[selectedSlot].transform.parent = handsTransform;

                iTween.MoveUpdate(hands[selectedSlot], iTween.Hash("position", defaultHandTransform.position, "time", 0.5f, "islocal", false));
                iTween.RotateUpdate(hands[selectedSlot], iTween.Hash("rotation", defaultHandTransform.rotation, "time", 0.5f, "islocal", true));
                if (Vector3.Distance(hands[selectedSlot].transform.position, defaultHandTransform.position) < 0.001f)
                {

                    grabbed = false;
                    grabbing = false;
                    canSwitch = true;
                }

            }
        }

        if (Input.GetButtonDown("Drop")) {
            if(weapons[selectedSlot] != null)
            {

                DropItem(weapons[selectedSlot], selectedSlot);
            }
        }
        
        if(!grabbed && !grabbing)
        {

            hands[selectedSlot].transform.parent = handsTransform;
        }

	}

    void ReachForItem(GameObject item)
    {
        //hands[selectedSlot].transform.position = Vector3.Lerp(hands[selectedSlot].transform.position, item.transform.position, Time.deltaTime * reachSpeed);
        //hands[selectedSlot].transform.rotation = Quaternion.Slerp(hands[selectedSlot].transform.rotation, gunScriptReach.holdTransform.rotation, Time.deltaTime * reachSpeed);
        iTween.MoveUpdate(hands[selectedSlot], iTween.Hash("position", item.transform.position, "time", 0.5f,"islocal",false));
        iTween.MoveUpdate(hands[selectedSlot], iTween.Hash("rotation", item.transform.rotation, "time", 0.5f, "islocal", true));

    }

    void ItemSway(Transform item)
    {
        
        moveOnX = Input.GetAxis("Mouse X") * Time.deltaTime * moveAmount;
        moveOnY = Input.GetAxis("Mouse Y") * Time.deltaTime * moveAmount;

        newPosition = new Vector3(defaultPosition.x + moveOnX, defaultPosition.y + moveOnY, defaultPosition.z);
        newPositionProxy = Vector3.Lerp(item.transform.localPosition, newPosition, Time.deltaTime * moveSpeed);
        if(activeGunScript != null)
        {

            if (activeGunScript.firing == true)
            {
                shotOffset.z = -0.03f;
            }
        }
        shotOffset = Vector3.Lerp(shotOffset, Vector3.zero, Time.deltaTime * 100f);

        item.transform.localPosition = newPositionProxy + shotOffset;
        
    }


    void SwitchItems(int newSlot)
    {
        if (canSwitch)
        {

            int previousSlot = selectedSlot;
            hands[previousSlot].SetActive(false);
            hands[newSlot].SetActive(true);
            selectedSlot = newSlot;
        }



    }
    

    void DropItem(GameObject item,int slot)
    {
        weapons[slot].transform.parent = null;
        Gun gunScript = weapons[slot].GetComponent<Gun>();
        gunScript.active = false;
        Rigidbody rigid = weapons[slot].GetComponent<Rigidbody>();
        rigid.isKinematic = false;
        rigid.useGravity = true;
        rigid.AddForce(camTransform.forward * throwForce,ForceMode.VelocityChange);
        Collider col = weapons[slot].GetComponent<Collider>();
        col.isTrigger = false;

        weapons[slot].layer = LayerMask.NameToLayer("Default"); ;
        foreach (Transform child in weapons[slot].transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Default");
        }

       

        weapons[slot] = null;

        
    }



    void EquipItem(GameObject item, int slot)
    {
        weapons[slot] = item;
        weapons[slot].transform.parent = hands[slot].transform;
        Gun gunScript = weapons[slot].GetComponent<Gun>();
        
        gunScript.active = true;
        Rigidbody rigid = weapons[slot].GetComponent<Rigidbody>();
        rigid.isKinematic = true;
        rigid.useGravity = false;
        Collider col = weapons[slot].GetComponent<Collider>();
        col.isTrigger = true;

        weapons[slot].transform.localPosition = gunScript.idlePosition;
        weapons[slot].transform.localRotation = Quaternion.identity;

        weapons[slot].layer = LayerMask.NameToLayer("Gun"); 
        foreach(Transform child in weapons[slot].transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Gun"); 
        }

    }

}
