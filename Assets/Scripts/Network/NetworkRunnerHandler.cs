using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class NetworkRunnerHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkRunner networkRunnerPrefab;

    NetworkRunner networkRunner;
    bool isStarting;
    bool hasStarted;
    bool hasAutoStarted;

    static NetworkRunnerHandler instance;

    SessionLobby sessionLobby = SessionLobby.Shared;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {
        if (instance != this)
        {
            return;
        }

        networkRunner = FindObjectOfType<NetworkRunner>();
        if (networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.name = "NetworkRunner";
        }

        networkRunner.ProvideInput = true;
        networkRunner.AddCallbacks(this);

        EnsureSceneManager(networkRunner);

        try
        {
            await networkRunner.JoinSessionLobby(sessionLobby);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Failed to join lobby. Starting host instead. Error: {ex.Message}");
            await StartHost();
        }
    }

    void EnsureSceneManager(NetworkRunner runner)
    {
        var SceneManager = runner.GetComponent<NetworkSceneManagerDefault>();

        if (SceneManager == null)
        {
            SceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }
    }

    async Task StartHost()
    {
        if (isStarting || hasStarted)
        {
            return;
        }
        isStarting = true;

        await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            Address = NetAddress.Any(),
            SessionName = "Test",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = networkRunner.GetComponent<NetworkSceneManagerDefault>(),
        });

        hasStarted = true;
        isStarting = false;
        Debug.Log("Host started");
    }

    async Task StartClient()
    {
        if (isStarting || hasStarted)
        {
            return;
        }

        isStarting = true;

        await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            Address = NetAddress.Any(),
            Scene = SceneManager.GetActiveScene().buildIndex,
            SessionName = "Test",
            SceneManager = networkRunner.GetComponent<NetworkSceneManagerDefault>(),
        });

        hasStarted = true;
        isStarting = false;
        Debug.Log("Client started");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
         if (hasAutoStarted || hasStarted || isStarting)
        {
            return;
        }

        hasAutoStarted = true;
        var hasSession = sessionList.Any(session => session.Name == "Test");
        if (hasSession)
        {
            _ = StartClient();
        }
        else
        {
            _ = StartHost();
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason reason)
    {
        hasStarted = false;
        isStarting = false;
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
}

