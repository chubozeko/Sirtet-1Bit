using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    public Transform defaultBlock;
    public Sprite[] blockSprites;
    public float gridBlockPreloadPercentage = 66.6f;
    public static int gridWidth = 12;
    public static int gridHeight = 20;
    private static Transform[,] grid = new Transform[gridWidth, gridHeight];
    private static Transform[] newLineGrid = new Transform[gridWidth];

    public GameObject activePiece;
    public Transform shadowPiece;
    public Transform clearingPiece;
    public PieceSpawner pieceSpawner;

    public Text t_blocksLeft;

    private float timer = 0f;

    void Start()
    {
        PreloadGridBlocks();
        shadowPiece.gameObject.SetActive(false);
        clearingPiece.gameObject.SetActive(false);
        timer = gridWidth + 1;
        StartCoroutine(LoadNewGridLine());
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            StartCoroutine(LoadNewGridLine());
            timer = gridWidth + 1;
        }
        t_blocksLeft.text = GetTotalGridBlocks().ToString();
    }

    void PreloadGridBlocks()
    {
        int preloadHeight = (int)(gridHeight * (gridBlockPreloadPercentage/100));
        for (int i=0; i<gridWidth; i++)
        {
            for (int j=0; j<preloadHeight; j++)
            {
                defaultBlock.GetComponent<SpriteRenderer>().sprite = blockSprites[Random.Range(0, blockSprites.Length)];
                GameObject newBlock = Instantiate(defaultBlock.gameObject, new Vector3(i, j, 0), Quaternion.identity);
                grid[i, j] = newBlock.transform;
            }    
        }
    }

    int GetTotalGridBlocks()
    {
        int total = 0;
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x,y] != null)
                    total++;
            }
        }
        return total;
    }

    IEnumerator LoadNewGridLine()
    {
        yield return new WaitForSeconds(1f);
        defaultBlock.GetComponent<SpriteRenderer>().sprite = blockSprites[Random.Range(0, blockSprites.Length)];
        for (int k=0; k<newLineGrid.Length; k++)
        {
            GameObject newBlock = Instantiate(defaultBlock.gameObject, new Vector3(k, -1, 0), Quaternion.identity);
            Color c = newBlock.gameObject.GetComponent<SpriteRenderer>().color; 
            c.a = 0.5f;
            newBlock.gameObject.GetComponent<SpriteRenderer>().color = c;
            newLineGrid[k] = newBlock.transform;
            yield return new WaitForSeconds(1f);
        }
        MoveRowsUp();
        AddNewLine();
        for (int j = 0; j < gridWidth; j++)
        {
            Destroy(newLineGrid[j].gameObject);
            newLineGrid[j] = null;
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
            Color c = shadowPiece.GetChild(i).transform.gameObject.GetComponent<SpriteRenderer>().color; 
            c.a = 0.25f;
            shadowPiece.GetChild(i).transform.gameObject.GetComponent<SpriteRenderer>().color = c;
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

    void AddNewLine()
    {
        for (int i = 0; i < gridWidth; i++)
        {
            GameObject newBlock = Instantiate(defaultBlock.gameObject, new Vector3(i, 0, 0), Quaternion.identity);
            grid[i, 0] = newBlock.transform;
        }
    }

    void MoveRowsUp()
    {
        for (int y = gridHeight - 1; y >= 0; y--)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                if (grid[j,y] != null)
                {
                    grid[j, y+1] = grid[j, y];
                    grid[j, y] = null;
                    grid[j, y+1].transform.position += new Vector3(0, 1, 0);
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
