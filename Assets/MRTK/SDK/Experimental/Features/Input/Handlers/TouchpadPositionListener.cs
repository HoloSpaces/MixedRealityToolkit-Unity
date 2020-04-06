using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.Events;

public class TouchpadPositionListener : MonoBehaviour, IMixedRealityInputHandler<Vector2>
{
    private System.Action<Vector3> changeManipulationOffset;
    private float zAxisOffset = 0f;
    private Vector2 lastInputDifference = Vector2.zero;
    private ManipulationEventData manipulationData;
    private Vector2 originalTouchPosition;

    public TouchpadPositionListener(ManipulationEventData call)
    {
        this.manipulationData = call;
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler<Vector2>>(this);
    }

    private void OnDisable()
    {
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

        Vector2 differenceVector = eventData.InputData - originalTouchPosition;
        zAxisOffset += differenceVector.y - lastInputDifference.y;
        changeManipulationOffset(manipulationData.Pointer.Rotation * Vector3.forward * zAxisOffset);
        lastInputDifference = differenceVector;
    }

    public void OnInputDown(InputEventData<Vector2> eventData)
    {
        originalTouchPosition = eventData.InputData;
    }

    public void OnInputUp(InputEventData<Vector2> eventData)
    {
        zAxisOffset = 0f;
    }
}
