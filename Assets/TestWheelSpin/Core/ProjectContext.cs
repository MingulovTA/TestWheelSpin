using System;
using TestWheelSpin.Movement;
using UnityEngine;

namespace TestWheelSpin.Core
{
    public class ProjectContext : MonoBehaviour
    {
        //Классический синглтон, обычно юзаю через наследование с дженериком от базового или ещё лучше - Zenject
        #region ClassicSingletonMonoBehaviour

        private static ProjectContext _instance;

        public static ProjectContext I
        {
            get
            {
                /*if (_instance == null)
                {
                    GameObject gm = GameObject.Find("ProjectContext");
                    if (gm != null)
                        _instance = gm.GetComponent<ProjectContext>();
                }*/

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if(_instance!=null && _instance!=this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            _tweener = new Tweener(this);
        }

        #endregion

        private Tweener _tweener;


        public Tweener Tweener => _tweener;

        private void LateUpdate()
        {
            _tweener.Update();
        }
    }
}
