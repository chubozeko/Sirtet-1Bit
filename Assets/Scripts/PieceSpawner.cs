using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    public GameObject[] pieces;
    private GameObject nextPiece;
    public Vector3 nextPieceLoadPosition;
    void Start()
    {
        LoadNextPiece();
        SpawnPiece();
    }

    void LoadNextPiece()
    {
        // Reset next piece
        Destroy(nextPiece);
        // Spawn the next piece 
        nextPiece = pieces[Random.Range(0, pieces.Length)];
        // Place nextPiece in the loading container on the right ("Next Piece")
        nextPiece = Instantiate(nextPiece, nextPieceLoadPosition, Quaternion.identity);
        nextPiece.GetComponent<Piece>().enabled = false;
    }

    public void SpawnPiece()
    {
        // Spawn a new piece based on the 'nextPiece'
        GameObject newPiece = Instantiate(nextPiece, transform.position, Quaternion.identity);
        newPiece.GetComponent<Piece>().enabled = true;
        // Load the next piece
        LoadNextPiece();
    }
}
