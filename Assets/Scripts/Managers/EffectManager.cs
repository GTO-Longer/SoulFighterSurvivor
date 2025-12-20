using System.Collections.Generic;
using Classes;
using DataManagement;
using UnityEngine;

namespace Managers
{
    public class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance;
        public List<Effect> effectList;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            effectList = new List<Effect>();
        }

        private void Update()
        {
            foreach (var effect in effectList)
            {
                effect.EffectUpdate();
                if (effect.owner != null)
                {
                    effect.effect.transform.position = effect.owner.transform.position;
                }
            }
        }

        public Effect CreateEffect(string effectName, GameObject owner)
        {
            var effect = new Effect(owner, ResourceReader.LoadPrefab(effectName));
            effectList.Add(effect);
            return effect;
        }

        public void DestroyEffect(Effect effect)
        {
            effectList.Remove(effect);
            Destroy(effect.effect.gameObject);
        }
    }
}