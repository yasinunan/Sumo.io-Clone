using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace Sumo.IO
{

    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI startTimerText;
        [SerializeField] private TextMeshProUGUI enemyCountText;


        //===============================================================================================

        void OnEnable()
        {
            LevelManager.Instance.OnScoreTextChanged += OnScoreTextChanged;
            LevelManager.Instance.OnEnemyListUpdated += OnEnemyListUpdated;
            LevelManager.Instance.OnUpdateCountDown += OnUpdateCountDown;
            LevelManager.Instance.OnStartCountDown += OnStartCountDown;
        }

        //===============================================================================================

        void OnDisable()
        {
            LevelManager.Instance.OnScoreTextChanged -= OnScoreTextChanged;
            LevelManager.Instance.OnEnemyListUpdated -= OnEnemyListUpdated;
            LevelManager.Instance.OnUpdateCountDown -= OnUpdateCountDown;
            LevelManager.Instance.OnStartCountDown -= OnStartCountDown;
        }

        //===============================================================================================

        void Start()
        {

        }

        //===============================================================================================

        private void OnScoreTextChanged()
        {
            int currentScore = LevelManager.Instance.datas.score;

            if (scoreText != null)
            {
                scoreText.text = currentScore.ToString();
            }
            else
            {
                Debug.Log("Score text is not attached");
            }

        }

        //===============================================================================================

        private void OnEnemyListUpdated(int enemyCount)
        {
            if (enemyCountText != null)
            {
                enemyCountText.text = enemyCount.ToString();
            }
            else
            {
                Debug.Log("Enemy count text is not attached");
            }
        }

        //===============================================================================================
        private void OnUpdateCountDown(int time)
        {
            if (time >= 10)
            {
                timerText.text = "00:" + time.ToString();
            }
            else
            {
                timerText.text = "00:0" + time.ToString();
            }
        }

        //===============================================================================================
        private void OnStartCountDown(int startTime)
        {

            startTimerText.text = "";

        }


    }
}
