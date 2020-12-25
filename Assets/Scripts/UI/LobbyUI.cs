﻿using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUIParent;
    [SerializeField] private TMP_Text leftTeamPlayersText;
    [SerializeField] private TMP_Text rightTeamPlayersText;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private Button startButton;
    [SerializeField] private MatchTracker matchTracker;
    private DodgeballNetworkManager dodgeballNetworkManager;
    private PlayerConnection playerConnection;

    private void Start()
    {
        dodgeballNetworkManager = (DodgeballNetworkManager)NetworkManager.singleton;
        if (NetworkServer.active)
            startButton.gameObject.SetActive(true);
        MatchTracker.ClientMatchStarted += HandleMatchStarted;
        PlayerConnection.ClientPlayerSpawned += HandlePlayerSpawned;
        PlayerConnection.ClientPlayerInfoUpdated += HandlePlayerInfoUpdated;
        DodgeballNetworkManager.ClientConnected += HandleClientConnected;
        DodgeballNetworkManager.ClientDisconnected += HandleClientDisconnected;
    }

    private void OnDestroy()
    {
        MatchTracker.ClientMatchStarted -= HandleMatchStarted;
        DodgeballNetworkManager.ClientDisconnected -= HandleClientDisconnected;
        PlayerConnection.ClientPlayerInfoUpdated -= HandlePlayerInfoUpdated;
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            lobbyUIParent.SetActive(!lobbyUIParent.activeSelf);
        }
    }

    public void HandleSaveClick()
    {
        string username = usernameInput.text;
        playerConnection.CmdSetUsername(username);
    }

    public void HandleLeaveClick()
    {
        if (NetworkServer.active)
            if (NetworkClient.isConnected)
                dodgeballNetworkManager.StopHost();
            else
                dodgeballNetworkManager.StopServer();
        else
            dodgeballNetworkManager.StopClient();
        SceneManager.LoadScene(0);
    }

    public void HandleStartClick()
    {
        matchTracker.StartMatch();
    }

    private void HandleMatchStarted()
    {
        lobbyUIParent.SetActive(false);
    }

    private void HandlePlayerSpawned(PlayerConnection connection)
    {
        playerConnection = connection;
        InitLobbyUI();
    }

    private void InitLobbyUI()
    {
        ConstructPlayersText();
        usernameInput.text = playerConnection.Username;
    }

    private void ConstructPlayersText()
    {
        leftTeamPlayersText.text = "";
        rightTeamPlayersText.text = "";
        foreach (PlayerConnection connection in dodgeballNetworkManager.PlayerConnections)
            AddToPlayersText(connection);
    }

    private void AddToPlayersText(PlayerConnection connection)
    {
        if (connection.IsLeftTeam)
            leftTeamPlayersText.text += $"{connection.Username}\n";
        else
            rightTeamPlayersText.text += $"{connection.Username}\n";
    }

    private void HandleClientConnected(NetworkConnection _conn)
    {
        ConstructPlayersText();
    }

    private void HandleClientDisconnected(NetworkConnection _conn)
    {
        ConstructPlayersText();
    }
    
    private void HandlePlayerInfoUpdated()
    {
        ConstructPlayersText();
    }
}
