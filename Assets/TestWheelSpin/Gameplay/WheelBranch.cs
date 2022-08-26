using System.Collections.Generic;
using UnityEngine;

namespace TestWheelSpin.Gameplay
{
    public class WheelBranch : MonoBehaviour
    {
        [SerializeField] private List<BallNode> _nodes;
        [SerializeField] private Circle _circle;


        public Circle Circle => _circle;

        public List<BallNode> Nodes => _nodes;

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}
