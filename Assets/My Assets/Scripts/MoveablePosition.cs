using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MoveablePosition : MonoBehaviourPunCallbacks
{
	public int xPos;
	public int yPos;
	public int owner;
	public bool occupied = false;
	public Material p1Mat;
	public Material p2Mat;
	public GameObject panel;
	public Unit occUnit;
	public Ray ray;
	public RaycastHit hit;

	private void Start()
	{
		ray.origin = new Vector3(transform.position.x, 0f, transform.position.z);
		ray.direction = Vector3.up;
		MeshRenderer getObject = GetComponentInChildren<MeshRenderer>();
		panel = getObject.gameObject;
		if( panel.GetComponent<Renderer>().material.name.Contains("P1"))
		{
			owner = 1;
		}
		else { owner = 2; }
	}

	[PunRPC]
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

	private void Update()
	{
		CheckAbove();
	}

	public void CheckAbove()
	{
		Debug.DrawRay( ray.origin, ray.direction, Color.red );
		if( Physics.Raycast( ray, out hit, 20f ) )
		{
			if( hit.transform.GetComponent<Unit>())
			{
				occUnit = hit.transform.GetComponent<Unit>();
				occupied = true;
				occUnit.currentPosition = this;
				occUnit.xPos = xPos;
				occUnit.yPos = yPos;
			}
			else { occupied = false; }
		}
	}

}
