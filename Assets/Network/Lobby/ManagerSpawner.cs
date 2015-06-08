using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class ManagerSpawner : MonoBehaviour {
	
	static bool doesExist = false;
	private bool isTheRealOne = false;
	
	public GameObject lobbyManagerPrefab;
	public GameObject playerManagerPrefab;
	public GameObject chatPrefab;
	
	public string currentScene;
	
	void Awake () {
		if(doesExist == false){
			doesExist = true;
			isTheRealOne = true;
			DontDestroyOnLoad(this.gameObject);
		}
		else if(doesExist == true){
			Destroy (this.gameObject);
		}
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	void OnLevelWasLoaded (int levelIndex) {
		currentScene = Application.loadedLevelName;
		
		// check if this is the real instance
		if(isTheRealOne == true){
		// check for the menu
		if(currentScene == "Menu"){
			// destroy all the persistent lobby objects
			Destroy (LobbyManager.singleton.gameObject);
			Destroy (PlayerManager.singleton.gameObject);
			Destroy (NetworkChat.singleton.gameObject);
			// finally, destroy this, because it is not necessary in the menu
			doesExist = false;
			Destroy (this.gameObject);
		}
		else{
			// check to see if any of the objects exist
			if(LobbyManager.singleton == null){
				Instantiate(lobbyManagerPrefab);
				
			}
			if(PlayerManager.singleton == null){
				Instantiate(playerManagerPrefab);
				
			}
			if(NetworkChat.singleton == null){
				Instantiate (chatPrefab);
			}
		}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
