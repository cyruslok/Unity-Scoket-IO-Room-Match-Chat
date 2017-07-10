using System.Collections;
using SocketIO;
using UnityEngine;

public class SocketJoinRoomsManager : MonoBehaviour {
	
	public UIManager ui_manager;
	[HideInInspector]
	public bool isGenMessage;

	private SocketIOComponent socket;
	private string sid;
	private int count = 0;
	private bool isInitFinish = false;

	// Use this for initialization
	void Start () {
		socket = GameObject.Find ("SocketIO").GetComponent<SocketIOComponent>();
		socket.On("open", OnSocketOpen);
		socket.On ("connect", OnSocketConnect);
		socket.On ("updatechat", OnUpdateChat);
		socket.On ("updaterooms", OnUpdateRooms);
		socket.On ("switchRooms", OnSwitchRooms);
		socket.On ("UpdateRoomState", OnUpdateRoomState);
	}
	void Update(){
		if (isGenMessage) {
			emit ("sendchat", new Vector3(Random.Range(1f, 1000f),Random.Range(1f, 1000f),Random.Range(1f, 1000f)).ToString() );
		}
	}


	public void startConnection(string ip){
		string address = "ws://"+ip+":4567/socket.io/?EIO=4&transport=websocket"; 
		socket.url = address;
	}


	public void emit(string ev , string msg){
		socket.Emit (ev, JSONObject.StringObject(msg));
	}

	public void OnSocketConnect(SocketIOEvent e){
		if (!isInitFinish) {
			sid = socket.sid;
			Debug.Log ("On Socket Connect >>>> ID : " + sid);
			socket.Emit ("userInit", JSONObject.StringObject(sid.Substring(0,5)));
			isInitFinish = true;	
		}
	}

	public void OnSocketOpen(SocketIOEvent e){
		//Debug.Log ("On Socket Open ("+e.name+") >>>>> GET >>>>> "  + e.data);
	}

	public void OnSwitchRooms(SocketIOEvent e){
		if (e.data.GetField ("id").str == sid) {
			JSONObject room_inforamtion = e.data.GetField ("room_information");
			ui_manager.setCurrentRoomName (e.data.GetField ("current_room").str + " ( " + room_inforamtion.GetField ("length").n.ToString () + " )");
		} else {
			socket.Emit ("checkRoomState");
		}
	}

	public void OnUpdateRoomState(SocketIOEvent e){
		if (e.data.GetField ("id").str == sid) {
			JSONObject room_inforamtion = e.data.GetField ("room_information");
			ui_manager.setCurrentRoomName (e.data.GetField ("current_room").str + " ( " + room_inforamtion.GetField ("length").n.ToString () + " )");
		}
	}

	public void OnUpdateRooms(SocketIOEvent e){
		//Debug.Log ("On Update Rooms ("+e.name+") >>>>> GET >>>>> "  + e.data);
		ui_manager.deleteRoomListContent ();
		JSONObject room_inforamtion = e.data.GetField ("room_information");
		ui_manager.setCurrentRoomName(e.data.GetField("current_room").str + " ( " + room_inforamtion.GetField("length").n.ToString() + " )");
		foreach (JSONObject item in e.data.GetField("rooms").list ) {
			ui_manager.addRoomsListContent (item.str);
		}
	}

	public void OnUpdateChat(SocketIOEvent e){
		Debug.Log ("On Update Chat ("+e.name+") >>>>> GET >>>>> "  + e.data);
		string id = e.data.GetField ("id").str;
		string msg = e.data.GetField ("msg").str;
		string username = e.data.GetField ("username").str;
		if (id != sid || username != "SERVER") {
			msg = username + ": " + msg;
		}
		ui_manager.addMessageListContent (msg);
		//Debug.Log (sent_by);
		//Debug.Log (msg);
	}

}
