using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class LobbyManager : MonoBehaviour {
		
	public static LobbyManager singleton = null;
	
	public bool isLobby = false;
	public bool canJoinGame = false;
	
	// game info struct
	public GameInfo gameInfo;
	public ServerStatus serverStatus;
	public NetworkPlayer serverNetPlayer;
	public NetworkView netView;
	
	// get game info IEnumerator variables
	WaitForEndOfFrame waitFrame = new WaitForEndOfFrame();
	bool hasGameInfo = false;
	
	// gui variables
	int screenWidth = 0;
	int screenHeight = 0;
	int midScreenX = 0;
	int midScreenY = 0;
	GUIObject[] lobbyGUIRects;
	
	void Awake () {
		singleton = this;
		DontDestroyOnLoad(this);
		FirstTimeLobbyLoaded();
	}
	
	// Use this for initialization
	void Start () {

	}
	
	void OnLevelWasLoaded () {
		Network.isMessageQueueRunning = true;
		// check for the lobby scene
		if(Application.loadedLevelName == "Lobby"){
			isLobby = true;
		}
		else{
			isLobby = false;
		}
	}
	
	
	// called on the server, by the server, when the game is about to start
	void LoadGameLevel () {
		netView.RPC("JoinGame",RPCMode.All);
		UpdateAllServerStatus();
	}
	
	void UpdateAllServerStatus () {
		netView.RPC ("UpdateServerStatus",RPCMode.All,(int)serverStatus);
	}
	
	[RPC]
	void UpdateServerStatus (int svStatus) {
		serverStatus = (ServerStatus)svStatus;
	}
	
	[RPC]
	public void JoinGame () {
		if(canJoinGame == true){
		Network.isMessageQueueRunning = false;
			Application.LoadLevel("Game");
		}
	}
	
	// called from Awake
	void FirstTimeLobbyLoaded () {
		// try my thought
		
		// stop recieving data on the game channel
		// since we are just loading into the lobby
		
		isLobby = true;
		netView = networkView;
		// only get the game data if im the server
		if(Network.isServer == false){
			// set local variable with servers network player on it, so i can send stuff to the server specifically
			serverNetPlayer = Network.connections[0];
			
			// stop recieving data from the server on the game channel (Ch1)
			Network.SetReceivingEnabled(serverNetPlayer,1,false);
		
			StartCoroutine(GetServerInfo());
		}
		SetupGUIRects();
		
	}
	
	void SetupGUIRects () {
		// the gui vars
		screenWidth = Screen.width;
		screenHeight = Screen.height;
		midScreenX = screenWidth/2;
		midScreenY = screenHeight/2;
		lobbyGUIRects = new GUIObject[12];
		
		// the disconnect button
		lobbyGUIRects[0].rect = new Rect(5,5,100,25);
		lobbyGUIRects[0].text = string.Empty;
		
	}
	
	public IEnumerator GetServerInfo () {
		// send request for game info
		hasGameInfo = false;
		netView.RPC ("REQ_GetGameInfo",serverNetPlayer);
		// wait for the game info to return
		while(hasGameInfo == false){
			yield return waitFrame;
		}
		
		// get all the player data
		// start the coroutine on the player manager
		PlayerManager.singleton.SetupPlayerList();
	}
	// <<<< GET GAME INFO MESSAGES >>>>
	# region GetGameInfo
	// called on the server
	[RPC]
	void REQ_GetGameInfo (NetworkMessageInfo info) {
		netView.RPC("REC_GetGameInfo",info.sender,(int)serverStatus,gameInfo.mapIndex,gameInfo.maxPlayers, (int)gameInfo.gameType);
	}
	
	// called on the client by the server
	[RPC]
	void REC_GetGameInfo (int svStatus, int mpIndx,int mxPl, int gmTyp) {
		serverStatus = (ServerStatus)svStatus;
		gameInfo.mapIndex = mpIndx;
		// TODO
		// get the other map info based on the index
		// map name
		// game type
		gameInfo.maxPlayers = mxPl;
		gameInfo.gameType = (GameType)gmTyp;
		
		// now we have the game info
		hasGameInfo = true;
	}
	#endregion
	
	void LoadLobbyFromGame () {
		// stop the network  messages from being sent
		Network.isMessageQueueRunning = false;
		Application.LoadLevel("Lobby");
	}
	
	// called on the server when a new player connects
	
	/*	THOUGHT HERE:
		A thought... what if the client just stops recieving on channel 2...
		will that stop all the channel 2 messages being recieved?
		
		hence all the other players dont have to stop sending them...
		or will that lead to some problems with network views not existing...
		
		i guess try it
		
		pause:
		
		i checked the function... i have to stop recieving from a specific player
		
		i thought this might work since the client only has 1 connection....
		but that means i either have to call it for each player who we dont know about yet... shit
		or if i call it on the server, will the network updates from other clients
		be ignored since they come via the server too?
		
		TEST RESULTS:
		yes, this does work. by stopping reception from the server, it does indeed stop stuff form other clients
		this makes it really easy :)
	*/
	void OnPlayerConnected (NetworkPlayer newPlayer){
		
	}
	
	// DOESNT WORK
	void OnConnectedToServer () {
		// try my thought above
		
		// stop recieving data on the game channel
		// since we are just loading into the lobby
		// set local variable with servers network player on it, so i can send stuff to the server specifically
		serverNetPlayer = Network.connections[0];
		// stop recieving data from the server on the game channel (Ch1)
		Network.SetReceivingEnabled(serverNetPlayer,1,false);		
		
	}
	
	void OnGUI () {
	
		// disconnect/close server button
		if(isLobby == true){
		if(Network.isServer == true){
			if(GUI.Button(lobbyGUIRects[0].rect,"Close Server")){
				// close the server
				CloseServer();
			}
		}
		else if(Network.isClient == true){
			if(GUI.Button(lobbyGUIRects[0].rect, "Disconnect")){
				// disconnect
				DisconnectFromServer();
			}
		}
		}
	}
	
	void OnDisconnectedFromServer (NetworkDisconnection dc){
		if(Application.isLoadingLevel == false && Application.loadedLevelName != "Menu"){
			Application.LoadLevel("Menu");
		}
	}	
	
	void CloseServer () {
		// unregister us from the master server
		MasterServer.UnregisterHost();
		// close server RPC
		Network.Disconnect(200);
		Application.LoadLevel("Menu");
	}
	
	
	void DisconnectFromServer () {
		Network.Disconnect(200);
		Application.LoadLevel("Menu");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
