using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TestWheelSpin.Core;
using TestWheelSpin.Gameplay.Settings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TestWheelSpin.Gameplay
{
    public class WheelGameplay : MonoBehaviour
    {
        [SerializeField] private WheelSettings _wheelSettings;
        [Space(20)]
        [SerializeField] private SimpleButton _exitButton;
        [SerializeField] private GameObject _inputLocker;
        [SerializeField] private WheelBranch _branchReference;
        [SerializeField] private Ball _ballPrefab;
        [SerializeField] private Transform _wheelTransform;

        private List<WheelBranch> _brances = new List<WheelBranch>();
        private List<BallNode> _nodeGrapth = new List<BallNode>();

        private float _halfGravityAngle;

        private void Awake()
        {
            _branchReference.Disable();
            _halfGravityAngle = _wheelSettings.GravityAngle/2f;
        }

        private void EraseAllBranches()
        {
            foreach (var wheelBranch in _brances)
            {
                Destroy(wheelBranch.gameObject);
            }
            _brances.Clear();
        }

        private void GenerateBranches()
        {
            for (int i = 0; i < _wheelSettings.BranchCount; i++)
            {
                WheelBranch newBranch = Instantiate(_branchReference, _branchReference.transform.parent);
                newBranch.Enable();
                newBranch.transform.localEulerAngles = Vector3.forward*(i*360/_wheelSettings.BranchCount);
                _brances.Add(newBranch);
            }
        }

        private void BuildNodeGraph()
        {
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
        }

        private void FillGraphWithBalls()
        {
            foreach (var wheelBranch in _brances)
            {
                foreach (var wheelBranchNode in wheelBranch.Nodes)
                {
                    wheelBranchNode.Ball = Instantiate(_ballPrefab, wheelBranchNode.transform);

                    wheelBranchNode.Ball.Init(
                        wheelBranchNode.ColorId,
                        BallPaletteFactory.GetColor(_wheelSettings.BallsPalettes,wheelBranchNode.ColorId), 
                        _wheelSettings.BallMovementSpeed,BallPressedHandler,BallReleasedHandler);
                    _nodeGrapth.Add(wheelBranchNode);
                }
            }
        }
        
        private void RemoveExcessBalls()
        {
            for (int i = 0; i < _wheelSettings.EmptyNodeCount; i++)
            {
                var filledNodes = _nodeGrapth.Where(n => n.Ball != null).ToList();
                int randomNodeIndex = Random.Range(0, filledNodes.Count - 1);
                Debug.Log($"REMOVE {filledNodes[randomNodeIndex].Ball.CurrentColorId} BALL");
                Destroy(filledNodes[randomNodeIndex].Ball.gameObject);
                filledNodes[randomNodeIndex] = null;
            }
        }
        private void RebuildBranches()
        {
            GenerateBranches();
            BuildNodeGraph();
            FillGraphWithBalls();
            RemoveExcessBalls();

            foreach (var wheelBranch in _brances)
            {
                wheelBranch.Circle.OnStartRotating += StartCircleRotatingHandler;
                wheelBranch.Circle.OnStopRotating += CompleteCircleRotatingHandler;
            }
            
        }

        private Coroutine _pressedBallCoroutine;
        private void BallPressedHandler(Ball pressedBall)
        {
            _nearestNode = null;
            BallNode ballNode = _nodeGrapth.FirstOrDefault(n => n.Ball == pressedBall);
            List<BallNode> nearestFreeNodes = new List<BallNode>();
            foreach (var ballNodeNearestNode in ballNode.NearestNodes)
            {
                if (ballNodeNearestNode.Ball == null)
                    nearestFreeNodes.Add(ballNodeNearestNode);
            }

            if (nearestFreeNodes.Count == 0)
            {
                //Проиграть анимацию Shake
                return;
            }

            ballNode.Ball = null;
            nearestFreeNodes.Add(ballNode);
            _pressedBallCoroutine = StartCoroutine(BallPressedMovement(pressedBall,nearestFreeNodes));
            
        }

        private BallNode _nearestNode = null;
        private IEnumerator BallPressedMovement(Ball pressedBall, List<BallNode> nearestFreeNodes)
        {
            Vector3 inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float minDistanceToNearestNode = float.PositiveInfinity;
            BallNode lastNearestNode = null;
            while (true)
            {
                inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                minDistanceToNearestNode = float.PositiveInfinity;
                foreach (var nearestFreeNode in nearestFreeNodes)
                {
                    float distance = Vector2.Distance(nearestFreeNode.transform.position, inputPosition);
                    if (distance < minDistanceToNearestNode)
                    {
                        _nearestNode = nearestFreeNode;
                        minDistanceToNearestNode = distance;
                    }
                }


                if (_nearestNode != lastNearestNode)
                {
                    pressedBall.transform.SetParent(_nearestNode.transform);
                    pressedBall.TryToMoveToNode();
                }
                lastNearestNode = _nearestNode;
                yield return null;
            }
        }

        private void BallReleasedHandler(Ball releasedBall)
        {
            if (_nearestNode==null)
                return;
            _nearestNode.Ball = releasedBall;
            releasedBall.TryToMoveToNode();
            StopCoroutine(_pressedBallCoroutine);
            RecalcBallsPositions();
        }

        private void StartCircleRotatingHandler()
        {
            _inputLocker.gameObject.SetActive(true);
        }
        
        private void CompleteCircleRotatingHandler()
        {
            _inputLocker.gameObject.SetActive(false);
            RecalcBallsPositions();
        }
        public void Show()
        {
            RebuildBranches();
            gameObject.SetActive(true);
            _exitButton.Disable();
            _inputLocker.gameObject.SetActive(true);
            _wheelTransform.localPosition = Vector3.down*20;
            ProjectContext.I.Tweener.LocalMoveTo(_wheelTransform,Vector3.zero,1,OnShowComplete);
        }

        private void OnShowComplete()
        {
            _exitButton.Enable();
            _inputLocker.gameObject.SetActive(false);
        }

        public void Hide()
        {
            _wheelTransform.localPosition = Vector3.zero;
            ProjectContext.I.Tweener.LocalMoveTo(_wheelTransform,Vector3.down*20,1,OnHideComplete);
            _exitButton.Disable();
            _inputLocker.gameObject.SetActive(true);
            foreach (var wheelBranch in _brances)
            {
                wheelBranch.Circle.OnStartRotating -= StartCircleRotatingHandler;
                wheelBranch.Circle.OnStopRotating -= CompleteCircleRotatingHandler;
            }
        }
        
        private void OnHideComplete()
        {
            EraseAllBranches();
            _inputLocker.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        public void HideMomentary()
        {
            EraseAllBranches();
            _wheelTransform.localPosition = Vector3.down*20;
            _exitButton.Disable();
            _inputLocker.gameObject.SetActive(false);
            gameObject.SetActive(false);
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
            _wheelTransform.localEulerAngles += Vector3.forward * (newAngle-_currentAngle);
            _currentAngle = newAngle;
            RecalcBallsPositions();
        }
        
        private float CalculeAngle(Vector2 start, Vector2 arrival)
        {
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
                    if (ballNodeNearestNode.Ball == null && 
                        angle > 270 - _halfGravityAngle &&
                        angle < 270 + _halfGravityAngle)
                    {
                        ballNodeNearestNode.Ball = ballNode.Ball;
                        ballNode.Ball = null;
                        ballNodeNearestNode.Ball.transform.SetParent(ballNodeNearestNode.transform);
                        ballNodeNearestNode.Ball.TryToMoveToNode();
                    }
                }
            }

            if (GameCompleted)
            {
                Debug.Log("ИГРА ОКОНЧЕНА!");
                Hide();
            }
        }

        private bool GameCompleted
        {
            get
            {
                foreach (var wheelBranch in _brances)
                {
                    foreach (var wheelBranchNode in wheelBranch.Nodes)
                    {
                        if (wheelBranchNode.Ball==null) continue;
                        if (wheelBranchNode.Ball.CurrentColorId != wheelBranchNode.ColorId)
                            return false;
                    }
                }

                return true;
            }
        }
    }
}
