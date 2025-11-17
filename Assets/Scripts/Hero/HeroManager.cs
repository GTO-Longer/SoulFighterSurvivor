using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Classes;

namespace Hero
{
    public class HeroManager : MonoBehaviour
    {
        public static Classes.Hero hero;
        
        void Awake()
        {
            // 创建Hero实例
            hero = (Classes.Hero)GetComponent<EntityData>().entity;
        }

        void Update()
        {
            hero.Move();
            hero.TargetCheck();
        }
    }
}
