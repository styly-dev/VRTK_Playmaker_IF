using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using HutongGames.PlayMaker;

namespace VRTKPM
{
    public class SVRTKPM_Util : MonoBehaviour
    {

        public GameObject controllerRightHand;
        public GameObject controllerLeftHand;
        public Transform headsetTransform;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            controllerRightHand = VRTK_DeviceFinder.GetControllerRightHand(true);
            controllerLeftHand = VRTK_DeviceFinder.GetControllerLeftHand(true);
            headsetTransform = VRTK_DeviceFinder.HeadsetTransform();

            // 変数のSet  
            var globalVariables = FsmVariables.GlobalVariables;
            globalVariables.GetFsmGameObject("controllerRightHand").Value = controllerRightHand;
            globalVariables.GetFsmGameObject("controllerLeftHand").Value = controllerLeftHand;
            globalVariables.GetFsmObject("headsetTransform").Value = headsetTransform;

            globalVariables.GetFsmVector3("controllerLeftHandPos").Value = controllerLeftHand.transform.position;
        }
    }
}
