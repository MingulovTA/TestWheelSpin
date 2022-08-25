using System;
using System.Collections.Generic;
using System.Linq;
using TestWheelSpin.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TestWheelSpin.Gameplay
{
    public class WheelGameplay : MonoBehaviour
    {
        [SerializeField] private SimpleButton _exitButton;
        [SerializeField] private GameObject _inputLocker;
        [SerializeField] private int _branchCount = 6;
        [SerializeField] private WheelBranch _branchReference;
        [SerializeField] private Ball _ballPrefab;
        [SerializeField] private Transform _wheelTransform;

        private List<WheelBranch> _brances = new List<WheelBranch>();
        private List<BallNode> _nodeGrapth = new List<BallNode>();

        private void Awake()
        {
            _branchReference.Disable();
        }

        private void RebuildBranches()
        {
            foreach (var wheelBranch in _brances)
            {
                Destroy(wheelBranch.gameObject);
            }
            _brances.Clear();

            
            for (int i = 0; i < _branchCount; i++)
            {
                WheelBranch newBranch = Instantiate(_branchReference, _branchReference.transform.parent);
                newBranch.Enable();
                newBranch.transform.localEulerAngles = Vector3.forward*(i*360/_branchCount);
                _brances.Add(newBranch);
            }
            
            for (var i = 0; i < _brances.Count; i++)
            {
                if (i == _brances.Count-1)
                {
                    _brances[i].Nodes[0].NearestNodes.Add(_brances[0].Nodes.First());
                }
                else
                {
                    _brances[i].Nodes[0].NearestNodes.Add(_brances[i+1].Nodes[0]);
                }
                
                if (i == 0)
                {
                    _brances[i].Nodes[0].NearestNodes.Add(_brances[_brances.Count-1].Nodes.First());
                }
                else
                {
                    _brances[i].Nodes[0].NearestNodes.Add(_brances[i-1].Nodes[0]);
                }
                
            }
            
            foreach (var wheelBranch in _brances)
            {
                foreach (var wheelBranchNode in wheelBranch.Nodes)
                {
                    wheelBranchNode.Ball = Instantiate(_ballPrefab, wheelBranchNode.transform);
                    wheelBranchNode.Ball.SetColor( wheelBranchNode.ColorId);
                    wheelBranchNode.Ball.transform.localPosition = Vector3.zero;
                    _nodeGrapth.Add(wheelBranchNode);
                }
            }

            for (int i = 0; i <= 2; i++)
            {
                var filledNodes = _nodeGrapth.Where(n => n.Ball != null).ToList();
                int randomNodeIndex = Random.Range(0, filledNodes.Count - 1);
                Destroy(filledNodes[randomNodeIndex].Ball.gameObject);
                filledNodes[randomNodeIndex] = null;
            }
            
        }
        public void Show()
        {
            RebuildBranches();
            gameObject.SetActive(true);
            _exitButton.Disable();
            _inputLocker.gameObject.SetActive(true);
            transform.localPosition = Vector3.down*20;
            ProjectContext.I.Tweener.LocalMoveTo(transform,Vector3.zero,1,OnShowComplete);
        }

        private void OnShowComplete()
        {
            _exitButton.Enable();
            _inputLocker.gameObject.SetActive(false);
        }

        public void Hide()
        {
            transform.localPosition = Vector3.zero;
            ProjectContext.I.Tweener.LocalMoveTo(transform,Vector3.down*20,1);
            _exitButton.Disable();
            _inputLocker.gameObject.SetActive(true);
        }

        public void HideMomentary()
        {
            transform.localPosition = Vector3.down*20;
            _exitButton.Disable();
            _inputLocker.gameObject.SetActive(false);
        }
        private void OnEnable()
        {
            _exitButton.OnDown += ExitButtonClickHandler;
        }

        private void OnDisable()
        {
            _exitButton.OnDown -= ExitButtonClickHandler;
        }

        private void ExitButtonClickHandler()
        {
            Hide();
        }

        private void OnMouseDown()
        {
            _currentAngle = CalculeAngle(transform.position,Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        private float _currentAngle;
        private void OnMouseDrag()
        {
            float newAngle = CalculeAngle(transform.position,Camera.main.ScreenToWorldPoint(Input.mousePosition));
            Debug.Log(newAngle);
            _wheelTransform.localEulerAngles += Vector3.forward * (newAngle-_currentAngle);
            _currentAngle = newAngle;
            RecalcBallsPositions();
        }
        
        private float CalculeAngle(Vector2 start, Vector2 arrival)
        {
            var deltaX = Math.Pow((arrival.x - start.x), 2);
            var deltaY = Math.Pow((arrival.y - start.y), 2);

            var radian = Math.Atan2((arrival.y - start.y), (arrival.x - start.x));
            var angle = (radian * (180 / Math.PI) + 360) % 360;

            return (float)angle;
        }

        private void RecalcBallsPositions()
        {
            foreach (var ballNode in _nodeGrapth)
            {
                if (ballNode.Ball==null) 
                    continue;
                foreach (var ballNodeNearestNode in ballNode.NearestNodes)
                {
                    float angle = CalculeAngle(ballNode.transform.position,
                        ballNodeNearestNode.transform.position);
                    if (ballNodeNearestNode.Ball == null && angle> 270-45&&angle<270+45)
                    {
                        ballNodeNearestNode.Ball = ballNode.Ball;
                        ballNode.Ball = null;
                        ballNodeNearestNode.Ball.transform.SetParent(ballNodeNearestNode.transform);
                        Debug.Log("Local Move");
                        ProjectContext.I.Tweener.Kill(ballNodeNearestNode.Ball.transform);
                        ProjectContext.I.Tweener.LocalMoveTo(ballNodeNearestNode.Ball.transform,Vector3.zero,1);
                    } 
                }
            }
        }
    }
}
