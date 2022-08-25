using UnityEngine;

namespace TestWheelSpin.Gameplay
{
    public class BallNode : MonoBehaviour
    {
        [SerializeField] private ColorId _colorId;

        public ColorId ColorId => _colorId;

        public Ball Ball { get; set; }
    }
}
