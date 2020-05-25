using Microsoft.MixedReality.Toolkit.Experimental.UI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HoloNonNativeKeyboard : NonNativeKeyboard
{
    [SerializeField] Button backspaceButton = default;

    public event Action<string> OnCharacterEntered;
    public event Action OnEnter;
    public event Action OnBackspace;
    public event Action OnClear;

    protected override void Start()
    {
        base.Start();
        OnTextSubmitted += (sender, e) => OnEnter?.Invoke();
        OnTextUpdated += TextUpdated;
        StartCoroutine(AttachButtonEvents());
    }

    IEnumerator AttachButtonEvents()
    {
        yield return null;
        backspaceButton.onClick.AddListener(() => OnBackspace?.Invoke());
    }

    string currentKeyboardText;
    private void TextUpdated(string text)
    {
        if (currentKeyboardText == null || currentKeyboardText.Length != text.Length)
        {
            if (text.Length == 0)
                OnClear?.Invoke();
            else
            {
                int changeAmount = currentKeyboardText == null ? text.Length : text.Length - currentKeyboardText.Length;
                if (changeAmount == 1)
                    OnCharacterEntered(text[text.Length - 1].ToString());
            }
        }

        currentKeyboardText = InputField.text;
    }
}
