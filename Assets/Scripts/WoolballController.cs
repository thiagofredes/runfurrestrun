using UnityEngine;
using System.Collections;

public class WoolballController : MonoBehaviour {

	private Coroutine moveCoroutine = null;
	private Coroutine oscilateCoroutine = null;
	private float angle = 0.0f;
	

	IEnumerator Oscilate(){
		while(true){
			angle += 5f;
			gameObject.transform.Translate (0.0f, Time.deltaTime*Mathf.Sin (Mathf.Deg2Rad*angle), 0.0f);
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator Move(float x){
		while(gameObject.transform.position.x != x){
			gameObject.transform.Translate(Time.deltaTime * (x - gameObject.transform.position.x), 0.0f, 0.0f);
			yield return new WaitForEndOfFrame();
		}
	}

	public void SetMove(float x){
		if(moveCoroutine != null){
			StopCoroutine(moveCoroutine);
		}
		if(oscilateCoroutine == null){
			oscilateCoroutine = StartCoroutine(Oscilate());
		}
		moveCoroutine = StartCoroutine(Move(x));
	}

	public void Die(){
		StopAllCoroutines();
		gameObject.GetComponent<Animator>().enabled = false;
	}
}
