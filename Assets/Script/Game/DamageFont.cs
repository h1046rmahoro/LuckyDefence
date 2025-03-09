using System.Collections;
using TMPro;
using UnityEngine;

namespace SB
{
    /// <summary>
    /// 데미지 표시 
    /// </summary>
    public class DamageFont : MonoBehaviour
    {
        #region Info

        [SerializeField]
        private TextMeshPro txtDamage = null;

        /// <summary>
        /// 데미지 입력 
        /// </summary>
        /// <param name="damage"> 표시할 데미지 </param>
        public void SetDamage(string damage)
        {
            txtDamage.text = damage;

            // 폰트 이동 연출 
            StartCoroutine(Move());
        }

        private IEnumerator Move()
        {
            Vector3 startPos = Vector3.zero;
            Vector3 targetPos = new Vector3(0, 0.2f, 0);
            float time = 0;
            const float DURATION = 0.6f;
            
            // 좌표 이동 
            while (time < DURATION)
            {
                time += Time.deltaTime;
                float progress = time / DURATION;
                
                Vector3 position = Vector3.Lerp(startPos, targetPos, progress);
                transform.localPosition = position;
                yield return null;
            }
            
            // 최종 좌표 설정 
            transform.localPosition = targetPos;
            gameObject.SetActive(false);
        }

        #endregion
    }
}
