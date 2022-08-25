using System;
using UnityEngine;

namespace TestWheelSpin.Core
{
    public class SimpleButton : MonoBehaviour
    {
        public event Action OnDown;
        public event Action OnUp;

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
        private void OnMouseDown()
        {
            OnDown?.Invoke();
        }

        private void OnMouseUp()
        {
            OnUp?.Invoke();
        }
    }
}
