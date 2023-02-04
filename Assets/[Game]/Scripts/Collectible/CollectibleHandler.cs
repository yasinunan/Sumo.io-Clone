using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Sumo.IO
{
    public class CollectibleHandler : MonoBehaviour
    {
        BoxCollider _collider;
        [SerializeField] GameObject scoreUpgradeText;

        [SerializeField] Vector3 rot;
        [SerializeField] private float time = 2f;

        private bool isTouchedBefore = false;

        //===============================================================================================

        void Start()
        {
            rot = new Vector3(0, 360, 0);
            _collider = GetComponent<BoxCollider>();
        }

        //===============================================================================================
        void Update()
        {
            if (!LevelManager.Instance.isGameStarted)
            {
                return;

            }
            Rotate();
        }

        //===============================================================================================

        private void Rotate()
        {
            transform.DOLocalRotate(rot, time, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear);
        }

        //===============================================================================================

        private void GetEaten()
        {
            scoreUpgradeText.SetActive(true);
            scoreUpgradeText.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f);
            scoreUpgradeText.transform.DOMoveY((transform.position.y + 5f), 0.5f);


            this.transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.1f).OnComplete(() =>
                          {
                              this.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0.5f).OnComplete(() =>
                              {
                                  CollectibleSpawner.Instance.ReturnToPool(this.gameObject);
                              });

                          });
        }

        //===============================================================================================

        void OnCollisionEnter(Collision other)
        {
            if (!isTouchedBefore)
            {
                _collider.enabled = false;
                isTouchedBefore = true;

                Debug.Log("Collectible touched.");
                CollectibleSpawner.Instance.spawnedCollectibles.Remove(this.gameObject);
                LevelManager.Instance.CollectibleGrabbed(other.gameObject);

                GetEaten();
            }

        }
    }
}


