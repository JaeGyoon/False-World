using UnityEngine;
using FalseWorld;

public class LobbyUI : MonoBehaviour
{
    public async void OnClickGameStart()
    {
        await SceneLoaderManager.Instance.LoadScene(SceneName.Stage);
    }
}
