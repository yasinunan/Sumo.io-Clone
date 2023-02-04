using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Pixelplacement;

namespace Sumo.IO
{
    public class CameraController : Singleton<CameraController>
    {
        //===================================================================================

        public GameObject goPlayer;

        Vector3 v3StartPos;
        Vector3 v3StartEulerAngles;
        Vector3 v3StartOffset;

        [SerializeField] private bool bFollowingPlayer = true;

        [SerializeField] ParticleSystem psConfetti;

        float fSmoothTime = 0.3f;
        Vector3 v3Velocity = Vector3.zero;

        public Camera camera;


        //===================================================================================

        void Awake()
        {
            camera = Camera.main;

            goPlayer = GameObject.Find("Player");

            v3StartPos = transform.position;
            v3StartEulerAngles = transform.localEulerAngles;
            if (goPlayer != null) v3StartOffset = transform.position - goPlayer.transform.position;
        }

        //===================================================================================
      

        void LateUpdate()
        {
            if (goPlayer == null || !IsFollowingPlayer())
            {
                return;
            }

            Vector3 v3TargetPosition = goPlayer.transform.position + v3StartOffset;
            Vector3 v3NewPos = Vector3.SmoothDamp(transform.position, v3TargetPosition, ref v3Velocity, fSmoothTime);
            v3NewPos = new Vector3(v3NewPos.x, v3NewPos.y, v3NewPos.z);
            transform.position = v3NewPos;
        }

        //===================================================================================

        void SetFollowingPlayer(bool bVal = true)
        {
            bFollowingPlayer = bVal;
        }

        //===================================================================================

        bool IsFollowingPlayer()
        {
            return bFollowingPlayer;
        }

        //===================================================================================

        void ResetCameraRotation()
        {
            transform.localEulerAngles = v3StartEulerAngles;
        }

        //===================================================================================

    }
}
