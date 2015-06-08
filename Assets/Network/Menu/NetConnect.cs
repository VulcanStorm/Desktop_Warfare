using UnityEngine;
using System.Collections;

public class NetConnect : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	// when we join a server, load the lobby automatically
	void OnConnectedToServer () {
		LoadLobby();
	}
	
	public static void LoadLobby () {
		Application.LoadLevel("Lobby");
	}
}
