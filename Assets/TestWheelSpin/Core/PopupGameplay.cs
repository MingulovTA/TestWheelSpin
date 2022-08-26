using UnityEngine;

namespace TestWheelSpin.Core
{
    public class PopupGameplay : MonoBehaviour
    {
        [Space(20)]
        [SerializeField] private float _hiddenYoffset = -20;
        [SerializeField] private Transform _popupTransform;
        [SerializeField] private SimpleButton _exitButton;
        [SerializeField] private GameObject _inputLocker;
        public void Show()
        {
            OnShowStart();
            gameObject.SetActive(true);
            _exitButton.Disable();
            LockInput();
            _popupTransform.localPosition = Vector3.up*_hiddenYoffset;
            ProjectContext.I.Tweener.LocalMoveTo(_popupTransform,Vector3.zero,1,OnShowComplete);
        }

        public void Hide()
        {
            _popupTransform.localPosition = Vector3.zero;
            ProjectContext.I.Tweener.LocalMoveTo(_popupTransform,Vector3.up*_hiddenYoffset,1,OnHideComplete);
            _exitButton.Disable();
            LockInput();
        }

        public void HideMomentary()
        {
            _popupTransform.localPosition = Vector3.up*_hiddenYoffset;
            _exitButton.Disable();
            UnlockInput();
            gameObject.SetActive(false);
            OnHideMomentary();
        }

        protected virtual void OnShowStart()
        {
        
        }

        protected virtual void OnShowComplete()
        {
            _exitButton.Enable();
            UnlockInput();
        }

        protected virtual void OnHideComplete()
        {
            UnlockInput();
            gameObject.SetActive(false);
        }
    
        protected virtual void OnHideMomentary()
        {
        
        }
    
        protected virtual void OnEnable()
        {
            _exitButton.OnDown += ExitButtonClickHandler;
        }

        protected virtual void OnDisable()
        {
            _exitButton.OnDown -= ExitButtonClickHandler;
        }

        protected void LockInput()
        {
            _inputLocker.gameObject.SetActive(true);
        }

        protected void UnlockInput()
        {
            _inputLocker.gameObject.SetActive(false);
        }
    
        private void ExitButtonClickHandler()
        {
            Hide();
        }
    }
}
