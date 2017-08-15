using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using HutongGames.PlayMaker;

namespace VRTKPM
{
    public class VRTKPM_IntaractableObject : VRTK_InteractableObject
    {
        public GameObject debugObj;

        private PlayMakerFSM[] FSMs;

        protected void Start()
        {
            // 初期時にobjにあるFSMを取得しておく  
            FSMs = GetComponents<PlayMakerFSM>();

        }

        // イベントを送信する関数  
        void SendEvent(string eventText)
        {
            FSMs = GetComponents<PlayMakerFSM>();
            if ( FSMs != null )
            {
                foreach (PlayMakerFSM fsm in FSMs)
                {
//                    Debug.Log("Send To FSM:" + eventText);
                    fsm.Fsm.Event(eventText);
                }
            }else
            {
//                Debug.LogError("FSMs is NULL!!!");
            }
        }

        void SetFsmBoolValiable(string name, bool val)
        {
            FSMs = GetComponents<PlayMakerFSM>();
            if (FSMs != null)
            {
                foreach (PlayMakerFSM fsm in FSMs)
                {
                    fsm.FsmVariables.GetFsmBool(name).Value = val;
                }
            }
        }

        GameObject preUsingObject = null;
        public override void StartUsing(GameObject currentUsingObject)
        {
            base.StartUsing(currentUsingObject);

            if ( preUsingObject != currentUsingObject)
            {
                var globalVariables = FsmVariables.GlobalVariables;

                preUsingObject = currentUsingObject;

                SendEvent("VRTK_StartUsing");

                Debug.Log("currentUsingObject.name:" + currentUsingObject.name);

                if( currentUsingObject.name == "RightController")
                {
                    SetFsmBoolValiable("UsingByRightController",true);
                    SendEvent("VRTK_StartUsing_R");
                }
                else if (currentUsingObject.name == "LeftController")
                {
                    SetFsmBoolValiable("UsingByLeftController", true);
                    SendEvent("VRTK_StartUsing_L");
                }

                globalVariables.GetFsmGameObject("currentUsingObject").Value = currentUsingObject;

                SetControllerEvents(currentUsingObject);
            }
        }

        public override void StopUsing(GameObject previousUsingObject)
        {
            base.StopUsing(previousUsingObject);
            SendEvent("VRTK_StopUsing");

            var globalVariables = FsmVariables.GlobalVariables;

            if (previousUsingObject.name == "RightController")
            {
                SetFsmBoolValiable("UsingByRightController", false);
                SendEvent("VRTK_StopUsing_R");
            }
            else if (previousUsingObject.name == "LeftController")
            {
                SetFsmBoolValiable("UsingByLeftController", false);
                SendEvent("VRTK_StopUsing_L");
            }

            globalVariables.GetFsmGameObject("currentUsingObject").Value = null;

            RemoveControllerEvents(previousUsingObject);

            preUsingObject = null;
        }

        private bool nearHead = false;
        protected override void Update()
        {
            base.Update();

            float distance = Vector3.Distance(Camera.main.transform.position, gameObject.transform.position);

            if(distance<1.0f)
            {
                if (!nearHead)
                {
                    nearHead = true;
                    SendEvent("VRTK_StartApproach");
                }
            }
            else
            {
                if (nearHead)
                {
                    nearHead = false;
                    SendEvent("VRTK_StopApproach");
                }
            }
        }

        public override void OnInteractableObjectTouched(InteractableObjectEventArgs e)
        {
            base.OnInteractableObjectTouched(e);
            SendEvent("VRTK_OnInteractableObjectTouched");
        }

        public override void OnInteractableObjectUntouched(InteractableObjectEventArgs e)
        {
            base.OnInteractableObjectUntouched(e);
            SendEvent("VRTK_OnInteractableObjectUntouched");
        }


        public override void OnInteractableObjectGrabbed(InteractableObjectEventArgs e)
        {
            base.OnInteractableObjectGrabbed(e);
            SendEvent("VRTK_OnInteractableObjectGrabbed");
        }

        public override void OnInteractableObjectUngrabbed(InteractableObjectEventArgs e)
        {
            base.OnInteractableObjectUngrabbed(e);
            SendEvent("VRTK_OnInteractableObjectUngrabbed");
        }

