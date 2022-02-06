using System.Collections;
using UnityEngine;
using System;

namespace ProjectGuardian
{
    public class GoldChestManagement : MonoBehaviour, IGoldChestManagement
    {

        [SerializeField] private int maxStoredGoldCount = 0;
        [SerializeField] private int storedGoldCount = 0;

        private float fullnessPercent = 0;

        public event Action<float> FullnessPercentChanged = delegate { };
        public event Action<int> StoredGoldChanged = delegate { };
        public event Action<Transform> GoldChestIsEmptied = delegate { };

        private void Start()
        {
            if (storedGoldCount == 0)
            {
                storedGoldCount = maxStoredGoldCount;
            }

            CalculateEventValues();
        }

        private void CalculateEventValues()
        {
            StoredGoldChanged(storedGoldCount);
            CheckGoldStatus();
            CalculateFullnessPercent();
        }

        private void CalculateFullnessPercent()
        {
            fullnessPercent = (float)storedGoldCount / maxStoredGoldCount;
            FullnessPercentChanged(fullnessPercent);
        }

        private void CheckGoldStatus()
        {
            if(storedGoldCount == 0)
            {
                GoldChestIsEmptied(this.transform);
            }
        }

        public void SetGoldCount(int goldCount)
        {
            maxStoredGoldCount = storedGoldCount = goldCount;
            CalculateEventValues();
        }

        public bool TakeGold(int takenGold)
        {
            if (storedGoldCount - takenGold >= 0)
            {
                storedGoldCount -= takenGold;
                CalculateEventValues();

                return true;
            }
            return false;
        }

        public bool PutGold(int putGold)
        {
            if (storedGoldCount + putGold <= maxStoredGoldCount)
            {
                storedGoldCount += putGold;

                CalculateEventValues();
                return true;
            }
            return false;
        }
    }
}