// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using UInput = UnityEngine.Input;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Uses the desktop mouse cursor instead of any mouse representation within the scene.
    /// It's movement is bound to screenspace.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/ScreenSpaceMousePointer")]
    public class ScreenSpaceMousePointer : BaseMousePointer
    {
        private Vector2 lastMousePosition;

        /// <inheritdoc />
        protected override string ControllerName => "ScreenSpace Mouse Pointer";

        public override Vector3 Position => transform.position;

        public override void OnPreCurrentPointerTargetChange()
        {
            transform.position = CameraCache.Main.transform.position;
        }

        /// <inheritdoc />
        public override void OnPreSceneQuery()
        {

            if (UInput.mousePosition.x < 0 ||
                UInput.mousePosition.y < 0 ||
                UInput.mousePosition.x > Screen.width ||
                UInput.mousePosition.y > Screen.height)
            {
                return;
            }

            Vector3 currentMousePosition = UInput.mousePosition;

            if ((lastMousePosition - (Vector2)currentMousePosition).magnitude >= MovementThresholdToUnHide)
            {
                SetVisibility(true);
            }

            lastMousePosition = currentMousePosition;

            Camera mainCamera = CameraCache.Main;
            Ray ray = mainCamera.ScreenPointToRay(currentMousePosition);
            Rays[0].CopyRay(ray, float.MaxValue);

            Vector2 wheelDelta = UInput.mouseScrollDelta;

            Quaternion rot = Quaternion.LookRotation(ray.direction);
            Vector3 forwardVector = rot * Vector3.forward;
            float scrollMultiplier = .1f; // hard coded value
            transform.position = transform.position + (forwardVector * wheelDelta.y * scrollMultiplier);
            transform.rotation = rot;
        }

        /// <inheritdoc />
        protected override void SetVisibility(bool visible)
        {
            base.SetVisibility(visible);
            Cursor.visible = visible;
        }
    }
}