using UnityEngine;
using System.Collections;

public class Start : MonoBehaviour {

	private AudioSource select;

	void Awake(){
		select = gameObject.GetComponent<AudioSource>();
	}

	public void LoadScene(int scene){
		select.Play();
		Application.LoadLevel (scene);
	}

	public void ExitGame(){
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif 
	}

}
