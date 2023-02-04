
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;
using System.Linq;

namespace Sumo.IO
{
    public class EnemyController : MonoBehaviour
    {

        NavMeshAgent agent;
        Rigidbody rb;
        Coroutine Bouncer;
        Tween myTween;

        [SerializeField] GameObject targetArea;
        [SerializeField] Vector3 targetPosition, targetCollectible, targetEnemy;

        [SerializeField] private bool isBouncing = false;
        private bool isThereOpponentInSight;
        private bool isThereCollectibleInSight;
        private bool isGrounded;

        [SerializeField] private float speed = 3f;
        [SerializeField] private float rotateSpeed = 1000f;


        private int intLayerEnemy, intLayerTarget, intLayerCollectible, intLayerPlayer, intLayerDeadZone;
        [SerializeField] private int score;
        [SerializeField] private int sightRange = 12;

        private const string strEnemy = "Enemy";
        private const string strTarget = "Target";
        private const string strCollectible = "Collectible";
        private const string strPlayer = "Player";
        private const string strDeadZone = "DeadZone";


        //===============================================================================================

        void OnEnable()
        {
            LevelManager.Instance.OnBumpedOpponent += OnBumpedOpponent;
            LevelManager.Instance.OnCollectibleGrabbed += OnCollectibleGrabbed;

        }

        //===============================================================================================

        void OnDisable()
        {
            LevelManager.Instance.OnBumpedOpponent -= OnBumpedOpponent;
            LevelManager.Instance.OnCollectibleGrabbed -= OnCollectibleGrabbed;

        }

        //===============================================================================================

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            agent = GetComponent<NavMeshAgent>();

            intLayerEnemy = LayerMask.NameToLayer(strEnemy);
            intLayerTarget = LayerMask.NameToLayer(strTarget);
            intLayerCollectible = LayerMask.NameToLayer(strCollectible);
            intLayerPlayer = LayerMask.NameToLayer(strPlayer);
            intLayerDeadZone = LayerMask.NameToLayer(strDeadZone);
        }

        //===============================================================================================

        void FixedUpdate()
        {
            if (!LevelManager.Instance.isGameStarted || LevelManager.Instance.isGameFinished)
            {
                return;
            }

            // CheckSightRange();

            GroundCheck();
        }

        //===============================================================================================

        void Update()
        {
            if (!LevelManager.Instance.isGameStarted)
            {
                return;
            }


            if (!isBouncing && isGrounded)
            {
                CalculateTarget();
                var lookPos = targetPosition - gameObject.transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotateSpeed);
                rb.velocity = transform.forward * speed;
            }

        }

        //===============================================================================================


        private void GroundCheck()
        {
            int layerMask = 1 << 6;

            isGrounded = (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), 2f, layerMask));

        }

        //===============================================================================================

        /*private void CheckSightRange()
        {
            RaycastHit hitOpponent, hitCollectible;
            isThereOpponentInSight = Physics.SphereCast(this.transform.position, radius, transform.forward, out hitOpponent, 1f, intLayerTarget, QueryTriggerInteraction.UseGlobal);
            isThereCollectibleInSight = Physics.SphereCast(this.transform.position, radius, transform.forward, out hitCollectible, 1f, intLayerCollectible, QueryTriggerInteraction.UseGlobal);
        }*/

        //===============================================================================================

        private void CalculateTarget()
        {


            GameObject[] collectibles = CollectibleSpawner.Instance.spawnedCollectibles.ToArray();
        
            targetCollectible = collectibles.OrderBy(go => (this.transform.position - go.transform.position).sqrMagnitude).First().transform.position;


            //GameObject[] enemies = LevelManager.Instance.datas.Enemies.ToArray();
            GameObject[] enemies = LevelManager.Instance.datas.Enemies.Where(f => f.GetInstanceID() != gameObject.GetInstanceID()).ToArray();

            targetEnemy = enemies.OrderBy(go => (this.transform.position - go.transform.position).sqrMagnitude).First().transform.position;



            float collectibleDistance = Vector3.Distance(transform.position, targetCollectible);
            float enemyDistance = Vector3.Distance(transform.position, targetEnemy);

            if (collectibleDistance <= enemyDistance)
            {

                targetPosition = targetCollectible;
            }
            else
            {
                targetPosition = targetEnemy;
            }
        }

        //===============================================================================================

        private IEnumerator Bounce(float _duration, float _force, Vector3 direction)
        {
            isBouncing = true;
            //agent.isStopped = true;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            float duration = _duration;
            float force = _force;
            if (myTween != null)
            {
                myTween.Kill();
            }

            // myTween = rb.DOMove(direction * force, duration).SetEase(Ease.Unset);
            //yield return myTween.WaitForCompletion();
            rb.AddForce(direction * force, ForceMode.Impulse);


            yield return new WaitForSeconds(duration);
            isBouncing = false;
            //agent.isStopped = false; ;
        }

        //===============================================================================================

        private void UpdateScale()
        {
            this.transform.DOScale(new Vector3(transform.localScale.x + 0.1f, transform.localScale.y + 0.1f, transform.localScale.z + 0.1f), 0.2f).SetEase(Ease.InSine);
        }

        //===============================================================================================

        private void UpdateScore()
        {
            score += 100;
        }

        //===============================================================================================

        private void OnBumpedOpponent(GameObject opponentGameObject, int opponentScore, Vector3 direction)
        {
            if (ReferenceEquals(this.gameObject, opponentGameObject))
            {
                Debug.Log("EnemyController: Someone hit my body");
                if (score > opponentScore)
                {
                    if (Bouncer != null)
                    {
                        StopCoroutine(Bouncer);
                    }

                    Bouncer = StartCoroutine(Bounce(0.5f, 8f, direction));
                }
                else
                {
                    if (Bouncer != null)
                    {
                        StopCoroutine(Bouncer);
                    }

                    Bouncer = StartCoroutine(Bounce(0.5f, 16f, direction));
                }
            }
            else if (ReferenceEquals(targetArea.gameObject, opponentGameObject))
            {
                Debug.Log("EnemyController: Someone hit my target");

                if (Bouncer != null)
                {
                    StopCoroutine(Bouncer);
                }

                Bouncer = StartCoroutine(Bounce(0.5f, 45f, direction));
            }

        }

        //===============================================================================================

        private void OnCollectibleGrabbed(GameObject collectorObject)
        {
            if (ReferenceEquals(this.gameObject, collectorObject))
            {
                UpdateScale();
            }
        }

        //===============================================================================================

        void OnCollisionEnter(Collision collision)
        {

            if (!LevelManager.Instance.isGameStarted && !isBouncing)
            {
                return;
            }

            var layerMask = collision.gameObject.layer;
            if (layerMask == intLayerEnemy || layerMask == intLayerTarget || layerMask == intLayerPlayer)
            {
                Debug.Log("EnemyController: collision detected.");
                LevelManager.Instance.BumpedOpponent(collision.gameObject, score, transform.forward);

            }
            else if (layerMask == intLayerDeadZone)
            {

                LevelManager.Instance.EnemyDefeated(this.gameObject);
                this.gameObject.SetActive(false);
            }
        }
    }
}

