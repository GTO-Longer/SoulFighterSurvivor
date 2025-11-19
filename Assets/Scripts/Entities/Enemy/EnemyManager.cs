using DataManagement;
using UnityEngine;


namespace Entities.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        private void Start()
        {

        }

        private void Update()
        {
            GetComponent<EntityData>().entity.Move();
        }
    }
}
