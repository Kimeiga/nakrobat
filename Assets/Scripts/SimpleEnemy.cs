using UnityEngine;
using System.Collections;

public class SimpleEnemy : MonoBehaviour {

    private NavMeshAgent agent;
    

    public float health;
    public Vector3 destination;

    public GameObject gun;
    public float gunDistance = 3;

    public GameObject targetGun;

    public Transform lookDestination;
    public Transform lookTransform;
    public Transform gunTransform;

    public int team; //0 = capsule, 1 = cube;

    public int status = 0; //0 = moving to enemy, 1 = moving to gun, 2 = grabbing gun, 3 = running away
    

	// Use this for initialization
	void Start () {

        health = 100;

        agent = GetComponent<NavMeshAgent>();
        lookTransform = transform.FindChild("Look Transform");
        gunTransform = lookTransform.FindChild("Gun Transform");

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        
        if (status  == 1)
        {
            if(Vector3.Distance(lookTransform.position,destination) <= gunDistance)
            {
                GrabGun(targetGun);
                SimpleBattle.Instance.guns.Remove(gun);
            }
        }
        
        if(status == 2)
        {

            iTween.MoveUpdate(targetGun, iTween.Hash("islocal", false, "time", 01f, "position", gunTransform.position, "oncompletetarget", gameObject, "oncomplete", "GrabGunComplete"));
            iTween.RotateUpdate(targetGun, iTween.Hash("islocal", false, "time", 01f, "rotation", gunTransform.rotation));

            agent.Stop();

            print("JELJF");

        }


        if(status != 2)
        {
            if (gun && status != 2)
            {

                destination = FindClosestEnemy(team).transform.position;
                status = 0;
            }
            else if (SimpleBattle.Instance.guns.Count > 0)
            {

                destination = FindClosestGun().transform.position;
                status = 1;
            }
            else
            {
                status = 3;



                destination = transform.position + ((FindClosestEnemy(team).transform.position - transform.position).normalized * -3);
            }

            if (destination != null)
            {

                lookTransform.LookAt(lookDestination);


                agent.destination = destination;
            }

        }
    }

    GameObject FindClosestGun()
    {
        float distance = Mathf.Infinity;
        GameObject closest = null;

        foreach (GameObject gun in SimpleBattle.Instance.guns)
        {
            Vector3 diff = gun.transform.position - transform.position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = gun;
                distance = curDistance;
            }
        }

        lookDestination = closest.transform;
        targetGun = closest.gameObject;
        return closest;
    }

    GameObject FindClosestEnemy(int team)
    {
        float distance = Mathf.Infinity;
        SimpleEnemy closest = null;

        if (team == 0)
        {

            foreach (SimpleEnemy enemy in SimpleBattle.Instance.aliveCubeTeam)
            {
                Vector3 diff = enemy.transform.position - transform.position;
                float curDistance = diff.sqrMagnitude;
                if(curDistance < distance)
                {
                    closest = enemy;
                    distance = curDistance;
                }
            }
        }


        if (team == 1)
        {

            foreach (SimpleEnemy enemy in SimpleBattle.Instance.aliveCapsuleTeam)
            {
                Vector3 diff = enemy.transform.position - transform.position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = enemy;
                    distance = curDistance;
                }
            }
        }

        lookDestination = closest.lookTransform;
        
        return closest.gameObject;
    }

    void GrabGun(GameObject gunToGrab)
    {

        //Vector3 oldPosition = gunToGrab.transform.position;
        //Quaternion oldQuaternion = gunToGrab.transform.rotation;
        

        

        SimpleGun simpleGunScript = gunToGrab.GetComponent<SimpleGun>();

        simpleGunScript.owned = true;

        Rigidbody rigid = gunToGrab.GetComponent<Rigidbody>();
        rigid.isKinematic = true;
        rigid.useGravity = false;

        Collider col = gunToGrab.GetComponent<Collider>();
        col.isTrigger = true;

        //gun.transform.localPosition = gunTransform.InverseTransformPoint(oldPosition);

        status = 2;

    }

    void GrabGunComplete()
    {

        status = 0;


        gun = targetGun;
        gun.transform.parent = gunTransform;

        targetGun = null;

        print("JELJFadf");
    }

}
