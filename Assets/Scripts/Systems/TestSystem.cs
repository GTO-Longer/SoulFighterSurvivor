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
                HeroManager.hero.TakeMagicRecover(999);
            }
            
            if (InfinityHealthPoint)
            {
                HeroManager.hero.TakeHeal(999);
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