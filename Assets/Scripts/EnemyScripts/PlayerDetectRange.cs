using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGuardian
{
    public class PlayerDetectRange : MonoBehaviour
    {
        GameObject m_ParentEnemyGO;

        void Awake()
        {
            m_ParentEnemyGO = transform.parent.parent.gameObject;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                m_ParentEnemyGO.GetComponent<EnemyController>().OnPlayerDetect(collision.gameObject);
            }

        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                m_ParentEnemyGO.GetComponent<EnemyController>().OnPlayerOutOfDetection();
            }

        }


    }
}

