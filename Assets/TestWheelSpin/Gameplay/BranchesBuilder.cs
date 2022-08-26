using System;
using System.Collections.Generic;
using System.Linq;
using TestWheelSpin.Core;
using TestWheelSpin.Gameplay.Settings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TestWheelSpin.Gameplay
{
    public class BranchesBuilder : MonoBehaviour
    {
        public static List<WheelBranch> GenerateBranches(WheelSettings wheelSettings, WheelBranch branchPrefab)
        {
            List<WheelBranch> branches = new List<WheelBranch>();
            for (int i = 0; i < wheelSettings.BranchCount; i++)
            {
                WheelBranch newBranch = Instantiate(branchPrefab, branchPrefab.transform.parent);
                newBranch.Enable();
                newBranch.transform.localEulerAngles = Vector3.forward*(i*360f/wheelSettings.BranchCount);
                branches.Add(newBranch);
            }

            return branches;
        }

        public static List<BallNode> BuildNodeGraph(List<WheelBranch> branches)
        {
            List<BallNode> nodeGraph = new List<BallNode>();
            for (var i = 0; i < branches.Count; i++)
            {
                if (i == branches.Count-1)
                {
                    branches[i].Nodes[0].NearestNodes.Add(branches[0].Nodes.First());
                }
                else
                {
                    branches[i].Nodes[0].NearestNodes.Add(branches[i+1].Nodes[0]);
                }
                
                if (i == 0)
                {
                    branches[i].Nodes[0].NearestNodes.Add(branches[branches.Count-1].Nodes.First());
                }
                else
                {
                    branches[i].Nodes[0].NearestNodes.Add(branches[i-1].Nodes[0]);
                }
                foreach (var ballNode in branches[i].Nodes)
                {
                    nodeGraph.Add(ballNode);
                }
            }

            return nodeGraph;
        }

        public static void FillGraphWithBalls(List<WheelBranch> branches, Ball ballPrefab, WheelSettings wheelSettings, Action<Ball> ballPressedHandler, Action<Ball> ballReleasedHandler)
        {
            foreach (var wheelBranch in branches)
            {
                foreach (var wheelBranchNode in wheelBranch.Nodes)
                {
                    wheelBranchNode.Ball = Instantiate(ballPrefab, wheelBranchNode.transform);

                    wheelBranchNode.Ball.Init(
                        wheelBranchNode.ColorId,
                        BallPaletteFactory.GetColor(wheelSettings.BallsPalettes,wheelBranchNode.ColorId), 
                        wheelSettings.BallMovementSpeed,ballPressedHandler,ballReleasedHandler);
                }
            }
        }
        
        public static void RemoveExcessBalls(WheelSettings wheelSettings, List<BallNode> nodeGraph)
        {
            int removedBallCounter = 0;

            List<BallNode> firstLevelNodes =
                nodeGraph.Where(ng => ng.NearestNodes.Count > 2 && ng.Ball != null).ToList();

            foreach (var firstLevelNode in firstLevelNodes)
            {
                if (removedBallCounter == wheelSettings.EmptyNodeCount)
                    break;
                Destroy(firstLevelNode.Ball.gameObject);
                firstLevelNode.Ball = null;
                removedBallCounter++;
            }
        }

        public static void ShuffleBalls(List<BallNode> nodeGraph)
        {
            for (int i = 0; i < 10; i++)
            {
                BallNode randomNode1 = nodeGraph[Random.Range(0, nodeGraph.Count - 1)];
                BallNode randomNode2 = nodeGraph[Random.Range(0, nodeGraph.Count - 1)];
                SwapBalls(randomNode1, randomNode2);
            }
        }

        private static void SwapBalls(BallNode node1, BallNode node2)
        {
            var tampBall = node1.Ball;
            node1.Ball = node2.Ball;
            node2.Ball = tampBall;
            
            if (node1.Ball!=null)
                node1.Ball.SetParent(node1);
            if (node2.Ball!=null)
                node2.Ball.SetParent(node2);
        }
    }
}
