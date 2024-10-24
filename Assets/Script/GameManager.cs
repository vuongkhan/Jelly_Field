using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Dictionary<JellyColor, int> destroyedJellyColors = new Dictionary<JellyColor, int>(); 
    public UIManager uiManager; 
    private bool isChecking = false; 

    // Ghi nhận màu Jelly bị hủy
    public void RecordDestroyedJellyColor(Color jellyColor, int count = 1)
    {
        JellyColor jellyColorEnum = ColorToJellyColor(jellyColor);
        if (jellyColorEnum == JellyColor.Unknown)
        {
            return;
        }
        if (!destroyedJellyColors.ContainsKey(jellyColorEnum))
        {
            destroyedJellyColors[jellyColorEnum] = 0; 
        }
        destroyedJellyColors[jellyColorEnum] += count;
        if (uiManager != null)
        {
            uiManager.UpdateUI(); 
        }
    }
    public int GetScoreForColor(JellyColor jellyColor)
    {
        if (destroyedJellyColors.ContainsKey(jellyColor))
        {
            return destroyedJellyColors[jellyColor];
        }
        return 0; 
    }
    private JellyColor ColorToJellyColor(Color color)
    {
        if (color == Color.magenta) return JellyColor.Magenta;
        if (color == Color.blue) return JellyColor.Blue;
        if (color == Color.red) return JellyColor.Red;
        if (color == Color.yellow) return JellyColor.Yellow;
        if (color == Color.green) return JellyColor.Green;
        if (color.r == 0.5f && color.g == 0.5f && color.b == 1f) return JellyColor.Grey;
        return JellyColor.Unknown; 
    }
    private IEnumerator CheckBlocksAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Block[] blocks = GameObject.FindObjectsOfType<Block>();
        bool allBlocksNotEmpty = true;
        foreach (Block block in blocks)
        {
            if (block.isEmpty)
            {
                allBlocksNotEmpty = false; 
                break; 
            }
        }
        if (allBlocksNotEmpty)
        {
            uiManager.GameOver();
        }

        isChecking = false;
    }

    private void CheckBlock()
    {
        if (isChecking) return;
        Block[] blocks = GameObject.FindObjectsOfType<Block>();
        bool allBlocksNotEmpty = true;
        foreach (Block block in blocks)
        {
            if (block.isEmpty)
            {
                allBlocksNotEmpty = false; 
                break; 
            }
        }
        if (allBlocksNotEmpty)
        {
            isChecking = true; 
            StartCoroutine(CheckBlocksAfterDelay(7.0f)); 
        }
        else
        {
            Debug.Log("Empty");
        }
    }

    void Update()
    {
        CheckBlock(); 
    }
}
