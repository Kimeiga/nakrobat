using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {
	
	



	public int grabState = 0;
	public bool canSwitch = true;
	public GameObject[] hands;
	public Transform defaultHandTransform;
	public Transform handsTransform;

	public GameObject[] weapons;

	public Transform[] weaponDefaultTransforms;

	public Transform camTransform;
	public float grabRange = 8;
	private RaycastHit hit;
	public LayerMask grabLayerMask;

	public bool firing = false;

	public float throwForce = 10;
	
	public int selectedSlot = 0;
	
	public GameObject reachObject;
	public float reachSpeed = 2;

	public Gun gunScriptReach;


	//weapon sway
	private float moveOnX;
	private float moveOnY;
	public float moveAmount = 1;
	public float moveSpeed = 2;
	private Vector3 newPosition;
	private Vector3 defaultPosition;
	private Vector3 newPositionProxy;
	public Vector3 shotOffset = Vector3.zero;

	private Vector3 swayOffset;
	

	// Use this for initialization
	void Start () {

		swayOffset = Vector3.zero;
		

		weaponDefaultTransforms = new Transform[3];

		for(int i = 0; i < weaponDefaultTransforms.Length; i++)
		{
			weaponDefaultTransforms[i] = defaultHandTransform;
		}
		




		weapons = new GameObject[3];

		selectedSlot = 0;
		hands[0].SetActive(true);

		defaultPosition = hands[0].transform.localPosition;

		grabState = 0;

		moveOnX = 0;
		moveOnY = 0;
		
		
	}


	// Update is called once per frame
	void Update () {
		
		

		//Switch Weapons
		if (Input.GetAxis("Mouse ScrollWheel") > 0.02f || Input.GetButtonDown("Slot 0"))
		{
			if (selectedSlot != 0)
			{
				SwitchItems(0);
			}

		}
		if (Input.GetButtonDown("Slot 1"))
		{
			if (selectedSlot != 1)
			{
				SwitchItems(1);
			}
		}
		if (Input.GetAxis("Mouse ScrollWheel") < -0.02f || Input.GetButtonDown("Slot 2"))
		{
			if (selectedSlot != 2)
			{
				SwitchItems(2);
			}
		}
		

		if (grabState == 1)
		{
			hands[selectedSlot].transform.parent = null;
			
			iTween.MoveUpdate(hands[selectedSlot], iTween.Hash("position", gunScriptReach.holdTransform, "time", 0.5f, "islocal", false));
			iTween.RotateUpdate(hands[selectedSlot], iTween.Hash("rotation", gunScriptReach.holdTransform, "time", 0.5f, "islocal", false));

			if (Vector3.Distance(hands[selectedSlot].transform.position, gunScriptReach.holdTransform.position) < 0.00001f)
			{
				grabState = 2; 
			}
			
		}


		if (grabState == 2)
		{


			weapons[selectedSlot] = reachObject;

			
			Rigidbody rigid = weapons[selectedSlot].GetComponent<Rigidbody>();
			rigid.isKinematic = true;
			rigid.useGravity = false;
			


			weapons[selectedSlot].transform.parent = hands[selectedSlot].transform;

			Gun gunScript = weapons[selectedSlot].GetComponent<Gun>();

			gunScript.active = true;
						
			Collider col = weapons[selectedSlot].GetComponent<Collider>();
			col.isTrigger = true;
			

			weapons[selectedSlot].layer = LayerMask.NameToLayer("Gun");
			foreach (Transform child in weapons[selectedSlot].transform)
			{
				child.gameObject.layer = LayerMask.NameToLayer("Gun");
			}
			

			hands[selectedSlot].transform.parent = handsTransform;

			Transform selectedTransform;

			/*
			if(gunScript.nameOfGun == "AKM")
			{
				selectedTransform = akmHandTransform;
				weaponDefaultTransforms[selectedSlot] = akmHandTransform;
				
			}
			else
			{
				selectedTransform = defaultHandTransform;
				weaponDefaultTransforms[selectedSlot] = defaultHandTransform;
				
			}
			*/

			//copied
			selectedTransform = defaultHandTransform;
			weaponDefaultTransforms[selectedSlot] = defaultHandTransform;


			iTween.MoveUpdate(hands[selectedSlot], iTween.Hash("position", selectedTransform.position, "time", 0.5f, "islocal", false));
			iTween.RotateUpdate(hands[selectedSlot], iTween.Hash("rotation", selectedTransform, "time", 0.2f, "islocal", false));


			if (Vector3.Distance(hands[selectedSlot].transform.position, selectedTransform.position) < 0.1f)
			{

				grabState = 0;
				canSwitch = true;
			}
			

		}


		if (Input.GetButtonDown("Drop")) {
			if(weapons[selectedSlot] != null)
			{
				DropItem(weapons[selectedSlot], selectedSlot);
			}
		}

		if(grabState == 0)
		{

			ItemSway(hands[selectedSlot].transform);
		}


		firing = false;

	}
	
	void ItemSway(Transform item)
	{

		//sway part

		moveOnX = Input.GetAxis("Mouse X") * Time.deltaTime * moveAmount;
		moveOnY = Input.GetAxis("Mouse Y") * Time.deltaTime * moveAmount;

		Vector3 swayOffsetTarget = new Vector3(moveOnX, moveOnY, 0);
		swayOffset = Vector3.Lerp(swayOffset, swayOffsetTarget, Time.deltaTime * moveSpeed);







		if (firing == true)
		{
			shotOffset.z = -0.03f;

			if(shotOffset.z <= -0.06f)
			{
				shotOffset.z = -0.06f;
			}
		}


		shotOffset = Vector3.Lerp(shotOffset, Vector3.zero, Time.deltaTime * 5f);

		item.transform.localPosition = weaponDefaultTransforms[selectedSlot].localPosition + swayOffset + shotOffset;




		//newPosition = new Vector3(weaponDefaultTransforms[selectedSlot].localPosition.x + moveOnX, weaponDefaultTransforms[selectedSlot].localPosition.y + moveOnY, weaponDefaultTransforms[selectedSlot].localPosition.z + shotOffset.z);
		//newPositionProxy = Vector3.Lerp(item.transform.localPosition, newPosition, Time.deltaTime * moveSpeed);
		


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
		weaponDefaultTransforms[selectedSlot] = defaultHandTransform;

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
	
}
