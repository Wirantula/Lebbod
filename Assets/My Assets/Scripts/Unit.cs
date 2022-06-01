using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int owner;
    public int pieceNumber;
    public int xPos;
    public int yPos;

	private void OnCollisionEnter( Collision collision )
	{
		Debug.Log( "i collided" );
		xPos = collision.gameObject.GetComponent<MoveablePosition>().xPos;
		yPos = collision.gameObject.GetComponent<MoveablePosition>().yPos;
	}


}
