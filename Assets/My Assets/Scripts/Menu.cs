using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks
{
	[Header( "Screens" )]
	public GameObject mainScreen;
	public GameObject lobbyScreen;

	[Header( "Main Screen" )]
	public Button createRoomButton;
	public Button joinRoomButton;

	[Header( "Lobby Screen" )]
	public TextMeshProUGUI playerListText;
	public Button startGameButton;

	private void Start()
	{
		createRoomButton.interactable = false;
		joinRoomButton.interactable = false;
	}

	public override void OnConnectedToMaster()
	{
		createRoomButton.interactable = true;
		joinRoomButton.interactable = true;
	}

	public void SetScreen (GameObject screen)
	{
		mainScreen.SetActive( false );
		mainScreen.SetActive( false );

		screen.SetActive( true );
	}

	public void OnCreateRoomButton(TMP_InputField roomNameInput)
	{
		NetworkManager._instance.CreateRoom( roomNameInput.text );
	}

	public void OnJoinRoomButton(TMP_InputField roomNameInput)
	{
		NetworkManager._instance.JoinRoom( roomNameInput.text );
	}
}
