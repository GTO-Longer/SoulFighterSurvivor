using DataManagement;
using UnityEngine;


namespace Entities.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        private void Update()
        {
            GetComponent<EntityData>().entity.Move();
            GetComponent<EntityData>().entity.Attack();
        }
    }
}
