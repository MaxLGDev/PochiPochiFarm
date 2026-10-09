using System;
using UnityEngine;

/// <summary>
/// Base for the small red dots on the HUD buttons.
/// Subclasses decide WHEN to refresh (which events) and WHAT to ask (ShouldShow).
/// Put this script on the button, NOT on the dot itself, because
/// an inactive object never receives OnEnable or Start.
/// </summary>
public abstract class NotificationDot : MonoBehaviour
{
    [SerializeField] private GameObject dot;

    protected abstract bool ShouldShow();

    // Override when the button this dot sits on can be locked.
    protected virtual bool IsUnlocked() => true;

    protected void Start()
    {
        Refresh();
    }

    protected void Refresh()
    {
        bool show = IsUnlocked() && ShouldShow();
        
        // Only touch the GameObject when the state actually changes.
        if (dot.activeSelf != show)
            dot.SetActive(show);
    }
}
