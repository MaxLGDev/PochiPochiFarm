using UnityEngine;

public class ChapterTabUI : MonoBehaviour
{
    [SerializeField] private JournalManager journalManager;
    [SerializeField] private JournalUI journalUI;
    [SerializeField] private int chapterIndex;

    public void OnTabClicked()
    {
        journalUI.ShowContents();
        journalUI.SelectChapter(journalManager.GetChapter(chapterIndex), chapterIndex);
    }
}
