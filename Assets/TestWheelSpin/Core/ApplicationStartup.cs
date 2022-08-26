using System;
using TestWheelSpin.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TestWheelSpin.Core
{
    public class ApplicationStartup : MonoBehaviour
    {
        [SerializeField] private PopupGameplay _wheelGameplay;

        private int _screenWidth;
        private int _screenHeight;
        private void Start()
        {
            RebuildScreenOrientation();
            _wheelGameplay.HideMomentary();
            
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;
            InvokeRepeating(nameof(CheckScreenSizeLoop),0.5f,0.5f);
        }
        private void OnMouseDown()
        {
            _wheelGameplay.Show();
        }

        private void CheckScreenSizeLoop()
        {
            if (Screen.width!=_screenWidth||Screen.height!=_screenHeight)
                RebuildScreenOrientation();
        }

        private void RebuildScreenOrientation()
        {
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;
            float aspectRation = (float)_screenWidth / _screenHeight;
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
