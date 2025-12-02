using System;
using Managers.EntityManagers;
using UnityEngine;

namespace Systems
{
    public class TestSystem : MonoBehaviour
    {
        public static TestSystem Instance { get; private set; }
        public bool InfinityMagicPoint = false;
        public bool InfinityHealthPoint = false;
        public bool SkillNoCoolDown = false;
        public bool LevelUp = false;

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
        }
    }
}