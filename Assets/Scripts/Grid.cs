using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Transform defaultBlock;
    public Sprite[] blockSprites;
    public float gridBlockPreloadPercentage = 66.6f;
    public static int gridWidth = 12;
    public static int gridHeight = 20;
    private static Transform[,] grid = new Transform[gridWidth, gridHeight];

    public GameObject activePiece;
    public Transform shadowPiece;
    public Transform clearingPiece;
    public PieceSpawner pieceSpawner;

    void Start()
    {
        PreloadGridBlocks();
        shadowPiece.gameObject.SetActive(false);
        clearingPiece.gameObject.SetActive(false);
    }

    void PreloadGridBlocks()
    {
        int preloadHeight = (int)(gridHeight * (gridBlockPreloadPercentage/100));
        for (int i=0; i<gridWidth; i++)
        {
            for (int j=0; j<preloadHeight; j++)
            {
                // defaultBlock.GetComponent<SpriteRenderer>().sprite = blockSprites[Random.Range(0, blockSprites.Length)];
                GameObject newBlock = Instantiate(defaultBlock.gameObject, new Vector3(i, j, 0), Quaternion.identity);
                grid[i, j] = newBlock.transform;
            }    
        }
    }

    public void SpawnNewPiece()
    {
        pieceSpawner.SpawnPiece();
        activePiece = pieceSpawner.GetActivePiece();
        UpdateShadowPiece(activePiece.transform);
    }

    public Vector3 GetSpawnPosition()
    {
        return pieceSpawner.transform.position;
    }

    public void UpdateShadowPiece(Transform activePiece)
    {
        if (!shadowPiece.gameObject.activeSelf)
            shadowPiece.gameObject.SetActive(true);
        shadowPiece.position = Vector3.zero;
        // Copy the activePiece's blocks
        List<Vector3> blockPositions = new List<Vector3>();
        foreach (Transform childBlock in activePiece)
        {
            blockPositions.Add(childBlock.position);
        }
        for(int i=0; i<shadowPiece.childCount; i++)
        {
            shadowPiece.GetChild(i).transform.position = blockPositions[i];
        }
        // Drop shadowPiece to the minimum drop distance
        int dropDistance = FindDropDistance(activePiece);
        shadowPiece.position -= new Vector3(0, dropDistance, 0);
        // Update clearingPiece based on shadowPiece
        UpdateClearingPiece(shadowPiece);
    }

    public void UpdateClearingPiece(Transform shadowPiece)
    {
        if (!clearingPiece.gameObject.activeSelf)
            clearingPiece.gameObject.SetActive(true);
        // Reset transform of clearingPiece
        clearingPiece.position = Vector3.zero;
        clearingPiece.rotation = new Quaternion(0, 0, 0, 0);
        // Copy the shadowPiece's blocks
        List<Vector3> blockPositions = new List<Vector3>();
        foreach (Transform childBlock in shadowPiece)
        {
            blockPositions.Add(childBlock.position);
        }
        for(int i=0; i<clearingPiece.childCount; i++)
        {
            clearingPiece.GetChild(i).transform.position = blockPositions[i];
        }
        // Mirror clearingPiece based on the bottom of the shadowPiece (rotate, then translate)
        clearingPiece.RotateAround(GetBottomPositionOfPiece(shadowPiece), new Vector3(1, 0, 0), 180f);
        clearingPiece.position -= new Vector3(0, 1, 0);
    }

    private int FindDropDistance(Transform piece)
    {
        int dist = gridHeight;
        foreach (Transform childBlock in piece)
        {
            if (childBlock.CompareTag("PieceBlock")) 
            {
                int roundedX = Mathf.RoundToInt(childBlock.transform.position.x);
                int roundedY = Mathf.RoundToInt(childBlock.transform.position.y);
                dist = Mathf.Min(dist, GetMinDistance(roundedX, roundedY));
            }
        }
        return dist;
    }

    private int GetMinDistance(int x, int y)
    {
        int dist = 0;
        for (int j = y; j > 0; j--)
        {
            if (grid[x, j-1] == null)
            {
                dist++;
            }
            else
            {
                return dist;
            }
        }
        return dist;
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
        // foreach (Transform childBlock in piece)
        // {
        //     if (childBlock.CompareTag("PieceBlock")) 
        //     {
        //         int roundedX = Mathf.RoundToInt(childBlock.transform.position.x);
        //         int roundedY = Mathf.RoundToInt(childBlock.transform.position.y);

        //         grid[roundedX, roundedY] = childBlock;
        //     }
        // }
        Destroy(piece.gameObject);
        ClearBlocksUnderneath();

    }

    void ClearBlocksUnderneath()
    {
        foreach (Transform childBlock in clearingPiece)
        {
            Vector2 coords = GetGridPosition(childBlock);
            if (coords.x >= 0 && coords.y >= 0)
            {
                if (grid[(int)coords.x, (int)coords.y] != null)
                {
                    Destroy(grid[(int)coords.x, (int)coords.y].gameObject);
                    grid[(int)coords.x, (int)coords.y] = null;
                }
            }
        }
    }

    void CheckForClearance()
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

    private Vector3 GetBottomPositionOfPiece(Transform piece)
    {
        Transform bottommost = piece.GetChild(0);
        float minDist = 30;
        Vector3 bottommostPosition = Vector3.zero;
        foreach (Transform childBlock in piece)
        {
            // childBlock.GetComponent<SpriteRenderer>().color = defaultPieceColor;
            int roundedX = Mathf.RoundToInt(childBlock.transform.position.x);
            int roundedY = Mathf.RoundToInt(childBlock.transform.position.y);
            Vector3 coords = new Vector3(roundedX, roundedY, 0f);
            if (roundedY < minDist)
            {
                minDist = roundedY;
                bottommost = childBlock;
                bottommostPosition = coords;
            }
        }
        // Debug.Log("Bottommost pieceBlock: " + bottommost.name + "; coords: " + bmCoords + "; dist = " + minDist);
        return bottommostPosition;
    }

    private Vector2 GetGridPosition(Transform blockPiece)
    {
        int roundedX = Mathf.RoundToInt(blockPiece.transform.position.x);
        int roundedY = Mathf.RoundToInt(blockPiece.transform.position.y);
        return new Vector2(roundedX, roundedY);
    }
}