        public override void OnInteractableObjectUsed(InteractableObjectEventArgs e)
        {
            base.OnInteractableObjectUsed(e);
//            SendEvent("VRTK_OnInteractableObjectUsed");
        }

        public override void OnInteractableObjectUnused(InteractableObjectEventArgs e)
        {
            base.OnInteractableObjectUnused(e);
//            SendEvent("VRTK_OnInteractableObjectUnused");
        }

        /// <summary>
        /// The StartTouching method is called automatically when the object is touched initially. It is also a override method to allow for overriding in inherited classes.
        /// </summary>
        /// <param name="currentTouchingObject">The game object that is currently touching this object.</param>
        public override void StartTouching(GameObject currentTouchingObject)
        {
            base.StartTouching(currentTouchingObject);
            SendEvent("VRTK_StartTouching");
            // 変数のSet  
            var globalVariables = FsmVariables.GlobalVariables;
            globalVariables.GetFsmGameObject("currentTouchingObject").Value = currentTouchingObject;
        }

        /// <summary>
        /// The StopTouching method is called automatically when the object has stopped being touched. It is also a override method to allow for overriding in inherited classes.
        /// </summary>
        /// <param name="previousTouchingObject">The game object that was previously touching this object.</param>
        public override void StopTouching(GameObject previousTouchingObject)
        {
            base.StopTouching(previousTouchingObject);
            SendEvent("VRTK_StopTouching");
            var globalVariables = FsmVariables.GlobalVariables;
            globalVariables.GetFsmGameObject("currentTouchingObject").Value = null;
        }

        /// <summary>
        /// The Grabbed method is called automatically when the object is grabbed initially. It is also a override method to allow for overriding in inherited classes.
        /// </summary>
        /// <param name="currentGrabbingObject">The game object that is currently grabbing this object.</param>
        public override void Grabbed(GameObject currentGrabbingObject)
        {
            base.Grabbed(currentGrabbingObject);
            SendEvent("VRTK_Grabbed");
            
            var globalVariables = FsmVariables.GlobalVariables;
            globalVariables.GetFsmGameObject("currentGrabbingObject").Value = currentGrabbingObject;
        }

        /// <summary>
        /// The Ungrabbed method is called automatically when the object has stopped being grabbed. It is also a override method to allow for overriding in inherited classes.
        /// </summary>
        /// <param name="previousGrabbingObject">The game object that was previously grabbing this object.</param>
        public override void Ungrabbed(GameObject previousGrabbingObject)
        {
            base.Ungrabbed(previousGrabbingObject);

            SendEvent("VRTK_Ungrabbed");
            var globalVariables = FsmVariables.GlobalVariables;
            globalVariables.GetFsmGameObject("currentGrabbingObject").Value = null;
        }


        /// <summary>
        /// The ToggleHighlight method is used to turn on or off the colour highlight of the object.
        /// </summary>
        /// <param name="toggle">The state to determine whether to activate or deactivate the highlight. `true` will enable the highlight and `false` will remove the highlight.</param>
        public override void ToggleHighlight(bool toggle)
        {
            base.ToggleHighlight(toggle);
            SendEvent("VRTK_ToggleHighlight");
        }

        /// <summary>
        /// The ResetHighlighter method is used to reset the currently attached highlighter.
        /// </summary>
        public override void ResetHighlighter()
        {
            base.ResetHighlighter();
            SendEvent("VRTK_ResetHighlighter");
        }
        

