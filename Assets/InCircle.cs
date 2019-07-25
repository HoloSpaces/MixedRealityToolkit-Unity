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
        Vector3 cursorPosition = pointer.position + pointer.forward * pointerDistance;
        Vector3 pointerToCenter = -pointer.position;
        float ponterToCenterDistance = pointerToCenter.magnitude;
        float pointerForwardToCenterAngle = Vector3.SignedAngle(pointerToCenter, pointer.forward, Vector3.up);
        float pointerToCenterAngle = Vector3.SignedAngle(pointerToCenter, Vector3.forward, Vector3.up);

        float sin = ponterToCenterDistance * Mathf.Sin(pointerForwardToCenterAngle * Mathf.Deg2Rad);

        float secondAngle = Mathf.Asin(sin) * Mathf.Rad2Deg;
        float distance = Mathf.Sin((secondAngle + pointerForwardToCenterAngle) * Mathf.Deg2Rad) * pointerToCenter.magnitude / sin;

        Handles.DrawWireDisc(Vector3.zero, Vector3.up, 1f);
        //Handles.DrawLine(pointer.position, Vector3.zero);
        //Handles.DrawLine(pointer.position, pointer.position + pointer.forward * distance);
        //Handles.DrawLine(Vector3.zero, pointer.position + pointer.forward * distance);

        Color prevColor = Handles.color;

        // We are in cone range
        //if (Mathf.Abs(sin) <= 1f)
        //{
        Handles.color = Color.blue;
        //    Handles.DrawLine(Vector3.zero, Quaternion.Euler(0f, 180f - secondAngle + pointerForwardToCenterAngle - pointerToCenterAngle, 0f) * Vector3.forward);
        Handles.DrawLine(pointer.position, cursorPosition);
        Handles.DrawSolidDisc(cursorPosition, Vector3.up, .05f);
        //}

        Handles.color = Color.green;
        // The pointer is inside the BoundingBox
        if (pointer.position.magnitude < 1f)
        {
            Handles.DrawSolidDisc(pointer.position + pointer.forward * distance, Vector3.up, .05f);
        }

        // The cursor is inside the BoundingBox
        else if (cursorPosition.magnitude < 1f)
        {
            Handles.DrawSolidDisc(cursorPosition.normalized, Vector3.up, .05f);
        }

        // Pointer and cursor are outside of the BoundingBox
        else if (Mathf.Abs(sin) <= 1f)
        {
            float angle;

            if (Vector3.Dot(pointer.forward, cursorPosition) <= 0f)
                angle = 180f - secondAngle;
            else
                angle = secondAngle;

            angle += pointerForwardToCenterAngle - pointerToCenterAngle;

            Handles.DrawSolidDisc(Quaternion.Euler(0f, angle, 0f) * Vector3.forward, Vector3.up, .05f);
        }
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
