using System.Collections.Generic;
using Classes;
using DataManagement;
using UnityEngine;

namespace Managers
{
    public class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance;
        private List<Effect> effectList;
        private Transform canvas;

        private void Awake()
        {
            Instance = this;
            canvas = GameObject.Find("SceneEffects").transform;
        }

        private void Start()
        {
            effectList = new List<Effect>();
        }

        private void Update()
        {
            for (var index = effectList.Count - 1; index >= 0; index--)
            {
                var effect = effectList[index];
                effect.EffectUpdate();
                DestroyAfterAnimation(effect);
                
                if (effect.owner != null)
                {
                    effect.effect.transform.position = effect.owner.transform.position;
                }
            }
        }

        public Effect CreateEffect(string effectName, GameObject owner)
        {
            var effect = new Effect(owner, ResourceReader.LoadPrefab($"Effects/{effectName}", transform, false));
            
            effect.effect.transform.position = owner.transform.position;
            effect.effect.SetActive(true);
            
            effectList.Add(effect);
            return effect;
        }

        public Effect CreateEffect(string effectName)
        {
            var effect = new Effect(null, ResourceReader.LoadPrefab($"Effects/{effectName}", transform, false));
            
            effect.effect.SetActive(true);
            
            effectList.Add(effect);
            return effect;
        }

        public Effect CreateCanvasEffect(string effectName, GameObject owner)
        {
            var effect = new Effect(owner, ResourceReader.LoadPrefab($"Effects/{effectName}", transform, false));
            
            effect.effect.transform.SetParent(canvas);
            effect.effect.transform.position = owner.transform.position;
            effect.effect.SetActive(true);
            
            effectList.Add(effect);
            return effect;
        }

        public void DestroyEffect(Effect effect)
        {
            effectList.Remove(effect);
            Destroy(effect.effect.gameObject);
        }

        private void DestroyAfterAnimation(Effect effect)
        {
            var animator = effect.effect.GetComponent<Animator>();
            if (animator == null) return;
            
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Finish"))
            {
                // 销毁对象
                DestroyEffect(effect);
            }
        }
    }
}