        private void SetControllerEvents(GameObject currentUsingObject)
        {
            VRTK_ControllerEvents controllerEvents = currentUsingObject.GetComponent<VRTK_ControllerEvents>();

            controllerEvents.TriggerPressed += new ControllerInteractionEventHandler(TriggerPressed);
            controllerEvents.TriggerReleased += new ControllerInteractionEventHandler(TriggerReleased);
            controllerEvents.TriggerTouchStart += new ControllerInteractionEventHandler(TriggerTouchStart);
            controllerEvents.TriggerTouchEnd += new ControllerInteractionEventHandler(TriggerTouchEnd);
            controllerEvents.TriggerHairlineStart += new ControllerInteractionEventHandler(TriggerHairlineStart);
            controllerEvents.TriggerHairlineEnd += new ControllerInteractionEventHandler(TriggerHairlineEnd);
            controllerEvents.TriggerClicked += new ControllerInteractionEventHandler(TriggerClicked);
            controllerEvents.TriggerUnclicked += new ControllerInteractionEventHandler(TriggerUnclicked);
            controllerEvents.TriggerAxisChanged += new ControllerInteractionEventHandler(TriggerAxisChanged);
            controllerEvents.GripPressed += new ControllerInteractionEventHandler(GripPressed);
            controllerEvents.GripReleased += new ControllerInteractionEventHandler(GripReleased);
            controllerEvents.GripTouchStart += new ControllerInteractionEventHandler(GripTouchStart);
            controllerEvents.GripTouchEnd += new ControllerInteractionEventHandler(GripTouchEnd);
            controllerEvents.GripHairlineStart += new ControllerInteractionEventHandler(GripHairlineStart);
            controllerEvents.GripHairlineEnd += new ControllerInteractionEventHandler(GripHairlineEnd);
            controllerEvents.GripClicked += new ControllerInteractionEventHandler(GripClicked);
            controllerEvents.GripUnclicked += new ControllerInteractionEventHandler(GripUnclicked);
            controllerEvents.GripAxisChanged += new ControllerInteractionEventHandler(GripAxisChanged);
            controllerEvents.TouchpadPressed += new ControllerInteractionEventHandler(TouchpadPressed);
            controllerEvents.TouchpadReleased += new ControllerInteractionEventHandler(TouchpadReleased);
            controllerEvents.TouchpadTouchStart += new ControllerInteractionEventHandler(TouchpadTouchStart);
            controllerEvents.TouchpadTouchEnd += new ControllerInteractionEventHandler(TouchpadTouchEnd);
            controllerEvents.TouchpadAxisChanged += new ControllerInteractionEventHandler(TouchpadAxisChanged);
            controllerEvents.ButtonOneTouchStart += new ControllerInteractionEventHandler(ButtonOneTouchStart);
            controllerEvents.ButtonOneTouchEnd += new ControllerInteractionEventHandler(ButtonOneTouchEnd);
            controllerEvents.ButtonOnePressed += new ControllerInteractionEventHandler(ButtonOnePressed);
            controllerEvents.ButtonOneReleased += new ControllerInteractionEventHandler(ButtonOneReleased);
            controllerEvents.ButtonTwoTouchStart += new ControllerInteractionEventHandler(ButtonTwoTouchStart);
            controllerEvents.ButtonTwoTouchEnd += new ControllerInteractionEventHandler(ButtonTwoTouchEnd);
            controllerEvents.ButtonTwoPressed += new ControllerInteractionEventHandler(ButtonTwoPressed);
            controllerEvents.ButtonTwoReleased += new ControllerInteractionEventHandler(ButtonTwoReleased);
            controllerEvents.AliasPointerOn += new ControllerInteractionEventHandler(AliasPointerOn);
            controllerEvents.AliasPointerOff += new ControllerInteractionEventHandler(AliasPointerOff);
            controllerEvents.AliasPointerSet += new ControllerInteractionEventHandler(AliasPointerSet);
            controllerEvents.AliasGrabOn += new ControllerInteractionEventHandler(AliasGrabOn);
            controllerEvents.AliasGrabOff += new ControllerInteractionEventHandler(AliasGrabOff);
            controllerEvents.AliasUseOn += new ControllerInteractionEventHandler(AliasUseOn);
            controllerEvents.AliasUseOff += new ControllerInteractionEventHandler(AliasUseOff);
            controllerEvents.AliasMenuOn += new ControllerInteractionEventHandler(AliasMenuOn);
            controllerEvents.AliasMenuOff += new ControllerInteractionEventHandler(AliasMenuOff);
            controllerEvents.AliasUIClickOn += new ControllerInteractionEventHandler(AliasUIClickOn);
            controllerEvents.AliasUIClickOff += new ControllerInteractionEventHandler(AliasUIClickOff);
            controllerEvents.ControllerEnabled += new ControllerInteractionEventHandler(ControllerEnabled);
            controllerEvents.ControllerDisabled += new ControllerInteractionEventHandler(ControllerDisabled);
            controllerEvents.ControllerIndexChanged += new ControllerInteractionEventHandler(ControllerIndexChanged);
        }


