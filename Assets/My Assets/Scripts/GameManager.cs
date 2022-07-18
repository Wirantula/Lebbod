using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    public bool gameEnded = false;
    public int playerTurn = 0;
    public PlayerController[] players;
	public string playerPrefabLocation;
	public Transform spawnPoint;
	private int playersInGame;
	public bool startGame = true;

	public static GameManager instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		players = new PlayerController[PhotonNetwork.PlayerList.Length];
		photonView.RPC( "ImInGame", RpcTarget.AllBuffered );
	}

	private void Update()
	{
		//if( startGame ) 
		//{
		//	foreach( PlayerController player in players )
		//	{
		//		players[playerTurn].photonView.RPC( "OnTurnStart", RpcTarget.All, players[playerTurn].id );
		//	}
		//	startGame = false;
		//}
	}

	[PunRPC]
	void ImInGame()
	{
		playersInGame++;

		if( playersInGame == PhotonNetwork.PlayerList.Length )
		{
			SpawnPlayer();
		}
	}

	[PunRPC]
	void OnTurnEnded(int currentPlayer)
	{
		if( currentPlayer == 1 ) players[currentPlayer].photonView.RPC( "OnTurnStart", RpcTarget.All, players[currentPlayer].id );
		if( currentPlayer == 2 ) players[0].photonView.RPC( "OnTurnStart", RpcTarget.All, players[0].id );
	}

	void SpawnPlayer() 
	{
		//instantiate player across network
		GameObject playerObj = PhotonNetwork.Instantiate( playerPrefabLocation, spawnPoint.position, Quaternion.identity );
		//get the playerscript
		PlayerController playerScript = playerObj.GetComponent<PlayerController>();

		//initialize player
		playerScript.photonView.RPC( "Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer );
		foreach( PlayerController item in players )
		{
			playerScript.photonView.RPC( "InitializeUnits", RpcTarget.All );
		}
	}

	public PlayerController GetPlayer(int playerId)
	{
		PlayerController playerToGet = players.First( x => x.id == playerId );
		return playerToGet;
	}

	public PlayerController GetPlayer(GameObject playerObj)
	{
		PlayerController playerToGet = players.First( x => x.gameObject == playerObj );
		return playerToGet;
	}

	public PlayerController GetOtherPlayer( int playerId )
	{
		PlayerController playerToGet = players.First( x => x.id != playerId );
		return playerToGet;
	}

	public PlayerController GetOtherPlayer( GameObject playerObj )
	{
		PlayerController playerToGet = players.First( x => x.gameObject != playerObj );
		return playerToGet;
	}

	public void EnableUnits(PlayerController player)
	{
		foreach( Unit unit in player.allUnits )
		{
			if( unit.owner == player.id ) unit.gameObject.SetActive( true );
		}
	}
}
