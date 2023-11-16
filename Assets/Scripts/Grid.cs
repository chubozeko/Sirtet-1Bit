using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public static int gridWidth = 12;
    public static int gridHeight = 20;
    private static Transform[,] grid = new Transform[gridWidth, gridHeight];
    void Start()
    {
        
    }

    public bool ValidMove(Transform piece)
    {
        foreach (Transform childBlock in piece)
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

    public void AddPieceToGrid(Transform piece)
    {
        foreach (Transform childBlock in piece)
        {
            if (childBlock.CompareTag("PieceBlock")) 
            {
                int roundedX = Mathf.RoundToInt(childBlock.transform.position.x);
                int roundedY = Mathf.RoundToInt(childBlock.transform.position.y);

                grid[roundedX, roundedY] = childBlock;
            }
        }

        CheckForLines();
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
