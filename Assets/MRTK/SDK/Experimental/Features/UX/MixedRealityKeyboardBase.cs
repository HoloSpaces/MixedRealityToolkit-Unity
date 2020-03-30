// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if WINDOWS_UWP
using Windows.UI.ViewManagement;
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
#endif

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// Class that can launch and hide a system keyboard specifically for Windows Mixed Reality
    /// devices (HoloLens 2, Windows Mixed Reality).
    /// 
    /// Implements a workaround for UWP TouchScreenKeyboard bug which prevents
    /// UWP keyboard from showing up again after it is closed.
    /// Unity bug tracking the issue https://fogbugz.unity3d.com/default.asp?1137074_rttdnt8t1lccmtd3
    /// </summary>
    public abstract class MixedRealityKeyboardBase : MonoBehaviour
    {
        [Experimental]
#pragma warning disable 414
        private object exp = null;
#pragma warning restore 414

        #region Properties

        public bool IsVisible => State == KeyboardState.Shown;

#if WINDOWS_UWP
        protected virtual string KeyboardText
        {
            get => keyboard?.text;
            set
            {
                if (keyboard != null)
                {
                    keyboard.text = value;
                }
            }
        }
#else
        protected abstract string KeyboardText { get; set; }
#endif

        #endregion properties

        #region Private fields

#if WINDOWS_UWP
        private InputPane inputPane = null;
        private TouchScreenKeyboard keyboard = null;
        private Coroutine stateUpdate;
#endif

        // Fields that are only used on non WINDOWS_UWP
#pragma warning disable 414
        [Header("NonNativeKeyboard")]
        [SerializeField] private NonNativeKeyboard nonNativeKeyboard = null;
        [SerializeField] private Transform spawnTransform = null;
        [SerializeField] private NonNativeKeyboard.LayoutType keyboardLayout = NonNativeKeyboard.LayoutType.Alpha;
#pragma warning restore 414

        private KeyboardState State = KeyboardState.Hidden;


#endregion private fields

#region Private enums

        private enum KeyboardState
        {
            Hiding,
            Hidden,
            Shown,
        }

#endregion Private enums

#region Unity functions

#if WINDOWS_UWP
        private void Start()
        {
            UnityEngine.WSA.Application.InvokeOnUIThread(() =>
            {
                inputPane = InputPane.GetForCurrentView();
                inputPane.Hiding += (inputPane, args) => OnKeyboardHiding();
                inputPane.Showing += (inputPane, args) => OnKeyboardShowing();
            }, false);
        }

        private IEnumerator UpdateState()
        {
            switch (State)
            {
                case KeyboardState.Shown:
                    UpdateText(keyboard?.text);
                    break;

                case KeyboardState.Hiding:
                    ClearText();
                    break;
            }

            yield return null;
        }
#endif
        private void OnDisable()
        {
            HideKeyboard();
        }

#endregion unity functions

        public virtual void ShowKeyboard()
        {
            // 2019/08/14: We show the keyboard even when the keyboard is already visible because on HoloLens 1
            // and WMR the events OnKeyboardShowing and OnKeyboardHiding do not fire
            //if (state == KeyboardState.Showing)
            //{
            //    Debug.Log($"MixedRealityKeyboard.ShowKeyboard called but keyboard already visible.");
            //    return;
            //}

            State = KeyboardState.Shown;

            ClearText();

#if WINDOWS_UWP
            if (keyboard != null)
            {
                UnityEngine.WSA.Application.InvokeOnUIThread(() => inputPane?.TryShow(), false);
            }
            else
            {
                keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
            }

            if (stateUpdate == null)
            {
                stateUpdate = StartCoroutine(UpdateState());
            }
#else

            if (nonNativeKeyboard.isActiveAndEnabled)
                HideKeyboard();

            if (spawnTransform != null)
            {
                nonNativeKeyboard.RepositionKeyboard(spawnTransform);
            }

            nonNativeKeyboard.OnClosed += DisableKeyboard;
            nonNativeKeyboard.OnTextSubmitted += DisableKeyboard;
            nonNativeKeyboard.OnTextUpdated += UpdateText;

            EventSystem.current.SetSelectedGameObject(null);

            nonNativeKeyboard.PresentKeyboard(KeyboardText, keyboardLayout);
#endif
        }

        public void HideKeyboard()
        {
            State = KeyboardState.Hidden;

#if WINDOWS_UWP
            UnityEngine.WSA.Application.InvokeOnUIThread(() => inputPane?.TryHide(), false);

            if (stateUpdate != null)
            {
                StopCoroutine(stateUpdate);
                stateUpdate = null;
            }
#else
            nonNativeKeyboard.OnTextUpdated -= UpdateText;
            nonNativeKeyboard.OnClosed -= DisableKeyboard;
            nonNativeKeyboard.OnTextSubmitted -= DisableKeyboard;

            nonNativeKeyboard.Close();
#endif
        }

#if !WINDOWS_UWP
        private void DisableKeyboard(object sender, EventArgs e) => HideKeyboard();
#endif

        #region Input pane event handlers

        private void OnKeyboardHiding()
        {
            if (State != KeyboardState.Hidden)
            {
                State = KeyboardState.Hiding;
            }
        }

        private void OnKeyboardShowing() { }

#endregion Input pane event handlers


        private void ClearText()
        {
#if WINDOWS_UWP
            if (keyboard != null)
#else
            if (nonNativeKeyboard != null)
#endif
            {
                KeyboardText = string.Empty;
            }
        }

        protected abstract void UpdateText(string text);
    }
}