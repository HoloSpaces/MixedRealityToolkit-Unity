using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InCircle : MonoBehaviour
{
    [SerializeField] private Transform pointer = default;
    [SerializeField] private float pointerDistance = 1.5f;

    private void OnDrawGizmos()
    {
        Vector3 flattendPointerPosition = Vector3.ProjectOnPlane(pointer.position, Vector3.up);
        Vector3 flattenedPointerForward = Vector3.ProjectOnPlane(pointer.forward, Vector3.up).normalized;

        Vector3 cursorPosition = flattendPointerPosition + flattenedPointerForward * pointerDistance;
        Vector3 pointerToCenter = -flattendPointerPosition;
        float ponterToCenterDistance = pointerToCenter.magnitude;
        float pointerForwardToCenterAngle = Vector3.SignedAngle(pointerToCenter, flattenedPointerForward, Vector3.up);
        float pointerToCenterAngle = Vector3.SignedAngle(pointerToCenter, Vector3.forward, Vector3.up);

        float sin = ponterToCenterDistance * Mathf.Sin(pointerForwardToCenterAngle * Mathf.Deg2Rad);

        float secondAngle = Mathf.Asin(sin) * Mathf.Rad2Deg;
        float distance = Mathf.Sin((secondAngle + pointerForwardToCenterAngle) * Mathf.Deg2Rad) * pointerToCenter.magnitude / sin;

        Handles.ArrowHandleCap(0, pointer.position, Quaternion.LookRotation(pointer.forward, pointer.up), .5f, EventType.Repaint);
        Handles.DrawWireDisc(Vector3.zero, Vector3.up, 1f);
        Handles.DrawLine(flattendPointerPosition, Vector3.zero);
        Handles.DrawLine(flattendPointerPosition, flattendPointerPosition + flattenedPointerForward * distance);
        Handles.DrawLine(Vector3.zero, flattendPointerPosition + flattenedPointerForward * distance);

        Color prevColor = Handles.color;

        // We are in cone range
        if (Mathf.Abs(sin) <= 1f)
        {
            Handles.color = Color.blue;
            Handles.DrawLine(Vector3.zero, Quaternion.Euler(0f, 180f - secondAngle + pointerForwardToCenterAngle - pointerToCenterAngle, 0f) * Vector3.forward);
            Handles.DrawLine(flattendPointerPosition, cursorPosition);
            Handles.DrawSolidDisc(cursorPosition, Vector3.up, .05f);
        }

        Handles.color = Color.green;
        // The pointer is inside the BoundingBox

        // The cursor is inside the BoundingBox
        if (cursorPosition.magnitude < 1f)
        {
            Handles.DrawSolidDisc(cursorPosition.normalized, Vector3.up, .05f);
        }
        else if(flattendPointerPosition.magnitude < 1f)
        {
            Handles.DrawSolidDisc(flattendPointerPosition + flattenedPointerForward * distance, Vector3.up, .05f);
        }

        // Pointer and cursor are outside of the BoundingBox
        else if (Mathf.Abs(sin) <= 1f)
        {
            float angle;

            if (Vector3.Dot(flattenedPointerForward, cursorPosition) <= 0f)
                angle = 180f - secondAngle;
            else
                angle = secondAngle;

            angle += pointerForwardToCenterAngle - pointerToCenterAngle;

            Handles.DrawSolidDisc(Quaternion.Euler(0f, angle, 0f) * Vector3.forward, Vector3.up, .05f);
        }

        // Overextended Rotation
        else
        {
            Handles.DrawSolidDisc(Vector3.zero, Vector3.up, .05f);

            float maxConeAngle = Mathf.Asin(1f / ponterToCenterDistance) * Mathf.Rad2Deg;
            float pointerToConeRatio = (90f - maxConeAngle) / maxConeAngle;

            Handles.DrawLine(Vector3.zero, Quaternion.Euler(0f, 180f - pointerToConeRatio * pointerForwardToCenterAngle - pointerToCenterAngle, 0f) * Vector3.forward);
            Handles.DrawSolidDisc(Quaternion.Euler(0f, 180f - pointerToConeRatio * pointerForwardToCenterAngle - pointerToCenterAngle, 0f) * Vector3.forward, Vector3.up, .05f);
        }

        Handles.color = prevColor;
    }
}
