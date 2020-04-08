using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.Events;

public class TouchpadPositionListener : MonoBehaviour, IMixedRealityInputHandler, IMixedRealityInputHandler<Vector2>
{
    private System.Action<Vector3> changeManipulationOffset;
    private float zAxisOffset = 0f;
    private float lastInputDifference = 0f;
    private ManipulationEventData manipulationData;
    private Vector2 originalTouchPosition;
    private float velocityFactor;
    private bool isOrigin = true;

    private static readonly float offsetThreshold = 0.05f;

    public TouchpadPositionListener(ManipulationEventData call, float velocityFactor)
    {
        this.manipulationData = call;
        this.velocityFactor = velocityFactor;
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler>(this);
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler<Vector2>>(this);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler>(this);
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler<Vector2>>(this);
    }

    public void Init(System.Action<Vector3> OnChangeManipulationOffset)
    {
        changeManipulationOffset = OnChangeManipulationOffset;
    }

    public void OnInputChanged(InputEventData<Vector2> eventData)
    {
        if (eventData.SourceId != manipulationData.Pointer.Controller?.InputSource.SourceId)
            return;

        if (isOrigin)
        {
            isOrigin = false;
            originalTouchPosition = eventData.InputData;
            return;
        }

        Vector2 differenceVector = eventData.InputData - originalTouchPosition;
        float additionalChange = differenceVector.y - lastInputDifference;

        if (Mathf.Abs(additionalChange) <= offsetThreshold) return;

        float calculatedChange = Mathf.Pow(Mathf.Epsilon, Mathf.Abs(additionalChange)) * velocityFactor;
        zAxisOffset += (additionalChange < 0f) ? -calculatedChange : calculatedChange;
        lastInputDifference = differenceVector.y;
        changeManipulationOffset(manipulationData.Pointer.Rotation * Vector3.forward * zAxisOffset);
    }

    public void OnInputDown(InputEventData eventData)
    {
        isOrigin = true;
        lastInputDifference = 0f;
        originalTouchPosition = Vector2.zero;
    }

    public void OnInputUp(InputEventData eventData)
    {
        isOrigin = true;
        lastInputDifference = 0f;
        originalTouchPosition = Vector2.zero;
    }
}
