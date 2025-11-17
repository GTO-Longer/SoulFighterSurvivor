using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Classes;

namespace Hero
{
    public class HeroManager : MonoBehaviour
    {
        public static Entity hero;
        
        void Start()
        {
            // 创建Hero实例
            hero = GetComponent<EntityData>().entity;
        }

        void Update()
        {
            hero.Move();
        }
    }
}
