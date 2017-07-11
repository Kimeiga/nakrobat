using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public int levelNumber;

    public float enemyRate;
    private float nextEnemy = 0;

    public float roundBetweenwait = 3;
    public float nextRoundTimeWait = 0;
    public bool setNextRoundTime;

    public bool roundActive = false;
    public bool roundFinished = true;
    public bool spawnEnemies = false;
    public bool startedRound = false;

    public AudioSource zombTextAudio;
    public AudioSource numberAudio;

    public GameObject[] enemies;
    public GameObject[] weapons;
    public GameObject[] health;

    public static List<ZombScript> aliveEnemies;

    public Transform[] enemySpawns;
    public Transform[] weaponSpawns;
    public Transform firstGunPlate;

    public static GameObject player;
    public static Transform fireTransform;
    public static PlayerStatus1 playerStatus1Script;


    public TextMesh levelNumberText;
    public TextMesh zombText;
    public TextMesh runnText;
    public TextMesh spedText;
    public TextMesh magneText;

    public TextMesh levelUpText;

    public int zombsSpawned = 0;
    public int runnsSpawned = 0;
    public int spedsSpawned = 0;
    public int magnesSpawned = 0;

    public TextMesh timeText;


    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

	// Use this for initialization
	void Start () {

        levelNumber = 0;

        aliveEnemies = new List<ZombScript>();

        Instantiate(weapons[Mathf.RoundToInt(Random.Range(0, 4))], firstGunPlate.transform.position, Quaternion.identity);
        

    }
	
	// Update is called once per frame
	void Update () {

        print(aliveEnemies.Count);

        if (startedRound == false)
        {
            if(setNextRoundTime == false)
            {

                nextRoundTimeWait = Time.time + roundBetweenwait;
                setNextRoundTime = true;
            }




            levelUpText.text = Mathf.RoundToInt(nextRoundTimeWait - Time.time).ToString();

            if(levelNumberText.text == "5")
            {
                numberAudio.Play();
            }
            if (levelNumberText.text == "4")
            {
                numberAudio.Play();
            }

        }
        else
        {
            levelUpText.text = "";
        }





        levelNumberText.text = "Level " + levelNumber.ToString();

        zombText.text = "Zombs: " + zombsSpawned;
        runnText.text = "Runns: " + runnsSpawned;
        spedText.text = "Speds: " + spedsSpawned;
        magneText.text = "Magnes: " + magnesSpawned;

        timeText.text = "Time: " + Mathf.Round(Time.timeSinceLevelLoad).ToString();
        


        if (roundFinished == true)
        {

            StartCoroutine(WaitAndNextRound(roundBetweenwait));
            
        }


        if (spawnEnemies)
        {

            //weapon first
            if (levelNumber % 3 == 0)
            {

                int randIndex  = Mathf.RoundToInt(Random.Range(0, weaponSpawns.Length));

                Instantiate(weapons[Mathf.RoundToInt(Random.Range(0, 3))], weaponSpawns[randIndex].transform.position, Quaternion.identity);


                weaponSpawns[randIndex].GetComponent<AudioSource>().Play();
            }

            //health now
            //weapon first
            if (levelNumber % 2 == 0)
            {

                int randIndex = Mathf.RoundToInt(Random.Range(0, weaponSpawns.Length));

                Instantiate(health[Mathf.RoundToInt(Random.Range(0, 2))], weaponSpawns[randIndex].transform.position, Quaternion.identity);


                weaponSpawns[randIndex].GetComponent<AudioSource>().Play();
            }




            //zombsSpawned = Mathf.RoundToInt(Mathf.Pow(2, levelNumber));

            zombsSpawned = levelNumber + 2;


            startedRound = true;


            spawnEnemies = false;

            StartCoroutine(StartSpawningZombs());
            StartCoroutine(StartSpawningRunns());
            StartCoroutine(StartSpawningSpeds());
            StartCoroutine(StartSpawningMagnes());





        }


        if(aliveEnemies.Count <= 0 && startedRound == true)
        {
            roundFinished = true;
            
            startedRound = false;

            setNextRoundTime = false;
        }


    }

    IEnumerator WaitAndNextRound(float waitTime)
    {


        roundFinished = false;

        yield return new WaitForSeconds(waitTime);



        levelNumber++;
        spawnEnemies = true;

    }

    IEnumerator StartSpawningZombs()
    {
        for (int i = 0; i < zombsSpawned; i++)
        {
            int randIndex = Mathf.RoundToInt(Random.Range(0, enemySpawns.Length));

            GameObject inst = (GameObject)Instantiate(enemies[0], enemySpawns[randIndex].position, Quaternion.identity);

            ZombScript zo = inst.GetComponent<ZombScript>();

            aliveEnemies.Add(zo);

            enemySpawns[randIndex].GetComponent<AudioSource>().Play();

            //zombsSpawned--;

            yield return new WaitForSeconds(enemyRate);


        }
    }

    IEnumerator StartSpawningRunns()
    {
        if (zombsSpawned >= 8)
        {
            runnsSpawned = Mathf.RoundToInt(Mathf.Pow(2, levelNumber - 4));
        }

        yield return new WaitForSeconds(enemyRate);

        if (zombsSpawned >= 6)
        {
            

            for (int i = 0; i < runnsSpawned; i++)
            {
                int randIndex = Mathf.RoundToInt(Random.Range(0, enemySpawns.Length));

                GameObject inst = (GameObject)Instantiate(enemies[1], enemySpawns[randIndex].position, Quaternion.identity);

                ZombScript zo = inst.GetComponent<ZombScript>();

                aliveEnemies.Add(zo);

                enemySpawns[randIndex].GetComponent<AudioSource>().Play();

                //runnsSpawned--;

                yield return new WaitForSeconds(enemyRate);

            }
        }
    }

    IEnumerator StartSpawningSpeds()
    {
        if (zombsSpawned >= 10)
        {
            spedsSpawned = Mathf.RoundToInt(Mathf.Pow(2, levelNumber - 4));
        }

        yield return new WaitForSeconds(enemyRate + 1);

        if (zombsSpawned >= 8)
        {

            for (int i = 0; i < spedsSpawned; i++)
            {
                int randIndex = Mathf.RoundToInt(Random.Range(0, enemySpawns.Length));

                GameObject inst = (GameObject)Instantiate(enemies[2], enemySpawns[randIndex].position, Quaternion.identity);

                ZombScript zo = inst.GetComponent<ZombScript>();

                aliveEnemies.Add(zo);


                enemySpawns[randIndex].GetComponent<AudioSource>().Play();

                //--;

                yield return new WaitForSeconds(enemyRate);

            }
        }
    }

    IEnumerator StartSpawningMagnes()
    {
        if (zombsSpawned >= 12)
        {
            magnesSpawned = Mathf.RoundToInt(Mathf.Pow(2, levelNumber - 4));
        }

        yield return new WaitForSeconds(enemyRate + 2);

        if (zombsSpawned >= 8)
        {

            for (int i = 0; i < magnesSpawned; i++)
            {
                int randIndex = Mathf.RoundToInt(Random.Range(0, enemySpawns.Length));

                GameObject inst = (GameObject)Instantiate(enemies[3], enemySpawns[randIndex].position, Quaternion.identity);

                ZombScript zo = inst.GetComponent<ZombScript>();

                aliveEnemies.Add(zo);


                enemySpawns[randIndex].GetComponent<AudioSource>().Play();

                //magnesSpawned--;

                yield return new WaitForSeconds(enemyRate);

            }
        }
    }

}
