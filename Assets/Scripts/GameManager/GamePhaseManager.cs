using UnityEngine;


namespace ProjectGuardian
{
    [RequireComponent(typeof(PortalManager))]
    [RequireComponent(typeof(GoldManager))]
    [RequireComponent(typeof(NpcManager))]
    public class GamePhaseManager : MonoBehaviour
    {
        private string gameOverText;

        private bool gameOverStatus = false;
        private bool isSpawnDone = false;
        private bool isAllGoldTaken = false;
        private bool isAllNpcGone = false;

        private PortalManager portalManager;
        private NpcManager npcManager;
        private GoldManager goldManager;


        private void Awake()
        {
            portalManager = GetComponent<PortalManager>();
            npcManager = GetComponent<NpcManager>();
            goldManager = GetComponent<GoldManager>();

            portalManager.SpawnDoneByPortals += OnSpawnDone;
            goldManager.GoldChestsAreEmptied += OnAllGoldTaken;
            npcManager.AllNpcGone += OnAllNpcGone;
        }

        private void Update()
        {
            CheckGameOver();
        }

        private void OnAllNpcGone()
        {
            if (isSpawnDone)
            {
                isAllNpcGone = true;
            }
        }

        private void OnSpawnDone()
        {
            isSpawnDone = true;
        }

        private void OnAllGoldTaken()
        {
            isAllGoldTaken = true;
        }

        // Check for gameover condition
        private void CheckGameOver()
        {
            if (!gameOverStatus)
            {
                if (isSpawnDone && isAllNpcGone)
                {
                    // on lose - they stole all of the stored gold, the portals done with spawning
                    if (isAllGoldTaken)
                    {
                        gameOverText = GameManagerStrings.GameOver_OnLose;
                    }
                    // on lose - they DIDNT stole all of the stored gold, the portals done with spawning
                    else if (!isAllGoldTaken)
                    {
                        gameOverText = GameManagerStrings.GameOver_OnWin;
                    }
                    gameOverStatus = true;

                    // display lose text once on gameover
                    Debug.Log(gameOverText);
                }
            }
        }
    }
}