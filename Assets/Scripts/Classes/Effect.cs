using System;
using UnityEngine;

namespace Classes
{
    public class Effect
    {
        public GameObject owner;
        public GameObject effect;
        public Action EffectUpdateEvent;

        public Effect(GameObject owner, GameObject effect)
        {
            this.owner = owner;
            this.effect = effect;
        }

        public void EffectUpdate()
        {
            EffectUpdateEvent?.Invoke();
        }
    }
}