        private void RemoveControllerEvents(GameObject currentUsingObject)
        {
            VRTK_ControllerEvents controllerEvents = currentUsingObject.GetComponent<VRTK_ControllerEvents>();

            controllerEvents.TriggerPressed -= new ControllerInteractionEventHandler(TriggerPressed);
            controllerEvents.TriggerReleased -= new ControllerInteractionEventHandler(TriggerReleased);
            controllerEvents.TriggerTouchStart -= new ControllerInteractionEventHandler(TriggerTouchStart);
            controllerEvents.TriggerTouchEnd -= new ControllerInteractionEventHandler(TriggerTouchEnd);
            controllerEvents.TriggerHairlineStart -= new ControllerInteractionEventHandler(TriggerHairlineStart);
            controllerEvents.TriggerHairlineEnd -= new ControllerInteractionEventHandler(TriggerHairlineEnd);
            controllerEvents.TriggerClicked -= new ControllerInteractionEventHandler(TriggerClicked);
            controllerEvents.TriggerUnclicked -= new ControllerInteractionEventHandler(TriggerUnclicked);
            controllerEvents.TriggerAxisChanged -= new ControllerInteractionEventHandler(TriggerAxisChanged);
            controllerEvents.GripPressed -= new ControllerInteractionEventHandler(GripPressed);
            controllerEvents.GripReleased -= new ControllerInteractionEventHandler(GripReleased);
            controllerEvents.GripTouchStart -= new ControllerInteractionEventHandler(GripTouchStart);
            controllerEvents.GripTouchEnd -= new ControllerInteractionEventHandler(GripTouchEnd);
            controllerEvents.GripHairlineStart -= new ControllerInteractionEventHandler(GripHairlineStart);
            controllerEvents.GripHairlineEnd -= new ControllerInteractionEventHandler(GripHairlineEnd);
            controllerEvents.GripClicked -= new ControllerInteractionEventHandler(GripClicked);
            controllerEvents.GripUnclicked -= new ControllerInteractionEventHandler(GripUnclicked);
            controllerEvents.GripAxisChanged -= new ControllerInteractionEventHandler(GripAxisChanged);
            controllerEvents.TouchpadPressed -= new ControllerInteractionEventHandler(TouchpadPressed);
            controllerEvents.TouchpadReleased -= new ControllerInteractionEventHandler(TouchpadReleased);
            controllerEvents.TouchpadTouchStart -= new ControllerInteractionEventHandler(TouchpadTouchStart);
            controllerEvents.TouchpadTouchEnd -= new ControllerInteractionEventHandler(TouchpadTouchEnd);
            controllerEvents.TouchpadAxisChanged -= new ControllerInteractionEventHandler(TouchpadAxisChanged);
            controllerEvents.ButtonOneTouchStart -= new ControllerInteractionEventHandler(ButtonOneTouchStart);
            controllerEvents.ButtonOneTouchEnd -= new ControllerInteractionEventHandler(ButtonOneTouchEnd);
            controllerEvents.ButtonOnePressed -= new ControllerInteractionEventHandler(ButtonOnePressed);
            controllerEvents.ButtonOneReleased -= new ControllerInteractionEventHandler(ButtonOneReleased);
            controllerEvents.ButtonTwoTouchStart -= new ControllerInteractionEventHandler(ButtonTwoTouchStart);
            controllerEvents.ButtonTwoTouchEnd -= new ControllerInteractionEventHandler(ButtonTwoTouchEnd);
            controllerEvents.ButtonTwoPressed -= new ControllerInteractionEventHandler(ButtonTwoPressed);
            controllerEvents.ButtonTwoReleased -= new ControllerInteractionEventHandler(ButtonTwoReleased);
            controllerEvents.AliasPointerOn -= new ControllerInteractionEventHandler(AliasPointerOn);
            controllerEvents.AliasPointerOff -= new ControllerInteractionEventHandler(AliasPointerOff);
            controllerEvents.AliasPointerSet -= new ControllerInteractionEventHandler(AliasPointerSet);
            controllerEvents.AliasGrabOn -= new ControllerInteractionEventHandler(AliasGrabOn);
            controllerEvents.AliasGrabOff -= new ControllerInteractionEventHandler(AliasGrabOff);
            controllerEvents.AliasUseOn -= new ControllerInteractionEventHandler(AliasUseOn);
            controllerEvents.AliasUseOff -= new ControllerInteractionEventHandler(AliasUseOff);
            controllerEvents.AliasMenuOn -= new ControllerInteractionEventHandler(AliasMenuOn);
            controllerEvents.AliasMenuOff -= new ControllerInteractionEventHandler(AliasMenuOff);
            controllerEvents.AliasUIClickOn -= new ControllerInteractionEventHandler(AliasUIClickOn);
            controllerEvents.AliasUIClickOff -= new ControllerInteractionEventHandler(AliasUIClickOff);
            controllerEvents.ControllerEnabled -= new ControllerInteractionEventHandler(ControllerEnabled);
            controllerEvents.ControllerDisabled -= new ControllerInteractionEventHandler(ControllerDisabled);
            controllerEvents.ControllerIndexChanged -= new ControllerInteractionEventHandler(ControllerIndexChanged);
        }
        private void TriggerPressed(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_TriggerPressed");
        }

