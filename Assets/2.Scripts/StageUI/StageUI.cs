using UnityEngine;
using FalseWorld;
public class StageUI : MonoBehaviour
{
    public async void OnClickReturnToLobby()
    {
        await SceneLoaderManager.Instance.LoadScene(SceneName.Lobby);
    }
}
