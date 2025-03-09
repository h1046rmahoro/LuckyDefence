using System.Collections.Generic;
using UnityEngine;

namespace SB
{
    /// <summary>
    /// 마법사 캐릭터 
    /// </summary>
    public class CharacterMagician : Character
    {
        #region Effect

        [SerializeField]
        private GameObject goBullet = null;

        /// <summary>
        ///  투사체 오브젝트 풀 
        /// </summary>
        List<GameObject> bullets = new List<GameObject>();

        /// <summary>
        /// 기본 공격 이펙트 
        /// </summary>
        protected override void AttackEffect()
        {
            // 투사체 생성 
            var bullet = CreateBullet();
            bullet.transform.position = transform.position;
            
            // 거리 계산 
            float distance = Vector3.Distance(transform.position, targetMonster.transform.position);
            
            // 투사체 이동 
            MoveBullet.StartMove(bullet, targetMonster.transform, distance * 0.125f, () =>
            {
                // 도달 시 투사체 제거 
                bullet.SetActive(false);
            });
        }

        /// <summary>
        /// 투사체 생성 
        /// </summary>
        /// <returns> 투사체 </returns>
        private GameObject CreateBullet()
        {
            // 비활성 투사체 검사 
            foreach (var bullet in bullets)
            {
                if (!bullet.activeSelf)
                {
                    bullet.SetActive(true);
                    return bullet;
                }
            }
            
            // 비활성 투사체가 없는경우 새로운 투사체 생성 
            GameObject go = Instantiate(goBullet, transform);
            bullets.Add(go);
            
            return go;
        }

        #endregion
    }
}