        private void TriggerReleased(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_TriggerReleased");
        }

        private void TriggerTouchStart(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_TriggerTouchStart");
        }

        private void TriggerTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_TriggerTouchEnd");
        }

        private void TriggerHairlineStart(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_TriggerHairlineStart");
        }

        private void TriggerHairlineEnd(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_TriggerHairlineEnd");
        }

        private void TriggerClicked(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_TriggerClicked");
        }

        private void TriggerUnclicked(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_TriggerUnclicked");
        }

        private void TriggerAxisChanged(object sender, ControllerInteractionEventArgs e)
        {


            SendEvent("VRTK_TriggerAxisChanged");

        }

        private float CalculateAngle(ControllerInteractionEventArgs e)
        {
            return 360 - e.touchpadAngle;
        }

        private void GripPressed(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_GripPressed");
        }

        private void GripReleased(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_GripReleased");
        }

        private void GripTouchStart(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_GripTouchStart");
        }

        private void GripTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_GripTouchEnd");
        }

        private void GripHairlineStart(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_GripHairlineStart");
        }

        private void GripHairlineEnd(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_GripHairlineEnd");
        }

        private void GripClicked(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_GripClicked");
        }

        private void GripUnclicked(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_GripUnclicked");
        }

        private void GripAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_GripAxisChanged");
        }

        private void TouchpadPressed(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_TouchpadPressed");
        }

        private void TouchpadReleased(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_TouchpadReleased");
        }

        private void TouchpadTouchStart(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_TouchpadTouchStart");
        }

        private void TouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_TouchpadTouchEnd");
        }

        private void TouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            float angle = CalculateAngle(e);
            var globalVariables = FsmVariables.GlobalVariables;
            globalVariables.GetFsmFloat("VRTK_TouchPadAngle").Value = angle;

            SendEvent("VRTK_TouchpadAxisChanged");
        }

        private void ButtonOneTouchStart(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_ButtonOneTouchStart");
        }

        private void ButtonOneTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_ButtonOneTouchEnd");
        }

        private void ButtonOnePressed(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_ButtonOnePressed");
        }

        private void ButtonOneReleased(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_ButtonOneReleased");
        }

        private void ButtonTwoTouchStart(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_ButtonTwoTouchStart");
        }

        private void ButtonTwoTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_ButtonTwoTouchEnd");
        }

        private void ButtonTwoPressed(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_ButtonTwoPressed");
        }

        private void ButtonTwoReleased(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_ButtonTwoReleased");
        }

        private void AliasPointerOn(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_AliasPointerOn");
        }

        private void AliasPointerOff(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_AliasPointerOff");
        }

        private void AliasPointerSet(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_AliasPointerSet");
        }

        private void AliasGrabOn(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_AliasGrabOn");
        }

        private void AliasGrabOff(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_AliasGrabOff");
        }

        private void AliasUseOn(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_AliasUseOn");
        }

        private void AliasUseOff(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_AliasUseOff");
        }

        private void AliasMenuOn(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_AliasMenuOn");
        }

        private void AliasMenuOff(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_AliasMenuOff");
        }

        private void AliasUIClickOn(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_AliasUIClickOn");
        }

        private void AliasUIClickOff(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_AliasUIClickOff");
        }

        private void ControllerEnabled(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_ControllerEnabled");
        }

        private void ControllerDisabled(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_ControllerDisabled");
        }

        private void ControllerIndexChanged(object sender, ControllerInteractionEventArgs e)
        {
            SendEvent("VRTK_ControllerIndexChanged");
        }
    }
}
