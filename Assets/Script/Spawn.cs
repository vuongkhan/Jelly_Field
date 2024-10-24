using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellySpawner : MonoBehaviour
{
    public GameObject jellyPrefab;  
    public Vector3 spawnPosition = new Vector3(0, 1, 0); 

    void Update()
    {
        JellyDrag[] existingJellies = FindObjectsOfType<JellyDrag>();
        if (existingJellies.Length == 0)
        {
            SpawnJelly();
        }
    }
    void SpawnJelly()
    {
        Instantiate(jellyPrefab, spawnPosition, Quaternion.identity);
        Debug.Log("Spawned a new Jelly at the specified position.");
    }
}
