using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager singleton;
	public GameInfo gameInfo;
	
	[HideInInspector]
	public bool mapLoaded = false;

	YieldInstruction waitFrame = new WaitForEndOfFrame();
	// Use this for initialization
	void Awake () {
		singleton = this;
	}
	
	void Start () {
		LoadMap();
		
	}
	
	void LoadMap () {
		// we havent loaded the map yet
		mapLoaded = false;
		// get the game info
		gameInfo = LobbyManager.singleton.gameInfo;
		// load the required map
		MapManager.singleton.LoadMap(gameInfo.mapIndex);
		StartCoroutine(SetupGame());
	}
	
	IEnumerator SetupGame () {
		// wait for the map to finish loading
		while(mapLoaded == false){
			yield return waitFrame;
		}
		
		// tell the player manager to get the required player data for the game
			// the parasite objects, and tell it to build them
			PlayerManager.singleton.SetupPlayerGameInfo();
			
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
}
