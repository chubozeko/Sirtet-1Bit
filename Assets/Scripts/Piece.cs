using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    private float prevTime;
    public float dropTime = 0.8f;
    public float dropMultiplier = 10f;
    public Transform rotationPoint;

    private Grid grid;
    // Grid Dimensions
    // public static int gridWidth = 12;
    // public static int gridHeight = 20;
    // private static Transform[,] grid = new Transform[gridWidth, gridHeight];
    void Start()
    {
        grid = FindObjectOfType<Grid>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) 
        {
            transform.position += new Vector3(-1, 0, 0);
            if (!grid.ValidMove(transform)) 
            {
                transform.position -= new Vector3(-1, 0, 0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.D)) 
        {
            transform.position += new Vector3(1, 0, 0);
            if (!grid.ValidMove(transform)) 
            {
                transform.position -= new Vector3(1, 0, 0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.W)) 
        {
            transform.RotateAround(rotationPoint.position, new Vector3(0, 0, 1), 90f);
            if (!grid.ValidMove(transform)) 
            {
                transform.RotateAround(rotationPoint.position, new Vector3(0, 0, 1), -90f);
            }
        }


        if (Time.time - prevTime > (Input.GetKey(KeyCode.S) ? dropTime / dropMultiplier : dropTime))
        {
            transform.position += new Vector3(0, -1, 0);
            if (!grid.ValidMove(transform)) 
            {
                transform.position -= new Vector3(0, -1, 0);
                grid.AddPieceToGrid(transform);
                // CheckForLines();

                this.enabled = false;
                // Only spawn a new piece if there is no other piece in the Spawner's position
                if (transform.position != FindObjectOfType<PieceSpawner>().transform.position) 
                {
                    FindObjectOfType<PieceSpawner>().SpawnPiece();
                }
                else
                {
                    // TODO: show GAME OVER panel
                    Debug.Log("GAME OVER!");
                }
                
            }
            prevTime = Time.time;
        }
    }

    

}
