using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SB
{
    public class EpicReqSlot : MonoBehaviour
    {
        #region UI

        /// <summary>
        /// 배경 이미지 
        /// </summary>
        [SerializeField]
        private Image imgBg = null;

        /// <summary>
        /// 보유중 표시 
        /// </summary>
        [SerializeField]
        private GameObject goOwned = null;
        
        /// <summary>
        /// 미보유 표시 
        /// </summary>
        [SerializeField]
        private GameObject goRequired = null;

        /// <summary>
        /// 캐릭터 이미지 
        /// </summary>
        [SerializeField]
        private Image imgCharacter = null;

        /// <summary>
        /// 캐릭터 스프라이트 
        /// </summary>
        [SerializeField]
        private List<Sprite> sprCharacter = new List<Sprite>();

        /// <summary>
        /// 캐릭터 정보 설정 
        /// </summary>
        /// <param name="type"> 보유 여부 설정 할 캐릭터 종류 </param>
        public bool SetInfo(Character.Type type)
        {
            bool isExist = InGame.Instance.IsExistCharacter(type);
            
            goOwned.SetActive(isExist);
            goRequired.SetActive(!isExist);

            switch (type)
            {
                case Character.Type.RareArcher:
                    imgBg.color = new Color32(124, 151, 255, 255);
                    imgCharacter.sprite = sprCharacter[0];
                    break;
                case Character.Type.RareWarrior:
                    imgBg.color = new Color32(124, 151, 255, 255);
                    imgCharacter.sprite = sprCharacter[1];
                    break;
                case Character.Type.HeroMagician:
                    imgBg.color = new Color32(209, 124, 255, 255);
                    imgCharacter.sprite = sprCharacter[2];
                    break;
                case Character.Type.HeroWarrior:
                    imgBg.color = new Color32(209, 124, 255, 255);
                    imgCharacter.sprite = sprCharacter[3];
                    break;
            }
            
            return isExist;
        }

        #endregion
    }
}
