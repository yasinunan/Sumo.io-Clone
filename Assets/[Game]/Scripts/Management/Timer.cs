using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sumo.IO
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private float gameStartCountDown = 4f;
        private float gamePlayCountDown;

        //===============================================================================================
        void OnEnable()
        {
            LevelManager.Instance.OnGameFinished += OnGameFinished;
        }

        //===============================================================================================

        void OnDisable()
        {
            LevelManager.Instance.OnGameFinished -= OnGameFinished;
        }

        //===============================================================================================


        void Start()
        {
            gamePlayCountDown = LevelManager.Instance.datas.timerCountDown;
            if (!LevelManager.Instance.isGameStarted)
            {
                StartCoroutine(GameStarter(gameStartCountDown));
            }

        }

        //===============================================================================================


        void Update()
        {
            if (!LevelManager.Instance.isGameStarted || LevelManager.Instance.isGameFinished)
            {
                return;
            }

            GameCountDown();

        }

        //===============================================================================================

        private void GameCountDown()
        {
            if (gamePlayCountDown > -0.1f)
            {
                gamePlayCountDown -= Time.deltaTime;
                LevelManager.Instance.UpdateCountDown((int)gamePlayCountDown);
            }
            else
            {

            }

        }

        //===============================================================================================


        private IEnumerator GameStarter(float startDuration)
        {

            gameStartCountDown -= Time.deltaTime;


            yield return new WaitForSeconds(startDuration);

            LevelManager.Instance.StartCountDown((int)gameStartCountDown);
            LevelManager.Instance.isGameStarted = true;
            LevelManager.Instance.GameStarted();


        }

        //===============================================================================================

        private void OnGameFinished()
        {
            gameStartCountDown = 4f;
            gamePlayCountDown = 59f;
            LevelManager.Instance.UpdateCountDown((int)gamePlayCountDown);

        }
    }

}
