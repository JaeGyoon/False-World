using UnityEngine;
using System.Threading.Tasks;
using FalseWorld;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : ManagerBase<SceneLoaderManager> 
{
    private bool isLoading;

    protected override void Awake()
    {
        base.Awake();
    }

    public async Task LoadScene(SceneName scene)
    {
        if ( isLoading)
        {
            return;
        }

        isLoading = true;

        AsyncOperation operration = SceneManager.LoadSceneAsync(scene.ToString());

        while ( !operration.isDone)
        {
            await Task.Yield();
            Debug.Log("씬 이동중...");
        }

        isLoading = false;
        Debug.Log($"씬 이동 완료: {scene.ToString()}");
    }
}
