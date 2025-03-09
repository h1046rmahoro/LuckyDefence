using System.Collections.Generic;
using UnityEngine;

namespace SB
{
    /// <summary>
    /// 셀의 캐릭터를 담는 클래스 
    /// </summary>
    public class CharacterHolder : MonoBehaviour
    {
        #region Character

        /// <summary>
        /// 내 캐릭터 슬롯 여부 
        /// </summary>
        [SerializeField]
        private bool isMyCharacter = false;
        
        /// <summary>
        /// 생성된 캐릭터 오브젝트 리스트  
        /// </summary>
        [SerializeField]
        List<Character> characters = new List<Character>();

        private Character.Type characterType = Character.Type.None;
        /// <summary>
        /// 배치된 캐릭터 종류 
        /// </summary>
        public Character.Type CharacterType => characterType;

        private int characterCount = 0;
        /// <summary>
        /// 현재 배치된 캐릭터 개수 
        /// </summary>
        public int CharacterCount => characterCount;
        
        /// <summary>
        /// 배치된 캐릭터의 등급 
        /// </summary>
        public Character.Type CharacterGrade
        {
            get
            {
                // 캐릭터가 없는경우 등급 없음 
                if (CharacterCount == 0)
                    return Character.Type.None;

                return characters[0].Grade;
            }
        }

        /// <summary>
        /// 캐릭터의 공격 범위 
        /// </summary>
        public float CharacterAttackRange
        {
            get
            {
                // 캐릭터가 없는경우 사거리 0 
                if(characterCount == 0)
                    return 0;
                
                return characters[0].AttackRange;
            }
        }

        /// <summary>
        /// 캐릭터 추가 
        /// </summary>
        /// <param name="characterType"> 추가하는 캐릭터 종류  </param>
        /// <param name="isImmediateRefresh"> 셀 즉시 갱신 여부  </param>
        public void AddCharacter(Character.Type characterType, bool isImmediateRefresh = true)
        {
            this.characterType = characterType;
            characterCount += 1;

            // 캐릭터 개수 증가 
            if (isMyCharacter)
                InGame.Instance.CharacterCount += 1;
            else
                InGame.Instance.AiCharacterCount += 1;
            
            // 셀 갱신 
            if(isImmediateRefresh)
                RefreshCell();
        }

        /// <summary>
        /// 캐릭터 1개 제거 
        /// </summary>
        public void RemoveCharacter()
        {
            // 캐릭터가 존재하지 않는경우 제거 불가
            if (CharacterCount == 0)
                return;

            // 캐릭터 오브젝트 제거 
            Destroy(characters[0].gameObject);
            characters.RemoveAt(0);

            // 캐릭터 개수 감소 
            --characterCount;
            
            // 캐릭터 개수 감소 
            if(isMyCharacter)
                InGame.Instance.CharacterCount -= 1;
            else
                InGame.Instance.AiCharacterCount -= 1;

            // 캐릭터가 남아있는경우 정보 초기화 하지 않음 
            if (CharacterCount != 0)
                return;
            
            // 정보 초기화
            characterType = Character.Type.None;
            characterCount = 0;
            characters.Clear();
        }
        
        /// <summary>
        /// 캐릭터 제거 
        /// </summary>
        public void RemoveAllCharacter()
        {
            // 캐릭터 개수 감소 
            if(isMyCharacter)
                InGame.Instance.CharacterCount -= CharacterCount;
            else
                InGame.Instance.AiCharacterCount -= CharacterCount;
            
            characterType = Character.Type.None;
            characterCount = 0;
            for (int i = characters.Count - 1; i >= 0; --i)
            {
                Destroy(characters[i].gameObject);
            }
            
            characters.Clear();
        }

        /// <summary>
        /// 캐릭터 빼오기
        /// </summary>
        /// <returns> 빼올 캐릭터 </returns>
        public Character PullCharacter()
        {
            // 캐릭터가 없는경우 작동 불가 
            if (CharacterCount == 0)
                return null;

            Character character = characters[characterCount - 1];

            characters.RemoveAt(CharacterCount - 1);
            characterCount -= 1;

            // 더이상 캐릭터가 남지 않은경우 정보 초기화 
            if (CharacterCount == 0)
            {
                characterType = Character.Type.None;
                characterCount = 0;
                characters.Clear();
            }
            
            return character;
        }

        public void AttachCharacter(Character character)
        {
            character.transform.parent = transform;
            characters.Add(character);
            characterCount += 1;

            Vector3 position = transform.position + new Vector3(0, 0.15f, 0);
            
            // 거리 계산 
            float distance = Vector3.Distance(character.transform.position, position);

            // 이동 시간 계산 
            float time = distance / 5;
            
            // 이동 시작 
            Move.StartMove(character.gameObject, position, time, RefreshCell);
        }

        /// <summary>
        /// 슬롯에 배치된 첫번째 캐릭터 
        /// </summary>
        /// <returns> 캐릭터 정보 </returns>
        public Character GetCharacter()
        {
            // 캐릭터가 없는경우 반환 불가 
            if (CharacterCount == 0)
                return null;

            return characters[0];
        }

        #endregion
        
        
        #region Visualize

        /// <summary>
        /// 셀 정보 갱신 
        /// </summary>
        public void RefreshCell()
        {
            // 부족한 캐릭터 오브젝트 생성 
            for (int i = characters.Count; i < characterCount; ++i)
            {
                Character character = InGame.Instance.InstantiateCharacter(characterType, transform);
                characters.Add(character);
            }
            
            // 캐릭터 개수 별 좌표 설정 
            switch (characterCount)
            {
                case 1:
                    characters[0].transform.localPosition = Vector3.zero;
                    break;
                case 2:
                    characters[0].transform.localPosition = new Vector3(-0.15f, 0, 0);
                    characters[1].transform.localPosition = new Vector3(0.15f, 0, 0);
                    break;
                case 3:
                    characters[0].transform.localPosition = new Vector3(0, 0.15f, 0);
                    characters[1].transform.localPosition = new Vector3(-0.15f, -0.15f, 0);
                    characters[2].transform.localPosition = new Vector3(0.15f, -0.15f, 0);
                    break;
            }
        }
        
        #endregion


    }
}
