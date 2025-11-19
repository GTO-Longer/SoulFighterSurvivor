using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataManagement;

namespace Entities.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            GetComponent<EntityData>().entity.Move();
        }
    }
}
