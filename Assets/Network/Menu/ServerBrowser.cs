using UnityEngine;
using System.Collections;

public struct GUIObject
{
		public Rect rect;
		public string text;
}

public struct ServerInfo
{
	
		public string mapName;
		public Ping ping;
	
}


public class ServerBrowser : MonoBehaviour {


	public bool showServerBrowser = false;

	HostData[] serverList = new HostData[0];

	ServerInfo[] extraServerInfo = new ServerInfo[0];

	// GUI variables
	GUIObject[] guiObjs;
	Rect[] serverListRects;
	Vector2 scrollPos = new Vector2();
	Rect scrollRect, scrollViewRect;
	Rect gameNameRect, gameTypeRect, plCntRect, mapRect, pingRect, isNatRect, conBtnRect;
	string mapName, pingStr;
	string serverName = "Un-named Server";
	bool useNat = true;

	void Awake (){
			SetGUIData ();
	}

	void SetGUIData (){

		// the scroll box data
		scrollPos = new Vector2();
		scrollRect = new Rect(210, 50, 730, 420);
		scrollViewRect = new Rect(210, 50, 700, 999);
		
		
		// the server list data;
		// the base object rects for each entry
		serverListRects = new Rect[7];
		// name label
		serverListRects[0] = new Rect(210, 15, 200, 30);
		// game type label
		serverListRects[1] = new Rect(410, 15, 100, 30);
		// players amount label
		serverListRects[2] = new Rect(510, 15, 100, 30);
		// map label
		serverListRects[3] = new Rect(610, 15, 100, 30);
		// ping label
		serverListRects[4] = new Rect(710, 15, 60, 30);
		// nat label
		serverListRects[5] = new Rect(770,15,60,30);
		// connect button
		serverListRects[6] = new Rect(830,15,100,30);

		guiObjs = new GUIObject[12];
		// refresh button
		guiObjs[0].rect = new Rect (10, 10, 100, 30);
		guiObjs[0].text = "Refresh";
		// background box
		guiObjs[1].rect = new Rect (200, 10, 750, 480);
		guiObjs[1].text = string.Empty;
		
	}
	
	
	// Use this for initialization
	void Start (){
			// Default unity address
			MasterServer.ipAddress = "67.225.180.24";
			Debug.Log (MasterServer.ipAddress);
			Debug.Log (MasterServer.port);
	}

	// Update is called once per frame
	void Update (){
		if(Network.peerType == NetworkPeerType.Disconnected){
		showServerBrowser = true;
		}
		else{
		//showServerBrowser = false;
		}
	}



