// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// A component that can be added to InputField to make them work with Windows Mixed Reality's system keyboard
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class UI_KeyboardInputField : KeyboardInputFieldBase<InputField>
    {
        public override void ShowKeyboard()
        {
            base.ShowKeyboard();
            KeyboardText = inputField.text;
        }

        protected override string KeyboardText { get => inputField.text; set => inputField.text = value; }
        protected override void UpdateText(string text) => inputField.text = text;

        protected override Graphic Text(InputField inputField) => inputField.textComponent;
        protected override Graphic PlaceHolder(InputField inputField) => inputField.placeholder;
    }
}