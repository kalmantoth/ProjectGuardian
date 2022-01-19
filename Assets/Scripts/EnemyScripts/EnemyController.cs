using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace ProjectGuardian
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] public Animator m_Animator;
        [SerializeField] public Collider2D m_PlayerCheckBox;

        Rigidbody2D m_Rigidbody2D;
        AIPath m_AIPath;

        public Transform m_Target;
        public GameObject m_CoinPlaceholder;

        GameObject m_RobbedGoldChest;

        public string m_EnemyName;
        public int m_AttackDamage;
        public float m_MovementSpeed;
        public int m_ActualHealthPoint;
        public int m_MaxHealthPoint;
        public bool m_IsInvulnerable;
        public bool m_IsMovingEnabled = true;
        float m_AttackSpeedModifier;
        float m_AttackSpeed;
        bool m_IsHurtStunned;
        bool m_IsAttacking;
        bool m_IsGotCoin;
        float m_StunDuration;
        float m_AttackDuration;

        public bool m_FacingRight = true;

        GameObject m_DetectedPlayer;

        void Awake()
        {
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_AIPath = GetComponent<AIPath>();
            m_AIPath.maxSpeed *= m_MovementSpeed;
            m_DetectedPlayer = null;

            // Ignore collision with another enemy layer GO
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"), true);

        }

        void Update()
        {
            m_AIPath.canMove = m_IsMovingEnabled;


            m_CoinPlaceholder.transform.gameObject.SetActive(m_IsGotCoin);

            if (m_Target != null && m_IsMovingEnabled)
            {
                SetMovePosition(m_Target.position);
            }

            if (m_Target == null) m_FacingRight = true;

            if (m_IsAttacking)
            {
                float AttackAnimationLength = Util.AnimationLength("Slime-Attack", m_Animator);
                m_IsMovingEnabled = false;


                if (m_AttackDuration < AttackAnimationLength)
                {
                    m_AttackDuration += Time.deltaTime;
                }
                else if (m_AttackDuration > AttackAnimationLength)
                {
                    m_IsAttacking = false;
                    m_IsMovingEnabled = true;
                    m_AttackDuration = 0;
                }
            }

            if (m_IsMovingEnabled)
            {
                float move = m_AIPath.velocity.x;

                if (move > 0 && !m_FacingRight)
                {
                    Flip();
                }
                else if (move < 0 && m_FacingRight)
                {
                    Flip();
                }
            }

            CheckHealth();

            m_Animator.SetFloat("Speed", Mathf.Abs(m_AIPath.velocity.x));
        }


        void CheckHealth()
        {
            if (m_ActualHealthPoint <= 0)
            {
                // NEED TO IMPLEMENT - play dying animation
                if(m_RobbedGoldChest != null) m_RobbedGoldChest.GetComponent<GoldChest>().ReturnStolenGold();

                DestroyItself();
            }
        }

        public void OnPlayerHit(PlayerController playerController)
        {
            Debug.Log("Player suffered " + m_AttackDamage + " damage.");
            playerController.TakeDamage(m_AttackDamage);
        }


        public void TakeDamage(int damage)
        {
            if (!m_IsInvulnerable)
            {
                m_Animator.SetTrigger("HurtTrigger");
                m_ActualHealthPoint -= damage;
            }
            
        }

        public void Attack()
        {
            if (!m_IsAttacking)
            {
                m_Animator.SetTrigger("AttackTrigger");
                m_IsAttacking = true;
                m_IsMovingEnabled = false;
            }
        }

        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        void SetMovePosition(Vector3 movePosition)
        {
            m_AIPath.destination = movePosition;
        }

        public bool TakeGoldFromGoldChest(GameObject goldChest)
        {
            if (!m_IsGotCoin)
            {
                m_RobbedGoldChest = goldChest;
                m_IsGotCoin = true;
                SetTarget(GameMaster.Instance.FindClosestEscapePortal(this.gameObject));
                return true;
            }

            return false;
        }

        void SetTarget(GameObject targetGO)
        {
            if (targetGO.layer == LayerMask.NameToLayer("Portal") && targetGO.GetComponent<Portal>().m_PortalType == PortalType.ESCAPE_PORTAL)
                m_AIPath.endReachedDistance = 0;
            m_Target = targetGO.transform;
        }

        public void EnemyEscapedWithGold()
        {
            GameMaster.Instance.GoldStolen();
            DestroyItself();
        }

        public void DestroyItself()
        {
            GameMaster.Instance.RemoveEnemy(this.gameObject);
            Destroy(this.gameObject);
        }

        public void OnPlayerDetectInAttackRange(ORIENTATION playerOrietation)
        {
            if ((playerOrietation == ORIENTATION.RIGHT && !m_FacingRight) || playerOrietation == ORIENTATION.LEFT && m_FacingRight)
                Flip();
            Attack();
        }

        public void OnPlayerDetect(GameObject detectedPlayer)
        {
            m_Target = detectedPlayer.transform;
        }

        public void OnPlayerOutOfDetection()
        {
            m_DetectedPlayer = null;

            if (m_IsGotCoin)
                m_Target = GameMaster.Instance.FindClosestEscapePortal(this.gameObject).transform;
            else
                m_Target = GameMaster.Instance.FindClosestGoldChest(this.gameObject).transform;
        }




    }
}

