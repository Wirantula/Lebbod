using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("Components")]
    [SerializeField]
    public Unit[] allUnits;
    public Player photonPlayer;

    public int id;
    public int chosenNumber;
    public int rolledNumber;
    public int stepsToMove;
    public int unitsCaptured = 0;
    public bool onTurnStart = false;
    public bool hasRolled = false;
    public bool hasChosen = false;
    public GameObject selectedObject;
    private GameObject moveToPosition;
    public Unit selectedUnit;
    public PlayerController instance;
    private MoveablePosition movePos;
    public Ray ray;
    public RaycastHit hit;
    public MoveablePosition[] moveablePositions;
    public TMP_InputField inputField;
    public TMP_Text diceRoll;
    public TMP_Text steps;
    public TMP_Text yourTurn;
    public Button rollDice;
    public Button chooseNumber;

    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;
        instance = this;
        GameManager.instance.players.Add( instance );
        Button[] allButtons = FindObjectsOfType<Button>();
        TMP_Text[] allText = FindObjectsOfType<TMP_Text>( true );
        if( photonView.IsMine )
        {
            foreach( Button button in allButtons )
            {
                if( button.gameObject.name == "RollDice" ) rollDice = button;
                if( button.gameObject.name == "ChooseNumber" ) chooseNumber = button;
            }
            rollDice.onClick.AddListener( delegate { RollDice(); } );
            chooseNumber.onClick.AddListener( delegate { ChooseNumber(); } );
            foreach( TMP_Text text in allText )
            {
                if( text.gameObject.name == "DiceRoll" ) diceRoll = text;
                if( text.gameObject.name == "StepsToMove" ) steps = text;
                if( text.gameObject.name == "YourTurn" ) yourTurn = text;
            }
        }
    }

    [PunRPC]
    public void InitializeUnits()
    {
        //call gamemanager for units
        GameManager.instance.EnableUnits( this );
    }

    // Start is called before the first frame update
    void Awake()
    {
        allUnits = GetComponentsInChildren<Unit>(true);
        moveablePositions = FindObjectsOfType<MoveablePosition>();
        if(FindObjectOfType<TMP_InputField>().name == "NumberField" ) inputField = FindObjectOfType<TMP_InputField>();
    }

	private void Start()
	{
        if( GameManager.instance.firstTurn && id == 1 && photonView.IsMine) { EndTurn(); }
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
        if( photonView.IsMine )
        {
            steps.text = "Steps to move: " + stepsToMove;
        }

        if( unitsCaptured >= 4 )
        {
            GameManager.instance.photonView.RPC( "GameHasBeenWon", RpcTarget.All, id );
        }
    }

    private void OnClick()
    {
        ray = Camera.main.ScreenPointToRay( Input.mousePosition );
        if(Physics.Raycast(ray.origin, ray.direction, out hit))
        {
            //move selected unit
            if(( selectedObject != null && hit.collider.GetComponent<MoveablePosition>() && stepsToMove > 0)|| ( selectedObject != null && hit.collider.GetComponent<Unit>().owner != id && hit.collider.GetComponent<Unit>().currentPosition && stepsToMove > 0 ) )
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
                        if( movePos.occUnit.owner == id ) { Debug.Log( "Own Unit" ); return; }
                        else if( movePos.owner == id && stepsToMove == 1 )
                        {
                            onTurnStart = false;
                            selectedObject.transform.position = new Vector3( moveToPosition.transform.position.x, selectedObject.transform.position.y, moveToPosition.transform.position.z );
                            stepsToMove--;
                            selectedUnit.hasMoved = true;
                            CaptureUnit(movePos.occUnit);
                        }
                        else if( movePos.owner == id && stepsToMove != 1 )
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
            else if( hit.collider.GetComponent<Unit>() && hasRolled && hasChosen)
            {
                if( hit.collider.GetComponent<Unit>().owner == id )
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
            diceRoll.text = "Rolled number: " + rolledNumber;
        }
	}

    public void EndTurn() 
    {
        Debug.Log( "turn has been played" );
        if( photonView.IsMine )
        {
            yourTurn.gameObject.SetActive( false );
            GameManager.instance.photonView.RPC( "OnTurnEnded", RpcTarget.All, id );
        }
    }

    [PunRPC]
    public void OnTurnStart(int idToExecute)
    {
        if( photonView.IsMine )
        {
            if( idToExecute == id )
            {
                onTurnStart = true;
                hasRolled = false;
                hasChosen = false;
                yourTurn.gameObject.SetActive( true );
            }
            else return;
        }
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
                    position.photonView.RPC("SwapOwner", RpcTarget.All);
                }
            }
        }
        else if( hasChosen ) Debug.Log( "Already chose number" );
        else Debug.Log( "Roll dice first" );
	}

    public void CaptureUnit(Unit unitToCapture)
    {
        Debug.Log( "Captured unit" );
        GameManager.instance.photonView.RPC( "UnitCapture", RpcTarget.All, id , unitToCapture.pieceNumber);
        unitsCaptured++;
	}

    [PunRPC]
    public void UnitGotCaptured(int unitGotCaptured)
    {
        GameManager.instance.DisableUnit( unitGotCaptured, id );
    }
}
