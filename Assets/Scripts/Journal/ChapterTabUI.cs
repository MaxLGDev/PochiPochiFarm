using UnityEngine;

public class ChapterTabUI : MonoBehaviour
{
    // --- References ---
    [SerializeField] private JournalManager journalManager;
    [SerializeField] private JournalUI journalUI;

    // --- Settings ---
    [SerializeField] private int chapterIndex;


    // ==============================
    // Public Methods
    // ==============================

    public void OnTabClicked()
    {
        journalUI.ShowContents();
        journalUI.SelectChapter(
            journalManager.GetChapter(chapterIndex),
            chapterIndex
        );
    }
}