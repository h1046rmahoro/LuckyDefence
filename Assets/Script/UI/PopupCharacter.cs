using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

namespace SB
{
    /// <summary>
    /// 캐릭터 관리 팝업 
    /// </summary>
    public class PopupCharacter : MonoBehaviour
    {
        #region UI

        /// <summary>
        /// 캐릭터 정보 중심점 
        /// </summary>
        [SerializeField]
        private Transform panelTransform = null;

        /// <summary>
        /// 업그레이드 버튼 
        /// </summary>
        [SerializeField]
        private Button btnUpgrade = null;
        
        /// <summary>
        /// 캐릭터가 속한 셀의 인덱스  
        /// </summary>
        private int cellIndex = 0;
        
        /// <summary>
        /// 캐릭터 등급 
        /// </summary>
        private Character.Type characterGrade = Character.Type.None;
        
        /// <summary>
        /// 캐릭터 개수 
        /// </summary>
        private int characterCount = 0;

        /// <summary>
        /// 캐릭터 정보 설정 
        /// </summary>
        /// <param name="index"> 정보를 볼 셀의 인덱스 </param>
        /// <param name="grade"> 캐릭터 등급 </param>
        /// <param name="count"> 현재 배치된 캐릭터 개수 </param>
        public void SetCharacterInfo(int index, Character.Type grade, int count)
        {
            // 정보 설정
            cellIndex = index;
            characterGrade = grade;
            characterCount = count;
            
            // 패널 위치 설정 
            var position = InGame.IndexToPosition(index);
            position = Camera.main.WorldToScreenPoint(position);
            panelTransform.position = position;
            
            // 업그레이드 버튼 제어 
            btnUpgrade.interactable = (grade != Character.Type.Epic && count >= 3);
        }
        
        /// <summary>
        /// 캐릭터 판매 
        /// </summary>
        public void SellCharacter()
        {
            InGame.Instance.RemoveCharacter(cellIndex);
            
            // 캐릭터 개수 감수
            --characterCount;
            
            // 업그레이드 버튼 비활성화 
            btnUpgrade.interactable = false;
            
            // 남은 캐릭터가 없는 경우 팝업 닫기 
            if (characterCount == 0)
            {
                ClosePopup();
                
                // 캐릭터 사거리 표시 닫기 
                InGame.Instance.HideAttackRange();
            }
        }

        /// <summary>
        /// 캐릭터 승급  
        /// </summary>
        public void UpgradeCharacter()
        {
            Character.Type upgradeGrade = Character.Type.None;

            switch (characterGrade)
            {
                case Character.Type.None:
                    upgradeGrade = Character.Type.Rare;
                    break;
                case Character.Type.Rare:
                    upgradeGrade = Character.Type.Hero;
                    break;
                case Character.Type.Hero:
                    upgradeGrade = Character.Type.Epic;
                    break;
                case Character.Type.Epic:
                    upgradeGrade = Character.Type.Epic;
                    break;
            }
            
            // 캐릭터 승급 
            InGame.Instance.UpgradeCharacter(cellIndex, upgradeGrade);
            
            // 캐릭터 사거리 표시 닫기 
            InGame.Instance.HideAttackRange();
            
            // 팝업 닫기 
            ClosePopup();
        }
        
        public void ClosePopup()
        {
            gameObject.SetActive(false);
        }

        #endregion
    }
}
