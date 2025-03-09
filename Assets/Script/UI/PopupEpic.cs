using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SB
{
    public class PopupEpic : MonoBehaviour
    {
        #region UI
        
        /// <summary>
        /// 필요 캐릭터 표시 슬록 
        /// </summary>
        [SerializeField]
        private List<EpicReqSlot> reqSlots = new List<EpicReqSlot>();
        
        /// <summary>
        /// 현재 표시중인 캐릭터 이미지  
        /// </summary>
        [SerializeField]
        private Image imgCharacter = null;
        
        /// <summary>
        /// 신화 캐릭터 이미지 
        /// </summary>
        [SerializeField]
        private List<Sprite> sprCharacter = new List<Sprite>();

        /// <summary>
        /// 소환 버튼 
        /// </summary>
        [SerializeField]
        private Button btnSummon = null;

        /// <summary>
        /// 현재 선택한 캐릭터 종류 
        /// </summary>
        private Character.Type characterType = Character.Type.None;
        
        private void OnEnable()
        {
            SetInfo(Character.Type.EpicMagician);
        }

        public void ClosePopup()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 정보 설정 
        /// </summary>
        /// <param name="type"> 설정 할 캐릭터 종류 </param>
        public void SetInfo(Character.Type type)
        {
            // 캐릭터 종류 설정 
            characterType = type;

            // 소환 가능 여부 
            bool isSummonAble = true;

            if (type == Character.Type.EpicMagician)
            {   // 에픽 마법사 
                if (!reqSlots[0].SetInfo(Character.Type.HeroMagician))
                    isSummonAble = false;
                if (!reqSlots[1].SetInfo(Character.Type.RareArcher))
                    isSummonAble = false;
                if (!reqSlots[2].SetInfo(Character.Type.RareWarrior))
                    isSummonAble = false;
                
                // 이미지 설정 
                imgCharacter.sprite = sprCharacter[0];
            }
            else if (type == Character.Type.EpicWarrior)
            {   // 에픽 전사 
                if (!reqSlots[0].SetInfo(Character.Type.HeroWarrior))
                    isSummonAble = false;
                if (!reqSlots[1].SetInfo(Character.Type.RareArcher))
                    isSummonAble = false;
                if (!reqSlots[2].SetInfo(Character.Type.RareWarrior))
                    isSummonAble = false;
                
                // 이미지 설정 
                imgCharacter.sprite = sprCharacter[1];
            }
            
            // 소환 버튼 제어 
            btnSummon.interactable = isSummonAble;
        }

        public void SummonEpicCharacter()
        {
            List<Character.Type> reqCharacters = new List<Character.Type>();
            switch (characterType)
            {
                case Character.Type.EpicMagician:
                    reqCharacters.Add(Character.Type.RareArcher);
                    reqCharacters.Add(Character.Type.RareWarrior);
                    reqCharacters.Add(Character.Type.HeroMagician);
                    break;
                case Character.Type.EpicWarrior:
                    reqCharacters.Add(Character.Type.RareArcher);
                    reqCharacters.Add(Character.Type.RareWarrior);
                    reqCharacters.Add(Character.Type.HeroWarrior);
                    break;
            }
            
            InGame.Instance.SummonEpicCharacter(characterType, reqCharacters);
            ClosePopup();
        }

        #endregion
    }
}
