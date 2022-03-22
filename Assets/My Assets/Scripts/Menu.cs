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
		lobbyScreen.SetActive( false );

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

	public void OnPlayerNameUpdate(TMP_InputField playerNameInput)
	{
		PhotonNetwork.NickName = playerNameInput.text;
	}

	public override void OnJoinedRoom()
	{
		SetScreen( lobbyScreen );

		photonView.RPC( "UpdateLobbyUI", RpcTarget.All );
	}

	public override void OnPlayerLeftRoom( Player otherPlayer )
	{
		UpdateLobbyUI();
	}

	[PunRPC]
	public void UpdateLobbyUI()
	{
		playerListText.text = "";

		//display all current players in lobby
		foreach(Player player in PhotonNetwork.PlayerList)
		{
			playerListText.text += player.NickName + "\n";
		}

		// only host can start game
		if(PhotonNetwork.IsMasterClient)
		{
			startGameButton.interactable = true;
		}
		else
		{
			startGameButton.interactable = false;
		}
	}

	public void OnLeaveLobbyButton()
	{
		PhotonNetwork.LeaveRoom();
		SetScreen( mainScreen );
	}

	public void OnStartGameButton()
	{
		NetworkManager._instance.photonView.RPC( "ChangeScene", RpcTarget.All, "Game" );
	}
}
