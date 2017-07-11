using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory3 : MonoBehaviour
{


    /// <summary>
    /// This is going to finish Inventory2 but do it with LeanTween so it's not so ugly.
    /// </summary>


    
    //ALSO MOVE OTHER HAND TO CORRESPONDING HAND HOLD IN PHASE 2 FOR FIRST TWO ROUTES TY

        
    
    //Poder Variables
    private bool canSwitch = true;
    private bool canGrab = true;


    //Initial Grab Raycast Variables
    public Transform playerCamera;
    public float grabRange = 3;
    public LayerMask grabLayerMask;
    private RaycastHit hit;


    //Grabbing Behaviour Variables
    private GameObject grabItem;
    private Item grabItemItemScript;
    
    private bool useTheRightHand;

    public float grabTime = 0.3f;

    
    //Hands Variables
    public Transform leftHandTransform;
    public Transform rightHandTransform;
    public Transform handTransform;


    //Inventory Variables
    private GameObject[] inventory;
    public int maxInventorySize = 10;

    private int inventoryIndex;
    public int startingInventoryIndex = 0;

    public Transform[] holsters;



    //Throw Variables
    public float throwForce = 10;





    void Start()
    {
        inventory = new GameObject[maxInventorySize];
        inventoryIndex = startingInventoryIndex;
        
        
    }


    void Update()
    {
        GrabbingBehavior();
    }

    void GrabbingBehavior()
    {

        //This Raycast just starts the grab

        //you can't grab (or attempt to grab) another item while you are grabbing
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, grabRange, grabLayerMask) && canGrab 
            && hit.transform.gameObject.GetComponent<Item>() != null)
        {
            //you could make it glow or something in here to show the player that he/she can grab it right HERE

            
            if (Input.GetButtonDown("Grab"))
            {

                GrabItem(hit.transform.gameObject);

            }
            
        }
        
    }


    

    

    void GrabItem( GameObject grabItem )
    {
        this.grabItem = grabItem;
        grabItemItemScript = grabItem.GetComponent<Item>();

        //5 routes for the grab to make it more realistic

        //Route 1: Current Item = hands, holster doesn't contain Grab Item
        //Route 2: CI = hands, holster contains GI
        //Route 3: CI =/= hands, =/= GI; holster doesn't contain GI
        //Route 4: CI =/= hands, =/= GI; holster contains GI
        //Route 5: CI = GI

        //Here CI would be inventory[inventoryIndex]
        //GI would be grabItem
        // the equalities would be handled by tag! (this is because we are doing this by item type)
        // To tell if the current item is HANDS we would probably just check if the current inventory is null 

        //We can make all of them different if statements at first to isolate them a bit easier, but later we can combine them (combine part of 1,2 and 3,4)

        

        //Because these routes have multiple phases, we will have to have multiple functions for each phase
        //ALSO!~ Because this is called from an OnButtonDown, we don't need oneshot booleans because this entire section of code is only called once


        
        //ROUTE 1
        if(inventory[inventoryIndex] == null && CheckItemTypeHolstered(grabItem) == false)
        {
            int slotToSwitchTo = GameManager2.instance.itemSlotOrder.FindIndex(obj => obj == grabItem.transform.tag);

            float distanceForRightHand = Vector3.Distance(rightHandTransform.position, grabItemItemScript.rightHandHoldTransform.position);
            float distanceForLeftHand = Vector3.Distance(leftHandTransform.position, grabItemItemScript.leftHandHoldTransform.position);

            print(distanceForLeftHand + "," + distanceForRightHand);

            useTheRightHand = distanceForRightHand < distanceForLeftHand;

            useTheRightHand = true;

            if (useTheRightHand)
            {
                //nullify the parent so it can grab it without trouble as you move around
                rightHandTransform.parent = null;

                //move it over to its hand hold
                //iTween.MoveUpdate(rightHandTransform.gameObject, iTween.Hash("position", grabObjectItemScript.rightHandHoldTransform, "time", 0.5f, "islocal", false));
                //iTween.RotateUpdate(rightHandTransform.gameObject, iTween.Hash("rotation", grabObjectItemScript.rightHandHoldTransform, "time", 0.5f, "islocal", false));

                LeanTween.move(rightHandTransform.gameObject, grabItemItemScript.rightHandHoldTransform, grabTime).setEase(LeanTweenType.easeInOutQuad).setOnComplete(Route1Phase2);
                LeanTween.rotate(rightHandTransform.gameObject, grabItemItemScript.rightHandHoldTransform.eulerAngles, grabTime).setEase(LeanTweenType.easeInOutQuad);


               
            }
            else
            {
                // nullify the parent so it can grab it without trouble as you move around
                leftHandTransform.parent = null;


                LeanTween.move(leftHandTransform.gameObject, grabItemItemScript.leftHandHoldTransform, grabTime).setEase(LeanTweenType.easeInOutQuad).setOnComplete(Route1Phase2);
                LeanTween.rotate(leftHandTransform.gameObject, grabItemItemScript.leftHandHoldTransform.eulerAngles, grabTime).setEase(LeanTweenType.easeInOutQuad);


            }
        }

        //ROUTE 2
        if (inventory[inventoryIndex] == null && CheckItemTypeHolstered(grabItem) == true)
        {

        }

        //ROUTE 3
        if(inventory[inventoryIndex] != null && CheckItemTypeEquiped(grabItem) == false && CheckItemTypeHolstered(grabItem) == false){

        }

        //ROUTE 4
        if (inventory[inventoryIndex] != null && CheckItemTypeEquiped(grabItem) == false && CheckItemTypeHolstered(grabItem) == true){

        }

        //ROUTE 5
        if(CheckItemTypeEquiped(grabItem) == true)
        {

        }



    }

    bool CheckItemTypeHolstered(GameObject item)
    {
        bool b = false;
        for (int i = 0; i < inventory.Length; i++)
        {
            if(i != inventoryIndex)
            {
                if (inventory[i] != null && inventory[i].tag == item.tag)
                {
                    b = true;
                }
            }
        }
        return b;

    }

    bool CheckItemTypeEquiped(GameObject item)
    {
        bool b = false;
        
        if(inventory[inventoryIndex] != null && inventory[inventoryIndex].tag == item.tag)
        {
            b = true;
        }

        return b;

    }

    void Route1Phase2()
    {

        print(Vector3.Distance(rightHandTransform.position, grabItemItemScript.rightHandHoldTransform.position));
        //put it in the inventory
        inventory[inventoryIndex] = grabItem;

        //deactivate rigidbody physics
        Rigidbody rigid = inventory[inventoryIndex].GetComponent<Rigidbody>();
        rigid.isKinematic = true;
        rigid.useGravity = false;


        grabItemItemScript.active = true;

        
        Collider col = grabItemItemScript.col;
        col.isTrigger = true;



        inventory[inventoryIndex].layer = LayerMask.NameToLayer("Item");
        foreach (Transform child in inventory[inventoryIndex].transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Item");
        }



        //get ready to pull it back by childing it to the hand that grabbed it
        if (useTheRightHand)
        {
            inventory[inventoryIndex].transform.parent = rightHandTransform;

            //Now we child the hand to the superhand transform to pull it back in smoothly

            rightHandTransform.parent = handTransform;

            LeanTween.moveLocal(rightHandTransform.gameObject, grabItemItemScript.localHoldPosition - grabItem.transform.localPosition, grabTime);
            LeanTween.rotateLocal(rightHandTransform.gameObject, grabItemItemScript.localHoldRotation - grabItem.transform.localRotation.eulerAngles, grabTime);

            
            LeanTween.move(leftHandTransform.gameObject, grabItemItemScript.leftHandHoldTransform, grabTime * 2);

            
            LeanTween.rotateLocal(leftHandTransform.gameObject, (Quaternion.Euler(grabItemItemScript.localHoldRotation) * Quaternion.Euler(grabItemItemScript.leftHandHoldTransform.localEulerAngles))
                .eulerAngles, grabTime * 2).setOnComplete(Route1Phase3);
            
            //sigh

        }
        else
        {


            print("ok");

            inventory[inventoryIndex].transform.parent = leftHandTransform;

            //Now we child the hand to the superhand transform to pull it back in smoothly

            leftHandTransform.parent = handTransform;


            LeanTween.moveLocal(leftHandTransform.gameObject, grabItemItemScript.localHoldPosition - grabItem.transform.localPosition, grabTime);
            LeanTween.rotateLocal(leftHandTransform.gameObject, grabItemItemScript.localHoldRotation, grabTime);

            LeanTween.move(rightHandTransform.gameObject, grabItemItemScript.rightHandHoldTransform, grabTime);
            LeanTween.rotateLocal(rightHandTransform.gameObject, Quaternion.Inverse(grabItemItemScript.rightHandHoldTransform.localRotation).eulerAngles, grabTime);



        }

    }

    void Route1Phase3()
    {
        print(Quaternion.Angle(leftHandTransform.rotation, grabItemItemScript.leftHandHoldTransform.rotation) + " , " + (Quaternion.Angle(leftHandTransform.localRotation, grabItemItemScript.leftHandHoldTransform.localRotation)));
    }

}








