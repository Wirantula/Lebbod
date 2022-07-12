using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveablePosition : MonoBehaviour
{
	public int xPos;
	public int yPos;
	public int owner;
	public bool occupied = false;
	public Material p1Mat;
	public Material p2Mat;
	public GameObject panel;

	private void Start()
	{
		MeshRenderer getObject = GetComponentInChildren<MeshRenderer>();
		panel = getObject.gameObject;
		if( panel.GetComponent<Renderer>().material.name.Contains("P1"))
		{
			owner = 1;
		}
		else { owner = 2; }
	}

	public void SwapOwner()
	{
		if(owner == 1)
		{
			panel.GetComponent<Renderer>().material = p2Mat;
			owner = 2;
		}
		else
		{
			panel.GetComponent<Renderer>().material = p1Mat;
			owner = 1;
		}
	}

	private void OnCollisionStay( Collision collision )
	{
		if(collision.gameObject.GetComponent<Unit>())
		{
			occupied = true;
		}
	}

	private void OnCollisionExit( Collision collision )
	{
		occupied = false;
	}

}
