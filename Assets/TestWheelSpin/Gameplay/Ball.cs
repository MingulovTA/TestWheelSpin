using System;
using System.Collections;
using UnityEngine;

namespace TestWheelSpin.Gameplay
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        private ColorId _currentColorId;
        private Coroutine _moveAnimation;
        private Action<Ball> _onPressed;
        private Action<Ball> _onReleased;

        private Transform _transform;
        private Transform Transform
        {
            get
            {
                if (_transform == null)
                    _transform = transform;
                return _transform;
            }
        }
        private float _movementSpeed;

        public ColorId CurrentColorId => _currentColorId;

        public void Init(ColorId newColorId, Color color, float movementSpeed, Action<Ball> onBallPressed, Action<Ball> onBallReleased)
        {
            _currentColorId = newColorId;
            _spriteRenderer.color = color;
            _movementSpeed = movementSpeed;
            _onPressed = onBallPressed;
            _onReleased = onBallReleased;
            Transform.localPosition = Vector3.zero;
        }
        public void TryToMoveToNode()
        {
            if (_moveAnimation!=null) return;
            _moveAnimation = StartCoroutine(ReturnToParentNodeAnimation());
        }

        public void SetParent(BallNode node)
        {
            Transform.SetParent(node.transform);
            Transform.localPosition = Vector3.zero;
        }

        private IEnumerator ReturnToParentNodeAnimation()
        {
            Transform.localScale = Vector3.one * 1.2f;
            while (Math.Abs(Vector3.Distance(Transform.localPosition, Vector3.zero)) > 0.01f)
            {
                Transform.localPosition =
                    Vector3.MoveTowards(
                        Transform.localPosition, 
                        Vector3.zero, 
                        Time.deltaTime * _movementSpeed);
                yield return null;
            }

            Transform.localScale = Vector3.one;
            _moveAnimation = null;
        }

        private void OnMouseDown()
        {
            if (Input.touchCount>1) return;
            Transform.localScale = Vector3.one * 1.2f;
            _onPressed?.Invoke(this);
        }

        private void OnMouseUp()
        {
            Transform.localScale = Vector3.one;
            _onReleased?.Invoke(this);
        }
    }
}
