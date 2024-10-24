using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public ColorRequirement[] levelConditions; 
    private GameManager gameManager; 

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); 
    }

    public void CheckLevelCompletion()
    {
        foreach (var condition in levelConditions)
        {
            if (gameManager.GetScoreForColor(condition.color) >= condition.requiredAmount)
            {
                Debug.Log("Successfull");
                return;
            }
        }
        Debug.Log("No");
    }
    public void RecordDestroyedJellyColor(Color jellyColor, int count)
    {
        gameManager.RecordDestroyedJellyColor(jellyColor, count);
        CheckLevelCompletion(); 
    }
}
