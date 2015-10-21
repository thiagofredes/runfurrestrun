using UnityEngine;
using System.Collections;

public class SimpleMover : MonoBehaviour {
	
	void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject.CompareTag("Boundary")){
			Destroy(gameObject);
		}
	}
	
	public void Move(float speed){
		GetComponent<Rigidbody2D>().velocity = Vector2.left * speed;
	}
}
