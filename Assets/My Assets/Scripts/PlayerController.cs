using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerNumber;
    public int chosenNumber;
    public int rolledNumber;
    public GameObject[] ownedPieces;
    public GameObject selectedObject;
    private GameObject moveToPosition;
    public Ray ray;
    public RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){ OnClick(); }

        if(selectedObject != null)
        {
            
		}
    }

    private void OnClick()
    {
        ray = Camera.main.ScreenPointToRay( Input.mousePosition );
        if(Physics.Raycast(ray.origin, ray.direction, out hit))
        {
            if( selectedObject != null && hit.collider.GetComponent<MoveablePosition>().gameObject )
            {
                moveToPosition = hit.transform.gameObject;
                selectedObject.transform.position = moveToPosition.transform.position;
                selectedObject = null;
            }
            else if( hit.collider.GetComponent<Unit>().owner == playerNumber )
            {
                if( hit.collider.GetComponent<Unit>().pieceNumber == chosenNumber || hit.collider.GetComponent<Unit>().pieceNumber == rolledNumber )
                {
                    selectedObject = hit.transform.gameObject;
                }
            }
            else return;
		}
    }

}
