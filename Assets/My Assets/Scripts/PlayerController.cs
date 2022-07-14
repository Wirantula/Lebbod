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
    public bool hasChosen = false;
    public GameObject[] ownedPieces;
    public GameObject selectedObject;
    private GameObject moveToPosition;
    public Unit selectedUnit;
    private MoveablePosition movePos;
    public Ray ray;
    public RaycastHit hit;
    public MoveablePosition[] moveablePositions;
    public TMP_InputField inputField;

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
            //move selected unit
            if(( selectedObject != null && hit.collider.GetComponent<MoveablePosition>() && stepsToMove > 0)|| ( selectedObject != null && hit.collider.GetComponent<Unit>().owner != playerNumber && hit.collider.GetComponent<Unit>().currentPosition && stepsToMove > 0 ) )
            {
                moveToPosition = hit.transform.gameObject;
                //check whether targeted is unit or position
                if( hit.collider.GetComponent<MoveablePosition>() )
                {
                    movePos = hit.transform.GetComponent<MoveablePosition>();
                }
                else { movePos = hit.transform.GetComponent<Unit>().currentPosition; }
                //check for possible movement in 1 positions each time
                if( ((( selectedUnit.xPos - movePos.xPos ) == -1 && selectedUnit.yPos == movePos.yPos)|| (( selectedUnit.xPos - movePos.xPos ) == 1 && selectedUnit.yPos == movePos.yPos))|| ((( selectedUnit.yPos - movePos.yPos ) == -1 && selectedUnit.xPos == movePos.xPos)|| (( selectedUnit.yPos - movePos.yPos ) == 1 && selectedUnit.xPos == movePos.xPos)))
                {
                    if( !movePos.occupied )
                    {
                        onTurnStart = false;
                        selectedObject.transform.position = new Vector3( moveToPosition.transform.position.x, selectedObject.transform.position.y, moveToPosition.transform.position.z );
                        stepsToMove--;
                        selectedUnit.hasMoved = true;
                    }
                    else
                    {
                        if( movePos.occUnit.owner == playerNumber ) { Debug.Log( "Own Unit" ); return; }
                        else if( movePos.owner == playerNumber && stepsToMove == 1 )
                        {
                            onTurnStart = false;
                            selectedObject.transform.position = new Vector3( moveToPosition.transform.position.x, selectedObject.transform.position.y, moveToPosition.transform.position.z );
                            stepsToMove--;
                            selectedUnit.hasMoved = true;
                            CaptureUnit();
                        }
                        else if( movePos.owner == playerNumber && stepsToMove != 1 )
                        {
                            Debug.Log( "Cant capture with moves left" );
                        }
                    }
                }
            }
            //unselect unit
            else if( selectedObject != null && hit.collider.GetComponent<Unit>() == selectedUnit && !selectedUnit.hasMoved)
            {
                selectedUnit.selectionParticle.SetActive( false );
                selectedObject = null;
                selectedUnit = null;
                moveToPosition = null;
                movePos = null;
            }
            //select unit
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
	}

    public void EndTurn() 
    {
        Debug.Log( "turn has been played" );
    }

    public void OnTurnStart()
    {
        onTurnStart = true;
        hasRolled = false;
        hasChosen = false;
	}

    public void ChooseNumber()
    {
        if( hasChosen == false && hasRolled )
        {
            chosenNumber = int.Parse( inputField.text );
            if( chosenNumber > 6 ) chosenNumber = 6;
            if( chosenNumber < 1 ) chosenNumber = 1;
            hasChosen = true;
            //if chosen and rolled number are the same swap all board tiles
            if( rolledNumber == chosenNumber && onTurnStart == true )
            {
                foreach( MoveablePosition position in moveablePositions )
                {
                    position.SwapOwner();
                }
            }
        }
        else if( hasChosen ) Debug.Log( "Already chose number" );
        else Debug.Log( "Roll dice first" );
	}

    public void CaptureUnit()
    {
        Debug.Log( "Captured unit" );
	}
}
