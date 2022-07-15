using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    public bool gameEnded = false;
    public int playerTurn = 1;
    public PlayerController[] players;
	public string playerPrefabLocation;
	public Transform spawnPoint;
	private int playersInGame;

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

	[PunRPC]
	void ImInGame()
	{
		playersInGame++;

		if( playersInGame == PhotonNetwork.PlayerList.Length ) 
			SpawnPlayer();
	}

	void SpawnPlayer() 
	{
		//instantiate player across network
		GameObject playerObj = PhotonNetwork.Instantiate( playerPrefabLocation, spawnPoint.position, Quaternion.identity );

		//get the playerscript
		PlayerController playerScript = playerObj.GetComponent<PlayerController>();
	}
}
