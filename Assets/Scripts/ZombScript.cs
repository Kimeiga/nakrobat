using UnityEngine;
using System.Collections;

public class ZombScript : MonoBehaviour {

    public float health;
    public float flyForce = 3;

    public bool dying = false;
    public float dyingStartTime;
    public float dieTime = 5;

    private NavMeshAgent agent;
    private GameObject player;
    private PlayerStatus1 playerStatus1Script;

    public GameObject gun;
    public Transform shootTransform;
    public float range = 4;
    public float fireRate = 0.4f;
    public float damage = 10;
    private float nextFire;
    
    public GameObject shotSphere;


    public float timeBetweenPlaces = 5;
    private float nextTimeBetweenPlaces = 0;
    public float roamRadius = 10;

    public ParticleSystem shootParticles;

    public ParticleSystem dieParticles;

   // public GameManager gameManager;

	// Use this for initialization
	void Start () {

        player = GameObject.Find("Bounce Player");
        playerStatus1Script = player.GetComponent<PlayerStatus1>();



        agent = GetComponent<NavMeshAgent>();

        nextFire = 0;

    }

    // Update is called once per frame
    void Update () {

        if (playerStatus1Script.alive == true)
        {

            agent.destination = player.transform.position;

            Vector3 point = player.transform.position;
            point.y = transform.position.y;
            transform.LookAt(point);


            gun.transform.LookAt(player.transform);

            if (Vector3.Distance(transform.position, player.transform.position) <= range)
            {
                //shoot
                if (Time.time > nextFire)
                {
                    playerStatus1Script.health -= damage;

                    Instantiate(shootParticles, shootTransform.position, shootTransform.rotation, shootTransform);

                    Instantiate(shotSphere, shootTransform.position, shootTransform.rotation, shootTransform);

                    nextFire = Time.time + fireRate;
                }

            }


            if (Time.time > dieTime + dyingStartTime && health <= 0)
            {
                Instantiate(dieParticles, transform.position, transform.rotation);
                GameManager.aliveEnemies.Remove(gameObject.GetComponent<ZombScript>());
                Destroy(gameObject);
            }
        }
        else
        {
            if(Time.time > nextTimeBetweenPlaces)
            {
                Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
                randomDirection += transform.position;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1);
                Vector3 finalPosition = hit.position;



                agent.destination = finalPosition;

                nextTimeBetweenPlaces = Time.time + timeBetweenPlaces;
            }

        }

	}

    public void TakeDamage(float damage, Vector3 gunPosition, Vector3 bulletPosition)
    {
        health -= damage;

        if(health <= 0)
        {

            //Vector3 dir = transform.position - gunPosition;

            //rigid.AddForceAtPosition(dir.normalized * flyForce, bulletPosition, ForceMode.VelocityChange);

            dying = true;
            dyingStartTime = Time.time;
        }

    }
}
