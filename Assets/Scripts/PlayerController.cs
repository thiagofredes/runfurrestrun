using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {


	public float[] lanesY;
	public Text[] goodiePointsTexts;
	public AudioClip jump;
	public AudioClip hit;
	public AudioClip meow;

	private Animator myanimator;
	private int currentLane;
	private bool jumping;
	private Vector2 jumpForce;
	private int collisions;
	private int goodieScore;
	private bool dead;
	private float runSpeed;
	private int goodiePoints;

	void Awake(){
		myanimator = gameObject.GetComponent<Animator>();
	}
	
	void Start () {
		runSpeed = 1.0f;
		jumping = false;
		collisions = 0;
		currentLane = 1;
		goodieScore = 0;
		goodiePoints = 100;
		dead = false;
		SetLayer(currentLane);
		myanimator.SetBool("walking", true);
	}

	IEnumerator Jump(){
		Vector3 s0 = gameObject.transform.position;
		float speed0 = 7.0f;
		float gravity = 18.0f;
		float y;
		float y0 = s0.y;
		float t = 0.0f;
		jumping = true;
		myanimator.SetTrigger("jump");
		do{
			t += Time.deltaTime;
			y = s0.y + speed0 * t - gravity * t * t/2;
			gameObject.transform.Translate (0.0f, y - y0, 0.0f);
			yield return new WaitForEndOfFrame();
			y0 = y;
		}while(gameObject.transform.position.y > s0.y);
		gameObject.transform.Translate(0.0f, s0.y - gameObject.transform.position.y, 0.0f);
		jumping = false;
		myanimator.SetTrigger("jump");
	}

	IEnumerator ShowPoints(int points){
		float totalTime = 1.0f;
		float catX = gameObject.transform.position.x;
		int initLane = currentLane;
		float y;
		goodiePointsTexts[initLane].text = "+100";
		while(totalTime > 0.0f){
			y = Mathf.Lerp (lanesY[initLane], lanesY[initLane] + 1.0f, 1.0f - totalTime);
			goodiePointsTexts[initLane].transform.position = Camera.main.WorldToScreenPoint(new Vector3(catX + 1.0f, y, 0.0f));
			yield return new WaitForEndOfFrame();
			totalTime -= 1.5f * Time.deltaTime;
		}
		goodiePointsTexts[initLane].text = "";
	}

	void Update(){
		if(!dead){
			if(!jumping){
				if(Input.GetKeyDown (KeyCode.Space)){
					if(!jumping){
						AudioSource.PlayClipAtPoint(jump, Camera.main.transform.position, 0.5f);
						StartCoroutine(Jump());
					}
				}
				else if(Input.GetKeyDown (KeyCode.UpArrow)){
					if(currentLane > 0 && !jumping){
						gameObject.transform.Translate(0.0f, lanesY[currentLane - 1] - gameObject.transform.position.y, 0.0f);
						currentLane --;
						SetLayer(currentLane);
					}
				}
				else if(Input.GetKeyDown (KeyCode.DownArrow)){
					if(currentLane < 2 && !jumping){
						gameObject.transform.Translate(0.0f, lanesY[currentLane + 1] - gameObject.transform.position.y, 0.0f);
						currentLane ++;
						SetLayer(currentLane);
					}
				}
			}
		}
	}

	void SetLayer(int layerNumber){
		gameObject.layer = LayerMask.NameToLayer("Lane " + layerNumber);
	}
	
	void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject.CompareTag("Obstacle")){
			collisions++;
			AudioSource.PlayClipAtPoint(hit, Camera.main.transform.position, 1f);
			Destroy(other.gameObject);
		}
		else if(other.gameObject.CompareTag("Goodie")){
			goodieScore+=100;
			StartCoroutine(ShowPoints(goodiePoints));
			AudioSource.PlayClipAtPoint(meow, Camera.main.transform.position, 1f);
			Destroy(other.gameObject);
		}
	}

	public bool isJumping(){
		return jumping;
	}

	public int GetCollisions(){
		return collisions;
	}

	public int GetGoodies(){
		return goodieScore;
	}

	public void IncreaseSpeed(float speed){
		runSpeed *= speed;
	}

	public void DecreaseSpeed(float speed){
		runSpeed /= speed;
	}

	public void Run(){
		if(myanimator.GetBool ("walking")){
			myanimator.SetBool("walking", false);
		}
		myanimator.SetFloat("speed multiplier", runSpeed);
	}

	public void Walk(){
		if(!myanimator.GetBool("walking")){
			myanimator.SetBool("walking", true);
		}
		myanimator.SetFloat("speed multiplier", runSpeed);
	}

	public void Die(){
		myanimator.enabled = false;
		dead = true;
	}
}