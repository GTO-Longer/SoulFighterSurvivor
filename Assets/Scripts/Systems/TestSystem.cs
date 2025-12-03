using Managers.EntityManagers;
using UnityEngine;

namespace Systems
{
    public class TestSystem : MonoBehaviour
    {
        public static TestSystem Instance { get; private set; }
        public bool InfinityMagicPoint;
        public bool InfinityHealthPoint;
        public bool SkillNoCoolDown;
        public bool LevelUp;
        public bool Gain20000Coin;

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
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

            if (LevelUp)
            {
                HeroManager.hero.LevelUp();
                LevelUp = false;
            }

            if (Gain20000Coin)
            {
                HeroManager.hero.coins.Value += 20000;
                Gain20000Coin = false;
            }
        }
    }
}