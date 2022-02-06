using UnityEngine;
using System;

namespace ProjectGuardian
{
    public interface IGoldChestManagement
    {
        public event Action<float> FullnessPercentChanged;
        public event Action<int> StoredGoldChanged;
        public event Action<Transform> GoldChestIsEmptied;

        public void SetGoldCount(int goldCount);
        public bool TakeGold(int takenGold);
        public bool PutGold(int putGold);
    }
}