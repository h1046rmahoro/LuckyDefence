using System;
using System.Collections;
using UnityEngine;

namespace SB
{
    public class Computer : MonoBehaviour
    {
        #region Info

        private void Start()
        {
            StartCoroutine(AiAction());
        }

        private IEnumerator AiAction()
        {
            var inGame = InGame.Instance;

            WaitForSeconds WFS = new WaitForSeconds(0.5f);
            
            while (true)
            {
                // 소환 시도 
                inGame.AiSummon();
                
                yield return WFS;
                
                // 업그레이드 시도 
                inGame.AiUpgrade();
                
                yield return WFS;

                // 에픽 조합 
                if (inGame.AiUpgradeEpic())
                    yield return WFS;

            }
            
        }

        #endregion
    }
}
