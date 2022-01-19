using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGuardian
{
    public class AttackRangeBox : MonoBehaviour
    {
        GameObject m_ParentEnemyGO;
        public ORIENTATION m_orientation = ORIENTATION.RIGHT;

        void Awake()
        {
            m_ParentEnemyGO = transform.parent.parent.gameObject;
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                m_ParentEnemyGO.GetComponent<EnemyController>().OnPlayerDetectInAttackRange(m_orientation);
            }

        }
    }
}


