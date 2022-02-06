using System.Collections;
using UnityEngine;

namespace ProjectGuardian
{
    public enum EntityType
    {
        PLAYER = 1,
        NPC = 2,
        ITEM = 3
    }

    public interface IEntity
    {
        EntityType entityType { get; }
        string parentTransform { get; }

        void SetParentTransform();



    }
}