using UnityEngine;
using UnityEngine.UI;
using System;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
{
    /// <summary>
    /// Interface for all non native keyboard implementations
    /// </summary>
    public interface INonNativeKeyboard
    {
        bool IsActiveAndEnabled { get;}

        event Action<string> OnTextUpdated;
        event EventHandler OnTextSubmitted;
        event Action OnBackSpace;
        event EventHandler OnClosed;
        event EventHandler OnPrevious;
        event EventHandler OnNext;
        event EventHandler OnPlacement;

        void PresentKeyboard();
        void PresentKeyboard(string startText);
        void PresentKeyboard(NonNativeKeyboard.LayoutType keyboardType); // TODO: global enums are prefered
        void PresentKeyboard(string startText, NonNativeKeyboard.LayoutType keyboardType);
        void RepositionKeyboard(Vector3 kbPos, float verticalOffset = 0.0f);
        void RepositionKeyboard(Transform objectTransform, BoxCollider aCollider = null, float verticalOffset = 0.0f);
        void Close();
        void Clear();

        // TODO: add all generic methods here
    }

}
