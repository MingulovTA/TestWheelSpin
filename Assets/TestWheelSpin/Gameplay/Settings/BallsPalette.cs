using System;
using UnityEngine;

namespace TestWheelSpin.Gameplay.Settings
{
    [Serializable]
    public class BallsPalette
    {
        [SerializeField] private ColorId _colorId;
        [SerializeField] private Color _color;

        public Color Color => _color;
        public ColorId ColorId => _colorId;
    }
}
