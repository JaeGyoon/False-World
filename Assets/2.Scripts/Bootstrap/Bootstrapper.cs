using UnityEngine;
using FalseWorld;

public class Bootstrapper : MonoBehaviour
{
    private async void Start()
    {
        GameCompositionRoot root = new GameCompositionRoot();

        await root.InitializeAsync();
    }
}
