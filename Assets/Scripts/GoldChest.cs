using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ProjectGuardian
{
    public class GoldChest : MonoBehaviour
    {
        public Sprite[] m_SpriteArray;
        Rigidbody2D m_Rigidbody2D;
        SpriteRenderer m_SpriteRenderer;
        public int m_MaxStoredGoldCount;
        public int m_CurrentStoredGoldCount;

        public Text m_ChestText;

        void Awake()
        {
            m_SpriteRenderer = this.GetComponent<SpriteRenderer>();
            m_Rigidbody2D = this.GetComponent<Rigidbody2D>();

            if (m_SpriteArray.Length != 5)
            {
                Debug.LogError("Not enough sprites was set as GoldChest SpriteArray parameter. It should be 5 different chest state sprite.");
            }
            m_ChestText.text = m_CurrentStoredGoldCount.ToString();


        }

        public void Start()
        {
            if (GameMaster.Instance != null) GameMaster.Instance.AddGoldChest(this.gameObject);
            
        }

        void Update()
        {
            // Checking stored gold for correct Sprite and Stashed gold text
            CheckStoredGold();
        }


        private void CheckStoredGold()
        {

            if(m_CurrentStoredGoldCount > m_MaxStoredGoldCount / 4 * 3)
            {
                m_SpriteRenderer.sprite = m_SpriteArray[0];
            }
            else if (m_CurrentStoredGoldCount > m_MaxStoredGoldCount / 4 * 2)
            {
                m_SpriteRenderer.sprite = m_SpriteArray[1];
            }
            else if (m_CurrentStoredGoldCount > m_MaxStoredGoldCount / 4 * 1)
            {
                m_SpriteRenderer.sprite = m_SpriteArray[2];
            }
            else if (m_CurrentStoredGoldCount >= 1)
            {
                m_SpriteRenderer.sprite = m_SpriteArray[3];
            }
            else
            {
                m_SpriteRenderer.sprite = m_SpriteArray[4];
            }

            m_ChestText.text = m_CurrentStoredGoldCount.ToString();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if(m_CurrentStoredGoldCount - 1 >= 0)
                {
                    if (collision.GetComponent<EnemyController>().TakeGoldFromGoldChest(this.gameObject))
                    {
                        m_CurrentStoredGoldCount -= 1;
                        Debug.Log("Enemy taken 1 coin from chest");
                    }
                }
            }
        }

        public void ReturnStolenGold()
        {
            m_CurrentStoredGoldCount += 1;
        }


    }

}
