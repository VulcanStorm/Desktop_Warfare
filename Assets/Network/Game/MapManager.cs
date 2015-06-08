using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour {
	
	public static MapManager singleton;
	
	public GameObject mapPrefab;
	public string[] maps;
	
	public string curMapId;
	public GameObject currentMapObj;
	string mapPath;
	
	
	void Awake () {
		singleton = this;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void LoadMap (int mapIndex) {
		// get the map name from the index
		curMapId = maps[mapIndex];
		// create the map path
		mapPath = "Maps/" + curMapId;
		mapPrefab = Resources.Load<GameObject>(mapPath);
		
		// now create the map
		currentMapObj = (GameObject)Instantiate (mapPrefab,Vector3.zero,Quaternion.identity);
		// tell the game manager that we have loaded the map
		GameManager.singleton.mapLoaded = true;
	}
}
