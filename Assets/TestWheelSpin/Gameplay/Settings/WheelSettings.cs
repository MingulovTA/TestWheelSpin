using System;
using System.Collections.Generic;
using UnityEngine;

namespace TestWheelSpin.Gameplay.Settings
{
    [Serializable]
    public class WheelSettings
    {
        [SerializeField] private float _wheelRotationMaxSpeed = 5;
        [SerializeField] private int _ballMovementSpeed = 5;
        [SerializeField] private float _gravityAngle = 45;
        [SerializeField] private int _branchCount = 6;
        [SerializeField] private int _emptyNodeCount = 2;
        [SerializeField] private List<BallsPalette> _ballsPalettes;

        public int EmptyNodeCount => _emptyNodeCount;
        public int BranchCount => _branchCount;
        public float GravityAngle => _gravityAngle;
        public int BallMovementSpeed => _ballMovementSpeed;
        public float WheelRotationMaxSpeed => _wheelRotationMaxSpeed;
        public List<BallsPalette> BallsPalettes => _ballsPalettes;
    }
}
