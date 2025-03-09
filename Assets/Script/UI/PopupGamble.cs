using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SB
{
    public class PopupGamble : MonoBehaviour
    {
        #region UI

        [SerializeField]
        private TextMeshProUGUI txtGem = null;
        
        /// <summary>
        /// 현재 소환된 캐릭터 개수 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI txtCharacterCount = null;

        private void OnEnable()
        {
            int count = InGame.Instance.CharacterCount;
            txtCharacterCount.text = $"{count}/20";

            InGame.Instance.OnGemChanged += OnGemChange;
            OnGemChange(InGame.Instance.Gem);
        }

        private void OnDisable()
        {
            InGame.Instance.OnGemChanged -= OnGemChange;
        }

        /// <summary>
        /// 젬 개수 변경 
        /// </summary>
        /// <param name="gem"> 현재 젬 개수 </param>
        private void OnGemChange(int gem)
        {
            txtGem.text = $"{gem}";
        }

        public void ClosePopup()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 레어 겜블 
        /// </summary>
        public void GambleRare()
        {
            // 20마리 제한으로 소환 불가 
            if (InGame.Instance.CharacterCount >= 20)
                return;
            
            // 젬 개수 부족인 경우 소환 불가 
            if (InGame.Instance.Gem < 1)
                return;
            
            // 확률 뽑기 
            int rnd = Random.Range(0, 100);

            // 60% 확률 
            if (rnd < 60)
            {
                InGame.Instance.Summon(Character.Type.Rare);
            }
            else
            {
                
            }
            
            // 젬 개수 차감 
            InGame.Instance.Gem -= 1;
        }

        /// <summary>
        /// 영웅 겜블 
        /// </summary>
        public void GambleHero()
        {
            // 20마리 제한으로 소환 불가 
            if (InGame.Instance.CharacterCount >= 20)
                return;
            
            // 젬 개수 부족인 경우 소환 불가 
            if (InGame.Instance.Gem < 1)
                return;
            
            // 확률 뽑기 
            int rnd = Random.Range(0, 100);

            // 20% 확률 
            if(rnd < 20)
                InGame.Instance.Summon(Character.Type.Hero);   
            
            // 젬 개수 차감 
            InGame.Instance.Gem -= 1;
        }

        /// <summary>
        /// 에픽 겜블 
        /// </summary>
        public void GambleEpic()
        {
            // 20마리 제한으로 소환 불가 
            if (InGame.Instance.CharacterCount >= 20)
                return;
            
            // 젬 개수 부족인 경우 소환 불가 
            if (InGame.Instance.Gem < 2)
                return;
            
            // 확률 뽑기 
            int rnd = Random.Range(0, 100);

            // 10% 확률 
            if(rnd < 10)
                InGame.Instance.Summon(Character.Type.Epic);

            // 젬 개수 차감 
            InGame.Instance.Gem -= 2;
        }

        #endregion
    }
}
