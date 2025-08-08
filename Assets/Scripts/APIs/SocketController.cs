using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Best.SocketIO;
using Best.SocketIO.Events;
using Newtonsoft.Json.Linq;
using DG.Tweening;


public class SocketController : MonoBehaviour
{

    [SerializeField] UIManager uiManager;
    internal SocketModel socketModel = new SocketModel();


    //WebSocket currentSocket = null;
    [SerializeField] internal bool isResultdone = false;

    private SocketManager manager;

    [SerializeField] internal JSFunctCalls JSManager;
    protected string nameSpace = "playground"; //BackendChanges
    private Socket gameSocket;


    protected string SocketURI = null;

    [SerializeField] protected string TestSocketURI = "http://localhost:5000/";


    [SerializeField]
    private string TestToken;

    protected string gameID = "SL-BE";

    internal bool isLoading;
    internal bool SetInit = false;
    private const int maxReconnectionAttempts = 6;
    private readonly TimeSpan reconnectionDelay = TimeSpan.FromSeconds(10);

    internal Action OnInit;
    internal Action ShowDisconnectionPopup;


    private bool isConnected = false; //Back2 Start
    private bool hasEverConnected = false;
    private const int MaxReconnectAttempts = 5;
    private const float ReconnectDelaySeconds = 2f;

    private float lastPongTime = 0f;
    private float pingInterval = 2f;
    private float pongTimeout = 3f;
    private bool waitingForPong = false;
    private int missedPongs = 0;
    private const int MaxMissedPongs = 5;
    private Coroutine PingRoutine; //Back2 end


    private void Awake()
    {
        // Debug.unityLogger.logEnabled = false;
        isLoading = true;
        SetInit = false;
        // Debug.unityLogger.logEnabled = false;
    }

    private void Start()
    {
        //OpenWebsocket();
        // OpenSocket();
    }

    void ReceiveAuthToken(string jsonData)
    {
        Debug.Log("Received data: " + jsonData);

        // Parse the JSON data
        var data = JsonUtility.FromJson<AuthTokenData>(jsonData);
        SocketURI = data.socketURL;
        myAuth = data.cookie;
        nameSpace = data.nameSpace;
        // Proceed with connecting to the server using myAuth and socketURL
    }

    string myAuth = null;

    internal void OpenSocket()
    {
        Debug.Log("OPeningt");
        SocketOptions options = new SocketOptions(); //Back2 Start
        options.AutoConnect = false;
        options.Reconnection = false;
        options.Timeout = TimeSpan.FromSeconds(3); //Back2 end
        options.ConnectWith = Best.SocketIO.Transports.TransportTypes.WebSocket;


#if UNITY_WEBGL && !UNITY_EDITOR
        JSManager.SendCustomMessage("authToken");
        StartCoroutine(WaitForAuthToken(options));
#else
        Func<SocketManager, Socket, object> authFunction = (manager, socket) =>
        {
            return new
            {
                token = TestToken,
            };
        };
        options.Auth = authFunction;
        // Proceed with connecting to the server
        SetupSocketManager(options);
#endif
    }

    private IEnumerator WaitForAuthToken(SocketOptions options)
    {
        // Wait until myAuth is not null
        while (myAuth == null)
        {
            yield return null;
        }

        // Once myAuth is set, configure the authFunction
        Func<SocketManager, Socket, object> authFunction = (manager, socket) =>
        {
            return new
            {
                token = myAuth,

            };
        };
        options.Auth = authFunction;

        Debug.Log("Auth function configured with token: " + myAuth);

        // Proceed with connecting to the server
        SetupSocketManager(options);
    }

    // internal void CloseSocket()
    // {
    //     uiManager.RaycastBlocker.SetActive(false);

    // }

    void CloseGame()
    {
        Debug.Log("Unity: Closing Game");
        StartCoroutine(CloseSocket());
    }

    internal IEnumerator CloseSocket() //Back2 Start
    {
        uiManager.RaycastBlocker.SetActive(true);
        ResetPingRoutine();
        //  SendData("EXIT");

        Debug.Log("Closing Socket");

        manager?.Close();
        manager = null;

        Debug.Log("Waiting for socket to close");

        yield return new WaitForSeconds(0.5f);

        Debug.Log("Socket Closed");
#if UNITY_WEBGL && !UNIFonTY_EDITOR
        JSManager.SendCustomMessage("OnExit");
#endif
    }
    private void OnSocketState(bool state)
    {
        if (state)
        {
            Debug.Log("my state is " + state);
            InitRequest("AUTH");
        }
    }
    private void OnSocketError(string data)
    {
        Debug.Log("Received error with data: " + data);
    }
    private void OnSocketAlert(string data)
    {
        Debug.Log("Received alert with data: " + data);
    }

    private void OnSocketOtherDevice(string data)
    {
        Debug.Log("Received Device Error with data: " + data);
    }

    private void AliveRequest()
    {
        SendData("YES I AM ALIVE");
    }

