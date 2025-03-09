using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SB
{
    /// <summary>
    /// 곡선 이동 
    /// </summary>
    public class MoveBezier : MonoBehaviour
    {
        /// <summary>
        /// 오브젝트 이동 
        /// </summary>
        /// <param name="go"> 이동 할 오브젝트 </param>
        /// <param name="pathList"> 이동 경로 포인트 </param>
        /// <param name="time"> 이동 시간 </param>
        /// <param name="callback"> 이동 완료 후 콜백 </param>
        public static void StartMove(GameObject go, Vector3[] pathList, float time, Action callback = null)
        {
            var move = go.AddComponent<MoveBezier>();
            move.path = pathList;
            move.duration = time;
            move.callback = callback;
            move.StartCoroutine(move.MoveObject());
        }

        /// <summary>
        /// 목표 좌표 
        /// </summary>
        private Vector3[] path = null;

        /// <summary>
        /// 이동 시간 
        /// </summary>
        private float duration = 1.0f;
        
        /// <summary>
        /// 콜백 
        /// </summary>
        private Action callback = null;
        
        /// <summary>
        /// 이동 코루틴 
        /// </summary>
        /// <returns></returns>
        private IEnumerator MoveObject()
        {
            Vector3 startPos = transform.position;
            float time = 0;
            
            // 좌표 이동 
            while (time < duration)
            {
                time += Time.deltaTime;
                float progress = time / duration;
                
                // 현재 위치 
                Vector3 position = Bezier.GetPosition(path, progress);
                
                transform.position = position;
                
                yield return null;
            }
            
            // 최종 좌표 설정 
            transform.position = path[^1];
            
            
            // 콜백 실행 
            if(callback != null)
                callback.Invoke();

            Destroy(this);
        }

    }
}
