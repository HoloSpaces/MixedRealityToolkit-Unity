using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class InputTest : InputSystemGlobalHandlerListener, IMixedRealityInputHandler, IMixedRealityInputHandler<Vector2>
{
    public void OnInputChanged(InputEventData<Vector2> eventData)
    {
        Debug.Log($"{ToString(eventData.MixedRealityInputAction)} , Input: {eventData.InputData}");
    }

    public void OnInputDown(InputEventData eventData)
    {
        Debug.Log(ToString(eventData.MixedRealityInputAction));
    }

    public void OnInputPressed(InputEventData<float> eventData)
    {
        Debug.Log(ToString(eventData.MixedRealityInputAction));
    }

    public void OnInputUp(InputEventData eventData){ }

    public void OnPositionInputChanged(InputEventData<Vector2> eventData) { }

    protected override void RegisterHandlers()
    {
        CoreServices.InputSystem.RegisterHandler<IMixedRealityInputHandler>(this);
        CoreServices.InputSystem.RegisterHandler<IMixedRealityInputHandler<Vector2>>(this);
    }

    protected override void UnregisterHandlers()
    {
        CoreServices.InputSystem.UnregisterHandler<IMixedRealityInputHandler>(this);
        CoreServices.InputSystem.UnregisterHandler<IMixedRealityInputHandler<Vector2>>(this);
    }

    private string ToString(MixedRealityInputAction action)
    {
        return $"Description: {action.Description}, ID: {action.Id}, Constraint: {action.AxisConstraint}";
    }
}
