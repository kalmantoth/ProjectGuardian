using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGuardian
{
    public enum PortalType
    {
        SPAWN_PORTAL = 0,
        ESCAPE_PORTAL = 1
    }

    public class Portal : MonoBehaviour
    {
        public PortalType m_PortalType;
        public float m_EnemySpawnCooldown;
        public float m_EnemySpawnCDMaxRndOffset;
        float m_EnemySpawnCDRndOffset;


        Rigidbody2D m_Rigidbody2D;
        float m_EnemySpawnCDTimer;
        bool m_SpawnEnabled;

        void Awake()
        {
            
            m_Rigidbody2D = this.GetComponent<Rigidbody2D>();
            CalculateSpawnCD();
        }

        private void Start()
        {
            if (GameMaster.Instance != null) GameMaster.Instance.AddPortal(this.gameObject);
        }

        void Update()
        {
            if(m_PortalType == PortalType.SPAWN_PORTAL && m_SpawnEnabled)
            {
                if ((m_EnemySpawnCooldown + m_EnemySpawnCDRndOffset) <= m_EnemySpawnCDTimer)
                {
                    SpawnEnemy();
                }

                m_EnemySpawnCDTimer += Time.deltaTime;
            }

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if(this.m_PortalType == PortalType.ESCAPE_PORTAL)
                    collision.GetComponent<EnemyController>().EnemyEscapedWithGold();
            }
        }


        void SpawnEnemy()
        {
            GameObject spawnedEnemy;

            spawnedEnemy = Instantiate(Resources.Load("Slime"), this.transform.position, Quaternion.identity) as GameObject;
            spawnedEnemy.GetComponent<EnemyController>().m_Target = GameMaster.Instance.FindClosestGoldChest(spawnedEnemy).transform;
            spawnedEnemy.transform.SetParent(GameObject.Find("Enemies").GetComponent<Transform>());

            m_EnemySpawnCDTimer = 0;
            CalculateSpawnCD();


            GameMaster.Instance.AddNewEnemy(spawnedEnemy);
                
            Debug.Log(spawnedEnemy.GetComponent<EnemyController>().name + " enemy spawned");
        }

        void CalculateSpawnCD()
        {
            m_EnemySpawnCDRndOffset = Random.Range(-m_EnemySpawnCDMaxRndOffset, m_EnemySpawnCDMaxRndOffset);
        }

        public void InitSpawnPortals(float enemySpawnCooldown, float enemySpawnCDMaxRndOffset)
        {
            m_EnemySpawnCooldown = enemySpawnCooldown;
            m_EnemySpawnCDMaxRndOffset = enemySpawnCDMaxRndOffset / 2 ;
        }

        public void EnableSpawn(bool enableSpawn)
        {
            m_SpawnEnabled = enableSpawn;
        }

    }

}
