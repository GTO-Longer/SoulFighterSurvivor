using DataManagement;
using UnityEngine;


namespace EntityManagers
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
