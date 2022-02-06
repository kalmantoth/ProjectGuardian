using UnityEngine;

namespace ProjectGuardian
{
    public interface ISpawnPortal : IPortal
    {
        public event System.Action PortalSpawnCompleted;

        public void EnableSpawn(bool enableSpawn);
        public void AddSpawnDataForPortal(SpawnData spawnData);
    }
}