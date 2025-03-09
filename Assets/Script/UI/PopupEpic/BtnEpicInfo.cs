using UnityEngine;

namespace SB
{
    /// <summary>
    /// 에픽 정보 보는 버튼 
    /// </summary>
    public class BtnEpicInfo : MonoBehaviour
    {
        #region Info

        /// <summary>
        /// 정보를 설정할 캐릭터 종류 
        /// </summary>
        [SerializeField]
        private Character.Type characterType = Character.Type.None;

        /// <summary>
        /// 정보 보여주는 팝업 
        /// </summary>
        [SerializeField]
        private PopupEpic popupEpic = null;

        /// <summary>
        /// 팝업 정보 설정 
        /// </summary>
        public void SetInfo()
        {
            popupEpic.SetInfo(characterType);
        }

        #endregion
    }
}
