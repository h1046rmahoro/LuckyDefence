using UnityEngine;
using UnityEngine.EventSystems;

namespace SB
{
    /// <summary>
    /// 캐릭터 필드 제어 
    /// </summary>
    public class FieldCell : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Info

        /// <summary>
        /// 셀 인덱스 
        /// </summary>
        [SerializeField]
        private int index = 0;

        /// <summary>
        /// 셀 인덱스 
        /// </summary>
        public int Index => index;

        /// <summary>
        /// 내 셀 여부  
        /// </summary>
        [SerializeField]
        private bool isMyCell = false;
        
        #endregion
        
        
        #region Character 
        
        /// <summary>
        /// 캐릭터 위치 보정 오브젝트 
        /// </summary>
        [SerializeField]
        private CharacterHolder characterHolder = null;

        /// <summary>
        /// 캐릭터 위치 보정 오브젝트 
        /// </summary>
        public CharacterHolder CharacterHolder => characterHolder;
        
        /// <summary>
        /// 배치된 캐릭터 종류 
        /// </summary>
        public Character.Type CharacterType => CharacterHolder.CharacterType;

        /// <summary>
        /// 현재 배치된 캐릭터 개수 
        /// </summary>
        public int CharacterCount => CharacterHolder.CharacterCount;

        /// <summary>
        /// 현재 배치된 캐릭터 등급 
        /// </summary>
        public Character.Type CharacterGrade => CharacterHolder.CharacterGrade;


        /// <summary>
        /// 캐릭터 추가 
        /// </summary>
        /// <param name="characterType"> 추가하는 캐릭터 종류  </param>
        /// <param name="isImmediateRefresh"> 셀 즉시 갱신 여부  </param>
        public void AddCharacter(Character.Type characterType, bool isImmediateRefresh = true)
        {
            CharacterHolder.AddCharacter(characterType, isImmediateRefresh);
        }

        /// <summary>
        /// 캐릭터 위치 변경 
        /// </summary>
        /// <param name="characterHolder"> 변경 할 캐릭터 홀더 </param>
        public void SwitchCharacter(CharacterHolder characterHolder)
        {
            // 정보 변경 
            this.characterHolder = characterHolder;
            
            // 부모 변경 
            CharacterHolder.transform.parent = transform;
            
            // 이동 목표 위치 설정 
            Vector3 pos = transform.position +  new Vector3(0, -0.15f, 0);
            
            // 거리 계산 
            float distance = Vector3.Distance(pos, CharacterHolder.transform.position);

            float time = 0;

            // 캐릭터가 있는 경우에만 이동 연출 발생 
            if (CharacterCount > 0)
                time = distance / 5;
            
            // 캐릭터 이동 
            Move.StartMove(CharacterHolder.gameObject, pos, time);
        }

        
        /// <summary>
        /// 캐릭터 1개 제거 
        /// </summary>
        public void RemoveCharacter()
        {
            CharacterHolder.RemoveCharacter();
        }
        
        /// <summary>
        /// 캐릭터 모두 제거 
        /// </summary>
        public void RemoveAllCharacter()
        {
            CharacterHolder.RemoveAllCharacter();
        }

        /// <summary>
        /// 캐릭터 빼오기
        /// </summary>
        /// <returns> 빼올 캐릭터 </returns>
        public Character PullCharacter()
        {
            return CharacterHolder.PullCharacter();
        }

        public void AttachCharacter(Character character)
        {
            CharacterHolder.AttachCharacter(character);
        }

        /// <summary>
        /// 셀 정보 갱신 
        /// </summary>
        public void RefreshCell()
        {
            CharacterHolder.RefreshCell();
        }

        #endregion

        #region Input

        private bool isMoveCharacter = false;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            // 배치된 캐릭터가 없는경우 작동하지 않음 
            if (CharacterCount == 0)
            {
                // 공격 사거리 닫기 
                InGame.Instance.HideAttackRange();
                InGame.Instance.CloseCharacterPopup();
                InGame.Instance.CloseCharacterInfoPopup();
                return;
            }

            // 좌표 계산 
            Vector3 pos = Camera.main.ScreenToWorldPoint(eventData.position);
            pos.z = 0;
            
            // 캐릭터 공격범위 표시 
            InGame.Instance.ShowAttackRange(CharacterHolder.transform, transform.position, CharacterHolder.CharacterAttackRange);
            
            // 캐릭터 관리팝업 
            if(isMyCell)
                InGame.Instance.ShowCharacterPopup(index, CharacterGrade, CharacterCount);
            
            // 캐릭터 정보 팝업 
            InGame.Instance.ShowCharacterInfoPopup(CharacterHolder.GetCharacter());
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // 내 셀이 아닌경우 작동하지 않음 
            if (!isMyCell)
                return;
            
            // 배치된 캐릭터가 없는경우 작동하지 않음 
            if (CharacterHolder.CharacterType == Character.Type.None)
                return;

            // 배치된 캐릭터가 없는경우 작동하지 않음 
            if (CharacterHolder.CharacterCount == 0)
                return;

            // 캐릭터 이동 설정 
            isMoveCharacter = true;
            
            // 좌표 계산 
            Vector3 pos = Camera.main.ScreenToWorldPoint(eventData.position);
            pos.z = 0;
            
            // 캐릭터 이동 시작 설정 
            InGame.Instance.BeginDragCharacter(pos);
            
            // 캐릭터 공격범위 표시 
            InGame.Instance.ShowAttackRange(CharacterHolder.transform, transform.position, CharacterHolder.CharacterAttackRange);
        }

        public void OnDrag(PointerEventData eventData)
        {
            // 내 셀이 아닌경우 작동하지 않음 
            if (!isMyCell)
                return;
            
            // 캐릭터 이동이 아닌경우 작동하지 않음 
            if (!isMoveCharacter)
                return;
            
            // 좌표 계산 
            Vector3 pos = Camera.main.ScreenToWorldPoint(eventData.position);
            pos.z = 0;

            // 캐릭터 이동 좌표 선택 
            InGame.Instance.DragCharacter(pos);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // 내 셀이 아닌경우 작동하지 않음 
            if (!isMyCell)
                return;
            
            // 캐릭터 이동이 아닌경우 작동하지 않음 
            if (!isMoveCharacter)
                return;
            
            // 좌표 계산 
            Vector3 pos = Camera.main.ScreenToWorldPoint(eventData.position);
            pos.z = 0;
            
            // 캐릭터 이동 
            InGame.Instance.SwapCharacter(index, pos);

            
            // 캐릭터 이동 종료 
            isMoveCharacter = false;
        }

        #endregion

    }
}