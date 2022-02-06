using System.Collections.Generic;
using UnityEngine;

namespace ProjectGuardian
{
    public class GoldManager : MonoBehaviour , IGoldManager
    {
        private PortalManager portalManager;

        [SerializeField] private int initialGoldCount = 0;

        [SerializeField] private bool setGoldAsSpawnCount = false;
        [SerializeField] private bool distributeGoldBetweenChest = false;

        private List<Transform> goldChests = new List<Transform>();
        private List<Transform> emptiedGoldChests = new List<Transform>();

        public event System.Action GoldChestsAreEmptied = delegate { };

        

        private void Awake()
        {
            portalManager = GetComponent<PortalManager>();

            goldChests.AddRange(Util.GetObjectsOnSceneByComponentType(typeof(IGoldChestManagement)));
            SubscribeOnSpawnGoldChests();
        }

        private void Start()
        {
            if (setGoldAsSpawnCount)
            {
                SetGoldMatchSpawnCount();
            }
            if (distributeGoldBetweenChest)
            {
                DistributeGoldBetweenGoldChests();
            }
        }

        private void SetGoldMatchSpawnCount()
        {
            initialGoldCount = portalManager.GetAllSpawnDataListPrefabCount();
        }

        private void DistributeGoldBetweenGoldChests()
        {
            int initGold = initialGoldCount;
            int goldPerGoldChest = initGold / goldChests.Count;

            for (int i = 0; i < goldChests.Count; i++)
            {
                goldChests[i].GetComponent<IGoldChestManagement>().SetGoldCount(goldPerGoldChest);
                initGold -= goldPerGoldChest;
                goldPerGoldChest = initGold / (goldChests.Count  - ( i + 1 ));
            }
        }

        private void SubscribeOnSpawnGoldChests()
        {
            foreach (Transform gC in goldChests)
            {
                gC.GetComponent<IGoldChestManagement>().GoldChestIsEmptied += OnGoldChestIsEmptied;
            }
        }

        private void OnGoldChestIsEmptied(Transform goldChest)
        {
            goldChests.Remove(goldChest);
            emptiedGoldChests.Add(goldChest);

            if(goldChests.Count == 0)
            {
                GoldChestsAreEmptied();
            }
        }

        public Transform GetClosestNonEmptyGoldChest(Transform npc)
        {
            return Util.FindClosestTransform(npc, goldChests);
        }

    }
}