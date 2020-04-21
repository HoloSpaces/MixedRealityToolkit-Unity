using System;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.UI;


namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// singlton class that listnes to the touch events 
    /// Meant to work with single callback listner at a time
    /// TODO: update to static method instead of callback, eg Input.mouseScrolldelta
    /// </summary>
    public class TouchpadPositionListener : IMixedRealityInputHandler, IMixedRealityInputHandler<Vector2>
    {
        private System.Action<float> onScrollCallBack;
        private ManipulationEventData manipulationData;
        private bool isOrigin = true;
        private Vector2 lastTouchPosition;
        private DateTime lastTouchTime;
        private static readonly float offsetThreshold = 0.01f;
        private static TouchpadPositionListener instance;
        public static TouchpadPositionListener Instance
        {
            get
            {
                if (instance == null) instance = new TouchpadPositionListener();
                return instance;
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public TouchpadPositionListener()
        {
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler>(this);
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler<Vector2>>(this);
        }

        /// <summary>
        /// Register scroll back 
        /// </summary>
        public void RegisterScrollCallback(ManipulationEventData maniPulationData, System.Action<float> callback)
        {
            manipulationData = maniPulationData;
            onScrollCallBack = callback; //
        }

        /// <summary>
        /// Unregister callback
        /// </summary>
        public void UnRegisterScrollCallback()
        {
            manipulationData = null;
            onScrollCallBack = null;
        }

        /// <summary>
        /// TODO: OnInputChanged will triggered on any 2xis input movements
        /// need to check against if its a touch source or not
        /// </summary>
        /// <param name="eventData"></param>
        public void OnInputChanged(InputEventData<Vector2> eventData)
        {
#if !UNITY_EDITOR
            if (manipulationData == null || (eventData.SourceId != manipulationData.Pointer.Controller?.InputSource.SourceId))
                return;

            if (isOrigin)
            {
                isOrigin = false;
                lastTouchPosition = eventData.InputData;
                lastTouchTime = eventData.EventTime;
                return;
            }

            float scrollDelta = (lastTouchPosition.y - eventData.InputData.y) * -1.0f;
            if (Mathf.Abs((float)scrollDelta) <= offsetThreshold) return; // if the finger not moved enough then return

            float touchDeltaTime = (float)(eventData.EventTime - lastTouchTime).TotalSeconds;
            float velocity = scrollDelta / touchDeltaTime; // calculating velocity dx/dt
            float finalDelta = velocity;

            lastTouchPosition = eventData.InputData;
            lastTouchTime = eventData.EventTime;
            onScrollCallBack?.Invoke(finalDelta); // Dont forget to multiply with Time.delta and the velocity vector(ScrollSensitivity) at callback side
#else
            onScrollCallBack?.Invoke(UnityEngine.Input.mouseScrollDelta.y);
#endif
        }

        public void OnInputDown(InputEventData eventData)
        {
            if (manipulationData == null) return;
            isOrigin = true;
            lastTouchTime = eventData.EventTime;
        }

        public void OnInputUp(InputEventData eventData)
        {
            if (manipulationData == null) return;
            isOrigin = true;
            lastTouchTime = eventData.EventTime;
        }
    }
}
