using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyManager : MonoBehaviour
{
    private bool hasScaled = false;
    public GameObject jellyPrefab;
    public Transform slot;
    public int jellyCount = 4;
    public float jellySpacing = 1.0f;
    public bool isSingle = false;
    private Dictionary<int, GameObject> jellyInstances = new Dictionary<int, GameObject>();

    private List<Color> jellyColors = new List<Color>
    {
        Color.blue,
        new Color(0.5f, 0.5f, 1f),
        new Color(0.5f, 0f, 0.5f),
        Color.yellow,
        Color.magenta,
        Color.red
    };

    void Start()
    {
        UpdateJellies(jellyCount);
    }

    public void UpdateJellies(int count)
    {
        ClearExistingJellies();

        int maxCount = Mathf.Min(count, 4);
        List<Color> usedColors = new List<Color>();

        for (int i = 0; i < maxCount; i++)
        {
            GameObject jelly = Instantiate(jellyPrefab, slot);
            jelly.name = "Jelly " + (i + 1).ToString();

            if (maxCount == 1)
            {
                jelly.transform.localScale = new Vector3(1.5f, 1.5f, 0.5f);
            }
            else
            {
                jelly.transform.localScale = CalculateScale(maxCount);
            }

            Vector3 position = CalculatePosition(i, maxCount);
            jelly.transform.localPosition = position;

            Color jellyColor;
            do
            {
                jellyColor = jellyColors[Random.Range(0, jellyColors.Count)];
            } while (usedColors.Contains(jellyColor));

            usedColors.Add(jellyColor);
            jelly.GetComponent<Renderer>().material.color = jellyColor;

            jellyInstances[i] = jelly;

            JellyMesh jellyScript = jelly.GetComponent<JellyMesh>();
            if (jellyScript != null)
            {
                jellyScript.jellyID = i + 1;
            }
        }

        for (int i = 0; i < maxCount; i++)
        {
            JellyMesh jellyScript = jellyInstances[i].GetComponent<JellyMesh>();

            if (jellyScript != null)
            {
                if (i % 2 == 0 && i + 1 < maxCount)
                {
                    jellyScript.connectObject = jellyInstances[i + 1];
                }
                else if (i % 2 != 0)
                {
                    jellyScript.connectObject = jellyInstances[i - 1];
                }
                else
                {
                    jellyScript.connectObject = null;
                }
            }
        }
    }

    private Vector3 CalculateScale(int count)
    {
        float scale = 1f / Mathf.Sqrt(count);
        return new Vector3(scale, scale, scale);
    }

    private Vector3 CalculatePosition(int index, int count)
    {
        float offset = jellySpacing;

        if (count == 1) return Vector3.zero;

        int rows = Mathf.CeilToInt(Mathf.Sqrt(count));
        int cols = Mathf.CeilToInt((float)count / rows);

        int row = index / cols;
        int col = index % cols;

        return new Vector3((col - (cols - 1) / 2f) * offset, (row - (rows - 1) / 2f) * offset, 0);
    }

    private void ClearExistingJellies()
    {
        foreach (GameObject jelly in jellyInstances.Values)
        {
            Destroy(jelly);
        }
        jellyInstances.Clear();
    }

    public void CheckJellyStatus()
    {
        bool anyJellyExists = false;

        foreach (var jelly in jellyInstances.Values)
        {
            if (jelly != null)
            {
                anyJellyExists = true;
                break;
            }
        }

        if (!anyJellyExists)
        {
            Destroy(slot.gameObject);
            return;
        }

        bool jelly1Exists = jellyInstances.ContainsKey(0) && jellyInstances[0] != null;
        bool jelly2Exists = jellyInstances.ContainsKey(1) && jellyInstances[1] != null;
        bool jelly3Exists = jellyInstances.ContainsKey(2) && jellyInstances[2] != null;
        bool jelly4Exists = jellyInstances.ContainsKey(3) && jellyInstances[3] != null;

        int existingJellyCount = (jelly1Exists ? 1 : 0) + (jelly2Exists ? 1 : 0) +
                                 (jelly3Exists ? 1 : 0) + (jelly4Exists ? 1 : 0);

        if (existingJellyCount == 1 && !isSingle)
        {
            GameObject singleJelly = null;

            if (jelly1Exists)
                singleJelly = jellyInstances[0];
            else if (jelly2Exists)
                singleJelly = jellyInstances[1];
            else if (jelly3Exists)
                singleJelly = jellyInstances[2];
            else if (jelly4Exists)
                singleJelly = jellyInstances[3];

            if (singleJelly != null)
            {
                JellyMesh jellyScript = singleJelly.GetComponent<JellyMesh>();
                if (jellyScript != null)
                {
                    singleJelly.transform.localScale = new Vector3(1.0f, 1.0f, 0.5f);
                    Vector3 newLocalPosition = singleJelly.transform.localPosition;
                    newLocalPosition.y = 0;
                    singleJelly.transform.localPosition = newLocalPosition;
                    isSingle = true;
                }
            }
        }
        else if (!hasScaled)
        {
            if (!jelly1Exists && !jelly2Exists && jelly3Exists && jelly4Exists)
            {

                JellyMesh jelly3Script = jellyInstances[2].GetComponent<JellyMesh>();
                JellyMesh jelly4Script = jellyInstances[3].GetComponent<JellyMesh>();

                if (jelly3Script != null && jelly4Script != null)
                {
                    jelly3Script.inverse = true;
                    jelly4Script.inverse = true;
                    jelly3Script.Resize(0.5f, "y");
                    jelly4Script.Resize(0.5f, "y");
                    hasScaled = true;
                }
            }
            else if (!jelly3Exists && !jelly4Exists && jelly1Exists && jelly2Exists)
            {

                JellyMesh jelly1Script = jellyInstances[0].GetComponent<JellyMesh>();
                JellyMesh jelly2Script = jellyInstances[1].GetComponent<JellyMesh>();

                if (jelly1Script != null && jelly2Script != null)
                {
                    jelly1Script.Resize(0.5f, "y");
                    jelly2Script.Resize(0.5f, "y");
                    hasScaled = true;
                }
            }
        }
    }

    void Update()
    {
        CheckJellyStatus();
    }
}
