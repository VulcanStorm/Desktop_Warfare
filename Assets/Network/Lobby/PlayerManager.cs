using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class PlayerManager : MonoBehaviour {
	
	public static PlayerManager singleton;
	
	public PlayerInfo[] playerList;
	
	NetworkPlayer serverNetPlayer;
	NetworkView netView;
	
	bool isLobby = false;
	
	// get player info vars
	int totalPlayerLobbyDataRecieved = 0;
	int totalPlayerGameDataRecieved = 0;
	WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
	
	// gui variables
	int screenWidth = 0;
	int screenHeight = 0;
	int midScreenX = 0;
	int midScreenY = 0;
	GUIObject[] lobbyGUIRects;
	
	void Awake () {
		singleton = this;
		DontDestroyOnLoad(this);
		FirstTimePlayerListLoaded();
		CreateGUIRects();
	}
	
	void CreateGUIRects (){
		// the gui vars
		screenWidth = Screen.width;
		screenHeight = Screen.height;
		midScreenX = screenWidth/2;
		midScreenY = screenHeight/2;
		lobbyGUIRects = new GUIObject[12];
		
		// join game button
		lobbyGUIRects[0].rect = new Rect(midScreenX-50,screenHeight-100,100,25);
		lobbyGUIRects[0].text = "Join Game";
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	void OnLevelWasLoaded () {
		// check for the lobby scene
		if(Application.loadedLevelName == "Lobby"){
			isLobby = true;
		}
		else{
			isLobby = false;
		}
	}
	
	void FirstTimePlayerListLoaded () {
		isLobby = true;
		netView = networkView;
		
		
		if(Network.isServer == false){
		// set local variable with servers network player on it, so i can send stuff to the server specifically
		serverNetPlayer = Network.connections[0];
		}
		// TODO
		// change this later
		else{
			// we are the server
			LobbyManager.singleton.canJoinGame = true;
		}
		
		
	}
	
	// called when we get the server info from the lobby
	public void SetupPlayerList () {
	// create a new player list
		playerList = new PlayerInfo[LobbyManager.singleton.gameInfo.maxPlayers];
		// fill all the player list slots with empty data
		for(int i=0;i<playerList.Length;i++){
			playerList[i] = new PlayerInfo();
		}
		// since we have a full player list
		StartCoroutine(GetPlayerLobbyInfo());
	}
	
	
	
	IEnumerator GetPlayerLobbyInfo () {
		
		NetworkChat.NewLocalMsg("PM: Getting Player Lobby Info");
		
		for(int i=0;i<playerList.Length;i++){
			totalPlayerLobbyDataRecieved = 0;
			NetworkChat.NewLocalMsg("PM: GettingPlayerLobbyInfo");
			netView.RPC ("REQ_AllPlayerData",serverNetPlayer,i);
			
			// data is sent in multiple updates, not 1 to save bandwidth
			// so wait until all the updates have arrived
			// this will stop stuff happening out of order
			while(totalPlayerLobbyDataRecieved < 4){
				yield return waitFrame;
			}
			NetworkChat.NewLocalMsg("PM: Got all lobby data for player "+i);
			
		}
		
		// now we can join the game
		LobbyManager.singleton.canJoinGame = true;
		
	}
	
	// sent to the server to get all of the player info
	// only called when first entered the lobby
	// server only
	[RPC]
	void REQ_AllPlayerData (int plIndex, NetworkMessageInfo info) {
		// send the data back to the player
		netView.RPC ("REC_PlayerData", info.sender,plIndex,"<<<<PLAYER DATA HERE>>>>");
		netView.RPC ("REC_VehicleData", info.sender,plIndex,"<<<<PLAYER DATA HERE>>>>");
		netView.RPC ("REC_LoadoutData", info.sender,plIndex,"<<<<PLAYER DATA HERE>>>>");
		netView.RPC ("REC_ArmourData", info.sender,plIndex,"<<<<PLAYER DATA HERE>>>>");
	}
	// <<<< CLIENT DATA RPC REGION >>>>
	#region client data rpc
	// when recieving data
	// client only, from server
	[RPC]
	void REC_PlayerData (int plIndex, string plData) {
		// add 1 to the amount of player data recieved
		totalPlayerLobbyDataRecieved ++;
		// set player data
		NetworkChat.NewLocalMsg("PM: Recieved Player Data for player "+plIndex);
	}
	
	[RPC]
	void REC_VehicleData (int plIndex, string plData) {
		// add 1 to the amount of player data recieved
		totalPlayerLobbyDataRecieved ++;
		// set vehicle data
		NetworkChat.NewLocalMsg("PM: Recieved Vehicle Data for player "+plIndex);
	}
	
	[RPC]
	void REC_LoadoutData (int plIndex, string plData) {
		// add 1 to the amount of player data recieved
		totalPlayerLobbyDataRecieved ++;
		// set loadout info
		NetworkChat.NewLocalMsg("PM: Recieved Loadout Data for player "+plIndex);
	}
	
	[RPC]
	void REC_ArmourData (int plIndex, string plData) {
		// add 1 to the amount of player data recieved
		totalPlayerLobbyDataRecieved ++;
		// set armour info
		NetworkChat.NewLocalMsg("PM: Recieved Armour Data for player "+plIndex);
	}
	#endregion
	
	// called from the lobby manager when 
	public void SetupPlayerGameInfo () {
		StartCoroutine(GetPlayerGameInfo());
	}
	
	IEnumerator GetPlayerGameInfo () {
		
		for(int i=0;i<playerList.Length;i++){
			totalPlayerGameDataRecieved = 0;
			netView.RPC ("REQ_AllPlayerGameData",serverNetPlayer,i);
			
			// data is sent in multiple updates, not 1 to save bandwidth
			// so wait until all the updates have arrived
			// this will stop stuff happening out of order
			while(totalPlayerGameDataRecieved < 4){
				yield return waitFrame;
			}
			
		}
	}
	
	// TODO fix this
	[RPC]
	void REC_AllPlayerGameData (int plIndex, NetworkMessageInfo info) {
		// send the data back to the player
		netView.RPC ("REC_PlayerData", info.sender,plIndex,"<<<<PLAYER DATA HERE>>>>");
		netView.RPC ("REC_VehicleData", info.sender,plIndex,"<<<<PLAYER DATA HERE>>>>");
		netView.RPC ("REC_LoadoutData", info.sender,plIndex,"<<<<PLAYER DATA HERE>>>>");
		netView.RPC ("REC_ArmourData", info.sender,plIndex,"<<<<PLAYER DATA HERE>>>>");
	}
	
	void OnGUI () {
		// specific lobby gui
		if(isLobby == true){
			// the join game button
			if(LobbyManager.singleton.canJoinGame == true){
				if(GUI.Button(lobbyGUIRects[0].rect,lobbyGUIRects[0].text)){
					// join game code
					LobbyManager.singleton.JoinGame();
				}
			}
			else{
				GUI.Box(lobbyGUIRects[0].rect,lobbyGUIRects[0].text);
			}
		}
	}
	
}
