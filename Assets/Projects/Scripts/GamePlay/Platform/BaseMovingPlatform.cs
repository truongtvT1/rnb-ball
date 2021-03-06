using System;
using System.Collections.Generic;
using DG.Tweening;
using ThirdParties.Truongtv.SoundManager;
using UnityEngine;

namespace GamePlay.Platform
{
    [ExecuteAlways]
    public class BaseMovingPlatform : MonoBehaviour
    {
        [SerializeField] protected Platform platform;
        [SerializeField] protected Transform pointParent;
        [SerializeField] protected LineRenderer lineRenderer;
        [SerializeField] protected float speed;
        [SerializeField] protected List<Transform> points;
        protected int CurrentPoint;

        
        private void Awake()
        {
            Init();
        }

        protected Transform GetTarget()
        {
            return points[0];
        }
        
        protected void Move(bool ignoreTimeScale = false,Action onComplete = null,Action onUpdate = null)
        {
            if (CurrentPoint >= points.Count - 1)
            {
                onComplete?.Invoke();
                return;
            }

            var distance = Vector2.Distance(platform.transform.position, points[CurrentPoint + 1].position);
            var time = distance / speed;
            Debug.Log("Move time = "+time);
            platform.transform
                .DOMove(points[CurrentPoint + 1].position, time)
                .SetEase(Ease.Linear)
                .SetUpdate(UpdateType.Fixed,ignoreTimeScale)
                .OnUpdate(()=>onUpdate?.Invoke())
                .OnComplete(() => { Move(ignoreTimeScale,onComplete); });
            CurrentPoint++;

        }


        protected void ReserveMove(bool ignoreTimeScale = false,Action onComplete = null,Action onUpdate = null)
        {

            if (CurrentPoint == 0)
            {
                onComplete?.Invoke();
                return;
            }

            var distance = Vector2.Distance(platform.transform.position, points[CurrentPoint - 1].position);
            var time = distance / speed;
            Debug.Log("ReserveMove time = "+time);
            
            platform.transform
                .DOMove(points[CurrentPoint - 1].position, time)
                .SetEase(Ease.Linear)
                .SetUpdate(UpdateType.Fixed,ignoreTimeScale)
                .OnUpdate(()=>onUpdate?.Invoke())
                .OnComplete(() => { ReserveMove(ignoreTimeScale,onComplete); });
            CurrentPoint--;
        }

        private void Init()
        {
            CurrentPoint = 0;
            points = new List<Transform>();
            var positionList = new List<Vector3>();
            foreach (Transform child in pointParent)
            {
                points.Add(child);
                positionList.Add(child.position);
            }

            if (points.Count <= 1) return;
            platform.transform.position = points[0].position;
            lineRenderer.positionCount = positionList.Count;
            lineRenderer.SetPositions(positionList.ToArray());
            lineRenderer.useWorldSpace = true;
        }
        void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Init();
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                UnityEditor.SceneView.RepaintAll();
            }
#endif
        }
    }
}