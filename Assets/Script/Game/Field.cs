using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SB
{
    /// <summary>
    /// 캐릭터가 배치되는 필드 
    /// </summary>
    public class Field : MonoBehaviour
    {
        #region Info

        /// <summary>
        /// 내 필드와 상대 필드 구분 
        /// </summary>
        [SerializeField]
        private bool isMyField = true;

        /// <summary>
        /// 필드의 캐릭터 슬롯 
        /// </summary>
        [SerializeField]
        private List<FieldCell> fieldCells = null;

        /// <summary>
        /// 몬스터 이동 경로 
        /// </summary>
        [SerializeField]
        private List<Transform> moveSpots = null;

        /// <summary>
        /// 몬스터 이동 경로 
        /// </summary>
        public List<Transform> MoveSpots => moveSpots;



        #endregion


        #region Character

        /// <summary>
        /// 캐릭터 추가 
        /// </summary>
        /// <param name="characterType"> 추가 할 캐릭터 종류 </param>
        /// <param name="isImmediateRefresh"> 셀 즉시 갱신 여부  </param>
        /// <returns> 캐릭터 추가된 위치 인덱스 </returns>
        public int AddCharacter(Character.Type characterType, bool isImmediateRefresh = true)
        {
            // 배치 가능한 동일 캐릭터 검색 
            foreach (FieldCell fieldCell in fieldCells)
            {
                // 캐릭터 종류 검사 
                if (fieldCell.CharacterType != characterType)
                    continue;

                // 현재 캐릭터 개수 확인  
                if (fieldCell.CharacterCount >= 3)
                    continue;

                // 셀에 캐릭터 추가 
                fieldCell.AddCharacter(characterType, isImmediateRefresh);
                return fieldCell.Index;
            }

            // 이미 존재하는 캐릭터가 없는 경우 빈 슬롯 선택 
            foreach (FieldCell fieldCell in fieldCells)
            {
                // 빈 슬롯 검사 
                if (fieldCell.CharacterType != Character.Type.None)
                    continue;

                // 셀에 캐릭터 추가 
                fieldCell.AddCharacter(characterType, isImmediateRefresh);
                return fieldCell.Index;
            }

            // 캐릭터 추가 실패 
            return -1;
        }

        /// <summary>
        /// 캐릭터 위치 이동 
        /// </summary>
        /// <param name="indexA"> 이동할 인덱스 A </param>
        /// <param name="indexB"> 이동할 인덱스 B </param>
        public void SwapCharacter(int indexA, int indexB)
        {
            // 같은 좌표는 변경하지 않음 
            if (indexA == indexB)
                return;

            // Character.Type characterTypeA = fieldCells[indexA].CharacterType;
            // Character.Type characterTypeB = fieldCells[indexB].CharacterType;
            //
            // int characterCountA = fieldCells[indexA].CharacterCount;
            // int characterCountB = fieldCells[indexB].CharacterCount;

            CharacterHolder characterParentsA = fieldCells[indexA].CharacterHolder;
            CharacterHolder characterParentsB = fieldCells[indexB].CharacterHolder;

            // 캐릭터 이동 
            fieldCells[indexA].SwitchCharacter(characterParentsB);
            fieldCells[indexB].SwitchCharacter(characterParentsA);
        }


        /// <summary>
        /// 캐릭터 1개 제거 
        /// </summary>
        /// <param name="index"> 캐릭터 1개 제거 할 셀 인덱스 </param>
        public void RemoveCharacter(int index)
        {
            fieldCells[index].RemoveCharacter();

            int count = fieldCells[index].CharacterCount;
            // 제거 후 2칸 남은경우 다른 셀에서 당겨오기 
            if (count == 2)
            {
                var character = PullOtherCharacter(index, fieldCells[index].CharacterType);
                
                // 다른 셀에 같은 캐릭터가 없는경우 추가 당겨오기 불가능 
                if (character == null)
                {
                    fieldCells[index].RefreshCell();
                }
                else
                {
                    fieldCells[index].AttachCharacter(character);
                }
            }
            else if (count == 1)
            {
                fieldCells[index].RefreshCell();
            }
        }

        /// <summary>
        /// 캐릭터 1개 제거 후 다른 셀에서 당겨오는 함수 
        /// </summary>
        /// <param name="index"> 당겨오기 시도하는 셀 인덱스 </param>
        /// <param name="characterType"></param>
        /// <returns></returns>
        private Character PullOtherCharacter(int index, Character.Type characterType)
        {
            for (int i = 0; i < fieldCells.Count; ++i)
            {
                // 인덱스가 같은경우 체크하지 않음 
                if (i == index)
                    continue;

                var cell = fieldCells[i];
                
                // 다른 캐릭터는 체크하지 않음 
                if(cell.CharacterType != characterType)
                    continue;
                
                // 3개가 다 찬 슬롯에서는 가져오지 않음  
                if(cell.CharacterCount > 2)
                    continue;
                    
                return cell.PullCharacter();
            }

            return null;
        }
        
        /// <summary>
        /// 캐릭터 모두 제거 
        /// </summary>
        /// <param name="index"> 캐릭터 제거 할 인덱스 </param>
        public void RemoveAllCharacter(int index)
        {
            fieldCells[index].RemoveAllCharacter();
        }

        /// <summary>
        /// 캐릭터 보유 여부 
        /// </summary>
        /// <param name="characterType"> 보유 체크 할 캐릭터 종류 </param>
        /// <returns> 보유 여부. true - 보유 중 </returns>
        public bool IsExistCharacter(Character.Type characterType)
        {
            foreach (FieldCell fieldCell in fieldCells)
            {
                if (fieldCell.CharacterType == characterType)
                    return true;
            }
            
            return false;
        }

        /// <summary>
        /// 에픽 캐륵터 조합 
        /// </summary>
        /// <param name="type"> 소환할 에픽 캐릭터 종류 </param>
        /// <param name="reqCharacters"> 소환에 소모되는 캐릭터 목록 </param>
        public void SummonEpicCharacter(Character.Type type, List<Character.Type> reqCharacters)
        {
            foreach (var reqCharacter in reqCharacters)
            {
                Character.Type reqType = reqCharacter;

                int index = GetLeastCharacterIndex(reqType);

                // 캐릭터 제거 
                RemoveCharacter(index);
            }

            // 캐릭터 추가 
            AddCharacter(type);
        }

        /// <summary>
        /// 가작 작은 개수의 캐릭터 인덱스 
        /// </summary>
        /// <param name="characterType"> 검사 할 캐릭터 종류 </param>
        /// <returns> 가장 작은 개수의 인덱스 값. 없는경우 -1 </returns>
        private int GetLeastCharacterIndex(Character.Type reqType)
        {
            int index = -1;

            foreach (FieldCell cell in fieldCells)
            {
                // 타입이 다른경우 비교하지 않음 
                if (cell.CharacterType != reqType)
                    continue;

                // 개수가 가장 적은 인덱스 저장 
                index = cell.Index;
                
                // 개수가 3개 미만이라면 더이상 비교 할 필요 없음  
                if (cell.CharacterCount < 3)
                    break;
            }

            return index;
        }

        /// <summary>
        /// 셀 갱신 
        /// </summary>
        /// <param name="index"> 갱신 할 셀 인덱스 </param>
        public void RefreshCell(int index)
        {
            fieldCells[index].RefreshCell();
        }
        
        #endregion


        #region Ai

        /// <summary>
        /// AI 캐릭터 업그레이드 
        /// </summary>
        public void AiUpgradeCharacter()
        {
            foreach (FieldCell fieldCell in fieldCells)
            {
                // Epic 등급은 업그레이드 불가 
                if (fieldCell.CharacterGrade >= Character.Type.Epic)
                    continue;
                
                // 캐릭터가 3개 모이면 업그레이드 가능 
                if (fieldCell.CharacterCount == 3)
                {
                    // 현재 등급 
                    var grade = fieldCell.CharacterGrade;

                    // 다음 등급 계산 
                    switch (grade)
                    {
                        case Character.Type.None:
                            grade = Character.Type.Rare;
                            break;
                        case Character.Type.Rare:
                            grade = Character.Type.Hero;
                            break;
                        case Character.Type.Hero:
                            grade = Character.Type.Epic;
                            break;
                    }
                    
                    // 업그레이드 캐릭터 타입 계산 
                    var type = InGame.Instance.GetRandomCharacterType((int)grade);

                    // 현재 셀 캐릭터 제거 
                    RemoveAllCharacter(fieldCell.Index);
                    
                    // 캐릭터 추가 
                    AddCharacter(type);
                    return;
                }
            }
        }

        /// <summary>
        /// epic 캐릭터 조합 
        /// </summary>
        /// <returns> 조합 성공 여부 </returns>
        public bool AiUpgradeEpic()
        {
            // Epic Magician 검사
            
            // 필요 캐릭터 리스트 
            List<Character.Type> reqCharacters = new List<Character.Type>();
            reqCharacters.Add(Character.Type.HeroMagician);
            reqCharacters.Add(Character.Type.RareArcher);
            reqCharacters.Add(Character.Type.RareWarrior);

            // 캐릭터 조합 검사 
            var indexs = CheckCompleteCharacters(reqCharacters);

            if (indexs != null)
            {
                // 조합 캐릭터 제거 
                foreach (int index in indexs)
                {
                    RemoveCharacter(index);
                }

                // 캐릭터 추가 
                AddCharacter(Character.Type.EpicMagician);
                return true;
            }


            // Epic Warrior 검사 
            
            // 필요 캐릭터 리스트 
            reqCharacters.Clear();
            reqCharacters.Add(Character.Type.HeroWarrior);
            reqCharacters.Add(Character.Type.RareArcher);
            reqCharacters.Add(Character.Type.RareWarrior);
            
            // 캐릭터 조합 검사 
            indexs = CheckCompleteCharacters(reqCharacters);

            // 캐릭터 추가 
            if (indexs != null)
            {
                // 조합 캐릭터 제거 
                foreach (int index in indexs)
                {
                    RemoveCharacter(index);
                }

                // 캐릭터 추가 
                AddCharacter(Character.Type.EpicMagician);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 조합 완료여부 검사 
        /// </summary>
        /// <param name="reqCharacters"> 검사할 캐릭터 리스트 </param>
        /// <returns> 조합 재료 캐릭터들의 인덱스 리스트 </returns>
        private List<int> CheckCompleteCharacters(List<Character.Type> reqCharacters)
        {
            // 캐릭터 슬롯 인덱스
            List<int> reqCharacterIndex = new List<int>();
            
            // 필요 캐릭터 검사 
            foreach (var reqCharacter in reqCharacters)
            {
                int index = GetLeastCharacterIndex(reqCharacter);
                
                // 캐릭터가 없는경우 조합 불가 
                if (index < 0)
                    return null;
                
                // 인덱스 추가 
                reqCharacterIndex.Add(index);
            }

            return reqCharacterIndex;
        }
        

        #endregion
    }
}