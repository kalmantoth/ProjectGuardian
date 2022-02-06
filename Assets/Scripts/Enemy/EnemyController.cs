using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace ProjectGuardian
{

    public enum EnemyType
    {
        SLIME = 0,
        SKELETON_ARCHER = 1
    }

    public class EnemyController : MonoBehaviour , IEntity
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

        public EntityType entityType { get; }
        public string parentTransform { get; }


        //  ai targeting/pathing
        //  basic info management
        //  sprite/animation logic
        //  health management (life/death)  << player too
        //  status management               << player too
        //  damagedealing management        << player too
        //  damagetake management           << player too
        //  goals handle
        //  player detection
        // etc.
        void Awake()
        {
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_AIPath = GetComponent<AIPath>();
            m_AIPath.maxSpeed *= m_MovementSpeed;

            // Ignore collision with another enemy layer GO
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"), true);


            // ha az npc akkor ad neki celpontot es igy celt eledesnel

        }

        void Start()
        {
            InitEnemy();
        }

        private void InitEnemy()
        {
            SetTarget(GameManager.Instance.GetClosestNonEmptyGoldChest(this.transform));
            GameManager.Instance.AddNewNpc(this.transform);
            SetParentTransform();
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

                ReturnStolenGold();
                

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

        public void TakeGoldFromGoldChest(GameObject goldChest)
        {
            if (!m_IsGotCoin)
            {
                m_RobbedGoldChest = goldChest;
                m_IsGotCoin = true;
                SetTarget(Util.FindClosestTransformByComponentType(this.transform, typeof(EscapePortal)));
            }

        }

        void SetTarget(Transform targetTrasform)
        {
            if (targetTrasform.gameObject.layer == LayerMask.NameToLayer("Portal") && targetTrasform.GetComponent<EscapePortal>() != null)
                m_AIPath.endReachedDistance = 0;
            m_Target = targetTrasform;
        }

        public void EnemyEscapedWithGold()
        {
            //GameManager.Instance.GoldStolen(); TODOOOODODODODO
            DestroyItself();
        }

        public void DestroyItself()
        {
            GameManager.Instance.RemoveNpc(this.transform);
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

            if (m_IsGotCoin)
                m_Target = Util.FindClosestTransformByComponentType(this.transform, typeof(EscapePortal));
            else
                m_Target = Util.FindClosestTransformByComponentType(this.transform, typeof(GoldChestManagement));
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("GoldChest"))
            {
                if (collision.GetComponent<IGoldChestManagement>().TakeGold(1))
                {
                    TakeGoldFromGoldChest(collision.gameObject);
                }
                else
                {
                    Debug.Log("Cant take gold, finding new GC with gold");
                    SetTarget(GameManager.Instance.GetClosestNonEmptyGoldChest(this.transform));
                }
                // it should handle if it can take gold or not
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("Portal"))
            {
                if (collision.GetComponent<EscapePortal>() && m_IsGotCoin)
                    EnemyEscapedWithGold();
            }
        }

        private void ReturnStolenGold()
        {
            if (m_RobbedGoldChest != null) m_RobbedGoldChest.GetComponent<IGoldChestManagement>().PutGold(1);
        }

        public void SetParentTransform()
        {
            this.transform.SetParent(GameObject.Find("Enemies").GetComponent<Transform>());
        }

    }
}

