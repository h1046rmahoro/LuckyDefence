using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SB
{
    public class ScreenInGame  : MonoBehaviour
    {
        private void OnEnable()
        {
            var inGame = InGame.Instance;
            
            inGame.OnWaveLevelChanged += OnWaveLevelChanged;
            inGame.OnWaveTimeChanged += OnWaveTimeChanged;
            inGame.OnMonsterCountChanged += ChangeMonsterCount;
            
            inGame.OnGoldChanged += ChangeGold;
            inGame.OnGemChanged += ChangeGem;
            inGame.OnCharacterCountChanged += ChangeCharacterCount;
        }

        private void OnDisable()
        {
            var inGame = InGame.Instance;
            if (inGame != null)
            {
                inGame.OnWaveLevelChanged -= OnWaveLevelChanged;
                inGame.OnWaveTimeChanged -= OnWaveTimeChanged;
                inGame.OnMonsterCountChanged -= ChangeMonsterCount;

                inGame.OnGoldChanged -= ChangeGold;
                inGame.OnGemChanged -= ChangeGem;
                inGame.OnCharacterCountChanged += ChangeCharacterCount;
            }
        }
        

        #region Info

        /// <summary>
        /// 현재 웨이브 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI txtWave = null;
        
        /// <summary>
        /// 지난 시간 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI txtTime = null;
        
        /// <summary>
        /// 현재 난이도 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI txtDifficult = null;
        
        /// <summary>
        /// 현재 몬스터 마릿수 
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI txtMonsterCount = null;
        
        /// <summary>
        /// 현재 몬스터 마릿수 게이지 
        /// </summary>
        [SerializeField]
        private Image imgMonsterCount = null;
        
        private void OnWaveLevelChanged(int waveLevel)
        {
            txtWave.text = $"WAVE {waveLevel}";
        }

        /// <summary>
        /// 웨이브 남은 시간 
        /// </summary>
        /// <param name="second"></param>
        private void OnWaveTimeChanged(int second)
        {
            txtTime.text = $"{00}:{second}";
        }

        #endregion


        #region UI

        /// <summary>
        /// 에픽 조합 팝업 열기 
        /// </summary>
        public void ShowEpicPopup()
        {
            InGame.Instance.ShowEpicPopup();
        }

        /// <summary>
        ///  게블 팝업 열기 
        /// </summary>
        public void ShowGamblePopup()
        {
            InGame.Instance.ShowGamblePopup();
        }

        /// <summary>
        /// 업그레이드 팝업 열기 
        /// </summary>
        public void ShowUpgradePopup()
        {
            InGame.Instance.ShowUpgradePopup();
        }

        #endregion
        

        #region Monster

        private void ChangeMonsterCount(int count, int maxCount)
        {
            txtMonsterCount.text = $"{count}/{maxCount}";
            imgMonsterCount.fillAmount = count / (float)maxCount;
        }

        #endregion
        
        
        #region Summon
        
        [SerializeField]
        private TextMeshProUGUI txtSummonPrice = null;
        
        [SerializeField]
        private TextMeshProUGUI txtGold = null;
        
        [SerializeField]
        private TextMeshProUGUI txtGem = null;
        
        [FormerlySerializedAs("txtHeroCount")] [SerializeField]
        private TextMeshProUGUI txtCharacterCount = null;
        
        /// <summary>
        /// 골드 변화 
        /// </summary>
        /// <param name="gold"> 현재 보유 골드 </param>
        private void ChangeGold(int gold)
        {
            txtGold.text = $"{gold}";
        }

        /// <summary>
        /// 젬 변화 
        /// </summary>
        /// <param name="gem"> 현재 보유 젬 </param>
        private void ChangeGem(int gem)
        {
            txtGem.text = $"{gem}";
        }

        /// <summary>
        /// 캐릭터 개수 변화
        /// </summary>
        /// <param name="characterCount"> 현재 보유중인 캐릭터 개수 </param>
        private void ChangeCharacterCount(int characterCount)
        {
            txtCharacterCount.text = $"{characterCount}/20";
        }

        /// <summary>
        /// 캐릭터 소환 
        /// </summary>
        public void Summon()
        {
            InGame.Instance.Summon();

            txtSummonPrice.text = InGame.Instance.SummonPrice.ToString();
        }

        #endregion
    }
}
