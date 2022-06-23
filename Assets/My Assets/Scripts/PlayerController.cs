using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public int playerNumber;
    public int chosenNumber;
    public int rolledNumber;
    public int stepsToMove;
    public GameObject[] ownedPieces;
    public GameObject selectedObject;
    private GameObject moveToPosition;
    private Unit selectedUnit;
    private MoveablePosition movePos;
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
            selectedUnit.selectionParticle.SetActive( true );
            if( selectedUnit.pieceNumber == chosenNumber && !selectedUnit.hasMoved)
            {
                stepsToMove = rolledNumber;
            }
            else if(!selectedUnit.hasMoved){ stepsToMove = chosenNumber; }
		}

        if( selectedObject != null && stepsToMove <= 0 )
        {
            selectedUnit.selectionParticle.SetActive( false );
            selectedObject = null;
            selectedUnit = null;
            moveToPosition = null;
            movePos = null;
            EndTurn();
        }
    }

    private void OnClick()
    {
        ray = Camera.main.ScreenPointToRay( Input.mousePosition );
        if(Physics.Raycast(ray.origin, ray.direction, out hit))
        {
            if( selectedObject != null && hit.collider.GetComponent<MoveablePosition>() && stepsToMove > 0)
            {
                moveToPosition = hit.transform.gameObject;
                movePos = hit.transform.GetComponent<MoveablePosition>();
                if( selectedUnit.xPos + 1 == movePos.xPos || selectedUnit.xPos - 1 == movePos.xPos || selectedUnit.yPos + 1 == movePos.yPos || selectedUnit.yPos - 1 == movePos.yPos ) 
                {
                    selectedObject.transform.position = moveToPosition.transform.position;
                    selectedUnit.xPos = movePos.xPos;
                    selectedUnit.yPos = movePos.yPos;
                    stepsToMove--;
                    selectedUnit.hasMoved = true;
                }
            }
            else if( selectedObject != null && hit.collider.GetComponent<Unit>() == selectedUnit && !selectedUnit.hasMoved)
            {
                selectedUnit.selectionParticle.SetActive( false );
                selectedObject = null;
                selectedUnit = null;
                moveToPosition = null;
                movePos = null;
            }
            else if( hit.collider.GetComponent<Unit>() )
            {
                if( hit.collider.GetComponent<Unit>().owner == playerNumber )
                {
                    if( hit.collider.GetComponent<Unit>().pieceNumber == chosenNumber || hit.collider.GetComponent<Unit>().pieceNumber == rolledNumber )
                    {
                        selectedObject = hit.transform.gameObject;
                        selectedUnit = hit.collider.GetComponent<Unit>();
                    }
                }
            }
		}
    }

    public void RollDice()
    {
        if(selectedUnit == null)
        {
            rolledNumber = Random.Range( 1, 7 );
        }
	}

    public void EndTurn() 
    {
        Debug.Log( "turn has been played" );
    }
}
