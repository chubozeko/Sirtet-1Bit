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
    public GameObject gameOverPanel;
    // Grid Dimensions
    public static int gridWidth = 12;
    public static int gridHeight = 20;

    private static Transform[,] grid = new Transform[gridWidth, gridHeight];
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) 
        {
            transform.position += new Vector3(-1, 0, 0);
            if (!ValidMove()) 
            {
                transform.position -= new Vector3(-1, 0, 0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.D)) 
        {
            transform.position += new Vector3(1, 0, 0);
            if (!ValidMove()) 
            {
                transform.position -= new Vector3(1, 0, 0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.W)) 
        {
            transform.RotateAround(rotationPoint.position, new Vector3(0, 0, 1), 90f);
            if (!ValidMove()) 
            {
                transform.RotateAround(rotationPoint.position, new Vector3(0, 0, 1), -90f);
            }
        }


        if (Time.time - prevTime > (Input.GetKey(KeyCode.S) ? dropTime / dropMultiplier : dropTime))
        {
            transform.position += new Vector3(0, -1, 0);
            if (!ValidMove()) 
            {
                transform.position -= new Vector3(0, -1, 0);
                AddToGrid();
                CheckForLines();

                this.enabled = false;
                // Only spawn a new piece if there is no other piece in the Spawner's position
                if (transform.position != FindObjectOfType<PieceSpawner>().transform.position) 
                {
                    FindObjectOfType<PieceSpawner>().SpawnPiece();
                }
                else
                {
                    gameOverPanel.SetActive(true);
                    Debug.Log("GAME OVER!");
                }
                
            }
            prevTime = Time.time;
        }
    }

    bool ValidMove()
    {
        foreach (Transform childBlock in transform)
        {
            if (childBlock.CompareTag("PieceBlock")) 
            {
                int roundedX = Mathf.RoundToInt(childBlock.transform.position.x);
                int roundedY = Mathf.RoundToInt(childBlock.transform.position.y);

                if (roundedX < 0 || roundedX >= gridWidth || roundedY < 0 || roundedY >= gridHeight)
                {
                    return false;
                }

                if (grid[roundedX, roundedY] != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    void AddToGrid()
    {
        foreach (Transform childBlock in transform)
        {
            if (childBlock.CompareTag("PieceBlock")) 
            {
                int roundedX = Mathf.RoundToInt(childBlock.transform.position.x);
                int roundedY = Mathf.RoundToInt(childBlock.transform.position.y);

                grid[roundedX, roundedY] = childBlock;
            }
        }
    }

    void CheckForLines()
    {
        for (int i = gridHeight-1; i >= 0; i--)
        {
            if (HasLine(i))
            {
                DeleteLine(i);
                RowDown(i);
            }
        }
    }

    bool HasLine(int i)
    {
        for (int j = 0; j < gridWidth; j++)
        {
            if (grid[j,i] == null)
            {
                return false;
            }
        }
        return true;
    }

    void DeleteLine(int i)
    {
        for (int j = 0; j < gridWidth; j++)
        {
            Destroy(grid[j,i].gameObject);
            grid[j,i] = null;
        }
    }

    void RowDown(int i)
    {
        for (int y = i; y < gridHeight; y++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                if (grid[j,y] != null)
                {
                    grid[j, y-1] = grid[j, y];
                    grid[j, y] = null;
                    grid[j, y-1].transform.position -= new Vector3(0, 1, 0);
                }
            }
        }
    }

}
