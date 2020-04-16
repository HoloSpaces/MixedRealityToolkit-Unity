using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// singlton class that listnes to the touch events 
/// </summary>
public class TouchpadPositionListener : IMixedRealityInputHandler, IMixedRealityInputHandler<Vector2>
{
    private System.Action<float> onScrollCallBack;
    private float zAxisOffset = 0f;
    private float lastInputDifference = 0f;
    private ManipulationEventData manipulationData;
    private Vector2 originalTouchPosition;
    private float velocityFactor = 1.0f;
    private bool isOrigin = true;

    private Vector2 lastTouchPosition;
    private static readonly float offsetThreshold = 0.05f;
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
    public void RegisterScrollCallback(float vlcVector, ManipulationEventData maniPulationData, System.Action<float> callback)
    {
        velocityFactor = vlcVector;
        manipulationData = maniPulationData;
        onScrollCallBack = callback;
    }

    /// <summary>
    /// Register unregister callback
    /// </summary>
    public void UnRegisterScrollCallback()
    {
        manipulationData = null;
        onScrollCallBack = null;
    }

    /// <summary>
    /// TOD: OnInputChanged will triggered on any 2xis input movements
    /// need to check against if its a touch source or not
    /// </summary>
    /// <param name="eventData"></param>
    public void OnInputChanged(InputEventData<Vector2> eventData)
    {
        if (manipulationData == null || (eventData.SourceId != manipulationData.Pointer.Controller?.InputSource.SourceId))
            return;

        if (isOrigin)
        {
            isOrigin = false;
            lastTouchPosition = eventData.InputData;
            return;
        }

        // TODO: applying velocity effect, current delta implementaion can be improved
        float scrollDelta = (lastTouchPosition.y - eventData.InputData.y)*-1.0f;
        float scrollValue = scrollDelta*Time.deltaTime* velocityFactor;

        onScrollCallBack(scrollValue);
        lastTouchPosition = eventData.InputData;

        /* old code from Robin for the velocity, we will reuse in the next update
        Vector2 differenceVector = eventData.InputData - originalTouchPosition;
        float additionalChange = differenceVector.y - lastInputDifference;
        if (Mathf.Abs(additionalChange) <= offsetThreshold) return;
        float calculatedChange = Mathf.Pow(Mathf.Epsilon, Mathf.Abs(additionalChange)) * velocityFactor;
        zAxisOffset += (additionalChange < 0f) ? -calculatedChange : calculatedChange;
        lastInputDifference = differenceVector.y;
        */ 
    }

    public void OnInputDown(InputEventData eventData)
    {
        if (manipulationData == null) return;
        isOrigin = true;
    }

    public void OnInputUp(InputEventData eventData)
    {
        if (manipulationData == null) return;
        isOrigin = true;
    }
}
