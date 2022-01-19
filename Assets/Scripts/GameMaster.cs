using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGuardian
{
    public class GameMaster : MonoBehaviour
    {

        private static GameMaster _instance;

        private List<GameObject> m_EscapePortals = new List<GameObject>();
        private List<GameObject> m_SpawnPortals = new List<GameObject>();
        private List<GameObject> m_GoldChests = new List<GameObject>();
        private List<GameObject> m_Enemies = new List<GameObject>();

        public int m_MaxEnemyCount;
        public int m_CurrentEnemyCount;

        public float m_EnemySpawnCooldown;
        public float m_EnemySpawnCDMaxRndOffset;

        int m_MaxStoredGold;
        int m_CurrentStoredGold;
        int m_TakenGold;
        int m_StolenGold;

        bool m_IsGameOver = false;
        string m_GameOverText;

        public bool m_SpawnEnemies = true;


        public static GameMaster Instance
        {
            get
            {
                if (_instance == null)
                    Debug.LogError("GameMaster is Null");
                return _instance;
            }
        }

        void Awake()
        {
            _instance = this;
        }


        void Start()
        {
            // Check amx stored gold
            foreach (GameObject gC in m_GoldChests)
            {
                m_MaxStoredGold += gC.GetComponent<GoldChest>().m_CurrentStoredGoldCount;
            }

            EnableSpawnPortals(m_SpawnEnemies);
        }

        void Update()
        {

            if (m_Enemies.Count == m_MaxEnemyCount)
            {
                m_SpawnEnemies = false;
                EnableSpawnPortals(m_SpawnEnemies);
            }

            CheckGoldStatus();
            CheckGameOver();
        }

        void EnableSpawnPortals(bool enableSpawn)
        {
            foreach (GameObject sP in m_SpawnPortals)
            {
                sP.GetComponent<Portal>().InitSpawnPortals(m_EnemySpawnCooldown, m_EnemySpawnCDMaxRndOffset);
                sP.GetComponent<Portal>().EnableSpawn(enableSpawn);
            }
        }


        public GameObject FindClosestEscapePortal(GameObject fromGO)
        {
            return Util.FindClosestGameobject(fromGO, m_EscapePortals);
        }

        public GameObject FindClosestGoldChest(GameObject fromGO)
        {
            return Util.FindClosestGameobject(fromGO, m_GoldChests);
        }

        void CheckGoldStatus()
        {
            m_CurrentStoredGold = 0;
            foreach (GameObject gC in m_GoldChests)
            {
                m_CurrentStoredGold += gC.GetComponent<GoldChest>().m_CurrentStoredGoldCount;
            }

            m_TakenGold = m_MaxStoredGold - m_CurrentStoredGold;
        }

        void CheckGameOver()
        {
            if (!m_IsGameOver)
            {
                if (m_StolenGold == m_MaxStoredGold && (!m_SpawnEnemies && m_Enemies.Count == 0))
                {
                    m_IsGameOver = true;
                    m_GameOverText = "You Let The Enemy PLUNDER The Castle! Shame On You!";
                }
                else if (m_StolenGold < m_MaxStoredGold && (!m_SpawnEnemies && m_Enemies.Count == 0))
                {
                    m_IsGameOver = true;
                    m_GameOverText = "You SAVED The Castle! Well Done Soldier!";
                }

                if (m_IsGameOver)
                {
                    Debug.Log(m_GameOverText);
                }
            }

        }

        public void AddNewEnemy(GameObject enemy)
        {
            m_Enemies.Add(enemy);
        }

        public void RemoveEnemy(GameObject enemy)
        {
            m_Enemies.Remove(enemy);
        }

        public void AddPortal(GameObject portal)
        {
            if(portal.GetComponent<Portal>().m_PortalType == PortalType.SPAWN_PORTAL)
                m_SpawnPortals.Add(portal);
            else if(portal.GetComponent<Portal>().m_PortalType == PortalType.ESCAPE_PORTAL)
                m_EscapePortals.Add(portal);
        }

        public void AddGoldChest(GameObject goldChest)
        {
            m_GoldChests.Add(goldChest);
        }

        public void GoldStolen()
        {
            m_StolenGold++;
        }



    }
}

