using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using UnityEngine.SceneManagement;


namespace Sumo.IO
{
    [DefaultExecutionOrder(-1)]
    public class LevelManager : Singleton<LevelManager>
    {

        public Transform enemyParent;
        public LevelData datas;
        [HideInInspector] public bool isGameStarted;
        [HideInInspector] public bool isGameFinished;

        [SerializeField] private float restartTime = 1f;


        //===============================================================================================

        public delegate void OnGameStartedDelegate();
        public event OnGameStartedDelegate OnGameStarted;
        public delegate void OnGameFinishedDelegate();
        public event OnGameFinishedDelegate OnGameFinished;

        public delegate void OnBumpedOpponentDelegate(GameObject gameObject, int score, Vector3 direction);
        public event OnBumpedOpponentDelegate OnBumpedOpponent;

        public delegate void OnEnemyDefeatedDelegate(GameObject gameObject);
        public event OnEnemyDefeatedDelegate OnEnemyDefeated;

        public delegate void OnEnemyListUpdatedDelegate(int enemyCount);
        public event OnEnemyListUpdatedDelegate OnEnemyListUpdated;

        public delegate void OnCollectibleGrabbedDelegate(GameObject gameObject);
        public event OnCollectibleGrabbedDelegate OnCollectibleGrabbed;

        public delegate void OnScoreTextChangedDelegate();
        public event OnScoreTextChangedDelegate OnScoreTextChanged;

        public delegate void OnUpdateCountDownDelegate(int time);
        public event OnUpdateCountDownDelegate OnUpdateCountDown;
        public delegate void OnStartCountDownDelegate(int time);
        public event OnStartCountDownDelegate OnStartCountDown;

        //===============================================================================================

        void Awake()
        {
            Application.targetFrameRate = 60;
            datas = datas = gameObject.AddComponent<LevelData>();
            isGameStarted = false;
            isGameFinished = false;
        }


        //===============================================================================================
        public IEnumerator RestartGame()
        {
            isGameFinished = true;

            yield return new WaitForSeconds(restartTime);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }


        //===============================================================================================

        ///
        /// Events
        ///

        public void GameStarted()
        {
            OnGameStarted?.Invoke();
        }

        //===============================================================================================

        public void GameFinished()
        {
            
            OnGameFinished?.Invoke();

            StartCoroutine(RestartGame());
        }

        //===============================================================================================

        public void BumpedOpponent(GameObject gameObject, int score, Vector3 direction)
        {
            OnBumpedOpponent?.Invoke(gameObject, score, direction);
        }

        //===============================================================================================

        public void EnemyDefeated(GameObject gameObject)
        {
            OnEnemyDefeated?.Invoke(gameObject);
        }

        //===============================================================================================

        public void EnemyListUpdated(int enemyCount)
        {
            OnEnemyListUpdated?.Invoke(enemyCount);
        }

        //===============================================================================================

        public void CollectibleGrabbed(GameObject collectorObject)
        {
            OnCollectibleGrabbed?.Invoke(collectorObject);
        }

        //===============================================================================================

        public void ScoreTextChanged()
        {
            OnScoreTextChanged?.Invoke();
        }

        //===============================================================================================

        public void UpdateCountDown(int time)
        {
            OnUpdateCountDown?.Invoke(time);
        }

        //===============================================================================================

        public void StartCountDown(int startTime)
        {
            OnStartCountDown?.Invoke(startTime);
        }
    }

}
