using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    public GameObject[] pieces;
    void Start()
    {
        SpawnPiece();
    }

    
    public void SpawnPiece()
    {
        Instantiate(pieces[Random.Range(0, pieces.Length)], transform.position, Quaternion.identity);
    }
}
