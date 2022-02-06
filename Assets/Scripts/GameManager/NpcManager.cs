using System.Collections.Generic;
using UnityEngine;

namespace ProjectGuardian
{
    public class NpcManager : MonoBehaviour , INpcManager
    {
        private List<Transform> npcs = new List<Transform>();

        public event System.Action AllNpcGone = delegate { };

        private void Update()
        {
            CheckNpcCount();
        }

        private void CheckNpcCount()
        {
            if (npcs.Count == 0)
            {
                AllNpcGone();
            }
        }

        public void AddNewNpc(Transform npc)
        {
            npcs.Add(npc);
        }

        public void RemoveNpc(Transform npc)
        {
            npcs.Remove(npc);
        }
    }
}