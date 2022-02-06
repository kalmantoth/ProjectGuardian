using System.Collections.Generic;
using UnityEngine;

namespace ProjectGuardian
{
    public class PortalManager : MonoBehaviour
    {
        [SerializeField] private bool spawnEnemies = true;
        private int spawnPortalsDoneCounter = 0;

        [SerializeField] private List<SpawnData> spawnDataList;

        private List<Transform> escapePortals = new List<Transform>();
        private List<Transform> spawnPortals = new List<Transform>();

        public event System.Action SpawnDoneByPortals = delegate { };

        private void Awake()
        {
            escapePortals.AddRange(Util.GetObjectsOnSceneByComponentType(typeof(EscapePortal)));
            spawnPortals.AddRange(Util.GetObjectsOnSceneByComponentType(typeof(ISpawnPortal)));

            SubscribeOnSpawnPortals();
        }

        private void Start()
        {
            EnableSpawnPortals(spawnEnemies);
        }

        private void SubscribeOnSpawnPortals()
        {
            foreach (Transform sP in spawnPortals)
            {
                sP.GetComponent<ISpawnPortal>().PortalSpawnCompleted += OnSpawnPortalsDone;
            }
        }

        private void EnableSpawnPortals(bool enableSpawn)
        {
            DistributeSpawnDataBetweenSpawnPortals();

            foreach (Transform sP in spawnPortals)
            {
                sP.GetComponent<ISpawnPortal>().EnableSpawn(enableSpawn);
            }
        }

        private void DistributeSpawnDataBetweenSpawnPortals()
        {
            int index = 0;
            while (index != spawnDataList.Count)
            {
                foreach (Transform sP in spawnPortals)
                {
                    sP.GetComponent<ISpawnPortal>().AddSpawnDataForPortal(spawnDataList[index++]);
                    if (index == spawnDataList.Count) break;
                }
            }
        }

        private void OnSpawnPortalsDone()
        {
            spawnPortalsDoneCounter++;
            if (spawnPortalsDoneCounter == spawnPortals.Count)
            {
                SpawnDoneByPortals();
            }
        }

        public int GetAllSpawnDataListPrefabCount()
        {
            if (spawnDataList != null && spawnDataList.Count != 0)
            {
                int allSpawnCount = 0;
                foreach (SpawnData sData in spawnDataList)
                {
                    allSpawnCount += sData.TargetSpawn;
                }
                return allSpawnCount;
            }
            return -1;
        }
    }
}