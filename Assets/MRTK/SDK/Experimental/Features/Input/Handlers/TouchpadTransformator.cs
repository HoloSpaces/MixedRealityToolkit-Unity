using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.Events;

public class TouchpadTransformator : MonoBehaviour, IMixedRealityInputHandler<Vector2>
{
    private System.Action<Vector3> changeManipulationOffset;
    private float zAxisOffset = 0f;
    private Vector2 lastInputChange = Vector2.zero;
    private ManipulationEventData call;

    public TouchpadTransformator(ManipulationEventData call)
    {
        this.call = call;
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
        zAxisOffset += (eventData.InputData.y - lastInputChange.y);
        changeManipulationOffset(call.Pointer.Rotation * Vector3.forward * zAxisOffset);
        lastInputChange = eventData.InputData;
    }

    public void OnInputDown(InputEventData eventData)
    {
    }

    public void OnInputUp(InputEventData eventData)
    {
        zAxisOffset = 0f;
    }
}
