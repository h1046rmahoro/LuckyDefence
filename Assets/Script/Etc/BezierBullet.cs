using System;
using System.Collections;
using UnityEngine;

namespace SB
{
    /// <summary>
    /// 곡선 투사체 이동 
    /// </summary>
    public class BezierBullet : MonoBehaviour
    {
        /// <summary>
        /// 오브젝트 이동 
        /// </summary>
        /// <param name="go"> 이동 할 오브젝트 </param>
        /// <param name="target"> 목표지점 position </param>
        /// <param name="time"> 이동 시간 </param>
        /// <param name="callback"> 이동 완료 후 콜백 </param>
        public static void StartMove(GameObject go, Transform target, float time, Action callback = null)
        {
            var move = go.AddComponent<BezierBullet>();
            move.target = target;
            move.duration = time;
            move.callback = callback;
            move.StartCoroutine(move.MoveObject());
        }
        
        /// <summary>
        /// 목표 좌표 
        /// </summary>
        private Transform target = null;

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
                
                // 중간 점 계산 
                float x = (startPos.x + target.position.x) * 0.5f;
                float y = Mathf.Max(startPos.y, target.position.y);
                Vector3 mid = new  Vector3(x, y, 0);
                
                // 경로 생성 
                Vector3[] path = new Vector3[3];
                path[0] = startPos;
                path[1] = mid;
                path[2] = target.position;

                // 현재 위치 
                Vector3 position = Bezier.GetPosition(path, progress);
                // 각도 조정을 위한 다음 예상 위치 
                Vector3 nextPos = Bezier.GetPosition(path, progress + 0.05f);
                
                // 투사체 각도 조정 
                float dx = nextPos.x - position.x;
                float dy = nextPos.y - position.y;
                float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
                
                transform.position = position;
                
                yield return null;
            }
            
            // 최종 좌표 설정 
            transform.position = target.position;
            
            
            // 콜백 실행 
            if(callback != null)
                callback.Invoke();

            Destroy(this);
        }

    }
}
