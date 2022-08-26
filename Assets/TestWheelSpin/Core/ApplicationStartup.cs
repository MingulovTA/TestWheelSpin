using System;
using TestWheelSpin.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TestWheelSpin.Core
{
    public class ApplicationStartup : MonoBehaviour
    {
        [SerializeField] private PopupGameplay _wheelGameplay;

        private ScreenOrientation _currentScreenOrientation;
        private void Start()
        {
            _currentScreenOrientation = Screen.orientation;
            RebuildScreenOrientation();
            _wheelGameplay.HideMomentary();
        }
        private void OnMouseDown()
        {
            _wheelGameplay.Show();
        }

        private void Update()
        {
            if (_currentScreenOrientation != Screen.orientation)
                RebuildScreenOrientation();
        }

        private void RebuildScreenOrientation()
        {
            _currentScreenOrientation = Screen.orientation;
            float aspectRation = (float)Screen.width / Screen.height;
            if ( aspectRation < 1)
            {
                _wheelGameplay.transform.localScale = Vector3.one * aspectRation;
            }
            else
            {
                _wheelGameplay.transform.localScale = Vector3.one;
            }
        }
        
    }
}
