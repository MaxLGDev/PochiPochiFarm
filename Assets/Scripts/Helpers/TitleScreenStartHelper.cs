using System.Collections;
using UnityEngine;

public class TitleScreenStartHelper : MonoBehaviour
{
    [SerializeField] private FadeAnim fade;
    [SerializeField] private GameObject blackPanel;
    [SerializeField] private GameObject titlePanel;

    private Coroutine startGameCo;

    private void Start()
    {
        Time.timeScale = 0;
        fade.SetInstant(false);
        blackPanel.SetActive(false);
    }

    public void StartGame()
    {
        if (startGameCo != null)
            return;

        startGameCo = StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine()
    {
        blackPanel.SetActive(true);
        fade.Fade(true);
        yield return new WaitForSecondsRealtime(fade.FadeDuration);

        titlePanel.SetActive(false);
        Time.timeScale = 1;

        fade.Fade(false);
        yield return new WaitForSecondsRealtime(fade.FadeDuration);
        
        blackPanel.SetActive(false);
        startGameCo = null;
    }
}
