using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour {

	public static ObjectPool instance;

	public GameObject pooledObject;
	public int poolSize = 20;
	public bool willGrow = false;

	private List<GameObject> pooledObjects;

	void Awake(){
		instance = this;
	}

	void Start(){
		pooledObjects = new List<GameObject>();
		for(int i=0; i<poolSize; i++){
			GameObject obj = (GameObject)Instantiate(pooledObject);
			obj.SetActive(false);
			pooledObjects.Add (obj);
		}
	}

	public GameObject GetPooledObject(){
		//if any object is inactive, returns it
		for(int i=0; i<pooledObjects.Count; i++){
			if(!pooledObjects[i].activeInHierarchy){
				return pooledObjects[i];
			}
		}

		//otherwise, and the pool can grow, creates a new one
		if(willGrow){
			GameObject obj = (GameObject)Instantiate(pooledObject);
			pooledObjects.Add (obj);
			return obj;
		}

		//finally if the pool is empty and cannot grow, returns null
		return null;
	}

}
