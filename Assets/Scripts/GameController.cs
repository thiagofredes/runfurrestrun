using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public static GameController instance;

	public Vector3[] lanesPosition;
	public GameObject smallWoolball;
	public GameObject giantWoolball;
	public GameObject goodies;
	public Text distanceText;
	public Text goodieText;
	public Text gameoverText;
	public float timeBetweenSpawns;
	public float obstaclesSpeed;
	public float speedIncrement;
	public float goodieProbability;
	public Vector3 woolballPosition1;
	public Vector3 woolballPosition2;
	public float[] woolballTimes;
	
	private GameObject player;
	private PlayerController playerController;
	private AudioSource audioSource;
	private GameObject giantWoolballObj;
	private WoolballController woolballController;
	private Vector3 woolballOriginalPosition;
	private Dictionary<int, GameObject> spawnedObjects;
	private int totalLanes;
	private int totalGoodies;
	private float totalDistance;
	private float currentSpeed;
	private bool gameOver;
	private bool restart;
	private int woolballCollisionCounter;
	private int woolballStage;
	private int playerCollisions;
	private int playerGoodies;
	private Coroutine currentCoroutine;
	

	void Awake(){
		instance = this;
		audioSource = gameObject.GetComponent<AudioSource>();
		audioSource.Play();
	}

	void Start () {
		currentSpeed = obstaclesSpeed;
		totalLanes = lanesPosition.Length;
		totalGoodies = goodies.GetComponent<Transform>().childCount;
		giantWoolballObj = Instantiate(giantWoolball);
		woolballOriginalPosition = giantWoolballObj.GetComponent<Transform>().position;
		woolballController = giantWoolballObj.GetComponent<WoolballController>();
		woolballCollisionCounter = 0;
		woolballStage = 0;
		playerCollisions = 0;
		playerGoodies = 0;
		player = GameObject.FindWithTag("Player");
		playerController = player.GetComponent<PlayerController>();
		totalDistance = 0.0f;
		gameOver = false;
		restart = false;
		spawnedObjects = new Dictionary<int, GameObject>();
		currentCoroutine = null;


		SetDistanceText();
		SetGoodieText();
		SetSpeed(obstaclesSpeed);

		StartCoroutine(SpawnObstacles());
		StartCoroutine(ManageCollisions());
		StartCoroutine(ManageDistance());
		StartCoroutine(ManagePlayerGoodies());
		StartCoroutine(ManageSpeed());
	}


	IEnumerator SpawnObstacles(){
		yield return new WaitForSeconds(3.0f);
		while(true){
			if(gameOver){
				yield break;
			}
			int lane = Random.Range (0, totalLanes);
			GameObject spawnedObject;
			if(Random.value < goodieProbability){
				int goodie_i = Random.Range(0, totalGoodies);
				GameObject goodie = goodies.GetComponent<Transform>().GetChild(goodie_i).gameObject;
				Vector3 instantiatePosition = new Vector3(lanesPosition[lane].x, lanesPosition[lane].y); // + goodie.GetComponent<BoxCollider2D>().size.y/2f);
				spawnedObject = Instantiate(goodie, instantiatePosition, Quaternion.identity) as GameObject;
				spawnedObject.layer = LayerMask.NameToLayer("Lane " + lane);
				spawnedObject.GetComponent<SimpleMover>().Move(currentSpeed);
			}
			else{
				Vector3 instantiatePosition = new Vector3(lanesPosition[lane].x, lanesPosition[lane].y);// + smallWoolball.GetComponent<BoxCollider2D>().size.y/2f);
				spawnedObject = Instantiate(smallWoolball, instantiatePosition, Quaternion.identity) as GameObject;
				spawnedObject.layer = LayerMask.NameToLayer("Lane " + lane);
				spawnedObject.GetComponent<SimpleMover>().Move(currentSpeed);
			}
			spawnedObjects.Add (spawnedObject.GetInstanceID(), spawnedObject);
			yield return new WaitForSeconds (Mathf.Max(0.5f, Random.value * timeBetweenSpawns));
		}
	}

	IEnumerator ManageWoolBallTime(int nextStage, Vector3 startPosition, float remainTime){
		while(remainTime > 0){
			remainTime -= 1.0f;
			yield return new WaitForSeconds(1.0f);
		}
		woolballStage = nextStage;
		woolballCollisionCounter--;
		woolballController.SetMove(startPosition.x);
		playerController.Walk();
		playerController.DecreaseSpeed (speedIncrement);
		SetSpeed(currentSpeed / speedIncrement);
	}

	IEnumerator ManageCollisions(){
		while(true){
			int newCollisions = playerController.GetCollisions();

			if(newCollisions > playerCollisions){
				woolballCollisionCounter++;
			}

			playerCollisions = newCollisions;

			if(woolballStage == 0){
				audioSource.pitch = 1.0f;
				if(woolballCollisionCounter == 2){
					woolballStage = 1;
				}
			}
			if(woolballStage == 1){
				if(woolballCollisionCounter == 2){
					woolballController.SetMove (woolballPosition1.x);
					SetSpeed(currentSpeed * speedIncrement);
					playerController.Run();
					currentCoroutine = StartCoroutine(ManageWoolBallTime(0, woolballOriginalPosition, woolballTimes[woolballCollisionCounter-2]));
					woolballStage = 2;
				}
			}
			if(woolballStage == 2){
				if(woolballCollisionCounter == 3){
					StopCoroutine(currentCoroutine);
					SetSpeed(currentSpeed * speedIncrement);
					playerController.IncreaseSpeed (speedIncrement);
					playerController.Run();
					woolballController.SetMove (woolballPosition2.x);
					audioSource.pitch = 1.25f;
					currentCoroutine = StartCoroutine(ManageWoolBallTime(4, woolballPosition1, woolballTimes[woolballCollisionCounter-2]));
					woolballStage = 3;
				}
			}
			if(woolballStage == 3){
				if(woolballCollisionCounter == 4){
					gameOver = true;
					yield break;
				}
			}
			if(woolballStage == 4){
				if(woolballCollisionCounter == 2){
					currentCoroutine = StartCoroutine(ManageWoolBallTime(0, woolballOriginalPosition, woolballTimes[woolballCollisionCounter-2]));
					woolballStage = 5;
				}
			}
			if(woolballStage == 5){
				audioSource.pitch = 1.0f;
				if(woolballCollisionCounter == 3){
					woolballStage = 2;
				}
			}

			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator ManageSpeed(){
		while(true){
			if(gameOver){
				yield break;
			}
			yield return new WaitForSeconds(10.0f);
			SetSpeed(currentSpeed * speedIncrement);
			playerController.IncreaseSpeed (speedIncrement);
		}
	}

	IEnumerator ManageDistance(){
		while(true){
			if(gameOver){
				yield break;
			}
			totalDistance ++;
			yield return new WaitForSeconds(1.0f);

		}
	}

	IEnumerator ManagePlayerGoodies(){
		while(true){
			if(gameOver){
				yield break;
			}
			playerGoodies = playerController.GetGoodies();
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator Restart(){
		while(true){
			if(Input.GetKey(KeyCode.R)){
				Application.LoadLevel (Application.loadedLevel);
				yield break;
			}
			yield return new WaitForEndOfFrame();
		}
	}

	void Update () {
		if(!gameOver){
			SetGoodieText ();
			SetDistanceText();
		}
		else if(!restart){
			StopAllCoroutines();
			gameoverText.text = "Game Over!\nPress 'R' to restart!";
			playerController.Die();
			woolballController.Die();
			foreach(KeyValuePair<int, GameObject> spawnedObject in spawnedObjects){
				if(spawnedObject.Value != null){
					spawnedObject.Value.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
					if(spawnedObject.Value.CompareTag("Obstacle")){
						spawnedObject.Value.GetComponent<Animator>().enabled = false;
					}
				}
			}
			restart = true;
			StartCoroutine(Restart());
		}
	}

	private void SetSpeed(float speed){
		currentSpeed = speed;
	}
	
	private void SetDistanceText(){
		distanceText.text = "Distance: " + totalDistance.ToString("#.##");
	}
	
	private void SetGoodieText(){
		goodieText.text = "Score: " + playerGoodies;
	}
}
