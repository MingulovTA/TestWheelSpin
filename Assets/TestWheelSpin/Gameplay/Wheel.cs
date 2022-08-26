using System;
using TestWheelSpin.Movement;
using UnityEngine;

namespace TestWheelSpin.Gameplay
{
    public class Wheel : MonoBehaviour
    {
        [SerializeField] private float _hiddenYoffset = -20;
        private Transform _transform;
        private float _currentAngle;
        private Action _rotateCallback;
        public Transform Transform
        {
            get
            {
                if (_transform == null)
                    _transform = transform;
                return _transform;
            }
        }

        public void Init(Action rotateCallback)
        {
            _rotateCallback = rotateCallback;
        }

        private void OnMouseDown()
        {
            _currentAngle = Tweener.GetAngleBetweenPoints(transform.position,Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        
        private void OnMouseDrag()
        {
            float newAngle = Tweener.GetAngleBetweenPoints(transform.position,Camera.main.ScreenToWorldPoint(Input.mousePosition));
            Transform.localEulerAngles += Vector3.forward * (newAngle-_currentAngle);
            _currentAngle = newAngle;
            _rotateCallback?.Invoke();
        }
    }
}
