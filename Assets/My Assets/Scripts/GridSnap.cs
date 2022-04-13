using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GridSnap : MonoBehaviour
{
    [SerializeField] private Vector3 gridSize = new Vector3( 1f, 1f, 1f );
    private Vector3 mOffset;
    private float mZCoord;

    private void SnapObjectToGrid()
    {
        var position = new Vector3(
                    ( Mathf.Round( this.transform.position.x / this.gridSize.x ) * this.gridSize.x ),
                    ( Mathf.Round( this.transform.position.y / this.gridSize.y ) * this.gridSize.y ),
                    ( Mathf.Round( this.transform.position.z / this.gridSize.z ) * this.gridSize.z ) );

        this.transform.position = position;
    }

    private void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint( gameObject.transform.position ).z;
        mOffset = gameObject.transform.position - GetMouseWorldPos();
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint( mousePoint );
    }

    private void OnMouseDrag()
    {
        if( Input.GetKey( KeyCode.LeftShift ) ) { transform.position = new Vector3( this.transform.position.x, GetMouseWorldPos().y + mOffset.y , this.transform.position.z ); }
        else { transform.position = new Vector3( GetMouseWorldPos().x + mOffset.x , this.transform.position.y, GetMouseWorldPos().z + mOffset.z  ); }
        SnapObjectToGrid();
    }
}
