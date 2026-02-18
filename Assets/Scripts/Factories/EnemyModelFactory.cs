using System;
using System.Collections.Generic;
using Classes.Entities;
using UnityEngine;
using DataManagement;
using Utilities;

namespace Factories
{
    public class EnemyModelFactory : MonoBehaviour
    {
        public static EnemyModelFactory Instance;

        private Dictionary<Enemy, (GameObject, Animator)> enemyModelDic;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                enemyModelDic = new Dictionary<Enemy, (GameObject, Animator)>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            foreach (var enemy in enemyModelDic.Keys)
            {
                if (enemy != null && enemy.isAlive)
                {
                    enemyModelDic[enemy].Item1.transform.position = enemy.gameObject.transform.position;
                    enemyModelDic[enemy].Item1.transform.localScale = new Vector3(enemy.scale.Value / 100f, enemy.scale.Value / 100f,enemy.scale.Value / 100f);
                    enemyModelDic[enemy].Item1.transform.eulerAngles = new Vector3(-enemy.GetEulerAngles().z, 90, -90);
                }
            }
        }

        public void CreateModel(Enemy enemy, string enemyType)
        {
            var model = ResourceReader.LoadPrefab($"EnemyModels/{enemyType}_Model", transform);
            var animator = model.GetComponent<Animator>();
            enemyModelDic.Add(enemy, (model, animator));
        }

        public void AttackAnimation(Enemy enemy, int specialIndex = -1)
        {
            enemyModelDic[enemy].Item2.Play("AttackState");
            enemyModelDic[enemy].Item2.SetInteger("SpecialIndex", specialIndex != -1 ? specialIndex : 0);
        }

        public void DeathAnimation(Enemy enemy)
        {
            var obj = enemyModelDic[enemy].Item1;
            var anim = enemyModelDic[enemy].Item2;
            enemyModelDic.Remove(enemy);
            
            anim.Play("DeathState");
            Async.SetAsync(2, null, null, () =>
            {
                Destroy(obj);
            });
        }
    }
}
