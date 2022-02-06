using System.Collections;
using UnityEngine;

namespace ProjectGuardian
{
    public interface ISpawnDataHandler
    {
        public float GetCooldown();
        public Transform SpawnResourcePrefab();
        public bool IsSpawnComplete();

    }
}