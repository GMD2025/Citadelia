using UnityEngine;
using UnityEngine.EventSystems;

public class FingerTracking : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool isTracking = false;
    private GameObject lastHighlightedTile;
    
    void Update()
    {
        RaycastHit2D hit;
        Vector2 touchPos;

        if (isTracking)
        {
            // TODO: maybe instead of old input system it is better to use the newer one?
            // then you just create an action in "InputSystem Actions" and just use Pointer which handles both Mouse and Touch
            if (Input.touchCount > 0)
            {
                touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                // TODO: is better to persist camera and don't search for it every update loop iteration
                Debug.Log("Finger Position: " + touchPos);
            }
            // computer 
            else if (Input.GetMouseButton(0))
            {
                touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Debug.Log("Mouse Position: " + touchPos); 
            }
            else return;

            hit = Physics2D.Raycast(touchPos, Vector2.zero);
            if (hit.collider != null)
            {
                GameObject tile = hit.collider.gameObject;

                if (tile != lastHighlightedTile)
                {
                    ResetPreviousHighlight();
                    HighlightTile(tile);
                    lastHighlightedTile = tile;
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isTracking = true;
        Debug.Log(Input.touchCount);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isTracking = false;
    }
    
    // TEMP
    void HighlightTile(GameObject tile)
    {
        tile.GetComponent<SpriteRenderer>().color = Color.yellow; // Change highlight effect as needed
        // TODO: it is better to persist (cache) components and don't search for them every update loop iteration
    }

    void ResetPreviousHighlight()
    {
        if (lastHighlightedTile != null)
        {
            lastHighlightedTile.GetComponent<SpriteRenderer>().color = Color.white; // Reset to default color
            lastHighlightedTile = null;
        }
    }
}