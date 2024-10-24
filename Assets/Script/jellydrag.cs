using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyDrag : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    private Vector3 initialScale;
    private Vector3 originalPosition;
    private float jellyFactor = 1.2f;
    public bool canDrag = true;

    private GameObject currentBlock;

    public LayerMask blockLayer;

    void Start()
    {
        initialScale = transform.localScale;
        originalPosition = transform.position;
    }

    void Update()
    {
        if (Input.touchCount > 0 && canDrag)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane + 10f));

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.transform == transform)
                        {
                            isDragging = true;
                            offset = transform.position - touchPosition;
                            transform.localScale = initialScale * jellyFactor;
                        }
                    }
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        transform.position = touchPosition + offset;
                        CheckForBlockCollisionFromJelly();
                    }
                    break;

                case TouchPhase.Ended:
                    if (isDragging)
                    {
                        isDragging = false;
                        transform.localScale = initialScale;

                        if (currentBlock != null)
                        {
                            Block blockComponent = currentBlock.GetComponent<Block>();
                            if (blockComponent != null && blockComponent.isEmpty)
                            {
                                transform.position = currentBlock.transform.position + new Vector3(0, 0, -0.5f);
                                blockComponent.isEmpty = false;
                                ResetBlockColor(currentBlock);
                                transform.SetParent(currentBlock.transform);
                                Destroy(this);
                            }
                            else
                            {
                                transform.position = new Vector3(originalPosition.x, originalPosition.y, transform.position.z);
                            }
                        }
                        else
                        {
                            transform.position = originalPosition;
                        }

                        currentBlock = null;
                    }
                    break;
            }
        }
    }

    void CheckForBlockCollisionFromJelly()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.forward, out hit, Mathf.Infinity, blockLayer))
        {
            Debug.DrawRay(transform.position, Vector3.forward * 10, Color.green, 10.0f);

            if (((1 << hit.collider.gameObject.layer) & blockLayer) != 0)
            {
                if (currentBlock != null && currentBlock != hit.collider.gameObject)
                {
                    ResetBlockColor(currentBlock);
                }

                currentBlock = hit.collider.gameObject;
                HighlightBlock(currentBlock);
                Debug.Log("Jelly hit block in Layer: " + currentBlock.name);
            }
        }
        else
        {
            if (currentBlock != null)
            {
                ResetBlockColor(currentBlock);
                currentBlock = null;
            }

            Debug.Log("Jelly hit nothing.");
        }
    }

    void HighlightBlock(GameObject block)
    {
        block.GetComponent<SpriteRenderer>().color = Color.red;
    }

    void ResetBlockColor(GameObject block)
    {
        block.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void SetCanDrag(bool value)
    {
        canDrag = value;
    }
}
