using UnityEditor;
using UnityEngine;

namespace ProjectGuardian
{
    [CreateAssetMenu(menuName = "Configuration Data/Spawn Data", fileName = "SpawnData")]
    public class SpawnData : ScriptableObject
    {
        [SerializeField] private int targetSpawn = 5;

        [SerializeField] private float spawnCooldown = 5f;
        [SerializeField] private float spawnCDMaxRndOffset = 0f;

        [SerializeField] private Transform spawnResourcePrefab = null;

        public int TargetSpawn { get => targetSpawn; }
        public Transform SpawnResourcePrefab { get => spawnResourcePrefab; }
        public float SpawnCooldown { get => spawnCooldown; }
        public float SpawnCDMaxRndOffset { get => spawnCDMaxRndOffset; }


    }
}