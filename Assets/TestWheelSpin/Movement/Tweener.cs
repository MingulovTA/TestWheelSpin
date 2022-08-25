using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TestWheelSpin.Movement
{
    public class Tweener
    {
        private MonoBehaviour _monoBehaviour;
        private List<Tween> _tweens = new List<Tween>();
        
        public Tweener(MonoBehaviour monoBehaviour)
        {
            _monoBehaviour = monoBehaviour;
            //_monoBehaviour.StartCoroutine(Update());
        }

        public void Update()
        {
            for (var i = _tweens.Count-1; i >= 0; i--)
            {
                _tweens[i].Update();
            }
        }

        public void MoveTo(Transform transform, Vector3 targetPosition, float time = 0, Action onComplete = null)
        {
            Tween tween = _tweens.FirstOrDefault(t => t.Transform == transform);
            if (tween!=null)
                tween.Kill();
            _tweens.Add(new Tween(transform,targetPosition,time,false, onComplete, OnComplete));
        }
        
        public void LocalMoveTo(Transform transform, Vector3 targetPosition, float time = 0, Action onComplete = null)
        {
            _tweens.Add(new Tween(transform,targetPosition,time,true, onComplete, OnComplete));
        }

        private void OnComplete(Tween tween)
        {
            _tweens.Remove(tween);
        }

        public void Kill(Transform transform)
        {
            foreach (var tween in _tweens.Where(t=>t.Transform==transform))
            {
                tween.Kill();
            }
        }
    }
}
