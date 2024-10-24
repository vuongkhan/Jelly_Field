using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool isEmpty = true;

    void Update()
    {
        if (transform.childCount == 0)
        {
            isEmpty = true; 
        }
        else
        {
            isEmpty = false; 
        }
    }
}
