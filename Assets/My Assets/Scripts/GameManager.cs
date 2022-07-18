using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    public bool gameEnded = false;
    public List<PlayerController> players;
	public string playerPrefabLocation;
	public Transform spawnPoint;
	private int playersInGame;
	public bool firstTurn = true;

	public static GameManager instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		//players = new PlayerController[PhotonNetwork.PlayerList.Length];
		photonView.RPC( "ImInGame", RpcTarget.AllBuffered );
	}

	private void Update()
	{

	}

	[PunRPC]
	void GameHasBeenWon(int id)
	{
		gameEnded = true;
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
		foreach( PlayerController item in players )
		{
			if( !firstTurn )
			{
				GetOtherPlayer( currentPlayer ).photonView.RPC( "OnTurnStart", RpcTarget.All, GetOtherPlayer( currentPlayer ).id );
			}
			if( firstTurn ) 
			{
				GetPlayer( 1 ).photonView.RPC( "OnTurnStart", RpcTarget.All, 1 );
				firstTurn = false;
			}
		}
	}

	[PunRPC]
	void UnitCapture( int playerWhoCaptured, int unitToCapture)
	{
		foreach( PlayerController item in players )
		{
			GetOtherPlayer( playerWhoCaptured ).photonView.RPC( "UnitGotCaptured", RpcTarget.All, unitToCapture);
		}
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
		return players.FirstOrDefault(x => x.id == playerId);
	}

	public PlayerController GetPlayer(GameObject playerObj)
	{
		return players.FirstOrDefault( x => x.gameObject == playerObj );
	}

	public PlayerController GetOtherPlayer( int playerId )
	{
		return players.FirstOrDefault( x => x.id != playerId );
	}

	public PlayerController GetOtherPlayer( GameObject playerObj )
	{
		return players.FirstOrDefault( x => x.gameObject != playerObj );
	}

	public void EnableUnits(PlayerController player)
	{
		foreach( Unit unit in player.allUnits )
		{
			if( unit.owner == player.id ) unit.gameObject.SetActive( true );
		}
	}

	public void DisableUnit(int unitToDisable, int playerId)
	{
		foreach( Unit unit in GetPlayer(playerId).allUnits )
		{
			if( unit.pieceNumber == unitToDisable ) unit.gameObject.SetActive( false );
		}
	}
}