    void OnConnected(ConnectResponse resp) //Back2 Start
    {
        Debug.Log("✅ Connected to server.");

        if (hasEverConnected)
        {
            uiManager.CheckAndClosePopups();
        }

        isConnected = true;
        hasEverConnected = true;
        waitingForPong = false;
        missedPongs = 0;
        lastPongTime = Time.time;
        SendPing();
    } //Back2 end
    private void OnError()
    {
        Debug.LogError("Socket Error");
    }
    private void OnDisconnected() //Back2 Start
    {
        Debug.LogWarning("⚠️ Disconnected from server.");
        isConnected = false;
        ResetPingRoutine();
    } //Back2 end
    private void OnPongReceived(string data) //Back2 Start
    {
        Debug.Log("✅ Received pong from server.");
        waitingForPong = false;
        missedPongs = 0;
        lastPongTime = Time.time;
        Debug.Log($"⏱️ Updated last pong time: {lastPongTime}");
        Debug.Log($"📦 Pong payload: {data}");
    } //Back2 end

    private void SendPing() //Back2 Start
    {
        ResetPingRoutine();
        PingRoutine = StartCoroutine(PingCheck());
    }
    void ResetPingRoutine()
    {
        if (PingRoutine != null)
        {
            StopCoroutine(PingRoutine);
        }
        PingRoutine = null;
    }

    private IEnumerator PingCheck()
    {
        while (true)
        {
            Debug.Log($"🟡 PingCheck | waitingForPong: {waitingForPong}, missedPongs: {missedPongs}, timeSinceLastPong: {Time.time - lastPongTime}");

            if (missedPongs == 0)
            {
                uiManager.CheckAndClosePopups();
            }

            // If waiting for pong, and timeout passed
            if (waitingForPong)
            {
                if (missedPongs == 2)
                {
                    uiManager.ReconnectionPopup();
                }
                missedPongs++;
                Debug.LogWarning($"⚠️ Pong missed #{missedPongs}/{MaxMissedPongs}");

                if (missedPongs >= MaxMissedPongs)
                {
                    Debug.LogError("❌ Unable to connect to server — 5 consecutive pongs missed.");
                    isConnected = false;
                    uiManager.DisconnectionPopup();
                    yield break;
                }
            }

            // Send next ping
            waitingForPong = true;
            lastPongTime = Time.time;
            Debug.Log("📤 Sending ping...");
            SendData("ping");
            yield return new WaitForSeconds(pingInterval);
        }
    } //Back2 end
    private void OnDisconnected(string response)
    {
        Debug.Log("Disconnected from the server");
        StopAllCoroutines();
        ShowDisconnectionPopup?.Invoke();
    }

    private void OnError(string response)
    {
        Debug.LogError("Error: " + response);
    }

    private void OnListenEvent(string data)
    {
        Debug.Log("Received some_event with data: " + data);
        ParseResponse(data);
    }
    private void OnResult(string data)
    {
        Debug.Log("Received some_event with data: " + data);
        ParseResponse(data);
    }

    private void SetupSocketManager(SocketOptions options)
    {
        // Create and setup SocketManager
#if UNITY_EDITOR
        this.manager = new SocketManager(new Uri(TestSocketURI), options);
#else
        this.manager = new SocketManager(new Uri(SocketURI), options);
#endif
        if (string.IsNullOrEmpty(nameSpace) | string.IsNullOrWhiteSpace(nameSpace))
        {
            gameSocket = this.manager.Socket;
        }
        else
        {
            Debug.Log("Namespace used :" + nameSpace);
            gameSocket = this.manager.GetSocket("/" + nameSpace);
        }
        // Set subscriptions
        gameSocket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
        gameSocket.On(SocketIOEventTypes.Disconnect, OnDisconnected); //Back2 Start
        gameSocket.On(SocketIOEventTypes.Error, OnError); //Back2 Start
        gameSocket.On<string>("game:init", OnListenEvent);
        gameSocket.On<string>("result", OnResult);

        gameSocket.On<bool>("socketState", OnSocketState);
        gameSocket.On<string>("internalError", OnSocketError);
        gameSocket.On<string>("alert", OnSocketAlert);
        gameSocket.On<string>("pong", OnPongReceived); //Back2 Start
        gameSocket.On<string>("AnotherDevice", OnSocketOtherDevice);
        manager.Open();
    }

    // Connected event handler implementation

    private void InitRequest(string eventName)
    {
        var initmessage = new { Data = new { GameID = gameID }, id = "Auth" };
        SendData(eventName, initmessage);
    }


