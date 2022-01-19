using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGuardian
{
    public class WeaponHitBoxCheck : MonoBehaviour
    {
        GameObject PlayerGO;

        void Awake()
        {
            PlayerGO = transform.parent.parent.gameObject;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {

                PlayerGO.GetComponent<PlayerController>().OnEnemyHit(collision.gameObject.GetComponent<EnemyController>());

            }

        }
    }
}

