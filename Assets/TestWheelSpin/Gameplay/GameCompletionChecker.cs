using System;
using System.Collections.Generic;

namespace TestWheelSpin.Gameplay
{
    public static class GameCompletionChecker 
    {
        public static void CheckCompleteGame(List<WheelBranch> branches, Action completeCallback)
        {
            if (IsGameComplete(branches))
                completeCallback?.Invoke();
        }
        private static bool IsGameComplete(List<WheelBranch> branches)
        {
            foreach (var wheelBranch in branches)
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
