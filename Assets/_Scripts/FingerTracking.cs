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
            // mobile
            if (Input.touchCount > 0)
            {
                touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
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