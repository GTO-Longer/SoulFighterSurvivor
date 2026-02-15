using System;
using Classes.Entities;
using UnityEngine;
using DataManagement;
using Managers.EntityManagers;
using Utilities;

namespace Managers
{
    public class HeroModelManager : MonoBehaviour
    {
        public static HeroModelManager instance;
        
        private Animator animator;
        private Hero hero;
        private GameObject model;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                
                hero = HeroManager.hero;
                model = ResourceReader.LoadPrefab($"HeroModels/{hero.heroName}_Model", transform);
                animator = model.GetComponent<Animator>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            transform.position = hero.gameObject.transform.position;
            transform.localScale = new Vector3(hero.scale.Value / 100f, hero.scale.Value / 100f, hero.scale.Value / 100f);
            transform.eulerAngles = hero.GetEulerAngles();
            
            animator.SetBool("IsMoving", hero.isMoving);
        }

        public void QSkillAnimation(int specialIndex = -1)
        {
            animator.SetTrigger("QSkill");
            if (specialIndex != -1)
            {
                animator.SetInteger("SpecialIndex", specialIndex);
            }
        }
        
        public void WSkillAnimation(int specialIndex = -1)
        {
            animator.SetTrigger("WSkill");
            if (specialIndex != -1)
            {
                animator.SetInteger("SpecialIndex", specialIndex);
            }
        }

        public void ESkillAnimation(int specialIndex = -1)
        {
            animator.SetTrigger("ESkill");
            if (specialIndex != -1)
            {
                animator.SetInteger("SpecialIndex", specialIndex);
            }
        }

        public void RSkillAnimation(int specialIndex = -1)
        {
            animator.SetTrigger("RSkill");
            if (specialIndex != -1)
            {
                animator.SetInteger("SpecialIndex", specialIndex);
            }
        }
    }
}
