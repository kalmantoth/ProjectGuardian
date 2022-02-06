using UnityEngine;

namespace ProjectGuardian
{
    public interface INpcManager
    {
        public event System.Action AllNpcGone;

        public void AddNewNpc(Transform npc);
        public void RemoveNpc(Transform npc);
    }
}