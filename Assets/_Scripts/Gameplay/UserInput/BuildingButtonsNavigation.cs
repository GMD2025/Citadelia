using System;
using System.Linq;
using _Scripts.Systems.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace _Scripts.Gameplay.UserInput
{
    public class BuildingButtonsNavigation : MonoBehaviour
    {
        [SerializeField] private GameObject buildingButtonContent;
        [SerializeField] private Button leftArrow;
        [SerializeField] private Button rightArrow;

        private void Start()
        {
            ButtonGenerator.OnButtonLoad += WhenButtonReady;
        }

        private void WhenButtonReady()
        {
            Debug.Log("HUI HUevich");
            var buttons = buildingButtonContent.GetComponentsInChildren<Button>();
            var leftNav = leftArrow.navigation;
            leftNav.mode = Navigation.Mode.Explicit;
            leftNav.selectOnRight = buttons.First();
            leftArrow.navigation = leftNav;

            var rightNav = rightArrow.navigation;
            rightNav.mode = Navigation.Mode.Explicit;
            rightNav.selectOnLeft = buttons.Last();
            rightArrow.navigation = rightNav;
        }
    }
}