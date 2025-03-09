using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace SB
{
    /// <summary>
    /// 기본 캐릭터 
    /// </summary>
    public class Character : MonoBehaviour
    {
        #region Info

        /// <summary>
        /// 캐릭터 종류 
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// 캐릭터 없음 
            /// </summary>
            None = 0,
            
            // Normal
            NormalArcher,
            NormalWarrior,
            
            // Rare
            Rare = 10000,
            RareArcher,
            RareWarrior,
            
            // Hero
            Hero = 20000,
            HeroMagician,
            HeroWarrior,
            
            // Epic
            Epic = 30000,
            EpicMagician,
            EpicWarrior
        }
        
        /// <summary>
        /// 캐릭터 레어도
        /// </summary>
        [SerializeField]
        private Type grade = Type.None;
        
        public Type Grade => grade;

        /// <summary>
        /// 캐릭터 종류 
        /// </summary>
        [SerializeField]
        public Type CharacterType = Type.None;

        
        [SerializeField]
        private float damage = 1;

        /// <summary>
        /// 기본 데미지 
        /// </summary>
        public float OriginDamage => damage;
        
        /// <summary>
        /// 캐릭터 데미지 
        /// </summary>
        public float Damage
        {
            get
            {
                // 업그레이드 데미지 적용 
                int upgradeCount = 1;
                switch (Grade)
                {
                    case Type.None:
                    case Type.Rare:
                        upgradeCount = InGame.Instance.RareUpgradeCount;
                        break;
                    case Type.Hero:
                        upgradeCount = InGame.Instance.HeroUpgradeCount;
                        break;
                    case Type.Epic:
                        upgradeCount = InGame.Instance.EpicUpgradeCount;
                        break;
                }

                return damage * upgradeCount;
            }
        }

        [SerializeField]
        private float attackSpeed = 1.0f;
        /// <summary>
        /// 캐릭터 공격 속도 
        /// </summary>
        public float AttackSpeed => attackSpeed;

        [SerializeField]
        private float attackRange = 1.0f;
        /// <summary>
        /// 공격 사거리 
        /// </summary>
        public float AttackRange => attackRange;

        /// <summary>
        /// 몬스터 사거리 계산을 위한 제곱 사거리 
        /// </summary>
        [SerializeField]
        private float srqAttackRange = 1.0f;
        
        #endregion
        
        
        #region Action

        /// <summary>
        /// 공격 대상 몬스터 
        /// </summary>
        protected Monster targetMonster = null;

        /// <summary>
        /// 공격 속도에 따른 딜레이 
        /// </summary>
        private WaitForSeconds attackDelay = null;

        /// <summary>
        /// 캐릭터 상태 
        /// </summary>
        private enum State
        {
            Idle,       // 아이들 상태. 적 탐색
            Attack      // 공격행동. 적이 멀어진경우 아이들로 전환 
        }
        /// <summary>
        /// 현재 캐릭터 상태 
        /// </summary>
        [SerializeField]
        private State state = State.Idle;

        private void Start()
        {
            attackDelay = new WaitForSeconds(1 / AttackSpeed);
            srqAttackRange = attackRange * attackRange;
            
            // 캐릭터 생성 시 행동 시작 
            StartCoroutine(DefaultRoutine());
        }

        /// <summary>
        /// 캐릭터 행동 코루틴 
        /// </summary>
        protected IEnumerator DefaultRoutine() 
        {
            while (true)
            {
                // 캐릭터 행동 
                yield return Action();
            }
        }

        /// <summary>
        /// 캐릭터 행동 
        /// </summary>
        /// <returns> 다음 행동까지의 딜레이 </returns>
        private WaitForSeconds Action()
        {
            switch (state)
            {
                case State.Idle:
                    SearchTarget();
                    break;
                case State.Attack:
                    return Attack();
                    break;
            }

            return null;
        }

        /// <summary>
        /// Idle 상태로 전환 
        /// </summary>
        private void SetIdle()
        {
            state = State.Idle;
            targetMonster = null;
            SetAnimation(AnimationState.Idle);
        }

        /// <summary>
        /// 사거리 내 적 탐색 
        /// </summary>
        private void SearchTarget()
        {
            targetMonster = InGame.Instance.FindAttackTargetMonster(transform.parent.position, srqAttackRange);

            if (targetMonster != null)
                state = State.Attack;
        }
        
        /// <summary>
        /// 공격 행동 
        /// </summary>
        /// <returns> 다음 행동까지의 딜레이 </returns>
        private WaitForSeconds Attack()
        {
            // 사거리 계산. 멀어진 경우 idle 상태로 전환 
            if (!IsAttackAble())
            {
                SetIdle();
                return null;
            }

            bool isDead = false;

            int rnd = Random.Range(0, 100);

            Skill skill = activeSkill[0];

            if (rnd < skill.Gravity)
            {    // 스킬 공격
                
                // 스킬 애니메이션 재생
                SetAnimation(GetAttackAnimationState(true));
                
                // 스킬 시전 위치 설정 
                Vector3 pos = transform.position;
                
                // 대상 기준 스킬인경우 위치 변경 
                if(skill.posType == Skill.SkillPosType.Target)
                    pos = targetMonster.transform.position;
                
                skill = InGame.Instance.GetSkillObject(skill, this);
                
                // 스킬 위치 지정 
                skill.transform.position = pos;
                
                // 스킬 공격 
                skill.Action();
            }
            else
            {   // 기본 공격 
                // 공격 애니메이션 재생
                SetAnimation(GetAttackAnimationState(false));

                // 공격 
                isDead = targetMonster.Hit(Damage);

                // 기본 공격 이펙트
                AttackEffect();
            }
            
            float dx = targetMonster.transform.position.x - transform.position.x;
            if (dx > 0)
            {
                sortingGroup.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                sortingGroup.transform.localScale = new Vector3(1, 1, 1);
            }

            if(isDead)
                SetIdle();

            return attackDelay;
        }

        /// <summary>
        /// 공격 사거리 체크  
        /// </summary>
        /// <returns> true - 공격 가능 </returns>
        private bool IsAttackAble()
        {
            // 대상이 없는경우 공격 불가 
            if (targetMonster == null)
                return false;
            
            // 몬스터가 죽은경우 공격 불가 
            if(targetMonster.IsDead)
                return false;
            
            // 대상 위치 
            Vector3 target = targetMonster.transform.position;
            float sqrDistance = (target - transform.parent.position).sqrMagnitude;

            return sqrDistance <= srqAttackRange;
        }

        private AnimationState GetAttackAnimationState(bool isSkill)
        {
            switch (CharacterType)
            {
                case Type.NormalArcher:
                case Type.RareArcher:
                    return isSkill ? AnimationState.SkillBow : AnimationState.AttackBow;
                case Type.NormalWarrior:
                case Type.RareWarrior:
                case Type.HeroWarrior:
                case Type.EpicWarrior:
                    return isSkill ? AnimationState.SkillSword : AnimationState.AttackSword;
                case Type.HeroMagician:
                case Type.EpicMagician:
                    return isSkill ? AnimationState.SkillMagic : AnimationState.AttackMagic;
            }

            return AnimationState.Idle;
        }
        
        #endregion


        #region Skill

        [SerializeField]
        private List<Skill> activeSkill = new List<Skill>();

        private List<Skill> skills = new List<Skill>();
        /// <summary>
        /// 보유 스킬 목록 
        /// </summary>
        public List<Skill> Skills => skills;
        
        /// <summary>
        /// 스킬 오브젝트 생성 
        /// </summary>
        /// <param name="skill"> 생성 할 스킬 </param>
        /// <returns> 생성 된 스킬  </returns>
        public Skill GetSkill(Skill skill)
        {
            return InGame.Instance.GetSkillObject(skill, this);
        }
        
        #endregion
        

        #region Effect

        /// <summary>
        /// 기본 공격 이펙트 
        /// </summary>
        protected virtual void AttackEffect()
        {
            
        }

        #endregion
        
        
        #region Animation
        
        /// <summary>
        /// 캐릭터 애니메이션 상태 종류 
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
        /// 캐릭터 애니메이션 제어 
        /// </summary>
        [SerializeField] private SPUM_Prefabs aniCharacter = null;

        private void SetAnimation(AnimationState state)
        {
            aniCharacter.PlayAnimation((int)state);
        }

        #endregion


        #region Sprite

        /// <summary>
        /// 스프라이트 Layer Order 변경 
        /// </summary>
       [SerializeField]
        private SortingGroup sortingGroup = null;
        
        /// <summary>
        /// 캐릭터 스프라이트 오더 값 변경 
        /// </summary>
        /// <param name="orderInLayer"> 변경 할 레이어 오더 값 </param>
        public void SetSortingGroup(int orderInLayer)
        {
            sortingGroup.sortingOrder = orderInLayer;
        }

        #endregion
        
    }
}