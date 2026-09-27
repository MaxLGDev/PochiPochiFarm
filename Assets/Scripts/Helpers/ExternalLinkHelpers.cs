using UnityEngine;

public class ExternalLinkHelpers : MonoBehaviour
{
    [SerializeField] private string githubUrl;
    [SerializeField] private string unityroomUrl;

    public void OpenGithubLink()
    {
        Application.OpenURL(githubUrl);
    }
    
    public void OpenUnityRoomLink()
    {
        Application.OpenURL(unityroomUrl);
    }
}
