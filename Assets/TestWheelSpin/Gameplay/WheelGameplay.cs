using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TestWheelSpin.Core;
using TestWheelSpin.Gameplay.Settings;
using TestWheelSpin.Movement;
using UnityEngine;

namespace TestWheelSpin.Gameplay
{
    public class WheelGameplay : PopupGameplay
    {
        [SerializeField] private WheelSettings _wheelSettings;
        [Space(20)]
        [SerializeField] private WheelBranch _branchReference;
        [SerializeField] private Ball _ballPrefab;
        [SerializeField] private Wheel _wheel;
        private List<WheelBranch> _brances = new List<WheelBranch>();
        private List<BallNode> _nodeGrapth = new List<BallNode>();
        private bool _isGameCompleted;

        private void Awake()
        {
            _branchReference.Disable();
            _wheel.Init(RecalcBallsPositions,_wheelSettings.WheelRotationMaxSpeed);
        }

        private void EraseAllBranches()
        {
            foreach (var wheelBranch in _brances)
            {
                wheelBranch.Circle.OnStartRotating -= StartCircleRotatingHandler;
                wheelBranch.Circle.OnStopRotating -= CompleteCircleRotatingHandler;
            }
            
            foreach (var wheelBranch in _brances)
                Destroy(wheelBranch.gameObject);
            _brances.Clear();
        }

        private void RebuildBranches()
        {
            _brances = BranchesBuilder.GenerateBranches(_wheelSettings, _branchReference);
            _nodeGrapth = BranchesBuilder.BuildNodeGraph(_brances);
            
            BranchesBuilder.FillGraphWithBalls(_brances,_ballPrefab,_wheelSettings,BallPressedHandler,BallReleasedHandler);
            BranchesBuilder.RemoveExcessBalls(_wheelSettings, _nodeGrapth);
            BranchesBuilder.ShuffleBalls(_nodeGrapth);
            
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

            if (nearestFreeNodes.Count != 0)
            {
                ballNode.Ball = null;
                nearestFreeNodes.Add(ballNode);
                _pressedBallCoroutine = StartCoroutine(BallPressedMovement(pressedBall,nearestFreeNodes));
            }
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
            LockInput();
        }
        
        private void CompleteCircleRotatingHandler()
        {
            UnlockInput();
            RecalcBallsPositions();
        }

        protected override void OnShowStart()
        {
            base.OnShowStart();
            _isGameCompleted = false;
            RebuildBranches();
        }
        
        
        protected override void OnHideComplete()
        {
            base.OnHideComplete();
            EraseAllBranches();
        }

        protected override void OnHideMomentary()
        {
            EraseAllBranches();
        }

        private void RecalcBallsPositions()
        {
            foreach (var ballNode in _nodeGrapth)
            {
                if (ballNode.Ball==null) 
                    continue;
                foreach (var ballNodeNearestNode in ballNode.NearestNodes)
                {
                    float angle = Tweener.GetAngleBetweenPoints(ballNode.transform.position,
                        ballNodeNearestNode.transform.position);
                    if (ballNodeNearestNode.Ball == null && 
                        angle > 270 - _wheelSettings.GravityAngle/2f &&
                        angle < 270 + _wheelSettings.GravityAngle/2f)
                    {
                        ballNodeNearestNode.Ball = ballNode.Ball;
                        ballNode.Ball = null;
                        ballNodeNearestNode.Ball.transform.SetParent(ballNodeNearestNode.transform);
                        ballNodeNearestNode.Ball.TryToMoveToNode();
                    }
                }
            }

            if (GameCompleted&&!_isGameCompleted)
            {
                _isGameCompleted = true;
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
