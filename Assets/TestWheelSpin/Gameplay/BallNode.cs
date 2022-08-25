using System.Collections.Generic;
using UnityEngine;

namespace TestWheelSpin.Gameplay
{
    public class BallNode : MonoBehaviour
    {
        [SerializeField] private ColorId _colorId;
        [SerializeField] private List<BallNode> _nearestNodes;

        public List<BallNode> NearestNodes => _nearestNodes;
        public ColorId ColorId => _colorId;

        public Ball Ball { get; set; }
    }
}
