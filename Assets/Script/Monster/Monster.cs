using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace SB
{
    public class Monster : MonoBehaviour
    {
        #region Info

        /// <summary>
        /// 몬스터 고유 아이디 
        /// </summary>
        [SerializeField]
        private int monsterId = 0;
        
        /// <summary>
        /// 몬스터 고유 아이디 
        /// </summary>
        public int Id => monsterId;

        /// <summary>
        /// 몬스터 사망여부 체크 
        /// </summary>
        public bool IsDead => Hp <= 0;

        [SerializeField]
        private float hp = 0;
        
        /// <summary>
        /// 현재 체력 
        /// </summary>
        public float Hp
        {
            get => hp;
            set
            {
                hp = Mathf.Clamp(value, 0, hpMax);
                RefreshHpBar();
                if(hp <= 0)
                    Dead();
            }
        }

        /// <summary>
        /// 최대 체력 
        /// </summary>
        [SerializeField]
        private float hpMax = 100;
        
        /// <summary>
        /// 이동 속도 
        /// </summary>
        [SerializeField]
        private float moveSpeed = 1.0f;
        
        /// <summary>
        /// 몬스터 이동 경로 
        /// </summary>
        [SerializeField]
        private List<Transform> monsterRoute = new List<Transform>();

        /// <summary>
        /// 이동 타겟 인덱스 
        /// </summary>
        private int routeIndex = 0;
        
        private Vector3 targetPos = Vector3.zero;

        /// <summary>
        /// 보스 몬스터 여부 
        /// </summary>
        [SerializeField]
        private bool isBoss = false;
        
        /// <summary>
        /// 보스 몬스터 여부 
        /// </summary>
        public bool IsBoss => isBoss;

        /// <summary>
        /// 몬스터 이동경로 설정 
        /// </summary>
        /// <param name="route"></param>
        public void SetRoute(List<Transform> route)
        {
            // 경로 추가 
            monsterRoute.Clear();
            monsterRoute.AddRange(route);
            
            // 첫번째 타겟 설정 
            routeIndex = 0;
            targetPos = route[routeIndex].position;
        }

        /// <summary>
        /// 다음 이동목표 설정 
        /// </summary>
        private void SetNextTarget()
        {
            // 방향에 맞게 좌 우 전환 
            if (routeIndex < 2)
                sprTop.transform.localScale = new Vector3(-1, 1, 1);
            else
                sprTop.transform.localScale = new Vector3(1, 1, 1);
            
            // 인덱스 증가 
            ++routeIndex;
            
            // 다음 인덱스가 없는경우 첫번째로 설정 
            if (routeIndex >= monsterRoute.Count)
                routeIndex = 0;
            
            // 목표 위치 설정 
            targetPos = monsterRoute[routeIndex].position;

        }

        #endregion
        

        #region Animation

        /// <summary>
        /// 몬스터 애니메이션 상태 종류 
        /// </summary>
        public enum AnimationState
        {
            Idle,
            Run,
            Death,
            Stun,
            AttackSword,
            AttackBow,
            AttackMagic,
            SkillSword,
            SkillBow,
            SkillMagic
        }

        /// <summary>
        /// 몬스터 애니메이션 제어 
        /// </summary>
        [SerializeField]
        private SPUM_Prefabs aniCharacter = null;

        /// <summary>
        /// 좌우 이동 표시를 위한 Transform
        /// </summary>
        [SerializeField]
        private Transform sprTop = null;

        private void SetAnimation(AnimationState state)
        {
            aniCharacter.PlayAnimation((int)state);
        }
        #endregion


        #region Action

        /// <summary>
        /// 몬스터 작동 시작 
        /// </summary>
        public void Action()
        {
            // 난이도에 따른 체력 증가 설정 
            int wave = InGame.Instance.WaveLevel;
            hpMax = hpMax * Mathf.Pow(1.1f, wave);
            hp = hpMax;
            RefreshHpBar();
            
            // 루트 초기화 
            routeIndex = 0;
            targetPos = monsterRoute[routeIndex].position;
            
            SetAnimation(AnimationState.Run);
        }

        private void Update()
        {
            // 사망한 몬스터는 이동 할 수 없음 
            if (IsDead)
                return;
            
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            float sqrDistance = Vector3.SqrMagnitude(targetPos - transform.position);
            
            if(sqrDistance < 0.0001f)
                SetNextTarget();
        }

        /// <summary>
        /// 데미지 가격 
        /// </summary>
        /// <param name="damage"> 가한 데미지 </param>
        /// <returns> 사망 여부 true - 사망 </returns>
        public bool Hit(float damage)
        {
            if (IsDead)
                return true;
            
            Hp -= damage;
            
            // 데미지 표시 
            Vector3 damagePos = transform.position + new Vector3(0, 0.5f, 0);
            ShowDamageFont(damagePos, $"{damage}");
            return Hp <= 0;
        }

        /// <summary>
        /// 몬스터 사망 처리 
        /// </summary>
        private void Dead()
        {
            // 몬스터 카운트 감소 
            InGame.Instance.MonsterCount -= 1;
            
            // 보상 지급
            if (isBoss)
            {
                // 골드 증가
                InGame.Instance.Gold += 50;
                InGame.Instance.AiGold += 50;
                
                // 젬 증가 
                InGame.Instance.Gem += 2;
            }
            else
            {
                // 골드 증가
                InGame.Instance.Gold += 1;
                InGame.Instance.AiGold += 1;
            }

            // 사망 연출 재생 
            SetAnimation(AnimationState.Death);

            // 오브젝트 비활성 
            StartCoroutine(DisableMonster());
        }

        private IEnumerator DisableMonster()
        {
            yield return new WaitForSeconds(1.0f);

            gameObject.SetActive(false);
        }

        #endregion


        #region Visualize
        
        [SerializeField]
        private SpriteRenderer sprHp = null;

        /// <summary>
        /// 데미지 폰트 프리팹 
        /// </summary>
        [SerializeField]
        private GameObject goDamageFont = null;

        /// <summary>
        /// 데미지 폰트 위치 
        /// </summary>
        [SerializeField]
        private Transform damageFontParent = null;
        
        /// <summary>
        /// 데미지 폰트 오브젝트 풀 
        /// </summary>
        private List<DamageFont> damageFontPool = new List<DamageFont>();

        
        public void RefreshHpBar()
        {
            float sizeX = (Hp / (float)hpMax) * 0.47f;
            sprHp.size = new Vector2(sizeX, 0.06f);
        }
        
        /// <summary>
        /// 입은 데미지 보여주기 
        /// </summary>
        /// <param name="position"> 폰트 노출 위치  </param>
        /// <param name="damage"> 입은 데미지 </param>
        public void ShowDamageFont(Vector3 position, string damage)
        {
            var font = GetDamageFont();
            font.transform.position = position;
            font.SetDamage(damage);
        }

        /// <summary>
        /// 데미지 폰트 생성 
        /// </summary>
        /// <returns> 생성 된 데미지 폰트 </returns>
        private DamageFont GetDamageFont()
        {
            foreach (DamageFont damageFont in damageFontPool)
            {
                if (damageFont.gameObject.activeSelf)
                    continue;
                
                damageFont.gameObject.SetActive(true);
                return damageFont;
            }

            GameObject go = Instantiate(goDamageFont, damageFontParent);
            DamageFont font = go.GetComponent<DamageFont>();
            
            return font;
        }

        #endregion
    }
}
