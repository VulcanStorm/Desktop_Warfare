﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkChat : MonoBehaviour {
	
	public static NetworkChat singleton;
	
	public bool showChat = false;
	public bool hasFocus = false;
	public int maxMessages = 15;
	public List<string> msgList = new List<string>();
	
	GUIObject chatBoxRect,sendBtnRect;
	
	Rect chatMsgRect,msgRect, chatInputRect;
	Rect scrollRect,scrollViewRect;
	Vector2 scrollPos, scrollPosition;
	string inputMsg = "";
	string msgToSend;
	string currentFocus = "";
	string plName = "Unnamed";
	Rect nameRect, nameLabelRect;
	GUIStyle guiStyle;
	bool hasStyle = false;
	bool hasNewMsg = false;
	int totalMsgHeight = 0;
	
	void Awake () {
		SetGUIObjectRects();
		singleton = this;
		DontDestroyOnLoad(this);
	}
	
	void Update () {
		if(Network.peerType != NetworkPeerType.Disconnected){
			showChat = true;
		}
		else{
			showChat = false;
		}
	}
	
	void SetGUIObjectRects () {
		
		// background box
		chatBoxRect.rect = new Rect(5,195,315,225);
		chatBoxRect.text = string.Empty;
		
		// input box
		chatInputRect = new Rect(10,385,250,30);
		
		// send button
		sendBtnRect.rect = new Rect(260,385,50,30);
		sendBtnRect.text = "Send";
		
		// scroll rects
		scrollRect = new Rect(10, 200, 300, 180);
		scrollViewRect = new Rect(10, 190, 280, 300);
		
		// base rect for chat messages
		chatMsgRect = new Rect(10,0,280,20);
		
		// input name rect
		nameLabelRect = new Rect(5,170,35,25);
		nameRect = new Rect(40,170,150,25);
		
	}
	
	
	[RPC]
	void NewMsg (string msg){
	if(msgList.Count == maxMessages){
		msgList.RemoveAt(0);
		}
		msgList.Add(msg);
		hasNewMsg = true;
	}
	
	public static void NewLocalMsg (string localMsg){
		singleton.NewMsg (localMsg);
	}
	
	
	void OnGUI () {
		if(hasStyle == false){
			guiStyle = GUI.skin.button.name;
			guiStyle.wordWrap = true;
			hasStyle = true;
		}
		
		if(showChat == true){
			
			
			// name input box
			GUI.Label(nameLabelRect,"Name:");
			plName = GUI.TextField(nameRect,plName,15);
			
			// background box
			GUI.Box(chatBoxRect.rect, chatBoxRect.text);
			
			
			// start the scroll view
			scrollViewRect.height = totalMsgHeight;
			scrollPos = GUI.BeginScrollView(scrollRect, scrollPos, scrollViewRect);
			
			int compoundYPos = 190;
			totalMsgHeight = 0;
			GUIContent gc = new GUIContent();
			
			for(int i=0;i<msgList.Count;i++){
				
				gc.text = msgList[i];
				msgRect = chatMsgRect;
				
				// calculate height needed
				int rectHeight = (int)guiStyle.CalcHeight(gc,msgRect.width);
				// set the height to the one calculated
				msgRect.height = rectHeight;
				// set the yPos to the end of the last msg
				msgRect.y = compoundYPos;
				
				// draw the label
				GUI.Label(msgRect,msgList[i]);
				
				// add the height of the last message to the y position
				compoundYPos += rectHeight;
				// add the height of the last message to the total height of the message stack
				totalMsgHeight += rectHeight;
				
				
			}
			// stop the scroll view
			GUI.EndScrollView();
			
			// TODO only scroll to the new message if
			// TODO we are looking at the bottom of the list
			
			// check if a new message has been received
			// if it has, scroll to that message
			if(hasNewMsg == true){
				scrollPos.y = totalMsgHeight;
				hasNewMsg = false;
			}
			
			GUI.SetNextControlName("ChatInputBox");
			
			inputMsg = GUI.TextField(chatInputRect,inputMsg);
			
			// check for sending
			// the default send button
			if(GUI.Button(sendBtnRect.rect,sendBtnRect.text)){
				SendChatMsg();
			}
			
			currentFocus = GUI.GetNameOfFocusedControl();
			
			if(currentFocus.Equals("ChatInputBox",System.StringComparison.OrdinalIgnoreCase) == true){
				if(Event.current.isKey == true){
					if(Event.current.keyCode == KeyCode.Return){
						SendChatMsg ();
					}
				}
			}
			
			
			
		}	
	}
	
	void SendChatMsg () {
		// calculate msg
		msgToSend = plName+": "+inputMsg;
		
		if(Network.connections.Length >0){
		networkView.RPC("NewMsg",RPCMode.All,msgToSend);
		}
		else{
		NewMsg (msgToSend);
		}
		inputMsg = "";
	}
	
	
}
