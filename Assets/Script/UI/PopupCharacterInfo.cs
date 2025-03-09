using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SB
{
    public class PopupCharacterInfo : MonoBehaviour
    {
        #region UI

        [SerializeField]
        private Image imgCharacter = null;
        
        [SerializeField]
        private TextMeshProUGUI txtName = null;
        
        [SerializeField]
        private TextMeshProUGUI txtDamage = null;

        [SerializeField]
        private TextMeshProUGUI txtAttackSpeed = null;
        
        [SerializeField]
        private TextMeshProUGUI txtSkillDesc = null;
        
        [SerializeField]
        private List<Sprite> sprCharacter = new  List<Sprite>();

        public void SetInfo(Character character)
        {
            imgCharacter.sprite = GetCharacterSprite(character.CharacterType);
            txtName.text = GetCharacterName(character.CharacterType);

            int originDamage = (int)character.OriginDamage;
            int addDamage = (int)(character.Damage - originDamage);
            if(addDamage > 0)
                txtDamage.text = $"{originDamage} + <color=green>{addDamage}</color>";
            else
                txtDamage.text = $"{originDamage}";
            txtAttackSpeed.text = $"{character.AttackSpeed}";
            
            txtSkillDesc.text = GetSkillDesc(character.CharacterType);
        }

        private Sprite GetCharacterSprite(Character.Type type)
        {
            switch (type)
            {
                case Character.Type.NormalArcher:
                    return sprCharacter[0];
                case Character.Type.NormalWarrior:
                    return sprCharacter[1];
                case Character.Type.RareArcher:
                    return sprCharacter[2];
                case Character.Type.RareWarrior:
                    return sprCharacter[3];
                case Character.Type.HeroMagician:
                    return sprCharacter[4];
                case Character.Type.HeroWarrior:
                    return sprCharacter[5];
                case Character.Type.EpicMagician:
                    return sprCharacter[6];
                case Character.Type.EpicWarrior:
                    return sprCharacter[7];
            }

            return null;
        }

        private string GetCharacterName(Character.Type type)
        {
            switch (type)
            {
                case Character.Type.NormalArcher:
                    return "N Archer";
                case Character.Type.NormalWarrior:
                    return "N Warrior";
                case Character.Type.RareArcher:
                    return "R Archer";
                case Character.Type.RareWarrior:
                    return "R Warrior";
                case Character.Type.HeroMagician:
                    return "H Magician";
                case Character.Type.HeroWarrior:
                    return "H Warrior";
                case Character.Type.EpicMagician:
                    return "E Magician";
                case Character.Type.EpicWarrior:
                    return "E Warrior";
            }

            return "";
        }

        /// <summary>
        /// 스킬 설명 
        /// </summary>
        /// <param name="type"> 스킬 설명 가져올 캐릭터 종류 </param>
        /// <returns> 스킬 설명 </returns>
        private string GetSkillDesc(Character.Type type)
        {
            switch (type)
            {
                case Character.Type.NormalArcher:
                    return "";
                case Character.Type.NormalWarrior:
                    return "";
                case Character.Type.RareArcher:
                    return "";
                case Character.Type.RareWarrior:
                    return "";
                case Character.Type.HeroMagician:
                    return "";
                case Character.Type.HeroWarrior:
                    return "";
                case Character.Type.EpicMagician:
                    return "";
                case Character.Type.EpicWarrior:
                    return "";
            }

            return "";
        }

        #endregion
    }
}
