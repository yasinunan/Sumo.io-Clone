using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Sumo.IO
{
    public class PlayerController : MonoBehaviour
    {
        Rigidbody rb;
        Coroutine Bouncer;
        Tween myTween;
        [SerializeField] Joystick joystick;   // Joystick scripti
        [SerializeField] GameObject target;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotateSpeed = 500f;
        private float vertical;
        private float horizontal;

        private int intLayerEnemy, intLayerTarget, intLayerCollectible, intLayerPlayer, intLayerDeadZone;
        [SerializeField] private int score;

        private const string strEnemy = "Enemy";
        private const string strTarget = "Target";
        private const string strCollectible = "Collectible";
        private const string strPlayer = "Player";
        private const string strDeadZone = "DeadZone";

        private bool isBouncing = false;
        [SerializeField] private bool isGrounded = false;

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

            intLayerEnemy = LayerMask.NameToLayer(strEnemy);
            intLayerTarget = LayerMask.NameToLayer(strTarget);
            intLayerCollectible = LayerMask.NameToLayer(strCollectible);
            intLayerPlayer = LayerMask.NameToLayer(strPlayer);
            intLayerDeadZone = LayerMask.NameToLayer(strDeadZone);
        }

        //===============================================================================================

        void Update()
        {
            if (!LevelManager.Instance.isGameStarted || LevelManager.Instance.isGameFinished)
            {
                return;
            }

            if (!isBouncing)
            {
                Move();
            }

        }

        //===============================================================================================

        private void FixedUpdate()
        {
            GroundCheck();
        }

        //===============================================================================================

        public void Move()
        {
            vertical = joystick.Vertical;
            horizontal = joystick.Horizontal;

            transform.Translate(0, 0, moveSpeed * Time.deltaTime); //  Oyun başladığında sürekli ileri hareket eder


            //  var step = moveSpeed * Time.deltaTime; // calculate distance to move
            // transform.localPosition = Vector3.MoveTowards(transform.position, new Vector3(0f, 0f, transform.position.z + 1f), step);

            rb.velocity = new Vector3(horizontal, 0f, vertical);

            if (rb.velocity != Vector3.zero)
            {
                Rotate(rb.velocity);
            }
        }

        //===============================================================================================

        private void Rotate(Vector3 rbVelocity)
        {
            Quaternion temp = Quaternion.LookRotation(rbVelocity, Vector3.up);
            rb.rotation = Quaternion.RotateTowards(transform.rotation, temp, rotateSpeed * Time.deltaTime);
        }

        //===============================================================================================

        private IEnumerator Bounce(float _duration, float _force)
        {
            isBouncing = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            float duration = _duration;
            float force = _force;



            if (myTween != null)
            {
                myTween.Kill();
            }

            // myTween = transform.DOLocalMove(this.transform.forward * distance * -1f, _duration).SetEase(Ease.Unset);
            // myTween = rb.DOMove(this.transform.forward * force * -1f, _duration).SetEase(Ease.Unset);

            //yield return myTween.WaitForCompletion();

            rb.AddForce(this.transform.forward * force * -1f, ForceMode.Impulse);
            yield return new WaitForSeconds(duration);
            isBouncing = false;
        }

        //===============================================================================================

        private void GroundCheck()
        {
            int layerMask = 1 << 6;

            isGrounded = (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), 2f, layerMask));

        }

        //===============================================================================================

        private void UpdateScale()
        {
            this.transform.DOScale(new Vector3(transform.localScale.x + 0.1f, transform.localScale.y + 0.1f, transform.localScale.z + 0.1f), 0.2f).SetEase(Ease.InSine);
        }

        //===============================================================================================

        private void UpdateScore()
        {
            LevelManager.Instance.datas.UpdateScore();
        }

        //===============================================================================================

        private void OnBumpedOpponent(GameObject opponentGameObject, int opponentScore, Vector3 direction)
        {
            if (ReferenceEquals(this.gameObject, opponentGameObject))
            {

                Debug.Log("PlayerController: Enemy hit my body");

                if (score > opponentScore)
                {
                    if (Bouncer != null)
                    {
                        StopCoroutine(Bouncer);
                    }

                    Bouncer = StartCoroutine(Bounce(0.5f, 15f));
                }
                else
                {
                    if (Bouncer != null)
                    {
                        StopCoroutine(Bouncer);
                    }

                    Bouncer = StartCoroutine(Bounce(0.8f, 25f));
                }


            }
            else if (ReferenceEquals(target.gameObject, opponentGameObject))
            {
                Debug.Log("PlayerController: Enemy hit my target");
                if (Bouncer != null)
                {
                    StopCoroutine(Bouncer);
                }

                Bouncer = StartCoroutine(Bounce(0.8f, 40f));
            }
        }

        //===============================================================================================

        private void OnCollectibleGrabbed(GameObject collectorObject)
        {
            if (ReferenceEquals(this.gameObject, collectorObject))
            {
                UpdateScale();
                UpdateScore();
            }
        }

        //===============================================================================================

        void OnCollisionEnter(Collision other)
        {
            if (!LevelManager.Instance.isGameStarted || LevelManager.Instance.isGameFinished)
            {
                return;
            }

            var layerMask = other.gameObject.layer;
            if (layerMask == intLayerEnemy)
            {
                Debug.Log("PlayerController: enemy collision detected.");
                LevelManager.Instance.BumpedOpponent(other.gameObject, score, transform.forward);

            }
            else if (layerMask == intLayerDeadZone)
            {
                Debug.Log("Player is dead");
                // restart
                LevelManager.Instance.GameFinished();
            }
            /*else if (layerMask == intLayerTarget)
            {
                Debug.Log("PlayerController: target collision detected.");
                LevelManager.Instance.BumpedOpponent(other.gameObject, score);

            }*/
        }

        //===============================================================================================

        void OnTriggerEnter(Collider other)
        {
            if (!LevelManager.Instance.isGameStarted && !isBouncing)
            {
                return;
            }

            var layerMask = other.gameObject.layer;
            /*if (layerMask == intLayerEnemy)
            {
                Debug.Log("PlayerController: enemy collision detected.");
                LevelManager.Instance.BumpedOpponent(other.gameObject, score);

            }
            /*else*/
            if (layerMask == intLayerTarget)
            {
                Debug.Log("PlayerController: target collision detected.");
                LevelManager.Instance.BumpedOpponent(other.gameObject, score, transform.forward);
            }

        }

    }
}
