using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	public static NetworkManager _instance;

	private void Awake()
	{
		// if an instance already exists and its not this one destroy this
		if( _instance != null && _instance != this )
		{
			gameObject.SetActive( false );
		}
		else
		{
			_instance = this;
			DontDestroyOnLoad( gameObject );
		}
	}

	private void Start()
	{
		PhotonNetwork.ConnectUsingSettings();

	}

	public void CreateRoom (string roomName)
	{
		PhotonNetwork.CreateRoom( roomName );
	}

	public void JoinRoom (string roomName)
	{
		PhotonNetwork.JoinRoom( roomName );
	}

	[PunRPC]
	public void ChangeScene( string sceneName)
	{
		PhotonNetwork.LoadLevel( sceneName );
	}
}
