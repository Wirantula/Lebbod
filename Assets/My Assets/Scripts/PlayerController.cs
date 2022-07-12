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
    public bool onTurnStart = false;
    public bool hasRolled = false;
    public GameObject[] ownedPieces;
    public GameObject selectedObject;
    private GameObject moveToPosition;
    private Unit selectedUnit;
    private MoveablePosition movePos;
    public Ray ray;
    public RaycastHit hit;
    public MoveablePosition[] moveablePositions;

    // Start is called before the first frame update
    void Start()
    {
        moveablePositions = FindObjectsOfType<MoveablePosition>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){ OnClick(); }
        if(selectedObject != null && onTurnStart == true)
        {
            
            if( selectedUnit.pieceNumber == chosenNumber && !selectedUnit.hasMoved)
            {
                selectedUnit.selectionParticle.SetActive( true );
                stepsToMove = rolledNumber;
            }
            else if(!selectedUnit.hasMoved){ selectedUnit.selectionParticle.SetActive( true ); stepsToMove = chosenNumber;}
		}

        if( selectedObject != null && stepsToMove <= 0 && onTurnStart == false)
        {
            selectedUnit.selectionParticle.SetActive( false );
            selectedUnit.hasMoved = false;
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
                if( ( selectedUnit.xPos - movePos.xPos ) + ( selectedUnit.yPos - movePos.yPos ) == 1 || ( selectedUnit.xPos - movePos.xPos ) + ( selectedUnit.yPos - movePos.yPos ) == -1 )
                {
                    if( selectedUnit.xPos + 1 == movePos.xPos || selectedUnit.xPos - 1 == movePos.xPos || selectedUnit.yPos + 1 == movePos.yPos || selectedUnit.yPos - 1 == movePos.yPos )
                    {
                        onTurnStart = false;
                        selectedObject.transform.position = new Vector3( moveToPosition.transform.position.x , selectedObject.transform.position.y, moveToPosition.transform.position.z);
                        selectedUnit.xPos = movePos.xPos;
                        selectedUnit.yPos = movePos.yPos;
                        stepsToMove--;
                        selectedUnit.hasMoved = true;
                    }
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
        if(selectedUnit == null && hasRolled == false)
        {
            rolledNumber = Random.Range( 1, 7 );
            hasRolled = true;
        }

        if(rolledNumber == chosenNumber && hasRolled == false)
        {
			foreach( MoveablePosition position in moveablePositions )
			{
                position.SwapOwner();
			}
		}
	}

    public void EndTurn() 
    {
        Debug.Log( "turn has been played" );
    }

    public void OnTurnStart()
    {
        onTurnStart = true;
        hasRolled = false;
	}

    public void CaptureUnit()
    {
        
	}
}
