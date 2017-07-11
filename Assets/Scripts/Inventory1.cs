using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory1 : MonoBehaviour
{


    //UGH
    public bool usingEmptySlot = true;


    public GameObject[] startingItems;
    private GameObject[] inventory;
    public int maxInventoryCount = 10;
    private int currentIndex;

    public int startingItemIndex;

    private Transform[] leftHandHolds;
    private Transform[] rightHandHolds;
    private Vector3[] localHoldPositions;
    private Quaternion[] localHoldRotations;
         

    public Transform rightHandTransform;
    public Transform leftHandTransform;
    public Transform handTransform;

    private Vector3 rightHandOriginalPos;
    private Quaternion rightHandOriginalRot;

    private Vector3 leftHandOriginalPos;
    private Quaternion leftHandOriginalRot;



    public enum HoldingState
    {
        idle, holstering, reachForNextItem, unholstering
    }

    public HoldingState currentHoldingState = HoldingState.idle;



    public Transform[] holsters;

    private int nextIndex;


    void Start()
    {

        //configure all arrays
        inventory = new GameObject[maxInventoryCount];
        leftHandHolds = new Transform[maxInventoryCount];
        rightHandHolds = new Transform[maxInventoryCount];
        localHoldPositions = new Vector3[maxInventoryCount];
        localHoldRotations = new Quaternion[maxInventoryCount];
        


        for (int i = 0; i < startingItems.Length; i++)
        {
            if (usingEmptySlot)
            {
                //this will change so that instead, each item will be placed into its corresponding spot
                //ie semi gun goes into semi gun slot etc
                inventory[i + 1] = startingItems[i];

            }
        }


        rightHandOriginalPos = rightHandTransform.localPosition;
        rightHandOriginalRot = rightHandTransform.localRotation;

        leftHandOriginalPos = leftHandTransform.localPosition;
        leftHandOriginalRot = leftHandTransform.localRotation;


        SetUpInventory();


    }


    void Update()
    {
        

        //Switch Weapons
        if (Input.GetButtonDown("Slot 0"))
        {
            if (currentIndex != 0)
            {
                currentHoldingState = HoldingState.holstering;
                nextIndex = 0;
                //SwitchItems(0);
            }

        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0.02f || Input.GetButtonDown("Slot 1"))
        {
            
            if (currentIndex != 1 && inventory[1] != null)
            {
                currentHoldingState = HoldingState.holstering;
                nextIndex = 1;

                print("Y");

                //SwitchItems(1);
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < -0.02f || Input.GetButtonDown("Slot 2"))
        {
            if (currentIndex != 2 && inventory[2] != null)
            {
                currentHoldingState = HoldingState.holstering;
                nextIndex = 2;
                //SwitchItems(2);
            }
        }

        



        if (currentHoldingState == HoldingState.holstering)
        {
            if(currentIndex == 0 && usingEmptySlot)
            {
                //don't need to holster if you are holding nothing
                currentHoldingState = HoldingState.reachForNextItem;
            }
            else
            {

                inventory[currentIndex].transform.parent = transform;

                
                iTween.MoveUpdate(inventory[currentIndex], iTween.Hash("position", holsters[currentIndex].transform, "time", 0.5f, "islocal", false));
                iTween.RotateUpdate(inventory[currentIndex], iTween.Hash("rotation", holsters[currentIndex].transform, "time", 0.5f, "islocal", false));
                
                /*
                inventory[currentIndex].transform.position = holsters[currentIndex].transform.position;
                inventory[currentIndex].transform.rotation = holsters[currentIndex].transform.rotation;
                */



                if (Vector3.Distance(inventory[currentIndex].transform.position, holsters[currentIndex].transform.position) < 0.001f)
                {
                    currentHoldingState = HoldingState.reachForNextItem;
                }
            }


        }

        if (currentHoldingState == HoldingState.reachForNextItem)
        {

            leftHandTransform.parent = handTransform;
            rightHandTransform.parent = handTransform;

            if (nextIndex == 0 && usingEmptySlot)
            {


                
                iTween.MoveUpdate(leftHandTransform.gameObject, iTween.Hash("position", leftHandOriginalPos, "time", 0.5f, "islocal", true));
                iTween.RotateUpdate(leftHandTransform.gameObject, iTween.Hash("rotation", leftHandOriginalRot, "time", 0.5f, "islocal", true));


                iTween.MoveUpdate(rightHandTransform.gameObject, iTween.Hash("position", rightHandOriginalPos, "time", 0.5f, "islocal", true));
                iTween.RotateUpdate(rightHandTransform.gameObject, iTween.Hash("rotation", rightHandOriginalRot, "time", 0.5f, "islocal", true));
                
                /*
                leftHandTransform.position = leftHandOriginalPos;
                leftHandTransform.rotation = leftHandOriginalRot;

                rightHandTransform.position = rightHandOriginalPos;
                rightHandTransform.rotation = rightHandOriginalRot;
                */

                if (Vector3.Distance(rightHandTransform.localPosition, rightHandOriginalPos) < 0.001f)
                {
                    //skip directly to idle because if you have nothing in your hands
                    //you don't need to reach for a gun
                    currentHoldingState = HoldingState.idle;
                    currentIndex = nextIndex;
                    nextIndex = -1;
                }
            }


            else
            {

                
                iTween.MoveUpdate(leftHandTransform.gameObject, iTween.Hash("position", leftHandHolds[nextIndex], "time", 0.5f, "islocal", false));
                iTween.RotateUpdate(leftHandTransform.gameObject, iTween.Hash("rotation", leftHandHolds[nextIndex], "time", 0.5f, "islocal", false));


                iTween.MoveUpdate(rightHandTransform.gameObject, iTween.Hash("position", rightHandHolds[nextIndex], "time", 0.5f, "islocal", false));
                iTween.RotateUpdate(rightHandTransform.gameObject, iTween.Hash("rotation", rightHandHolds[nextIndex], "time", 0.5f, "islocal", false));
                
                

                /*
                leftHandTransform.position = leftHandHolds[nextIndex].position;
                leftHandTransform.rotation = leftHandHolds[nextIndex].rotation;

                rightHandTransform.position = rightHandHolds[nextIndex].position;
                rightHandTransform.rotation = rightHandHolds[nextIndex].rotation;
                */


                if (Vector3.Distance(rightHandTransform.position, rightHandHolds[nextIndex].position) < 0.001f)
                {
                    currentHoldingState = HoldingState.unholstering;
                }
            }
            

        }


        if(currentHoldingState == HoldingState.unholstering)
        {

            leftHandTransform.parent = inventory[nextIndex].transform;
            rightHandTransform.parent = inventory[nextIndex].transform;

            inventory[nextIndex].transform.parent = handTransform;


            iTween.MoveUpdate(inventory[nextIndex], iTween.Hash("position", localHoldPositions[nextIndex], "time", 0.5f, "islocal", true));
            iTween.RotateUpdate(inventory[nextIndex], iTween.Hash("rotation", localHoldRotations[nextIndex], "time", 0.5f, "islocal", true));


            /*
            inventory[nextIndex].transform.position = localHoldPositions[nextIndex];
            inventory[nextIndex].transform.rotation = localHoldRotations[nextIndex];
            */



            if (Vector3.Distance(inventory[nextIndex].transform.localPosition, localHoldPositions[nextIndex]) < 0.001f)
            {
                currentHoldingState = HoldingState.idle;
                currentIndex = nextIndex;
                nextIndex = -1;
            }


        }



    }






    void SetUpInventory()
    {
        //call this in Start() to configure the inventory when you first start a level



        //if there is nothing in the inventory, set the current item to doublehands and that's it.
        //there's nothing to do here
        if(startingItems.Length == 0)
        {

        }


        currentIndex = startingItemIndex;
        nextIndex = -1;


        //if we start off already having selected a item, set some stuff that would normally be set when
        //we switch between items
        if (inventory[currentIndex] != null)
        {

            if (inventory[currentIndex].GetComponent<Item>() != null)
            {
                Item itemScript = inventory[currentIndex].GetComponent<Item>();

                //set the hand positions directly to the hold positions
                //this is the only case where you would oneshot set them during the hold state

                rightHandTransform.position = itemScript.rightHandHoldTransform.position;
                rightHandTransform.rotation = itemScript.rightHandHoldTransform.rotation;

                leftHandTransform.position = itemScript.leftHandHoldTransform.position;
                leftHandTransform.rotation = itemScript.leftHandHoldTransform.rotation;


                //set the item as the parent for the hands
                rightHandTransform.transform.parent = inventory[currentIndex].transform;
                leftHandTransform.transform.parent = inventory[currentIndex].transform;



            }
            else
            {
                Debug.LogError("The first selected object in the inventory doesn't have an Item script, silly.");
            }

        }

        //store the hand holds in each of the guns that you spawn with so that you don't have to do it every frame
        //you will have to do this when you get items too
        //you will have to remove them when you give them
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] != null)
            {
                leftHandHolds[i] = inventory[i].GetComponent<Item>().leftHandHoldTransform;
                rightHandHolds[i] = inventory[i].GetComponent<Item>().rightHandHoldTransform;

                localHoldPositions[i] = inventory[i].GetComponent<Item>().localHoldPosition;
                localHoldRotations[i] = Quaternion.Euler(inventory[i].GetComponent<Item>().localHoldRotation);


                //make sure that anything not selected is holstered
                if(i != startingItemIndex)
                {

                    inventory[i].transform.parent = transform;

                    inventory[i].transform.position = holsters[i].transform.position;
                    inventory[i].transform.rotation = holsters[i].transform.rotation;
                }


            }
            else
            {
                leftHandHolds[i] = null;
                rightHandHolds[i] = null;
                localHoldPositions[i] = Vector3.zero;
                localHoldRotations[i] = Quaternion.identity;
            }



        }
        




    }























}
