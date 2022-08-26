using System;
using System.Collections.Generic;
using System.Linq;
using TestWheelSpin.Core;
using TestWheelSpin.Gameplay.Settings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TestWheelSpin.Gameplay
{
    public class BranchesBuilder:MonoBehaviour
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
            for (int i = 0; i < wheelSettings.EmptyNodeCount; i++)
            {
                var filledNodes = nodeGraph.Where(n => n.Ball != null).ToList();
                int randomNodeIndex = Random.Range(0, filledNodes.Count - 1);
                ProjectContext.I.Instantiate();
                Destroy(filledNodes[randomNodeIndex].Ball.gameObject);
                filledNodes[randomNodeIndex] = null;
            }
        }
    }
}
