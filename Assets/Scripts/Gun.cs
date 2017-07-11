using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	public bool active;

	private AudioSource aud;

	public string nameOfGun;

	public int ammo;

	public bool auto;
	public bool fireCommand;

	public float fireRate = 0.02f;
	private float nextFire;
	public float range = 1000;

	public float damage = 20;

	private RaycastHit hit;
	public LayerMask fireMask;
	public GameObject bullet;
	public GameObject hitBullet;
	public GameObject critBullet;

	private bool madeBullet = false;


	public Transform holdTransform;

	
	public Vector3 idlePosition;
	

	public float kickback;

	public bool firing = false;

	public Material[] gunMaterials;
	public bool beingAimedAt = false;
	private bool beingAimedAtOld = false;
	public float colorLerpValue = 0;
	private float currentLerpTime;
	public Color[] initialEmissionColors;
	public Color[] finalEmissionColors;
	public Renderer[] gunRenderer;
	public float colorMod = 5;

	public Transform camTransform;
	public GameObject player;
	private PlayerStatus1 playerStatus1Script;
	private float grabRange;
	private LayerMask grabLayerMask;


	// Use this for initialization
	void Start () {

		aud = GetComponent<AudioSource>();

		camTransform = Camera.main.transform;
		player = GameObject.Find("Bounce Player");

		playerStatus1Script = player.GetComponent<PlayerStatus1>();




		beingAimedAtOld = false;

		colorLerpValue = 0;

		nextFire = 0;

		initialEmissionColors = new Color[gunMaterials.Length];
		finalEmissionColors = new Color[initialEmissionColors.Length];

		for(int i = 0; i< gunMaterials.Length; i++)
		{
			initialEmissionColors[i] = gunMaterials[i].GetColor("_EmissionColor");
			finalEmissionColors[i] = initialEmissionColors[i] * colorMod;
		}

	}
	
	// Update is called once per frame
	void Update () {

		if (auto)
		{
			fireCommand = Input.GetButton("Fire");
		}
		if (!auto)
		{
			fireCommand = Input.GetButtonDown("Fire");
		}

		grabRange = playerStatus1Script.grabRange;
		grabLayerMask = playerStatus1Script.grabLayerMask;

		if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, grabRange, grabLayerMask))
		{
			if(hit.transform.gameObject == gameObject)
			{
				beingAimedAt = true;


				if (Input.GetButton("Grab") && playerStatus1Script.weapons[playerStatus1Script.selectedSlot] == null && playerStatus1Script.grabState == 0)
				{
					
					playerStatus1Script.grabState = 1;


					playerStatus1Script.reachObject = gameObject;
					playerStatus1Script.canSwitch = false;
					playerStatus1Script.gunScriptReach = GetComponent<Gun>();
				}

			}
			else
			{
				beingAimedAt = false;
			}
		}




		if (beingAimedAt == !beingAimedAtOld)
		{
			if (beingAimedAt)
			{
				
				iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", 0.2f, "onupdatetarget", gameObject, "onupdate", "ValueToCallback"));
				
			}

			else
			{
				iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", 0.2f, "onupdatetarget", gameObject, "onupdate", "ValueToCallback"));


			}
		}

		beingAimedAtOld = beingAimedAt;



		foreach (Renderer ren in gunRenderer)
		{
			for(int i = 0; i < gunMaterials.Length; i++)
			{
				if(ren.sharedMaterial == gunMaterials[i])
				{

					ren.sharedMaterial.SetColor("_EmissionColor", Color.Lerp(initialEmissionColors[i], finalEmissionColors[i], colorLerpValue));
				}
			}
		}

		if (firing)
		{
			firing = false;
		}

		if (active)
		{

			if (fireCommand)
			{
				if (Time.time > nextFire && ammo> 0)
				{

					firing = true;

					playerStatus1Script.firing = true;
					madeBullet = false;


					//fire
					if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, range, fireMask))
					{

						if (hit.transform.gameObject.tag == "Enemy" && !madeBullet)
						{
							GameObject bul = (GameObject)Instantiate(hitBullet, hit.point, Quaternion.identity);

							bul.transform.parent = hit.transform;

							if(hit.transform.parent.GetComponent<ZombScript>() != null)
							{

								ZombScript zombScript = hit.transform.parent.GetComponent<ZombScript>();

								zombScript.TakeDamage(damage, transform.position, hit.point);
							}

							madeBullet = true;
							

						}
						if (hit.transform.gameObject.tag == "Enemy Crit" && !madeBullet)
						{
							GameObject bul = (GameObject)Instantiate(critBullet, hit.point, Quaternion.identity);

							bul.transform.parent = hit.transform;

							if (hit.transform.parent.GetComponent<ZombScript>() != null)
							{
								ZombScript zombScript = hit.transform.parent.GetComponent<ZombScript>();
								zombScript.TakeDamage(damage * 3, transform.position, hit.point);
							}

							madeBullet = true;
							
						}
						else if(!madeBullet)
						{
							Instantiate(bullet, hit.point, Quaternion.identity);

							madeBullet = true;
							
						}

					}
					nextFire = Time.time + fireRate;

					aud.PlayOneShot(aud.clip);

					ammo--;
				}
			}
			


		}


	}
	


	void ValueToCallback(float newValue)
	{
		colorLerpValue = newValue;
	}
}
