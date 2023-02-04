using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using DG.Tweening;


namespace Sumo.IO
{
    public class CollectibleSpawner : Singleton<CollectibleSpawner>
    {
        public GameObject prefab;
        [SerializeField] Transform trDestructiblesHolder;
        [SerializeField] private int totalSpawnAmount = 30;
        [SerializeField] private float spawnTime = 1f;
        [SerializeField] private Stack<GameObject> objectPool = new Stack<GameObject>();
        public List<GameObject> spawnedCollectibles;

        //===============================================================================================

        void OnEnable()
        {
            LevelManager.Instance.OnGameStarted += OnGameStarted;
        }

        //===============================================================================================
        void OnDisable()
        {
            LevelManager.Instance.OnGameStarted -= OnGameStarted;
        }

        //===============================================================================================

        void Start()
        {
            for (int i = 0; i < totalSpawnAmount; i++)
            {
                GameObject collectible = Instantiate(prefab);
                collectible.transform.parent = trDestructiblesHolder;
                ReturnToPool(collectible);
            }


        }

        //===============================================================================================

        private void OnGameStarted()
        {
            StartCoroutine(SpawnCollectible());
        }


        //===============================================================================================

        IEnumerator SpawnCollectible()
        {
            while (true)
            {
                Vector2 v2Pos = Random.insideUnitCircle * 14f;
                Vector3 v3Pos = new Vector3(v2Pos.x, 15f, v2Pos.y);

                GameObject collectible = GetObjectFromPool();
                spawnedCollectibles.Add(collectible);
                collectible.transform.position = v3Pos;

                collectible.transform.DOMoveY(0f, 0.8f).SetEase(Ease.InSine);

                yield return new WaitForSeconds(spawnTime);


            }
        }

        //===============================================================================================

        GameObject GetObjectFromPool()
        {

            if (objectPool.Count > 0)
            {

                GameObject collectible = objectPool.Pop();


                collectible.gameObject.SetActive(true);


                return collectible;
            }

            return Instantiate(prefab);
        }

        //===============================================================================================

        public void ReturnToPool(GameObject collectible)
        {
            // Objeyi inaktif hale getir (böylece obje artık ekrana çizilmeyecek ve objede
            // Update vs. fonksiyonlar varsa, bu fonksiyonlar obje havuzdayken çalıştırılmayacak)
            collectible.gameObject.SetActive(false);

            // Objeyi havuza ekle
            objectPool.Push(collectible);
        }
    }
}

