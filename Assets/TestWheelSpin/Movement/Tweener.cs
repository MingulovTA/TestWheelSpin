using System;
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
            if (_tweensd.ContainsKey(transform))
            {
                _tweensd[transform].Kill();
                _tweensd.Remove(transform);
            }
            _tweens.Add(new Tween(transform,targetPosition,time,true, onComplete, OnComplete));
        }
        
        private Dictionary<Transform,Tween> _tweensd = new Dictionary<Transform, Tween>();

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
        
        public static float MoveTowardsValue(float current, float target, float maxDistanceDelta)
        {
            float num1 = target - current;
            float num4 = num1 *  num1;
            if ( Math.Abs(num4) < 0.001f || maxDistanceDelta >= 0.0 && num4 <= maxDistanceDelta * maxDistanceDelta)
                return target;
            float num5 = (float) Math.Sqrt(num4);
            return current + num1 / num5 * maxDistanceDelta;
        }
        
        public static float GetAngleBetweenPoints(Vector2 start, Vector2 arrival)
        {
            var radian = Math.Atan2((arrival.y - start.y), (arrival.x - start.x));
            var angle = (radian * (180 / Math.PI) + 360) % 360;
            return (float)angle;
        }
    }
}
