using UnityEngine;

public class ChapterTabUI : MonoBehaviour
{
    [SerializeField] private JournalUI journalUI;
    [SerializeField] private Chapter chapter;

    public void OnTabClicked()
    {
        journalUI.SelectChapter(chapter);
    }
}
