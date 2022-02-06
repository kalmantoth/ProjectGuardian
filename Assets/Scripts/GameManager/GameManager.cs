using UnityEngine;

namespace ProjectGuardian
{

    [RequireComponent(typeof(GamePhaseManager))]
    [RequireComponent(typeof(PortalManager))]
    [RequireComponent(typeof(GoldManager))]
    [RequireComponent(typeof(NpcManager))]
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        //private GamePhaseManager gamePhaseManager;
        //private PortalManager portalManager;
        private IGoldManager goldManager;
        private INpcManager npcManager;

        private void Awake()
        {
            _instance = this;

            //gamePhaseManager = GetComponent<GamePhaseManager>();
            //portalManager = GetComponent<PortalManager>();
            goldManager = GetComponent<IGoldManager>();
            npcManager = GetComponent<INpcManager>();
        }

        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError(GameManagerStrings.GameManager_Instace_IsNull);
                }
                return _instance;
            }
        }

        public void AddNewNpc(Transform npc)
        {
            npcManager.AddNewNpc(npc);
        }

        public void RemoveNpc(Transform npc)
        {
            npcManager.RemoveNpc(npc);
        }

        public Transform GetClosestNonEmptyGoldChest(Transform npc)
        {
            return goldManager.GetClosestNonEmptyGoldChest(npc);
        }
    }
}

