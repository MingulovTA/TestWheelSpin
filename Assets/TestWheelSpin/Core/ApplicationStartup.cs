using System;
using TestWheelSpin.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TestWheelSpin.Core
{
    public class ApplicationStartup : MonoBehaviour
    {
        [SerializeField] private WheelGameplay _wheelGameplay;

        private void Start()
        {
            _wheelGameplay.HideMomentary();
        }
        private void OnMouseDown()
        {
            _wheelGameplay.Show();
        }
    }
}
