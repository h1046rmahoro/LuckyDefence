using System;
using TMPro;
using UnityEngine;

namespace SB
{
    public class PopupUpgrade : MonoBehaviour
    {
        #region UI

        /// <summary>
        /// 골드 개수 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI txtGold = null;
        
        /// <summary>
        /// 젬 개수 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI txtGem = null;


        /// <summary>
        /// Rare 등급 레벨 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI txtRareLevel = null;
        
        /// <summary>
        /// Hero 등급 레벨 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI txtHeroLevel = null;
        
        /// <summary>
        /// Epic 등급 레벨 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI txtEpicLevel = null;
        
        /// <summary>
        /// Normal, Rare 업그레이드 가격 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI txtRarePrice = null;
        
        /// <summary>
        /// Hero 업그레이드 가격 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI txtHeroPrice = null;
        
        /// <summary>
        /// Epic 업그레이드 가격 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI txtEpicPrice = null;


        private void OnEnable()
        {
            var inGame = InGame.Instance;

            inGame.OnGoldChanged += OnGoldChange;
            inGame.OnGemChanged += OnGemChange;

            OnGoldChange(inGame.Gold);
            OnGemChange(inGame.Gem);

            int priceRare = 30 * inGame.RareUpgradeCount;
            txtRarePrice.text = $"{priceRare}";

            int priceHero = 50 * inGame.HeroUpgradeCount;
            txtHeroPrice.text = $"{priceHero}";

            int priceEpic = inGame.EpicUpgradeCount + 1;
            txtEpicPrice.text = $"{priceEpic}";
        }

        private void OnDisable()
        {
            var inGame = InGame.Instance;
            
            inGame.OnGoldChanged -= OnGoldChange;
            inGame.OnGemChanged -= OnGemChange;
        }

        /// <summary>
        /// 팝업 닫기 
        /// </summary>
        public void ClosePopup()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 골드 변경 
        /// </summary>
        /// <param name="gold"> 현재 골드 </param>
        private void OnGoldChange(int gold)
        {
            txtGold.text = $"{gold}";
        }

        /// <summary>
        /// 젬 개수 변경 
        /// </summary>
        /// <param name="gem"> 현재 보유 젬 개수 </param>
        private void OnGemChange(int gem)
        {
            txtGem.text = $"{gem}";
        }

        public void UpgradeRare()
        {
            var inGame = InGame.Instance;
            
            // 가격 계산 
            int priceRare = 30 * inGame.RareUpgradeCount;

            // 골드 부족 
            if (inGame.Gold < priceRare)
                return;

            // 업그레이드 
            inGame.RareUpgradeCount += 1;

            // 골드 차감 
            inGame.Gold -= priceRare;

            
            // 상승가격 설정
            priceRare += 30;
            txtRarePrice.text = $"{priceRare}";
            
            // 상승 레벨 설정
            txtRareLevel.text = $"Lv.{inGame.RareUpgradeCount}";
        }

        public void UpgradeHero()
        {
            var inGame = InGame.Instance;

            // 가격 계산 
            int priceHero = 50 * inGame.HeroUpgradeCount;
            
            // 골드 부족 
            if (inGame.Gold < priceHero)
                return;

            // 업그레이드 
            inGame.HeroUpgradeCount += 1;

            // 골드 차감 
            inGame.Gold -= priceHero;

            // 상승가격 설정
            priceHero += 50;
            txtHeroPrice.text = $"{priceHero}";

            // 상승 레벨 설정
            txtHeroLevel.text = $"Lv.{inGame.HeroUpgradeCount}";
        }

        public void UpgradeEpic()
        {
            var inGame = InGame.Instance;

            // 가격 계산 
            int priceEpic = inGame.EpicUpgradeCount + 1;
            
            // 젬 부족 
            if (inGame.Gem < priceEpic)
                return;

            // 업그레이드 
            inGame.EpicUpgradeCount += 1;

            // 골드 차감 
            inGame.Gem -= priceEpic;

            // 상승가격 설정
            priceEpic += 1;
            txtEpicPrice.text = $"{priceEpic}";
            
            // 상승 레벨 설정
            txtEpicLevel.text = $"Lv.{inGame.EpicUpgradeCount}";
        }

        #endregion
    }
}
