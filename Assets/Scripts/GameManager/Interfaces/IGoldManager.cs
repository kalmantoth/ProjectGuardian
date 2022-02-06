using UnityEngine;

namespace ProjectGuardian
{
    public interface IGoldManager
    {
        public event System.Action GoldChestsAreEmptied;

        public Transform GetClosestNonEmptyGoldChest(Transform npc);
    }
}