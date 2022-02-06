using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectGuardian
{
    public class SpawnPortal : MonoBehaviour, ISpawnPortal
    { 
        private bool spawnEnabled;

        private float spawnCDTimer;
        private float spawnCooldown;

        private Queue<SpawnData> spawnDataQueue;

        private ISpawnDataHandler spawnDataHandler;

        public event System.Action PortalSpawnCompleted = delegate { };

        public bool isPortalOpen { get; }

        void Awake()
        {
            spawnDataQueue = new Queue<SpawnData>();
        }

        private void Update()
        {
            HandleSpawnData();
            HandleSpawn();
            IsPortalSpawnCompleted();
        }


        private void HandleSpawnData()
        {
            if (spawnDataQueue.Count != 0 && (spawnDataHandler == null || spawnDataHandler.IsSpawnComplete()))
            {
                spawnDataHandler = new SpawnDataHandler(spawnDataQueue.Dequeue());
                spawnCooldown = spawnDataHandler.GetCooldown();
                spawnCDTimer = 0f;
            }
        }

        private void HandleSpawn()
        {
            if (spawnEnabled && !spawnDataHandler.IsSpawnComplete())
            {
                if (spawnCooldown <= spawnCDTimer)
                {
                    SpawnAtPortal(spawnDataHandler.SpawnResourcePrefab());
                    spawnCDTimer = 0f;
                }
                else
                {
                    spawnCDTimer += Time.deltaTime;
                }
            }
        }

        private void IsPortalSpawnCompleted()
        {
            if (spawnEnabled && spawnDataQueue.Count == 0 && (spawnDataHandler == null || spawnDataHandler.IsSpawnComplete()))
            {
                spawnEnabled = false;
                PortalSpawnCompleted();
            }
        }

        private void SpawnAtPortal(Transform spawnedPrefab)
        {
            _ = Instantiate(Resources.Load(spawnedPrefab.name), this.transform.position, Quaternion.identity) as Transform;
        }

        public void EnableSpawn(bool enableSpawn)
        {
            spawnEnabled = enableSpawn;
        }

        public void AddSpawnDataForPortal(SpawnData spawnData)
        {
            spawnDataQueue.Enqueue(spawnData);
        }

        public void OpenPortal()
        {

        }

        public void ClosePortal()
        {

        }
    }
}
