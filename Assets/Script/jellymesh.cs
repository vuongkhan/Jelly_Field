using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyMesh : MonoBehaviour
{
    public float scaleMultiplier = 1.5f; 
    public GameObject connectObject;      
    public int jellyID;                   
    public float resizeAmount = 0.5f;    
    public string resizeDirection = "x"; 
    public bool inverse = false;          

    // Biến cho các đặc tính Jelly
    public float Intensity = 1f;
    public float Mass = 1f;
    public float stiffness = 1f;
    public float damping = 0.7f;
    public bool isUse = false;

    private Mesh originalMesh, meshClone;
    private MeshRenderer meshRenderer;
    private JellyVertex[] jellyVertices;
    private Vector3[] vertexArray;
    private GameManager gameManager; 
    private LevelManager levelManager; 

    void Start()
    {
        originalMesh = GetComponent<MeshFilter>().sharedMesh;
        meshClone = Instantiate(originalMesh);
        GetComponent<MeshFilter>().sharedMesh = meshClone;

        meshRenderer = GetComponent<MeshRenderer>();

        jellyVertices = new JellyVertex[meshClone.vertices.Length];
        vertexArray = new Vector3[meshClone.vertices.Length];

        for (int i = 0; i < meshClone.vertices.Length; i++)
        {
            jellyVertices[i] = new JellyVertex(i, transform.TransformPoint(meshClone.vertices[i]));
        }
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>(); 
        }
        GetComponent<Collider>().isTrigger = true; 

        gameManager = FindObjectOfType<GameManager>();
        levelManager = FindObjectOfType<LevelManager>();
    }
    public void Resize(float amount, string direction)
    {
        float scaleOffset = amount / 2f;

        if (direction == "x")
        {
            float positionOffsetX = inverse ? -scaleOffset : scaleOffset;
            transform.position += new Vector3(positionOffsetX, 0, 0);
            transform.localScale += new Vector3(amount, 0, 0);
            ApplyJellyEffect();
        }
        else if (direction == "y")
        {
            float positionOffsetY = inverse ? -scaleOffset : scaleOffset;
            transform.position += new Vector3(0, positionOffsetY, 0);
            transform.localScale += new Vector3(0, amount, 0);
            ApplyJellyEffect();
        }
    }

    private void ApplyJellyEffect()
    {
        for (int i = 0; i < jellyVertices.Length; i++)
        {
            Vector3 target = transform.TransformPoint(vertexArray[jellyVertices[i].ID]);
            float intensity = (1 - (meshRenderer.bounds.max.y - target.y) / meshRenderer.bounds.size.y) * Intensity;
            jellyVertices[i].Shake(target, Mass, stiffness, damping);
            target = transform.InverseTransformPoint(jellyVertices[i].Position);
            vertexArray[jellyVertices[i].ID] = Vector3.Lerp(vertexArray[jellyVertices[i].ID], target, intensity);
        }

        meshClone.vertices = vertexArray;
        meshClone.RecalculateNormals();
    }
    private IEnumerator MergeAndDestroy(JellyMesh otherJelly)
    {

        float mergeDuration = 1.0f;
        float elapsedTime = 0f;

        Vector3 initialPosition = transform.position;
        Vector3 otherInitialPosition = otherJelly.transform.position;
        Vector3 mergePosition = (initialPosition + otherInitialPosition) / 2f;

        while (elapsedTime < mergeDuration)
        {
            float t = Mathf.SmoothStep(0, 1, elapsedTime / mergeDuration);
            transform.position = Vector3.Lerp(initialPosition, mergePosition, t);
            otherJelly.transform.position = Vector3.Lerp(otherInitialPosition, mergePosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = mergePosition;
        otherJelly.transform.position = mergePosition;
        Color mergedColor = meshRenderer.material.color;
        int count = 2; 
        FindObjectOfType<LevelManager>().RecordDestroyedJellyColor(mergedColor, count);
        Destroy(otherJelly.gameObject);
        Destroy(this.gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        JellyMesh otherJelly = other.GetComponent<JellyMesh>();
        if (otherJelly != null && otherJelly != this)
        {
            StartCoroutine(HandleCollisionWithDelay(otherJelly));
        }
    }

    private IEnumerator HandleCollisionWithDelay(JellyMesh otherJelly)
    {
        yield return new WaitForSeconds(0.5f);

        Color currentColor = meshRenderer.material.color;
        Color otherColor = otherJelly.GetComponent<MeshRenderer>().material.color;

        float colorTolerance = 0.1f; 
        if (Mathf.Abs(currentColor.r - otherColor.r) < colorTolerance &&
            Mathf.Abs(currentColor.g - otherColor.g) < colorTolerance &&
            Mathf.Abs(currentColor.b - otherColor.b) < colorTolerance)
        {
            StartCoroutine(MergeAndDestroy(otherJelly));

        }
    }

    private void OnDestroy()
    {
        if (connectObject != null && connectObject.activeInHierarchy)
        {
            JellyMesh connectedJelly = connectObject.GetComponent<JellyMesh>();
            if (connectedJelly != null && connectedJelly.gameObject != this.gameObject)
            {

                connectedJelly.ScaleOnConnectedDestroyed();
            }
        }
        else
        {
            Debug.Log("Error");
        }
    }

    public void ScaleOnConnectedDestroyed()
    {
        inverse = (jellyID == 2 || jellyID == 4);
        Resize(resizeAmount, resizeDirection);
        SoundManager.instance.PlaySoundEffect("Destroy");
    }

    void FixedUpdate()
    {
        vertexArray = originalMesh.vertices;

        for (int i = 0; i < jellyVertices.Length; i++)
        {
            Vector3 target = transform.TransformPoint(vertexArray[jellyVertices[i].ID]);
            float intensity = (1 - (meshRenderer.bounds.max.y - target.y) / meshRenderer.bounds.size.y) * Intensity;
            jellyVertices[i].Shake(target, Mass, stiffness, damping);
            target = transform.InverseTransformPoint(jellyVertices[i].Position);
            vertexArray[jellyVertices[i].ID] = Vector3.Lerp(vertexArray[jellyVertices[i].ID], target, intensity);
        }

        meshClone.vertices = vertexArray;
        meshClone.RecalculateNormals();
    }
}

public class JellyVertex
{
    public int ID;
    public Vector3 Position;
    private Vector3 velocity, force;

    public JellyVertex(int _id, Vector3 _pos)
    {
        ID = _id;
        Position = _pos;
    }

    public void Shake(Vector3 target, float mass, float stiffness, float damping)
    {
        force = (target - Position) * stiffness;
        velocity = (velocity + force / mass) * damping;
        Position += velocity;

        if ((velocity + force / mass).magnitude < 0.001f)
        {
            Position = target;
        }
    }
}
