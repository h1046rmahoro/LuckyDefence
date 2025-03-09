using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace SB
{
    public class SpawnZone : MonoBehaviour
    {
        #region Spawn

        /// <summary>
        /// 몬스터 부모 오브젝트 
        /// </summary>
        [SerializeField]
        private Transform monsterHolder = null;

        /// <summary>
        /// 몬스터 오브젝트 풀 
        /// </summary>
        private Dictionary<int, List<Monster>> monsterObjectPool = new Dictionary<int, List<Monster>>();

        /// <summary>
        /// 타이머 오브젝트 
        /// </summary>
        [SerializeField]
        private GameObject goTimer = null;

        /// <summary>
        /// 타이머 텍스트 
        /// </summary>
        [SerializeField]
        private TextMeshPro txtTimer = null;

        /// <summary>
        /// 1초 대기 
        /// </summary>
        private WaitForSeconds WFS_ONE = new WaitForSeconds(1.0f);
        
        /// <summary>
        /// 몬스터 생성 
        /// </summary>
        /// <param name="prefabMonster"> 몬스터 프리팹  </param>
        /// <param name="route"> 몬스터 이동 경로 </param>
        /// <returns> 소환 된 몬스터  </returns>
        public Monster SpawnMonster(Monster prefabMonster, List<Transform> route)
        {
            // 오브젝트 풀 리스트 추가 
            if (!monsterObjectPool.ContainsKey(prefabMonster.Id))
            {
                monsterObjectPool.Add(prefabMonster.Id, new List<Monster>());
            }

            // 오브젝트 풀 검사 
            var skills = monsterObjectPool[prefabMonster.Id];
            foreach (var poolObject in skills)
            {
                if (poolObject.gameObject.activeSelf)
                    continue;

                // 몬스터 활성화 
                poolObject.gameObject.SetActive(true);
                poolObject.transform.position = transform.position;
                poolObject.Action();

                return poolObject;
            }
            
            // 새로운 몬스터 생성 
            GameObject go = Instantiate(prefabMonster.gameObject, transform.position, Quaternion.identity, monsterHolder);
            Monster monster = go.GetComponent<Monster>();
            monster.SetRoute(route);
            monster.Action();
            
            // 오브젝트 풀 등록 
            monsterObjectPool[monster.Id].Add(monster);
            
            return monster;
        }

        /// <summary>
        /// 현재 활성화 중인 몬스터  
        /// </summary>
        /// <returns> 활성화 몬스터 리스트 </returns>
        public List<Monster> GetActiveMonster()
        {
            List<Monster> monsters = new List<Monster>();

            foreach (var monsterList in monsterObjectPool.Values)
            {
                foreach (Monster monster in monsterList)
                {
                    // 죽은 몬스터 포함하지 않음 
                    if(monster.IsDead)
                        continue;

                    // 비활성 몬스터 포함하지 않음 
                    if (!monster.gameObject.activeSelf)
                        continue;
                    
                    // 몬스터 추가 
                    monsters.Add(monster);
                }
            }

            return monsters;
        }

        /// <summary>
        /// 보스 생존여부 체크 
        /// </summary>
        /// <returns> 보스 생존여부 </returns>
        public bool IsBossAlive()
        {
            // 현재 활성 몬스터 리스트 
            var monsters = GetActiveMonster();
            
            // 몬스터 체크 
            foreach (Monster monster in monsters)
            {
                // 보스 몬스터 체크 
                if (monster.IsBoss)
                {
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// 타이머 시작 
        /// </summary>
        public void StartTimer()
        {
            StartCoroutine(Timer());
        }

        /// <summary>
        /// 타이머 제어 
        /// </summary>
        private IEnumerator Timer()
        {
            goTimer.SetActive(true);
            for (int i = 4; i > 0; --i)
            {
                txtTimer.text = $"{i}";
                yield return new WaitForSeconds(1f);
            }

            goTimer.SetActive(false);
        }

        #endregion
    }
}
