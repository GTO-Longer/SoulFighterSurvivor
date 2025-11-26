using EntityManagers;
using UnityEngine;

namespace Systems
{
    public class TestSystem : MonoBehaviour
    {
        public bool InfinityMagicPoint = false;
        public bool InfinityHealthPoint = false;
        public bool SkillNoCoolDown = false;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if (InfinityMagicPoint)
            {
                HeroManager.hero.magicPoint = HeroManager.hero.maxMagicPoint;
            }
            
            if (InfinityHealthPoint)
            {
                HeroManager.hero.healthPoint = HeroManager.hero.maxHealthPoint;
            }
            
            if (SkillNoCoolDown)
            {
                foreach (var skill in HeroManager.hero.skillList)
                {
                    skill.coolDownTimer = 999f;
                }
            }
        }
    }
}