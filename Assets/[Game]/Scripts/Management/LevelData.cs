using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Sumo.IO
{
    public class LevelData : MonoBehaviour
    {
        public GameObject enemyParent;
        public int score = 0;
        public int remainingTime;
        public int timerCountDown = 60;
        public List<GameObject> Enemies = new List<GameObject>();

        //===============================================================================================

        void OnEnable()
        {
            LevelManager.Instance.OnEnemyDefeated += OnEnemyDefeated;
            LevelManager.Instance.OnGameFinished += OnGameFinished;
        }

        //===============================================================================================

        void OnDisable()
        {
            LevelManager.Instance.OnEnemyDefeated -= OnEnemyDefeated;
            LevelManager.Instance.OnGameFinished -= OnGameFinished;

        }

        //===============================================================================================

        void Start()
        {
            Transform enemyParent = LevelManager.Instance.enemyParent;
            foreach (Transform child in enemyParent)
            {
                Enemies.Add(child.gameObject);
                Debug.Log(child.name);
            }

            LevelManager.Instance.EnemyListUpdated(Enemies.Count);
        }

        //===============================================================================================

        private void OnGameFinished()
        {
            score = 0;
            LevelManager.Instance.ScoreTextChanged();
            Enemies.Clear();
        }

        //===============================================================================================

        public void OnEnemyDefeated(GameObject enemy)
        {
            Enemies.Remove(enemy);
            LevelManager.Instance.EnemyListUpdated(Enemies.Count);
        }

        //===============================================================================================
        public void UpdateScore()
        {
            score += 100;
            LevelManager.Instance.ScoreTextChanged();
        }
    }
}

