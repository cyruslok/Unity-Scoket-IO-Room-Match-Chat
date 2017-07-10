using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public SocketJoinRoomsManager socketManager;

	public Transform RoomsListContent;
	public Transform MessageListContent;

	public GameObject roomListItem;
	public GameObject msgItem;
	public GameObject renamePlane;
	public GameObject startPlane;

	public ScrollRect scroll_view_message;

	public Text text_CurrentRoomName;
	public Text input_text;
	public Text input_rename_text;
	public Text input_host_address_text;

	public Button btn_send;
	public Button btn_AddRoom;
	public Button btn_rename;
	public Button btn_rename_ok;
	public Button btn_rename_cancel;
	public Button btn_start_connection;
	public Button btn_gen_message;

	void Start(){
		renamePlane.SetActive (false);
		btn_AddRoom.onClick.AddListener (() => BtnAddRoomOnClick ());
		btn_send.onClick.AddListener (() => BtnSendMessage ());
		btn_rename.onClick.AddListener ( () => changeRenameActive ());
		btn_rename_cancel.onClick.AddListener (() => changeRenameActive ());
		btn_rename_ok.onClick.AddListener (() => RenameOkOnClick ());
		btn_start_connection.onClick.AddListener (() => BtnStartConnectionOnClick ());
		btn_gen_message.onClick.AddListener (() => BtnGenMessageOnClick ());
	}


	public void addRoomsListContent(string room_name){
		GameObject item = (GameObject)Instantiate (roomListItem, Vector3.zero,Quaternion.identity, RoomsListContent);
		item.transform.Find ("text_room_name").GetComponent<Text>().text = room_name;
		item.GetComponent<Button> ().onClick.AddListener (() => RoomItemOnClick (room_name));
	}
	public void deleteRoomListContent(){
		for (int i = 0; i < RoomsListContent.childCount; i++) {
			Destroy (RoomsListContent.GetChild (i).gameObject);
		}
	}

	public void addMessageListContent(string msg){
		GameObject item = (GameObject)Instantiate (msgItem, Vector3.zero,Quaternion.identity, MessageListContent);
		item.GetComponent<Text> ().text = msg;
		scroll_view_message.normalizedPosition = new Vector2(0, 0);
		if (MessageListContent.childCount > 100) {
			deleteMessageListContent ();
		}
	}
	public void deleteMessageListContent(){
		for (int i = 0; i < MessageListContent.childCount; i++) {
			Destroy (MessageListContent.GetChild (i).gameObject);
		}
	}

	public void changeRenameActive(){
		renamePlane.SetActive (!renamePlane.activeSelf);
	}
	public void RenameOkOnClick(){
		changeRenameActive ();
		socketManager.emit ("rename", input_rename_text.text );
	}

	public void RoomItemOnClick(string room_name){
		deleteMessageListContent ();
		socketManager.emit ("switchRoom", room_name);
	}

	public void setCurrentRoomName(string name){
		text_CurrentRoomName.text = name;
	}


	void BtnAddRoomOnClick(){
		deleteRoomListContent ();
		socketManager.emit ("create", "Room_"+RoomsListContent.childCount.ToString());
	}
	void BtnSendMessage(){
		if (input_text.text != "") {
			socketManager.emit ("sendchat", input_text.text );
		}
	}

	void BtnStartConnectionOnClick(){
		socketManager.startConnection (input_host_address_text.text);
		//startPlane.SetActive (false);
	}

	void BtnGenMessageOnClick(){
		socketManager.isGenMessage = !socketManager.isGenMessage;
	}


}


