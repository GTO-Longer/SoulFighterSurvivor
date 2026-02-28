using DG.Tweening;
using Managers.EntityManagers;
using Systems;
using UnityEngine;
using UnityEngine.UI;

namespace Classes.Equipments
{
    public class GoldTrophy : Equipment
    {
        public GoldTrophy() : base("GoldTrophy")
        {
            
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            
            // 游戏结束
            ShopSystem.Instance.CloseShopPanel();
            HeroManager.hero.isAlive = false;
            GameObject.Find("HUD/WinBackground").GetComponent<Image>().DOFade(0.95f, 1.5f);
            GameObject.Find("HUD/WinBackground").GetComponent<Image>().DOFade(1f, 2.8f).OnComplete(() =>
            {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
            });
        }
        
        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription);
            return true;
        }
    }
}