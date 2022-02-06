using System.Collections;
using UnityEngine;

namespace ProjectGuardian
{
    public class SpawnDataHandler : ISpawnDataHandler
    {
        private bool isSpawnComplete;

        private float calculatedCooldown;

        private int spawned = 0;

        private SpawnData spawnData;

        private void CalculateSpawnCooldown()
        {
            if (spawnData.SpawnCDMaxRndOffset != 0)
            {
                float tempOffset = spawnData.SpawnCDMaxRndOffset / 2;
                float rnd = Random.Range(-tempOffset, tempOffset);
                calculatedCooldown = spawnData.SpawnCooldown + rnd;
            }
            else
                calculatedCooldown = spawnData.SpawnCooldown;
        }

        public float GetCooldown() => calculatedCooldown;

        public Transform SpawnResourcePrefab()
        {
            spawned++;
            if (spawned == spawnData.TargetSpawn)
            {
                isSpawnComplete = true;
                spawned = 0;
            }
            return spawnData.SpawnResourcePrefab;

        }

        public SpawnDataHandler(SpawnData spawnData)
        {
            this.spawnData = spawnData;
            isSpawnComplete = false;
            CalculateSpawnCooldown();
        }

        public bool IsSpawnComplete()
        {
            return isSpawnComplete;
        }
    }
}