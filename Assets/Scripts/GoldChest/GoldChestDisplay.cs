using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ProjectGuardian
{
    [RequireComponent(typeof(GoldChestManagement))]
    public class GoldChestDisplay: MonoBehaviour
    {
        
        [Tooltip("Default 5 sprites as GoldChest fullness levels.\nWhere 1st is empty and 5th is full.")]
        [SerializeField]  private Sprite[] spriteArray;

        private SpriteRenderer spriteRenderer;
        private Text chestText;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            chestText = GetComponentInChildren<Text>();

            GetComponent<IGoldChestManagement>().FullnessPercentChanged += CalculateFullnessLevelToSprites;
            GetComponent<IGoldChestManagement>().StoredGoldChanged += ShowCurrentlyStoredGoldAsText;

            CheckInitSpriteArray();
        }
        private void CheckInitSpriteArray()
        {
            if (spriteArray.Length != 5)
            {
                Debug.LogError("Not enough sprites was set as GoldChest SpriteArray parameter. It should be 5 different chest state sprite.");
            }
        }

        /// <summary>
        /// Logic to calculate the correct chest sprite by fullness percent
        /// </summary>
        private void CalculateFullnessLevelToSprites(float fullnessPercent)
        {
            if (fullnessPercent == 0)
            {
                ChangeSprite(0);
            }
            else if (0 > fullnessPercent && fullnessPercent <= 0.33)
            {
                ChangeSprite(1);
            }
            else if (0.33 < fullnessPercent && fullnessPercent <= 0.66)
            {
                ChangeSprite(2);
            }
            else if (0.66 < fullnessPercent && fullnessPercent < 1)
            {
                ChangeSprite(3);
            }
            else if (fullnessPercent == 1)
            {
                ChangeSprite(4);
            }
        }

        private void ChangeSprite(int index)
        {
            spriteRenderer.sprite = spriteArray[index];
        }

        private void ShowCurrentlyStoredGoldAsText(int goldCount)
        {
            chestText.text = goldCount.ToString();
        }

    }
}
