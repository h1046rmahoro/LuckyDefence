using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SB
{
    /// <summary>
    /// 인게임 제어 
    /// </summary>
    public class InGame : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(Wave());
        }

        #region Instance

        private static InGame instance = null;
        
        /// <summary>
        /// InGame 인스턴스 
        /// </summary>
        public static InGame Instance
        {
            get
            {
                // 인스턴스 초기화가 되지 않은경우 씬에서 찾기 
                if (instance == null)
                    instance = FindFirstObjectByType(typeof(InGame)) as InGame;
                
                return instance;
            }
        }

        private void Awake()
        {
            // 싱글톤 인스턴스 체크 
            if (instance != null && instance != this)
                Destroy(gameObject);
            else
                instance = this;
        }

        private void OnDestroy()
        {
            instance = null;
        }

        #endregion


        #region Field
        
        /// <summary>
        /// 내 필드 
        /// </summary>
        [Header("Fields")]
        [SerializeField] 
        private Field myField = null;

        /// <summary>
        /// 상대방 필드 
        /// </summary>
        [SerializeField] 
        private Field enemyField = null;

        /// <summary>
        /// position을 좌표로 변환 
        /// </summary>
        /// <param name="position"> 좌표로 변환할 position </param>
        /// <returns> 보드 좌표 </returns>
        public static (int x, int y) PositionToCoord(Vector3 position)
        {
            // 셀 한칸 크기 
            float cellLength = 0.832f;

            // x 좌표 계산. 보드의 가장 좌측값 보정 2.496f
            int x = (int)((position.x + 2.496f) / cellLength);
            
            // y 좌표 계산. 보드의 가장 상단값 보정 0.446f 
            int y = (int)(((0.446f - position.y)) / cellLength);
            
            // 좌표 최소, 최대값 예외처리 
            x = Mathf.Clamp(x, 0, 5);
            y = Mathf.Clamp(y, 0, 2);
            
            return (x, y);
        }

        /// <summary>
        /// 보드의 좌표값을 position으로 변경 
        /// </summary>
        /// <param name="coord"> 변경할 보드의 좌표값 </param>
        /// <returns> 해당 좌표의 position </returns>
        public static Vector3 CoordToPosition((int x, int y) coord)
        {
            return CoordToPosition(coord.x, coord.y);
        }

        /// <summary>
        /// 보드의 좌표값을 position으로 변경 
        /// </summary>
        /// <param name="x"> x 좌표 값 </param>
        /// <param name="y"> y 좌표 값 </param>
        /// <returns> 해당 좌표의 position </returns>
        public static Vector3 CoordToPosition(int x, int y)
        {
            // 셀 한칸 크기 
            float cellLength = 0.832f;
            
            // 인덱스 기준 좌표 계산. 0,0의 위치 -2.08f, 0.036f
            Vector3 pos = new Vector3(-2.08f + (x * cellLength), 0.036f - (y * cellLength), 0);

            return pos;
        }

        /// <summary>
        /// 좌표를 인덱스로 변환 
        /// </summary>
        /// <param name="coord"> 셀의 좌표값 </param>
        /// <returns> 셀의 인덱스. 좌상단부터 아래로 0, 1, 2 ... </returns>
        public static int CoordToIndex((int x, int y) coord)
        {
            return coord.x * 3 + coord.y;
        }

        /// <summary>
        /// 인덱스를 좌표로 변환 
        /// </summary>
        /// <param name="index"> 셀의 인덱스. 좌상단부터 아래로 0, 1, 2 ... </param>
        /// <returns> 셀의 좌표값 </returns>
        public static (int x, int y) IndexToCoord(int index)
        {
            int x = index / 3;
            int y = index % 3;

            return (x, y);
        }

        /// <summary>
        /// 해당 인덱스의 position
        /// </summary>
        /// <param name="index">  셀의 인덱스. 좌상단부터 아래로 0, 1, 2 ... </param>
        /// <returns> 셀의 position </returns>
        public static Vector3 IndexToPosition(int index)
        {
            var coord = IndexToCoord(index);

            return CoordToPosition(coord);
        }

        #endregion
        

        #region Gold

        [SerializeField]
        private int aiGold = 100;
        /// <summary>
        /// AI 골드 
        /// </summary>
        public int AiGold
        {
            get => aiGold;
            set => aiGold = value;
        }

        /// <summary>
        /// 보유 골드. 에디터 수정 및 확인 위해 SerializedField 
        /// </summary>
        [SerializeField]
        private int myGold = 100;

        /// <summary>
        /// 보유 골드 
        /// </summary>
        public int Gold
        {
            get => myGold;
            set
            {
                myGold = value;
                
                if(OnGoldChanged != null)
                    OnGoldChanged.Invoke(myGold);
            }
        }

        /// <summary>
        /// 골드 변화 델리게이트 
        /// </summary>
        public delegate void GoldChangedDelegate(int gold);
        
        /// <summary>
        /// 골드 변화 이벤트 
        /// </summary>
        public event GoldChangedDelegate OnGoldChanged;

        #endregion


        #region Gem

        /// <summary>
        /// 현재 보유중인 젬 수량 
        /// </summary>
        private int myGem = 0;
        
        /// <summary>
        /// 현재 보우중인 젬 수량 
        /// </summary>
        public int Gem
        {
            get => myGem;
            set
            {
                myGem = value;
                if (OnGemChanged != null)
                {
                    OnGemChanged.Invoke(myGem);
                }
            }
        }

        /// <summary>
        /// 젬 변경 델리게이트 
        /// </summary>
        public delegate void GemChangedDelegate(int gem);

        /// <summary>
        /// 젬 변경 이벤트 
        /// </summary>
        public event GemChangedDelegate OnGemChanged = null;

        #endregion

        
        #region Character
        
        /// <summary>
        /// 캐릭터 프리팹 
        /// </summary>
        [Header("Character")]
        [SerializeField]
        private List<GameObject> goCharacters = new List<GameObject>();

        /// <summary>
        /// 캐릭터 드래그 시작 좌표값 
        /// </summary>
        private Vector3 dragStartPosition = Vector3.zero;
        
        /// <summary>
        /// 드래그 유닛 표시 이미지  
        /// </summary>
        [SerializeField]
        private Transform characterDragSelectUnit = null;
        
        /// <summary>
        /// 드래그 유닛 셀 정착지 표시 이미지 
        /// </summary>
        [SerializeField]
        private Transform characterDragTargetCell = null;

        /// <summary>
        /// 드래그 라인 
        /// </summary>
        [SerializeField]
        private SpriteRenderer characterDragLine = null;

        /// <summary>
        /// 공격 사거리 표시 라인 
        /// </summary>
        [SerializeField]
        private SpriteRenderer characterAttackRange = null;

        /// <summary>
        /// 소환 횟수 
        /// </summary>
        private int summonCount = 0;
        
        /// <summary>
        /// 캐릭터 소환 비용 
        /// </summary>
        public int SummonPrice => summonCount * 2 + 20;

        private int characterCount = 0;

        /// <summary>
        /// 현재 캐릭터 개수 
        /// </summary>
        public int CharacterCount
        {
            get => characterCount;
            set
            {
                characterCount = value;
                
                if(OnCharacterCountChanged != null)
                    OnCharacterCountChanged.Invoke(characterCount);
            }
        }
        
        /// <summary>
        /// 영웅 개수 변경 델리게이트 
        /// </summary>
        public delegate void CharacterCountChangedDelegate(int characterCount);
        
        /// <summary>
        /// 영웅 개수 변경 이벤트 
        /// </summary>
        public event CharacterCountChangedDelegate OnCharacterCountChanged = null;
        
        /// <summary>
        /// 소환 이펙트 부모 오브젝트 
        /// </summary>
        [SerializeField]
        private Transform goSummonEffectParent = null;
        
        /// <summary>
        /// 소환 이펙트 프리팹 
        /// </summary>
        [SerializeField]
        private GameObject goSummonEffect = null;

        /// <summary>
        /// 소환 이펙트 오브젝트 풀 
        /// </summary>
        private List<GameObject> goEffectObjectPool = new List<GameObject>();
        
        /// <summary>
        /// 캐릭터 소환
        /// </summary>
        /// <param name="grade"> 소환 할 등급 </param>
        public void Summon(Character.Type grade = Character.Type.None)
        {
            // 20마리 제한으로 소환 불가 
            if(CharacterCount >= 20)
                return;

            // 가격 계산 
            int price = 20 + summonCount * 2;
            
            // 돈이 없는 경우 소환 불가 
            if (price > Gold)
                return;
            
            Character.Type cType = GetRandomCharacterType((int)grade);
            
            // 필드에 캐릭터 추가 
            int index = myField.AddCharacter(cType, false);
            
            // 소환 횟수 증가 
            if(index != -1)
                ++summonCount;

            // 소환 이펙트 재생 
            StartCoroutine(SummonEffect(index));
            
            // 골드 차감 
            Gold -= price;
        }

        private IEnumerator SummonEffect(int index)
        {
            Vector3 startPos = goSummonEffectParent.transform.position;
            Vector3 endPos = IndexToPosition(index);
            
            // 중간 점 계산 
            float x = (startPos.x + endPos.x) * 0.5f;
            float y = Mathf.Max(startPos.y, endPos.y) + 2;
            Vector3 mid = new  Vector3(x, y, 0);
            
            Vector3[] path = new Vector3[3];
            path[0] = startPos;
            path[1] = mid;
            path[2] = endPos;

            GameObject goEffect = GetSummonEffect();

            MoveBezier.StartMove(goEffect, path, 0.5f, () =>
            {
                // 소환 이펙트 제거 
                goEffect.SetActive(false);
                
                // 셀 갱신 
                myField.RefreshCell(index);
            });
            
            yield return null;
        }
        
        /// <summary>
        /// 소환 이펙트 생성 
        /// </summary>
        /// <returns> 생성된 소환 이펙트 </returns>
        private GameObject GetSummonEffect()
        {
            foreach (var effect in goEffectObjectPool)
            {
                if (!effect.activeSelf)
                {
                    effect.SetActive(true);
                    return effect;
                }
            }

            GameObject go = Instantiate(goSummonEffect, goSummonEffectParent);
            go.transform.position = goSummonEffectParent.position;
            goEffectObjectPool.Add(go);

            return go;
        }
        
        /// <summary>
        /// 캐릭터 업그레이드
        /// </summary>
        /// <param name="index"> 업그레이드 할 셀의 인덱스 </param>
        /// <param name="grade"> 업그레이드 등급 </param>
        public void UpgradeCharacter(int index, Character.Type grade)
        {
            RemoveAllCharacter(index);
            var characterType = GetRandomCharacterType((int)grade);

            // 신규 캐릭터 추가 
            myField.AddCharacter(characterType);
        }

        /// <summary>
        /// 캐릭터 1개 제거 
        /// </summary>
        /// <param name="index"> 캐릭터 1개 제거 할 셀 인덱스 </param>
        public void RemoveCharacter(int index)
        {
            myField.RemoveCharacter(index);
        }
        
        /// <summary>
        /// 캐릭터 모두 제거 
        /// </summary>
        /// <param name="index"> 캐릭터 제거 할 인덱스 </param>
        public void RemoveAllCharacter(int index)
        {
            myField.RemoveAllCharacter(index);
        }

        /// <summary>
        /// 캐릭터 이동 시작
        /// </summary>
        /// <param name="position"> 입력중인 position </param>
        public void BeginDragCharacter(Vector3 position)
        {
            // 팝업 닫기
            CloseCharacterPopup();
            
            // 드래그 관련 오브젝트 활성화 
            characterDragSelectUnit.gameObject.SetActive(true);
            characterDragTargetCell.gameObject.SetActive(true);
            characterDragLine.gameObject.SetActive(true);
            
            // 좌표값 계산 
            var coord = PositionToCoord(position);

            // 목표 위치 설정 
            Vector3 pos = CoordToPosition(coord);
            characterDragSelectUnit.position = pos;
            characterDragTargetCell.position = pos;
            characterDragLine.transform.position = pos;
            
            // 드래그 라인 크기 조정 
            characterDragLine.size = new Vector2(0, 0.04f);

            dragStartPosition = pos;
        }
        

        /// <summary>
        /// 캐릭터 이동 좌표 선택  
        /// </summary>
        /// <param name="position"> 입력중인 position </param>
        public void DragCharacter(Vector3 position)
        {
            // 좌표값 계산 
            var coord = PositionToCoord(position);

            // 목표 위치 설정 
            Vector3 pos = CoordToPosition(coord);
            characterDragTargetCell.position = pos;
            
            // 드래그 라인 위치 조정 
            Vector3 center = (dragStartPosition + pos) * 0.5f;
            characterDragLine.transform.position = center;
            
            // 드래그 라인 크기 조정
            float distance = Vector3.Distance(dragStartPosition, pos);
            Vector2 size = new Vector2(distance * 0.5f, 0.04f);
            characterDragLine.size = size;
            
            // 드래그 라인 각도 조정 
            float x = pos.x - dragStartPosition.x;
            float y = pos.y - dragStartPosition.y;
            float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            characterDragLine.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        /// <summary>
        /// 캐릭터 이동 
        /// </summary>
        /// <param name="index"> 이동하는 캐릭터의 셀 Index </param>
        /// <param name="position"> 입력중인 position </param>
        public void SwapCharacter(int index, Vector3 position)
        {
            characterDragSelectUnit.gameObject.SetActive(false);
            characterDragTargetCell.gameObject.SetActive(false);
            characterDragLine.gameObject.SetActive(false);
            
            var coord = PositionToCoord(position);

            int target = CoordToIndex(coord);

            myField.SwapCharacter(index, target);
        }

        /// <summary>
        /// 랜덤 캐릭터 뽑기 
        /// </summary>
        /// <param name="grade"> 뽑기 등급 고정. 0인경우 랜덤 생성 </param>
        /// <returns> 뽑힌 랜덤 캐릭터 타입 </returns>
        public Character.Type GetRandomCharacterType(int grade = 0)
        {
            // 사전 지정되지 않은 경우 등급 랜덤 결정 
            if (grade == 0)
            {
                //grade = Random.Range(0, 4) * 10000;
            }

            // 캐릭터 랜덤 결정 
            int index = Random.Range(1, 3);
            
            // 등급 + 캐릭터 설정 
            return (Character.Type)(grade + index);
        }

        /// <summary>
        /// 캐릭터 프리팹 
        /// </summary>
        /// <param name="characterType"> 가져올 캐릭터 종류 </param>
        /// <returns> 로드할 캐릭터 프리팹 </returns>
        private GameObject GetCharacterPrefab(Character.Type characterType)
        {
            GameObject go = null;
            switch (characterType)
            {
                case Character.Type.None:
                    break;

                case Character.Type.NormalArcher:
                    go = goCharacters[0];
                    break;

                case Character.Type.NormalWarrior:
                    go = goCharacters[1];
                    break;

                case Character.Type.Rare:
                    break;

                case Character.Type.RareArcher:
                    go = goCharacters[2];
                    break;

                case Character.Type.RareWarrior:
                    go = goCharacters[3];
                    break;

                case Character.Type.Hero:
                    break;

                case Character.Type.HeroMagician:
                    go = goCharacters[4];
                    break;

                case Character.Type.HeroWarrior:
                    go = goCharacters[5];
                    break;

                case Character.Type.Epic:
                    break;

                case Character.Type.EpicMagician:
                    go = goCharacters[6];
                    break;

                case Character.Type.EpicWarrior:
                    go = goCharacters[7];
                    break;
                default:
                    break;
            }

            return go;
        }
        
        /// <summary>
        /// 캐릭터 생성 
        /// </summary>
        /// <param name="characterType"> 생성 할 캐릭터 종류 </param>
        /// <param name="parent"> 캐릭터 생성 부모 </param>
        /// <returns> 생성된 캐릭터 </returns>
        public Character InstantiateCharacter(Character.Type characterType, Transform parent)
        {
            GameObject go = Instantiate<GameObject>(GetCharacterPrefab(characterType), parent);
            
            Character character = go.GetComponent<Character>();
            return character;
        }

        /// <summary>
        /// 가장 가까운 공격 가능 몬스터 1마리 찾기 
        /// </summary>
        /// <param name="position"> 공격자 위치 </param>
        /// <param name="sqrAttackRange"> 공격 범위 </param>
        /// <returns> 공격 가능한 몬스터 </returns>
        public Monster FindAttackTargetMonster(Vector3 position, float sqrAttackRange)
        {
            Monster target = null;
            float distance = float.MaxValue;
            
            // 사거리 내 몬스터 체크 
            var monsters = new List<Monster>();
            monsters.AddRange(mySpawnZone.GetActiveMonster());
            monsters.AddRange(enemySpawnZone.GetActiveMonster());
            
            foreach (var monster in monsters)
            {
                if (monster.IsDead)
                    continue;
                
                float sqrDistance = (position - monster.transform.position).sqrMagnitude;
                
                // 사거리 밖인경우 더이상 검사하지 않음
                if(sqrDistance > sqrAttackRange)
                    continue;
                
                // 최단거리 몬스터 체크 
                if (sqrDistance < distance)
                {
                    // 해당 몬스터 적용 
                    distance = sqrDistance;
                    target = monster;
                }
            }
            
            return target;
        }

        /// <summary>
        /// 위치로부터 사거리 내의 몬스터
        /// </summary>
        /// <param name="position"> 검사 기준 position </param>
        /// <param name="sqrAttackRange"> 공격 범위 </param>
        /// <returns></returns>
        public List<Monster> FindAttackRangeMonster(Vector3 position, float sqrAttackRange)
        {
            List<Monster> attackAbleMonsters = new List<Monster>();
            
            // 사거리 내 몬스터 체크 
            var monsters = new List<Monster>();
            monsters.AddRange(mySpawnZone.GetActiveMonster());
            monsters.AddRange(enemySpawnZone.GetActiveMonster());
            
            foreach (var monster in monsters)
            {
                if (monster.IsDead)
                    continue;
                
                float sqrDistance = (position - monster.transform.position).sqrMagnitude;
                
                // 사거리 밖인경우 더이상 검사하지 않음
                if(sqrDistance > sqrAttackRange)
                    continue;
                
                // 리스트에 추가 
                attackAbleMonsters.Add(monster);
            }

            return attackAbleMonsters;
        }

        /// <summary>
        /// 공격 사거리 표시 
        /// </summary>
        /// <param name="target"> 사거리 표시를 추가할 부모 </param>
        /// <param name="position"> 사거리 중심 위치 </param>
        /// <param name="range"> 표시 할 사거리 </param>
        public void ShowAttackRange(Transform parents, Vector3 position, float range)
        {
            float size = range * 2;
            
            characterAttackRange.transform.parent = parents;
            
            characterAttackRange.gameObject.SetActive(true);
            characterAttackRange.transform.position = position;
            characterAttackRange.size = new Vector2(size, size);
        }

        /// <summary>
        /// 공격 사거리 표시 중단 
        /// </summary>
        public void HideAttackRange()
        {
            characterAttackRange.gameObject.SetActive(false);
        }

        /// <summary>
        /// 캐릭터 보유 여부 체크 
        /// </summary>
        /// <param name="characterType"> 체크 할 캐릭터 종류 </param>
        /// <returns> 캐릭터 보유 여부. true - 보유 중 </returns>
        public bool IsExistCharacter(Character.Type characterType)
        {
            return myField.IsExistCharacter(characterType);
        }

        /// <summary>
        /// 에픽 캐륵터 조합 
        /// </summary>
        /// <param name="type"> 소환할 에픽 캐릭터 종류 </param>
        /// <param name="reqCharacters"> 소환에 소모되는 캐릭터 목록 </param>
        public void SummonEpicCharacter(Character.Type type, List<Character.Type> reqCharacters)
        {
            myField.SummonEpicCharacter(type, reqCharacters);
        }

        #endregion
        
        
        #region Skill

        /// <summary>
        /// 스킬 오브젝트 풀 
        /// </summary>
        private Dictionary<int, List<Skill>> skillObjectPool = new Dictionary<int, List<Skill>>();
        
        public Skill GetSkillObject(Skill skill, Character character)
        {
            // 오브젝트 풀 리스트 추가 
            if (!skillObjectPool.ContainsKey(skill.Id))
            {
                skillObjectPool.Add(skill.Id, new List<Skill>());
            }
            
            // 오브젝트 풀 검사 
            var skills = skillObjectPool[skill.Id];
            foreach (var poolObject in skills)
            {
                if (poolObject.gameObject.activeSelf)
                    continue;

                poolObject.gameObject.SetActive(true);
                poolObject.Character = character;

                return poolObject;
            }
            
            // 오브젝트 풀에서 검사하지 못한경우 생성 
            GameObject go = Instantiate(skill.gameObject, Vector3.zero, Quaternion.identity, transform);
            skill = go.GetComponent<Skill>();
            skill.Character = character;

            skillObjectPool[skill.Id].Add(skill);
            
            return skill;
        }
        
        #endregion


        #region Upgrade

        private int rareUpgradeCount = 1;
        /// <summary>
        /// Normal-Rare 업그레이드 횟수 
        /// </summary>
        public int RareUpgradeCount
        {
            get => rareUpgradeCount;
            set => rareUpgradeCount = value;
        }

        private int heroUpgradeCount = 1;
        /// <summary>
        /// Hero 업그레이드 횟수 
        /// </summary>
        public int HeroUpgradeCount
        {
            get => heroUpgradeCount;
            set => heroUpgradeCount = value;
        }

        private int epicUpgradeCount = 1;
        /// <summary>
        /// Epic 업그레이드 횟수 
        /// </summary>
        public int EpicUpgradeCount
        {
            get => epicUpgradeCount;
            set => epicUpgradeCount = value;
        }

        #endregion


        #region Computer
        
        /// <summary>
        /// AI의 소환 횟수 
        /// </summary>
        public int AiSummonCount { get; set; }
        
        /// <summary>
        /// AI의 캐릭터 개수 
        /// </summary>
        public int AiCharacterCount { get; set; }

        /// <summary>
        /// AI 캐릭터 소환 
        /// </summary>
        public void AiSummon()
        {
            // 20마리 제한으로 소환 불가 
            if(AiCharacterCount >= 20)
                return;

            // 가격 계산 
            int price = 20 + AiSummonCount * 2;
            
            // 돈이 없는 경우 소환 불가 
            if (price > AiGold)
                return;

            Character.Type cType = GetRandomCharacterType();
            
            // 필드에 캐릭터 추가 
            int index = enemyField.AddCharacter(cType);
            
            // 소환 횟수 증가 
            if (index != -1)
                AiSummonCount += 1;

            // 골드 차감 
            AiGold -= price;
        }

        /// <summary>
        /// Ai 캐릭터 업그레이드  
        /// </summary>
        public void AiUpgrade()
        {
            enemyField.AiUpgradeCharacter();
        }

        /// <summary>
        /// Epic 조합 
        /// </summary>
        /// <returns> 조합 성공 여부 </returns>
        public bool AiUpgradeEpic()
        {
            return enemyField.AiUpgradeEpic();
        }

        #endregion


        #region Monster

        /// <summary>
        /// 몬스터 프리팹 
        /// </summary>
        [Header("Monster")] 
        [SerializeField]
        private List<Monster> goMonsters = new List<Monster>();
        
        /// <summary>
        /// 현재 소환된 몬스터 수 
        /// </summary>
        [SerializeField]
        private int monsterCount = 0;
        
        /// <summary>
        /// 현재 소환된 몬스터의 수 
        /// </summary>
        public int MonsterCount
        {
            get => monsterCount;
            set
            {
                monsterCount = Mathf.Clamp(value, 0, 100);;
                
                if(OnMonsterCountChanged != null)
                    OnMonsterCountChanged.Invoke(monsterCount, MaxMonsterCount);
            }
        }
        
        private int maxMonsterCount = 100;
        /// <summary>
        /// 최대 누적 가능 몬스터 수 
        /// </summary>
        public int MaxMonsterCount => maxMonsterCount;
        
        /// <summary>
        /// 몬스터 수량 변화 델리게이트 
        /// </summary>
        public delegate void MonsterCountChangedDelegate(int count, int maxCount);
        
        /// <summary>
        /// 몬스터 수량 변화 이벤트 
        /// </summary>
        public event MonsterCountChangedDelegate OnMonsterCountChanged;

        /// <summary>
        /// 내 몬스터 스폰 위치 
        /// </summary>
        [SerializeField]
        private SpawnZone mySpawnZone = null;

        /// <summary>
        /// 상대 몬스터 스폰 위치 
        /// </summary>
        [SerializeField]
        private SpawnZone enemySpawnZone = null;

        
        /// <summary>
        /// 소환 딜레이 
        /// </summary>
        private WaitForSeconds SpawnDelay = new WaitForSeconds(0.375f);

        public void SpawnMonster(Monster prefabMonster, int count = 1)
        {
            StartCoroutine(Spawn(prefabMonster, count));
        }

        private IEnumerator Spawn(Monster prefabMonster, int count)
        {
            bool isMySpawn = true;
            
            for (int i = 0; i < count; ++i)
            {
                if (isMySpawn)
                    mySpawnZone.SpawnMonster(prefabMonster, myField.MoveSpots);
                else
                    enemySpawnZone.SpawnMonster(prefabMonster, enemyField.MoveSpots);
                
                // 스폰 위치 전환 
                isMySpawn = !isMySpawn;
                
                // 몬스터 소환횟수 증가 
                ++MonsterCount;

                yield return SpawnDelay;
            }
            
            
            yield break;
        }
        
        #endregion


        #region Wave

        /// <summary>
        /// 현재 진행 중인 웨이브 레벨 
        /// </summary>
        [SerializeField]
        private int waveLevel = 0;

        /// <summary>
        /// 현재 진행중 웨이브 레벨 
        /// </summary>
        public int WaveLevel
        {
            get => waveLevel;
            set
            {
                waveLevel = value;
                
                if (OnWaveLevelChanged != null)
                    OnWaveLevelChanged.Invoke(waveLevel);
            }
        }
        
        /// <summary>
        /// 웨이브 레벨 변경 델리게이트 
        /// </summary>
        public delegate void WaveLevelChangeDelegate(int waveLevel);
        
        /// <summary>
        /// 웨이브 레벨 변경 이벤트 
        /// </summary>
        public event WaveLevelChangeDelegate OnWaveLevelChanged;

        /// <summary>
        /// 웨이브 남은시간 델리게이트 
        /// </summary>
        public delegate void WaveTimeDelegate(int second);

        /// <summary>
        /// 웨이브 시간 변경 이벤트 
        /// </summary>
        public event WaveTimeDelegate OnWaveTimeChanged = null;

        public enum GameState
        {
            Loading,    // 시작 전          - 3초
            Wave,       // 웨이브 진행       - 20초
            Boss,       // 보스전           - 60초
            Reward,      // 보스전 보상       - 15초
            GameOver,   // 게임 종료 
        }

        public GameState gameState = GameState.Loading;

        private IEnumerator Wave()
        {
            while (true)
            {
                switch (gameState)
                {
                    case GameState.Loading:
                        yield return WaveLoading();
                        break;
                    case GameState.Wave:
                        yield return WaveSpawn();
                        break;
                    case GameState.Boss:
                        yield return WaveBossSpawn();
                        break;
                    case GameState.Reward:
                        yield return WaveReward();
                        break;
                    case GameState.GameOver:
                        break;
                }

                yield return null;
            }
        }

        /// <summary>
        /// 게임 시작 전 로딩 
        /// </summary>
        private IEnumerator WaveLoading()
        {
            // 4초간 진행 
            float time = 4.0f;
            int second = 0;

            // 시간 제어 
            while (time > 0.0f)
            {
                time -= Time.deltaTime;
                int now = (int)time;

                // 시간이 변경된경우 시간 변화 콜백 
                if (now != second)
                {
                    second = now;
                    if (OnWaveTimeChanged != null)
                        OnWaveTimeChanged(second);
                }

                yield return null;
            }

            // 로딩시간 종료 시 웨이브 시작 
            gameState = GameState.Wave;
            ++waveLevel;
        }

        /// <summary>
        /// 일반 웨이브 진행.
        /// </summary>
        private IEnumerator WaveSpawn()
        {
            // 일반 몬스터 
            var monster = goMonsters[0];
            
            // 몬스터 40마리 소환 예약 
            SpawnMonster(monster, 40);
            
            // 20초간 진행 
            float time = 20.0f;
            int second = 0;

            // 시간 제어 
            while (time > 0.0f)
            {
                time -= Time.deltaTime;
                int now = (int)time;

                // 시간이 변경된경우 시간 변화 콜백 
                if (now != second)
                {
                    // 다음 웨이브 타이머 재생 
                    if (second == 5)
                    {
                        mySpawnZone.StartTimer();
                        enemySpawnZone.StartTimer();
                    }

                    second = now;
                    if (OnWaveTimeChanged != null)
                        OnWaveTimeChanged(second);
                }

                // 몬스터 100마리 이상인 경우 게임 종료 
                if (MonsterCount >= 100)
                {
                    gameState = GameState.GameOver;
                    ShowGameOverPopup();
                    yield break;
                }

                yield return null;
            }

            // 10 스테이지마다 보스 스테이지 진행 
            if (WaveLevel % 10 == 0)
            {
                gameState = GameState.Boss;
            }
                
            
            // 웨이브 레벨 증가 
            ++WaveLevel;
            
            yield break;
        }

        private IEnumerator WaveBossSpawn()
        {
            // 보스 몬스터 
            var monster = goMonsters[1];
            
            // 몬스터 2마리 소환 예약 
            SpawnMonster(monster, 2);
            
            // 60초간 진행 
            float time = 60.0f;
            int second = 0;

            // 시간 제어 
            while (time > 0.0f)
            {
                time -= Time.deltaTime;
                int now = (int)time;

                // 시간이 변경된경우 시간 변화 콜백 
                if (now != second)
                {
                    // 보스 사망 체크.
                    // 보스가 모두 죽은경우 즉시 웨이브 종료 
                    if (!mySpawnZone.IsBossAlive() && !enemySpawnZone.IsBossAlive())
                    {
                        break;
                    }

                    second = now;
                    if (OnWaveTimeChanged != null)
                        OnWaveTimeChanged(second);
                }

                yield return null;
            }
            
            // 보스 사망 체크 
            if (mySpawnZone.IsBossAlive() || enemySpawnZone.IsBossAlive())
            {
                gameState = GameState.GameOver;
                ShowGameOverPopup();
                yield break;
            }
            
            // 보상 수령으로 이동
            gameState = GameState.Reward;
            
            yield break;
            
        }

        private IEnumerator WaveReward()
        {
            // 보상 수령 UI 등장
            
            
            // 15초간 진행 
            float time = 15.0f;
            int second = 0;

            // 시간 제어 
            while (time > 0.0f)
            {
                time -= Time.deltaTime;
                int now = (int)time;

                // 시간이 변경된경우 시간 변화 콜백 
                if (now != second)
                {
                    // 다음 웨이브 타이머 재생 
                    if (second == 5)
                    {
                        mySpawnZone.StartTimer();
                        enemySpawnZone.StartTimer();
                    }
                    
                    second = now;
                    if (OnWaveTimeChanged != null)
                        OnWaveTimeChanged(second);
                }

                yield return null;
            }
            
            gameState = GameState.Wave;
            
            yield break;
        }

        #endregion


        #region UI

        /// <summary>
        /// 캐릭터 관리 팝업 
        /// </summary>
        [Header("UI")]
        [SerializeField]
        private PopupCharacter popupCharacter = null;
        
        /// <summary>
        /// 겜블 팝업 
        /// </summary>
        [SerializeField]
        private PopupGamble popupGamble = null;
        
        /// <summary>
        /// 에픽 조합 팝업 
        /// </summary>
        [SerializeField]
        private PopupEpic popupEpic = null;
        
        /// <summary>
        /// 업그레이드 팝업 
        /// </summary>
        [SerializeField]
        private PopupUpgrade popupUpgrade = null;
        
        /// <summary>
        /// 캐릭터 정보 팝업 
        /// </summary>
        [SerializeField]
        private PopupCharacterInfo popupCharacterInfo = null;

        /// <summary>
        /// 게임오버 팝업 
        /// </summary>
        [SerializeField]
        private PopupGameOver popupGameOver = null;
        
        /// <summary>
        /// 캐릭터 관리 팝업 노출 
        /// </summary>
        /// <param name="cellIndex"> 셀 인덱스 </param>
        /// <param name="grade"> 캐릭터 등급  </param>
        /// <param name="characterCount"> 현재 캐릭터 개수  </param>
        public void ShowCharacterPopup(int cellIndex, Character.Type grade, int characterCount)
        {
            popupCharacter.gameObject.SetActive(true);
            popupCharacter.SetCharacterInfo(cellIndex, grade, characterCount);

        }

        /// <summary>
        /// 캐릭터 관리 팝업 닫기 
        /// </summary>
        public void CloseCharacterPopup()
        {
            popupCharacter.ClosePopup();
        }

        /// <summary>
        /// 겜블 팝업 열기 
        /// </summary>
        public void ShowGamblePopup()
        {
            popupGamble.gameObject.SetActive(true);
        }

        /// <summary>
        /// 겜블 팝업 닫기 
        /// </summary>
        private void CloseGamblePopup()
        {
            popupGamble.gameObject.SetActive(false);    
        }

        /// <summary>
        /// 에픽 조합 팝업 열기 
        /// </summary>
        public void ShowEpicPopup()
        {
            popupEpic.gameObject.SetActive(true);
        }

        /// <summary>
        /// 에픽 조합 팝업 닫기 
        /// </summary>
        public void CloseEpicPopup()
        {
            popupEpic.gameObject.SetActive(false);
        }

        /// <summary>
        /// 업그레이드 팝업 열기 
        /// </summary>
        public void ShowUpgradePopup()
        {
            popupUpgrade.gameObject.SetActive(true);
        }

        /// <summary>
        /// 업그레이드 팝업 닫기 
        /// </summary>
        public void CloseUpgradePopup()
        {
            popupUpgrade.gameObject.SetActive(false);
        }

        /// <summary>
        /// 캐릭터 정보 팝업 열기 
        /// </summary>
        /// <param name="character"> 정보를 확인 할 캐릭터 </param>
        public void ShowCharacterInfoPopup(Character character)
        {
            // 캐릭터 정보가 없는경우 보여줄 수 없음 
            if (character == null)
            {
                CloseCharacterInfoPopup();
                return;
            }

            popupCharacterInfo.gameObject.SetActive(true);
            popupCharacterInfo.SetInfo(character);
        }

        /// <summary>
        /// 캐릭터 정보 팝업 닫기 
        /// </summary>
        public void CloseCharacterInfoPopup()
        {
            popupCharacterInfo.gameObject.SetActive(false);
        }

        /// <summary>
        /// 게임오버 팝업 
        /// </summary>
        public void ShowGameOverPopup()
        {
            popupGameOver.gameObject.SetActive(true);
        }

        #endregion
    }
}