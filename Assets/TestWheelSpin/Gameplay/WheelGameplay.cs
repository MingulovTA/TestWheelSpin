using System;
using System.Collections.Generic;
using TestWheelSpin.Core;
using UnityEngine;

namespace TestWheelSpin.Gameplay
{
    public class WheelGameplay : MonoBehaviour
    {
        [SerializeField] private SimpleButton _exitButton;
        [SerializeField] private GameObject _inputLocker;
        [SerializeField] private int _branchCount = 6;
        [SerializeField] private WheelBranch _branchReference;
        [SerializeField] private Ball _ballPrefab;

        private List<WheelBranch> _brances = new List<WheelBranch>();

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

            
            for (int i = 0; i <= _branchCount; i++)
            {
                WheelBranch newBranch = Instantiate(_branchReference, _branchReference.transform.parent);
                newBranch.Enable();
                newBranch.transform.localEulerAngles = Vector3.forward*(i*360/_branchCount);
                _brances.Add(newBranch);
            }
            
            foreach (var wheelBranch in _brances)
            {
                foreach (var wheelBranchNode in wheelBranch.Nodes)
                {
                    wheelBranchNode.Ball = Instantiate(_ballPrefab, wheelBranchNode.transform);
                    wheelBranchNode.Ball.SetColor( wheelBranchNode.ColorId);
                    wheelBranchNode.Ball.transform.localPosition = Vector3.zero;
                }
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
        
    
    }
}