	void OnGUI (){
		useGUILayout = false;
		if (showServerBrowser == true) {
	
			// refresh server list
			if (GUI.Button (guiObjs[0].rect, guiObjs[0].text)) {
					GetServerList ();
			}
			
			serverName = GUI.TextField(new Rect(10, 40, 180, 30), serverName,20);
			useNat = GUI.Toggle(new Rect(10,110,100,30),useNat,"UseNAT?");
			
			if(Network.isClient == false){
				if (GUI.Button (new Rect(10, 70, 100, 30), "Host Server")) {
					StartCoroutine(RegisterTestServer ());
				}
			}
			
			
			// <<<<---------- display server list ---------->
			// the background box
			GUI.Box (guiObjs[1].rect, guiObjs[1].text);
			
			// the top bar
			GUI.Box (serverListRects[0], "Game Name");
			GUI.Box (serverListRects[1], "Game Type");
			GUI.Box (serverListRects[2], "Players");
			GUI.Box (serverListRects[3], "IP Address");
			GUI.Box (serverListRects[4], "Ping");
			GUI.Box (serverListRects[5], "NAT?");
			
			// the scroll view
			scrollRect.height = (serverList.Length)*30;
			scrollPos = GUI.BeginScrollView (scrollRect, scrollPos, scrollViewRect);
			string plCnt;
			
			for (int i=0; i<serverList.Length; i++) {
				int rectYPos = (30 * i)+50;
				
				gameNameRect = serverListRects[0];
				gameNameRect.y = rectYPos;
	
				gameTypeRect = serverListRects[1];
				gameTypeRect.y = rectYPos;
	
				plCntRect = serverListRects[2];
				plCntRect.y = rectYPos;
				plCnt = serverList[i].connectedPlayers + "/" + serverList [i].playerLimit;
	
				mapRect = serverListRects[3];
				mapRect.y = rectYPos;
				mapName = extraServerInfo[i].mapName;
	
				pingRect = serverListRects[4];
				pingRect.y = rectYPos;
		
				if (extraServerInfo[i].ping.isDone == true) {
					pingStr = extraServerInfo[i].ping.time.ToString () + "ms";
				} else {
					pingStr = "pinging...";
				}
				
				isNatRect = serverListRects[5];
				isNatRect.y = rectYPos;
				
				conBtnRect = serverListRects[6];
				conBtnRect.y = rectYPos;
				
				GUI.Box (gameNameRect, serverList[i].gameName);
				GUI.Box (gameTypeRect, serverList[i].gameType);
				GUI.Box (plCntRect, plCnt);
				GUI.Box (mapRect, serverList[i].ip[0]);
				GUI.Box (pingRect, pingStr);
				//print ("serverList[i] useNat is "+serverList[i].useNat.ToString());
				GUI.Box (isNatRect, serverList[i].useNat.ToString());
				if(GUI.Button (conBtnRect,"Connect")){
					//Network.Connect(serverList[i]);
					ConnectToServer(serverList[i]);
				}
	
			}
			GUI.EndScrollView ();
	
	
		}
		else{
		if(Network.isServer == true){
			if (GUI.Button (new Rect(10, 70, 100, 30), "Stop Server")) {
				StartCoroutine(UnregisterTestServer());
			}
		}
		else if(Network.isClient == true){
			if (GUI.Button (new Rect(10, 70, 100, 30), "Disconnect")) {
				DisconnectFromServer();
			}
		}
		}
	}		
	
	
	
	void OnMasterServerEvent (MasterServerEvent msEvent){
	
		if (msEvent == MasterServerEvent.HostListReceived) {
			serverList = MasterServer.PollHostList ();
			extraServerInfo = new ServerInfo[serverList.Length];
			GetPingFromServers ();
		}
	}
	
	void GetPingFromServers (){
	
		for (int i=0; i<extraServerInfo.Length; i++) {
			extraServerInfo [i].ping = new Ping (serverList [i].ip.ToString ());
			print (serverList [i].ip.ToString ());
		}
	
	}

	void GetServerList (){
		MasterServer.ClearHostList ();
		MasterServer.RequestHostList ("DesktopWarfare");

	}
	
	void ConnectToServer (HostData data){
		Network.Connect(data);
	}
	
	void DisconnectFromServer (){
		Network.Disconnect();
	}
	
	IEnumerator UnregisterTestServer () {
		MasterServer.UnregisterHost();
		Network.Disconnect();
		yield return new WaitForSeconds(1);
		GetServerList ();
	}
	
	IEnumerator RegisterTestServer (){
	if(Network.isServer == true){
		MasterServer.UnregisterHost();
		Network.Disconnect();
		}
		yield return new WaitForSeconds(1);
		Network.InitializeServer (32, 25002, useNat);
		MasterServer.RegisterHost ("DesktopWarfare",serverName, "Mmm... Comments...");
			yield return new WaitForSeconds(1);
			//GetServerList ();
			
		NetConnect.LoadLobby();
	}
	
	void OnFailedToConnectToMasterServer (NetworkConnectionError info){
		Debug.Log (info);
	}
	
	void OnFailedToConnect (NetworkConnectionError info){
		Debug.Log (info);
	}
	
	



}
