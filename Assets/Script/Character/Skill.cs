using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SB
{
    public class Skill : MonoBehaviour
    {
        #region Info

        /// <summary>
        /// 스킬을 시전 한 캐릭터 
        /// </summary>
        public Character Character = null;

        // 스킬 발동 확률 
        [SerializeField]
        private int gravity = 1;
        
        public int Gravity => gravity;
        
        /// <summary>
        /// 공격 사거리 
        /// </summary>
        [SerializeField]
        private float attackRange = 1.0f;
        
        /// <summary>
        /// 공격 사거리 제곱 
        /// </summary>
        private float sqrAttackRange = 1.0f;

        
        /// <summary>
        /// 데미지 배율 
        /// </summary>
        [SerializeField]
        private float damageRate = 1.0f;
        
        /// <summary>
        /// 반복 횟수 
        /// </summary>
        [SerializeField]
        private int repeatCount = 1;
        
        /// <summary>
        /// 반복 지연 시간 
        /// </summary>
        [SerializeField]
        private float delayTime = 0.0f;
        
        /// <summary>
        /// 스킬 적용 최대 대상 수 
        /// </summary>
        [SerializeField]
        private int targetCount = 0;
        
        

        #endregion

        [SerializeField]
        private int skillId = 0;
        /// <summary>
        /// 스킬 고유 아이디 
        /// </summary>
        public int Id => skillId;
        
        /// <summary>
        /// 스킬의 타입 
        /// </summary>
        public enum SkillType
        {
            Instant,    // 단발형 스킬 
            Dot,        // 디버프형 스킬 
            Area,       // 설치형 스킬 
            Buff,       // 아군 버프 스킬 
        }

        /// <summary>
        /// 스킬 시전 위치 
        /// </summary>
        public enum SkillPosType
        {
            Self,       // 내 위치 
            Target,     // 대상 위치 
        }
        
        /// <summary>
        /// 스킬 시전 위치 
        /// </summary>
        public SkillPosType posType = SkillPosType.Self;
        
        /// <summary>
        /// 현재 스킬의 종류 
        /// </summary>
        public SkillType skillType = SkillType.Instant;

        #region Action

        /// <summary>
        /// 스킬 작동 시작 
        /// </summary>
        public void Action()
        {
            sqrAttackRange = attackRange * attackRange;
            StartCoroutine(Attack());
        }


        // 타겟 획득

        protected List<Monster> GetTarget(int count = int.MaxValue)
        {
            var monsters = InGame.Instance.FindAttackRangeMonster(transform.position, sqrAttackRange);

            // 목록보다 대상 수가 더 적은 경우 거리별 계산 
            if (count < monsters.Count)
            {
                // 거리별 정렬 
                monsters.Sort((Monster a, Monster b) =>
                {
                    Vector3 pos = transform.position;

                    float distanceA = Vector3.SqrMagnitude(a.transform.position - pos);
                    float distanceB = Vector3.SqrMagnitude(b.transform.position - pos);

                    return distanceA.CompareTo(distanceB);
                });
            }

            // 목표 대상 선별 
            List<Monster> monsterList = new List<Monster>();
            for (int i = 0; i < count && i < monsters.Count; ++i)
            {
                monsterList.Add(monsters[i]);
            }

            return monsterList;
        }
        
        // 공격 

        private IEnumerator Attack()
        {
            WaitForSeconds delay = null;
            
            // 반복 횟수만큼 반복 
            for (int i = 0; i < repeatCount; ++i)
            {
                // 스킬 대상 획득 
                var monsters = GetTarget(targetCount);
                
                // 공격 
                foreach (Monster monster in monsters)
                {
                    monster.Hit(Character.Damage * damageRate);
                }

                // 딜레이 시간 생성 
                if (delay == null && delayTime > 0)
                    delay = new WaitForSeconds(delayTime);

                yield return delay;
            }
            
            // 오브젝트 비활성 
            gameObject.SetActive(false);
        }
        
        #endregion
    }
}
