using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProjectGuardian
{
    public class AttackHitBoxCheck : MonoBehaviour
    {

        GameObject EnemyGO;
        // Start is called before the first frame update
        void Awake()
        {
            EnemyGO = transform.parent.parent.gameObject;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && this.gameObject.activeSelf)
            {
                Debug.Log("Attack on player");
                EnemyGO.GetComponent<EnemyController>().OnPlayerHit(collision.gameObject.GetComponent<PlayerController>());
            }
        }
    }
}

