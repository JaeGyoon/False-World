using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using FalseWorld;

public class Bootstrapper : MonoBehaviour
{
    public static AddressableLoader addressableLoader {  get; private set; }

    private async void Start()
    {
        // 어드레서블 로더 객체 생성
        addressableLoader = new AddressableLoader();

        await InitializeManagers();

        await SceneLoaderManager.Instance.LoadScene(SceneName.Lobby);


    }

    private async Task InitializeManagers()
    {
        Debug.Log("Bootstrap Start!");

        await SaveManager.Instance.Initialize();

        //await Addressables.InitializeAsync().Task;

        Debug.Log("Addressable Assets Init");

        await GameManager.Instance.Initialize();

        Debug.Log("Bootstrap End");
    }
}
