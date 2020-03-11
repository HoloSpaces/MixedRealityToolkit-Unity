// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.Input;
#if WINDOWS_UWP
using UnityEngine.EventSystems;
#endif

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// Base class explicitly launching Windows Mixed Reality's system keyboard for InputField and TMP_InputField
    /// To be attached to the same GameObject with either of the components
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class KeyboardInputFieldBase<T> : MixedRealityKeyboardBase, IMixedRealityPointerHandler
#if WINDOWS_UWP
    , IDeselectHandler
#endif
    where T : Selectable
    {
        [Experimental]
        protected T inputField;

        void OnValidate()
        {
            inputField = GetComponent<T>();

            DisableRaycastTarget(Text(inputField));
            DisableRaycastTarget(PlaceHolder(inputField));
        }

        private void DisableRaycastTarget(Graphic graphic)
        {
            if (graphic != null)
            {
                graphic.raycastTarget = false;
            }
        }

        protected virtual void Awake()
        {
            if ((inputField = GetComponent<T>()) == null)
            {
                Destroy(this);
                Debug.LogWarning($"There is no {typeof(T).ToString()} on GameObject {name}, removing this component");
            }
        }

#if WINDOWS_UWP

        #region IDeselectHandler implementation

        public void OnDeselect(BaseEventData eventData) => HideKeyboard();

        #endregion

#endif

        #region IMixedRealityPointerHandler implementation

        public void OnPointerDragged(MixedRealityPointerEventData eventData) { }
        public void OnPointerUp(MixedRealityPointerEventData eventData) { }
        public void OnPointerClicked(MixedRealityPointerEventData eventData) => ShowKeyboard();
        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
#if !WINDOWS_UWP
            ShowKeyboard();
#endif
        }

        #endregion

        protected abstract Graphic Text(T inputField);
        protected abstract Graphic PlaceHolder(T inputField);
    }
}
