using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory2 : MonoBehaviour
{




    
    //ALSO MOVE OTHER HAND TO CORRESPONDING HAND HOLD IN PHASE 2 FOR FIRST TWO ROUTES TY




        private float holdingTimer;




    private bool canSwitch = true;
    private bool canGrab = true;

    public Transform playerCamera;
    public float grabRange = 3;
    public LayerMask grabLayerMask;
    private RaycastHit hit;



    private bool grabbing = false;
    private GameObject grabObject;
    private Item grabObjectItemScript;

    public enum GrabState
    {
        grabbing, pulling, idle, returningHandToGun //you'll need the last one for route 3 because there are 3 steps!!!~
    }

    public GrabState grabState;

    public enum GrabRoute
    {
        selecting, switching, holstering, idle
    }

    public GrabRoute grabRoute;

    private bool grabPhase1OneShot;
    private bool grabPhase2OneShot;
    private bool useTheRightHand;


    public float distanceBetweenHandandHoldAtGrab = 0.001f;

    public Transform leftHandTransform;
    public Transform rightHandTransform;
    public Transform handTransform;

    private GameObject[] inventory;
    public int maxInventorySize = 10;

    public Transform[] holsters;


    private int inventoryIndex;
    public int startingInventoryIndex = 0;



    public float throwForce = 10;
    public float throwForceHolster = 3;


    void Start()
    {
        inventory = new GameObject[maxInventorySize];
        inventoryIndex = startingInventoryIndex;

        grabRoute = GrabRoute.idle;
    }


    void Update()
    {


        GrabbingBehavior();
    }

    void GrabbingBehavior()
    {


        //you can't grab (or attempt to grab) another item while you are grabbing
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, grabRange, grabLayerMask) && grabbing != true && canGrab)
        {
            //you could make it glow or something in here to show the player that he/she can grab it



            //the only thing that we check before you start to grab an item is whether or not it has an Item script
            //(and if canGrab is true and if you press the button of course)
            //this means that no matter what other state, you are grabbing that item
            //the subsidiary cases are handled by the code in "if(grabbing)"
            if (hit.transform.gameObject.GetComponent<Item>() != null)
            {
                if (Input.GetButtonDown("Grab"))
                {
                    grabbing = true;

                    grabObject = hit.transform.gameObject;
                    grabObjectItemScript = hit.transform.gameObject.GetComponent<Item>();
                }
            }
        }



        //I need to make sure to make some of the things in here happen oneshot
        //the only ones that need to be continuous are the itweens
        if (!grabbing)
        {

            grabPhase1OneShot = true;
            grabPhase2OneShot = true;

        }





        if (grabbing)
        {

            //can't switch items while grabbing
            canSwitch = false;


            //BASICALLY: grabbing a new item will either select it (ie, you ready the weapon as soon as you grab it),
            //holster it (you put the gun away as soon as you grab it) (this includes switchholstering),
            //or switch it (you drop the gun you currently have while picking up this one)


            //route 1
            //You are selecting it only when you have nothing in your hands currently (you are on slot 0)
            //this means that you are switching to the corresponding slot of the item in the process
            //ALSOOOOOOO: if you have the same type of item in your holster when you pick up the new item
            //it auto drops so that you don't have two of the same gun

            //route 2
            //You are switching it if you are holding an item of the same type and you have to switch
            //because you can only have one item of each type

            //route 3
            //You are holstering it when you are holding an item of a different type. FURTHER, you are switchholstering it
            //when you have an item of the same type in your holster, and it needs to be replaced by this new item;



            //If you are selecting it: whichever hand is closest to its corresponding hand hold on the item goes out and grabs the gun
            //when pulling, you also switch to the item's slot (actually you do this in phase 1), and activate it (at end of phase 2)

            //If you are switching it: whichever hand is closest to its corresponding hand hold on the item goes out and grabs the gun
            //while the hand is grabbing, the other hand is throwing the current item either to the left or right, depending on if
            //it is the left hand, or the right hand (left with left). Then the hand that grabbed, pulls the new item in
            //and its identical to the second stage of selecting

            //If you are holstering it: whichever hand is corresponding to the item's holster on the body reaches out for it and grabs it
            //the other hand stays still for the whole time because its holding the current item. Then the hand that grabbed moves the new item
            //toward its corresponding holster until the positions match. If there was an item already in that place, it gets ejected with 
            //a little push outward in the FIRST stage of holstering



            //set up the initial grab state (all of these routes have 2 phases, so its ok to use the same grab states throughout
            grabState = GrabState.grabbing;


            //route 1 - youre on slot 0 and you have nothing
            if (inventoryIndex == 0 && inventory[inventoryIndex] == null)
            {
                grabRoute = GrabRoute.selecting;
            }


            //now check for route 2 and 3

            if (inventory[inventoryIndex] != null && grabRoute != GrabRoute.selecting)
            {
                //route 2
                if (inventory[inventoryIndex].transform.tag == grabObject.transform.tag)
                {
                    grabRoute = GrabRoute.switching;
                }
            }


            //failsafe
            if (grabRoute == GrabRoute.idle)
            {
                print("there is a case in the grab routes that is not being handleeeeeddddd!");
            }



            if (grabRoute == GrabRoute.selecting)
            {
                

                if (grabState == GrabState.grabbing)
                {
                    //ok, let's do all the oneshots first
                    if (grabPhase1OneShot)
                    {


                        //First, switch to the corresponding slot
                        //I'll have to write a dictionary in another script (maybe game manager) that handles this
                        int slotToSwitchTo = GameManager2.instance.itemSlotOrder.FindIndex(obj => obj == grabObject.transform.tag);

                        


                        //AAAAAAAAAAAAA
                        //BEEEEEFORE YOU DO THIS YOU HAVE TO AUTODROP THE GUN OF THE SAME TYPE FROM THE HOLSTER IF YOU HAVE IT
                        if (inventory[inventoryIndex] != null)
                        {

                            //this is essentially a oneshot because once the function is complete, inventory[inventoryIndex] will be null
                            //it will be not null after inventory[inventoryIndex] is set in phase 2, but that is out of the scope of this
                            //code


                            DropFromHolster(inventoryIndex);
                        }



                        //find which hand is closest to corresponding hand hold
                        float distanceForRightHand = Vector3.Distance(rightHandTransform.position, grabObjectItemScript.rightHandHoldTransform.position);
                        float distanceForLeftHand = Vector3.Distance(leftHandTransform.position, grabObjectItemScript.leftHandHoldTransform.position);

                        useTheRightHand = distanceForRightHand >= distanceForLeftHand;

                        if (useTheRightHand)
                        {
                            //nullify the parent so it can grab it without trouble as you move around
                            rightHandTransform.parent = null;

                        }
                        else
                        {
                            // nullify the parent so it can grab it without trouble as you move around
                            leftHandTransform.parent = null;

                        }


                        //make it a oneshot
                        grabPhase1OneShot = false;
                        
                    }


                    //here we will do all of the continuous things for the first phase of route 
                    if (useTheRightHand)
                    {

                        //move it over to its hand hold
                        iTween.MoveUpdate(rightHandTransform.gameObject, iTween.Hash("position", grabObjectItemScript.rightHandHoldTransform, "time", 0.5f, "islocal", false));
                        iTween.RotateUpdate(rightHandTransform.gameObject, iTween.Hash("rotation", grabObjectItemScript.rightHandHoldTransform, "time", 0.5f, "islocal", false));


                        //when its close enough, then go to phase 2!~
                        if (Vector3.Distance(rightHandTransform.position, grabObjectItemScript.rightHandHoldTransform.position) < distanceBetweenHandandHoldAtGrab)
                        {
                            grabState = GrabState.pulling;
                        }
                    }
                    else
                    {

                        //move it over to its hand hold
                        iTween.MoveUpdate(leftHandTransform.gameObject, iTween.Hash("position", grabObjectItemScript.leftHandHoldTransform, "time", 0.5f, "islocal", false));
                        iTween.RotateUpdate(leftHandTransform.gameObject, iTween.Hash("rotation", grabObjectItemScript.leftHandHoldTransform, "time", 0.5f, "islocal", false));


                        //when its close enough, then go to phase 2!~
                        if (Vector3.Distance(leftHandTransform.position, grabObjectItemScript.leftHandHoldTransform.position) < distanceBetweenHandandHoldAtGrab)
                        {
                            grabState = GrabState.pulling;
                        }

                        
                    }

                }


                if (grabState == GrabState.pulling)
                {

                    //now lets do the phase 2 oneshots
                    if (grabPhase2OneShot)
                    {
                        //put it in the inventory
                        inventory[inventoryIndex] = grabObject;

                        //deactivate rigidbody physics
                        Rigidbody rigid = inventory[inventoryIndex].GetComponent<Rigidbody>();
                        rigid.isKinematic = true;
                        rigid.useGravity = false;


                        grabObjectItemScript.active = true;


                        //Collider col = inventory[inventoryIndex].GetComponent<Collider>();
                        Collider col = grabObjectItemScript.col;
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

                        }
                        else
                        {
                            inventory[inventoryIndex].transform.parent = leftHandTransform;

                            //Now we child the hand to the superhand transform to pull it back in smoothly

                            leftHandTransform.parent = handTransform;


                        }

                        grabPhase2OneShot = false;
                        
                    }



                    if (useTheRightHand)
                    {

                        iTween.MoveUpdate(rightHandTransform.gameObject, iTween.Hash("position", grabObjectItemScript.localHoldPosition, "time", 0.5f, "islocal", true));
                        iTween.RotateUpdate(rightHandTransform.gameObject, iTween.Hash("rotation", grabObjectItemScript.localHoldRotation, "time", 0.5f, "islocal", true));




                        //move the other hand toooooooooooooooo!!!!!!!!!


                        iTween.MoveUpdate(leftHandTransform.gameObject, iTween.Hash("position", grabObjectItemScript.leftHandHoldTransform, "time", 0.2f, "islocal", false));
                        iTween.RotateUpdate(leftHandTransform.gameObject, iTween.Hash("rotation", grabObjectItemScript.leftHandHoldTransform, "time", 0.2f, "islocal", false));




                        //check when you've finished pulling in the item

                        //false or true for islocal???!?

                        if (Vector3.Distance(rightHandTransform.localPosition, grabObjectItemScript.localHoldPosition) < distanceBetweenHandandHoldAtGrab)
                        {
                            //back into idle; you can switch now
                            grabState = GrabState.idle;
                            canSwitch = true;

                            //call off the whole function
                            grabbing = false;
                            grabRoute = GrabRoute.idle;
                        }
                    }
                    else
                    {

                        iTween.MoveUpdate(leftHandTransform.gameObject, iTween.Hash("position", grabObjectItemScript.localHoldPosition, "time", 0.5f, "islocal", true));
                        iTween.RotateUpdate(leftHandTransform.gameObject, iTween.Hash("rotation", grabObjectItemScript.localHoldRotation, "time", 0.5f, "islocal", true));



                        //move the other hand toooooooooooooooo!!!!!!!!!

                        //false or true for islocal???!?!

                        iTween.MoveUpdate(rightHandTransform.gameObject, iTween.Hash("position", grabObjectItemScript.rightHandHoldTransform, "time", 0.2f, "islocal", false));
                        iTween.RotateUpdate(rightHandTransform.gameObject, iTween.Hash("rotation", grabObjectItemScript.rightHandHoldTransform, "time", 0.2f, "islocal", false));




                        //check when you've finished pulling in the item

                        if (Vector3.Distance(leftHandTransform.localPosition, grabObjectItemScript.localHoldPosition) < distanceBetweenHandandHoldAtGrab)
                        {
                            //back into idle; you can switch now
                            grabState = GrabState.idle;
                            canSwitch = true;

                            //call off the whole function
                            grabbing = false;

                            grabRoute = GrabRoute.idle;
                        }
                        
                    }

                }

            }

            if (grabRoute == GrabRoute.switching)
            {

                //we have to throw the current gun to the side, but I want the hand throwing it to stick to it for a bit to sell the effect
                //this means we need a timer float
                //we will thus first turn on the rigidbody and do physics, then after the timer runs out, do the rest of the "drop" instructions
                //like take it off the gun layer
                //IMPORTANT: this has to happen before the other hand grabs the new item to avoid an array error

                if(grabState == GrabState.grabbing)
                {
                    


                    if (grabPhase1OneShot)
                    {
                        //FIRST find which hand is closest to corresponding hand hold
                        float distanceForRightHand = Vector3.Distance(rightHandTransform.position, grabObjectItemScript.rightHandHoldTransform.position);
                        float distanceForLeftHand = Vector3.Distance(leftHandTransform.position, grabObjectItemScript.leftHandHoldTransform.position);

                        useTheRightHand = distanceForRightHand >= distanceForLeftHand;


                        //activate rigidbody physics
                        Rigidbody rigid = inventory[inventoryIndex].GetComponent<Rigidbody>();
                        rigid.isKinematic = false;
                        rigid.useGravity = true;

                        if (useTheRightHand)
                        {
                            //throw current item to left
                            rigid.AddForce(transform.right * -1 * throwForceHolster, ForceMode.VelocityChange);

                        }
                        else
                        {

                            //throw current item to right
                            rigid.AddForce(transform.right * throwForceHolster, ForceMode.VelocityChange);

                        }

                        //now that its been thrown, start a timer
                        holdingTimer = Time.time;


                        

                        if (useTheRightHand)
                        {
                            //nullify the parent so it can grab it without trouble as you move around
                            rightHandTransform.parent = null;

                        }
                        else
                        {
                            // nullify the parent so it can grab it without trouble as you move around
                            leftHandTransform.parent = null;

                        }

                        
                        grabPhase1OneShot = false;
                    }





                    //once this timer runs out, do the rest of the dropping instructions (i set it at 0.3)
                    if (Time.time > 0.1f + holdingTimer && inventory[inventoryIndex] != null)
                    {

                        //this doesn't seem to be being called...
                        print("delayed action being called");


                        //set it to inactive
                        Item itemScript = inventory[inventoryIndex].GetComponent<Item>();
                        itemScript.active = false;

                        //set the layers to default again
                        inventory[inventoryIndex].layer = LayerMask.NameToLayer("Default"); ;
                        foreach (Transform child in inventory[inventoryIndex].transform)
                        {
                            child.gameObject.layer = LayerMask.NameToLayer("Default");
                        }

                        //make its collider not a trigger so it collides and stuff
                        //Collider col = inventory[inventoryIndex].GetComponent<Collider>();

                        Collider col = grabObjectItemScript.col;
                        col.isTrigger = false;


                        //you have to unchild it from the body
                        inventory[inventoryIndex].transform.parent = null;



                        //take it out of the inventory
                        //you have to do this at the very end or else you can't reference it in the code
                        inventory[inventoryIndex] = null;



                    }







                    //now continuous stuff for first phase
                    if (useTheRightHand)
                    {

                        //move it over to its hand hold
                        iTween.MoveUpdate(rightHandTransform.gameObject, iTween.Hash("position", grabObjectItemScript.rightHandHoldTransform, "time", 0.5f, "islocal", false));
                        iTween.RotateUpdate(rightHandTransform.gameObject, iTween.Hash("rotation", grabObjectItemScript.rightHandHoldTransform, "time", 0.5f, "islocal", false));


                        //when its close enough, then go to phase 2!~
                        if (Vector3.Distance(rightHandTransform.position, grabObjectItemScript.rightHandHoldTransform.position) < distanceBetweenHandandHoldAtGrab)
                        {
                            grabState = GrabState.pulling;

                            print("pulling now");
                        }
                        
                    }
                    else
                    {

                        //move it over to its hand hold
                        iTween.MoveUpdate(leftHandTransform.gameObject, iTween.Hash("position", grabObjectItemScript.leftHandHoldTransform, "time", 0.5f, "islocal", false));
                        iTween.RotateUpdate(leftHandTransform.gameObject, iTween.Hash("rotation", grabObjectItemScript.leftHandHoldTransform, "time", 0.5f, "islocal", false));


                        //when its close enough, then go to phase 2!~
                        if (Vector3.Distance(leftHandTransform.position, grabObjectItemScript.leftHandHoldTransform.position) < distanceBetweenHandandHoldAtGrab)
                        {
                            grabState = GrabState.pulling;

                            print("pulling now");
                        }
                        
                        
                    }


                    print(Time.time > 0.3f + holdingTimer);
                    print(Time.time + "time");
                    print(0.3f + holdingTimer + "holdingtimer");





                }

                if (grabState == GrabState.pulling)
                {

                    //now lets do the phase 2 oneshots
                    if (grabPhase2OneShot)
                    {
                        //put it in the inventory
                        inventory[inventoryIndex] = grabObject;

                        //deactivate rigidbody physics
                        Rigidbody rigid = inventory[inventoryIndex].GetComponent<Rigidbody>();
                        rigid.isKinematic = true;
                        rigid.useGravity = false;


                        grabObjectItemScript.active = true;

                        

                        Collider col = grabObjectItemScript.col;
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

                        }
                        else
                        {
                            inventory[inventoryIndex].transform.parent = leftHandTransform;

                            //Now we child the hand to the superhand transform to pull it back in smoothly

                            leftHandTransform.parent = handTransform;


                        }

                        grabPhase2OneShot = false;
                    }



                    if (useTheRightHand)
                    {

                        iTween.MoveUpdate(rightHandTransform.gameObject, iTween.Hash("position", grabObjectItemScript.localHoldPosition, "time", 0.5f, "islocal", true));
                        iTween.RotateUpdate(rightHandTransform.gameObject, iTween.Hash("rotation", grabObjectItemScript.localHoldRotation, "time", 0.5f, "islocal", true));



                        //check when you've finished pulling in the item

                        if (Vector3.Distance(rightHandTransform.localPosition, grabObjectItemScript.localHoldPosition) < distanceBetweenHandandHoldAtGrab)
                        {
                            //back into idle; you can switch now
                            grabState = GrabState.idle;
                            canSwitch = true;

                            //call off the whole function
                            grabbing = false;
                            grabRoute = GrabRoute.idle;

                            print("finished");
                        }
                    }
                    else
                    {

                        iTween.MoveUpdate(leftHandTransform.gameObject, iTween.Hash("position", grabObjectItemScript.localHoldPosition, "time", 0.5f, "islocal", true));
                        iTween.RotateUpdate(leftHandTransform.gameObject, iTween.Hash("rotation", grabObjectItemScript.localHoldRotation, "time", 0.5f, "islocal", true));



                        //check when you've finished pulling in the item

                        if (Vector3.Distance(leftHandTransform.localPosition, grabObjectItemScript.localHoldPosition) < distanceBetweenHandandHoldAtGrab)
                        {
                            //back into idle; you can switch now
                            grabState = GrabState.idle;
                            canSwitch = true;

                            //call off the whole function
                            grabbing = false;
                            grabRoute = GrabRoute.idle;

                            print("finished");
                        }
                        
                    }

                }


            }








        }
    }



    void DropFromHolster(int index)
    {

        //this is essentially the same as dropping the gun normally, except you don't have to do a lot of the things
        //that you have to do when you have the gun equipped, like switching the layers of the meshes of the item back
        //to default from "item"

        //another difference is that it will pop outward from the holster (you'll calculate the vector by subtracting your
        //own position from the position of the holster in question



        //you don't have to set if it is active, the layers, 




        //activate rigidbody physics
        Rigidbody rigid = inventory[index].GetComponent<Rigidbody>();
        rigid.isKinematic = false;
        rigid.useGravity = true;


        //make its collider not a trigger so it collides and stuff
        //Collider col = inventory[index].GetComponent<Collider>();

        Collider col = grabObjectItemScript.col;
        col.isTrigger = false;


        //you have to unchild it from the body
        inventory[index].transform.parent = null;


        //now at the very end, you can eject it away from the body

        rigid.AddForce(holsters[index].position - transform.position * throwForceHolster, ForceMode.VelocityChange);




        //take it out of the inventory
        //you have to do this at the very end or else you can't reference it in the code
        inventory[index] = null;




    }








}








