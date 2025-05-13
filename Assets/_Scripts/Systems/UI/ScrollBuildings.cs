using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Systems.UI
{
    public class ScrollBuildings : MonoBehaviour
    {
        [SerializeField] private Button arrowLeft;
        [SerializeField] private Button arrowRight;
        [SerializeField] private RectTransform content;
    
        private int currentIndex = 0;
    
        void Awake()
        {
            arrowLeft.onClick.AddListener(ScrollLeft);
            arrowRight.onClick.AddListener(ScrollRight);
        }

        void ScrollLeft()
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                UpdatePosition();
            }
        }
    
        void ScrollRight()
        {
            if (currentIndex < content.childCount - 1)
            {
                currentIndex++;
                UpdatePosition();
            }
        }

    
        private void UpdatePosition()
        {
            float newX = -currentIndex * 80;
            content.anchoredPosition = new Vector2(newX, content.anchoredPosition.y);
            UpdateButtons();
        }
    
        private void UpdateButtons()
        {
            arrowLeft.interactable = currentIndex > 0;
            arrowRight.interactable = currentIndex < content.childCount - 1;
        }
    
    }
}
