using UnityEngine;
using UnityEngine.EventSystems;

namespace SB
{
    /// <summary>
    /// 모든 팝업 및 패널을 닫기위한 클래스 
    /// </summary>
    public class CloseAllPopup : MonoBehaviour, IPointerClickHandler
    {
        #region Input

        public void OnPointerClick(PointerEventData eventData)
        {
            var inGame = InGame.Instance;
            inGame.HideAttackRange();
            inGame.CloseCharacterPopup();
            inGame.CloseCharacterInfoPopup();
        }

        #endregion
    }
}
