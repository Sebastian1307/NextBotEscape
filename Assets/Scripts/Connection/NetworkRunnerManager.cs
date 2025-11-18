using Fusion;
using UnityEngine;

//This script controls the selection of player type on menu (Host or client) uses a index for the game scene because... emmm. 
//is necessary? i guess??? :3
public class NetworkRunnerManager : MonoBehaviour
{
    
    public NetworkRunner runnerPrefab;
    public int gameSceneIndex = 1;

    public async void StartHost()
    {
        var runner = Instantiate(runnerPrefab);
        DontDestroyOnLoad(runner);

        runner.ProvideInput = true;

        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host,
            Scene = SceneRef.FromIndex(gameSceneIndex),
            SessionName = "Room",
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
        });
    }

    public async void StartClient()
    {
        var runner = Instantiate(runnerPrefab);
        DontDestroyOnLoad(runner);

        runner.ProvideInput = true;

        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Client,
            Scene = SceneRef.FromIndex(gameSceneIndex),
            SessionName = "Room",
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>()
        });
    }
}