    private void ParseResponse(string jsonObject)
    {
        Debug.Log(jsonObject);
        Debug.Log(jsonObject);
        Root myData = JsonConvert.DeserializeObject<Root>(jsonObject);

        string id = myData.id;
        // JObject resp = JObject.Parse(jsonObject);

        // string id = resp["id"].ToString();
        // var message = resp["message"];
        // var gameData = message["GameData"];

        // if (message["PlayerData"] != null)
        //     socketModel.playerData = message["PlayerData"].ToObject<PlayerData>();
        switch (id)
        {
            case "initData":
                {
                    socketModel.initGameData = myData.gameData;
                    socketModel.uIData = myData.uiData;
                    socketModel.InitMultipliers = myData.features;
                    socketModel.playerData = myData.player;
                    // socketModel.uIData.symbols = message["UIData"]["paylines"]["symbols"].ToObject<List<Symbol>>();
                    // socketModel.uIData.wildMultiplier = gameData["wildMultiplier"].ToObject<List<double>>();
                    // socketModel.uIData.BatsMultiplier = gameData["BatsMultiplier"].ToObject<List<double>>();
                    // socketModel.initGameData.Bets = gameData["Bets"].ToObject<List<double>>();
                    // socketModel.initGameData.lineData = gameData["Lines"].ToObject<List<List<int>>>();

                    OnInit?.Invoke();
                    Debug.Log("init data" + JsonConvert.SerializeObject(socketModel.initGameData));

                    break;
                }
            case "ResultData":
                {
                    socketModel.resultGameData = myData;
                    socketModel.playerData = myData.player;
                    Debug.Log("result data" + JsonConvert.SerializeObject(socketModel.resultGameData));
                    isResultdone = true;
                    break;
                }
            case "gambleDraw":
                {
                    socketModel.playerData = myData.player;

                    socketModel.gambleData.currentWinning = myData.payload.currentWinning;
                    socketModel.gambleData.playerWon = myData.payload.playerWon;
                    socketModel.gambleData.coin = myData.payload.coin;
                    // Debug.Log("result" + JsonConvert.SerializeObject(socketModel.gambleData));
                    isResultdone = true;

                    break;
                }
            case "gambleCollect":
                {
                    socketModel.playerData = myData.player;
                    socketModel.gambleData.currentWinning = myData.payload.winAmount;
                    // socketModel.gambleData.currentWinning = message["currentWinning"].ToObject<double>();
                    // socketModel.gambleData.balance = message["balance"].ToObject<double>();
                    // Debug.Log("collect" + JsonConvert.SerializeObject(socketModel.gambleData));
                    isResultdone = true;
                    break;
                }
            case "ExitUser":
                {
                    gameSocket.Disconnect();
                    if (this.manager != null)
                    {
                        Debug.Log("Dispose my Socket");
                        this.manager.Close();
                    }
#if UNITY_WEBGL && !UNITY_EDITOR
                    JSManager.SendCustomMessage("onExit");
#endif
                    break;
                }
        }

    }
    internal void AccumulateResult(int currBet)
    {
        isResultdone = false;
        MessageData message = new MessageData();
        message.payload = new SentDeta();
        message.type = "SPIN";

        message.payload.betIndex = currBet;
        // Serialize message data to JSON
        string json = JsonUtility.ToJson(message);
        SendDataWithNamespace("request", json);
    }
    internal void OnGamble()
    {
        isResultdone = false;
        MessageData message = new MessageData();
        message.payload = new SentDeta();
        message.type = "GAMBLE";
        message.payload.Event = "init";
        // Serialize message data to JSON
        string json = JsonUtility.ToJson(message);
        SendDataWithNamespace("request", json);


    }
    internal void GambleDraw(string selectedside, string gambleOption, double currBet)
    {
        isResultdone = false;
        MessageData message = new MessageData();
        message.payload = new SentDeta();
        message.type = "GAMBLE";
        message.payload.gambleOption = gambleOption;
        message.payload.selectedSide = selectedside;
        message.payload.lastWinning = currBet;
        message.payload.Event = "draw";
        // Serialize message data to JSON
        string json = JsonUtility.ToJson(message);
        SendDataWithNamespace("request", json);
    }
    internal void GambleCollect()
    {
        isResultdone = false;
        MessageData message = new MessageData();
        message.payload = new SentDeta();
        message.type = "GAMBLE";
        // message.payload.gambleOption = gambleOption;
        // message.payload.selectedSide = selectedside;
        // message.payload.lastWinning = currBet;
        message.payload.Event = "collect";
        // Serialize message data to JSON
        string json = JsonUtility.ToJson(message);
        SendDataWithNamespace("request", json);
    }

    private void SendDataWithNamespace(string eventName, string json = null)
    {
        // Send the message
        if (gameSocket != null && gameSocket.IsOpen)
        {
            if (json != null)
            {
                gameSocket.Emit(eventName, json);
                Debug.Log("JSON data sent: " + json);
            }
            else
            {
                gameSocket.Emit(eventName);
            }
        }
        else
        {
            Debug.LogWarning("Socket is not connected.");
        }
    }
    internal void SendData(string eventName, object message = null)
    {

        if (gameSocket == null || !gameSocket.IsOpen)
        {
            Debug.LogWarning("Socket is not connected.");
            return;
        }
        if (message == null)
        {
            gameSocket.Emit(eventName);
            return;
        }
        isResultdone = false;
        string json = JsonConvert.SerializeObject(message);
        gameSocket.Emit(eventName, json);
        Debug.Log("JSON data sent: " + json);
    }
}
