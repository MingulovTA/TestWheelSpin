using System;
using System.Collections.Generic;
using TestWheelSpin.Gameplay.Settings;
using TestWheelSpin.Movement;

namespace TestWheelSpin.Gameplay
{
    public static class SimpleBallsPhysic
    {
        public static void RebuildBallsPositions(List<BallNode> nodeGrapth, WheelSettings wheelSettings)
        {
            Tuple<BallNode,BallNode> nodeWithUnstableBall = GetNodeWithUnstableBall(nodeGrapth, wheelSettings);
            
            while (nodeWithUnstableBall!=null)
            {
                MoveUnstableBall(nodeWithUnstableBall);
                nodeWithUnstableBall = GetNodeWithUnstableBall(nodeGrapth, wheelSettings);
            }
        }

        private static Tuple<BallNode,BallNode> GetNodeWithUnstableBall(List<BallNode> nodeGrapth, WheelSettings wheelSettings)
        {
            foreach (var ballNode in nodeGrapth)
            {
                if (ballNode.Ball==null) 
                    continue;
                foreach (var ballNodeNearestNode in ballNode.NearestNodes)
                {
                    float angle = Tweener.GetAngleBetweenPoints(ballNode.transform.position,
                        ballNodeNearestNode.transform.position);
                    if (ballNodeNearestNode.Ball == null && 
                        angle > 270 - wheelSettings.GravityAngle/2f &&
                        angle < 270 + wheelSettings.GravityAngle/2f)
                    {
                        return new Tuple<BallNode, BallNode>(ballNode,ballNodeNearestNode);
                    }
                }
            }

            return null;
        }

        private static void MoveUnstableBall(Tuple<BallNode,BallNode> nodeWithUnstableBall)
        {
            nodeWithUnstableBall.Item2.Ball = nodeWithUnstableBall.Item1.Ball;
            nodeWithUnstableBall.Item1.Ball = null;
            nodeWithUnstableBall.Item2.Ball.transform.SetParent(nodeWithUnstableBall.Item2.transform);
            nodeWithUnstableBall.Item2.Ball.TryToMoveToNode();
        }
    }
}
