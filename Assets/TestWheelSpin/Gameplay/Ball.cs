using System;
using System.Collections.Generic;
using UnityEngine;

namespace TestWheelSpin.Gameplay
{
    public class Ball : MonoBehaviour
    {
        private readonly Dictionary<ColorId,Color32> _ballsPalletes = new Dictionary<ColorId, Color32>
        {
            {ColorId.Black, new Color32(128,128,128,255)},
            {ColorId.Blue, new Color32(80,100,255,255)},
            {ColorId.Green, new Color32(140,255,105,255)},
            {ColorId.Cyan, new Color32(0,255,255,255)},
        };
        
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private ColorId _currentColorId;

        public void SetColor(ColorId newColorId)
        {
            _currentColorId = newColorId;
            _spriteRenderer.color = _ballsPalletes[newColorId];
        }

        private Vector3 _cubeVelocity;
        private void Update()
        {
            _cubeVelocity = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
                _cubeVelocity += Vector3.up;
            if (Input.GetKey(KeyCode.S))
                _cubeVelocity += Vector3.down;
            if (Input.GetKey(KeyCode.A))
                _cubeVelocity += Vector3.left;
            if (Input.GetKey(KeyCode.D))
                _cubeVelocity += Vector3.right;
            _cubeVelocity *= Time.deltaTime*4;
            transform.Translate(_cubeVelocity);
        }
    }
}
