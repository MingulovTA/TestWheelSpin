using System;
using System.Collections;
using UnityEngine;

namespace TestWheelSpin.Gameplay
{
    public class Circle : MonoBehaviour
    {
        [SerializeField] private BallNode _node1;
        [SerializeField] private BallNode _node2;

        private bool _rotatingInProgress;
        private Transform _transform;
        private Vector3 _currentAngle;

        public Transform Transform
        {
            get
            {
                if (_transform == null)
                    _transform = transform;
                return _transform;
            }
        }

        public event Action OnStartRotating;
        public event Action OnStopRotating;
        private void OnMouseDown()
        {
            if (_rotatingInProgress) return;
            Rotate();
        }

        private void Rotate()
        {
            OnStartRotating?.Invoke();
            _rotatingInProgress = true;
            //transform.DOLocalRotate(transform.localEulerAngles + Vector3.forward * 180, 1,RotateMode.FastBeyond360).OnComplete(SwapNodes);
            StartCoroutine(Rotate180());
        }

        private IEnumerator Rotate180()
        {
            float currentAngleZ = Transform.localEulerAngles.z;
            float targetAngleZ = currentAngleZ + 180;
            while (Math.Abs(currentAngleZ - targetAngleZ) > 0.01f)
            {
                yield return null;
                currentAngleZ = MoveTowardsValue(currentAngleZ, targetAngleZ, Time.deltaTime*300f);
                _currentAngle.z = currentAngleZ;
                Transform.localEulerAngles = _currentAngle;
            }

            SwapNodes();
        }
        
        public float MoveTowardsValue(float current, float target, float maxDistanceDelta)
        {
            float num1 = target - current;
            float num4 = num1 *  num1;
            if ( Math.Abs(num4) < 0.001f || maxDistanceDelta >= 0.0 && num4 <= maxDistanceDelta * maxDistanceDelta)
                return target;
            float num5 = (float) Math.Sqrt(num4);
            return current + num1 / num5 * maxDistanceDelta;
        }

        private void SwapNodes()
        {
            _rotatingInProgress = false;
            Vector3 node1Position = _node1.transform.localPosition;
            _node1.transform.localPosition = _node2.transform.localPosition;
            _node2.transform.localPosition = node1Position;
            
            Ball tempBall = _node1.Ball;
            _node1.Ball = _node2.Ball;
            _node2.Ball = tempBall;

            if (_node1.Ball != null)
            {
                _node1.Ball.transform.SetParent(_node1.transform);
                _node1.Ball.transform.localPosition = Vector3.zero;
            }
            
            if (_node2.Ball != null)
            {
                _node2.Ball.transform.SetParent(_node2.transform);
                _node2.Ball.transform.localPosition = Vector3.zero;
            }
            OnStopRotating?.Invoke();
        }
    }
}
