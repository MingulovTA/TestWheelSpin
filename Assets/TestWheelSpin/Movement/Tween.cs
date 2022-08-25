using System;
using UnityEngine;

namespace TestWheelSpin.Movement
{
    public class Tween
    {
        private Transform _transform;
        private Vector3 _targetPosition;
        private bool _isLocal;
        private float _time;
        private Action<Tween> _completeTween;
        private Action _completeCallback;
        private Vector3 _velocity;
        private float _speed;
        private float _maxTime;

        public Transform Transform => _transform;

        private Vector3 CurrentPosition => _isLocal ? _transform.localPosition : _transform.position;
        private Vector3 _startCurrentPosition;
        public Tween(Transform transform, Vector3 targetPosition, float time, bool isLocal, Action completeCallback, Action<Tween> completeTween)
        {
            _time = time;
            _maxTime = time;
            _transform = transform;
            _targetPosition = targetPosition;
            _isLocal = isLocal;
            _completeCallback = completeCallback;
            _completeTween = completeTween;
            _speed = Vector3.Distance(CurrentPosition, _targetPosition) / _time;
            _startCurrentPosition = CurrentPosition;
            _velocity = (_targetPosition - _startCurrentPosition)/_maxTime;
            
        }

        public void Kill()
        {
            _completeCallback?.Invoke();
            _completeTween?.Invoke(this);
        }

        public void Update()
        {
            Debug.Log("Update");
            //_speed = Vector3.Distance(CurrentPosition, _targetPosition) / _time;
            //_velocity = ((_targetPosition - _startCurrentPosition) * _time) / _maxTime;

            if (_isLocal)
                _transform.localPosition += _velocity*Time.deltaTime;
            else
                _transform.position += _velocity*Time.deltaTime;

            _time -= Time.deltaTime;
            if (_time < 0)
            {
                if (_isLocal)
                    _transform.localPosition = _targetPosition;
                else
                    _transform.position = _targetPosition;
                Kill();
            }
        }
    }
}