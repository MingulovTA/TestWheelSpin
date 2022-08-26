using System;
using TestWheelSpin.Movement;
using UnityEngine;

namespace TestWheelSpin.Gameplay
{
    public class Wheel : MonoBehaviour
    {
        private Transform _transform;
        private float _lastAngle;
        private Vector3 _currentEulerAngles;
        private Action _rotateCallback;
        private float _rotationSpeed;
        private bool _rotatingInProgress;
        private float _targetAngleZ;

        private Transform Transform
        {
            get
            {
                if (_transform == null)
                    _transform = transform;
                return _transform;
            }
        }

        public void Init(Action rotateCallback, float rotationSpeed)
        {
            _rotateCallback = rotateCallback;
            _rotationSpeed = rotationSpeed;
        }

        private void OnMouseDown()
        {
            _lastAngle = Tweener.GetAngleBetweenPoints(Transform.position,Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        
        private void OnMouseDrag()
        {
            float newAngle = Tweener.GetAngleBetweenPoints(transform.position,Camera.main.ScreenToWorldPoint(Input.mousePosition));
            
            if (_lastAngle > 270 && _lastAngle < 360 && newAngle > 0 && newAngle < 90)
                _lastAngle = newAngle;
            
            if (newAngle > 270 && newAngle < 360 && _lastAngle > 0 && _lastAngle < 90)
                _lastAngle = newAngle;
            
            Transform.localEulerAngles += Vector3.forward * (newAngle-_lastAngle)*_rotationSpeed;
            _lastAngle = newAngle;
            _rotateCallback?.Invoke();
        }
    }
}
