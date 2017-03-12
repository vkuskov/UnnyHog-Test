using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class NetworkManager : MonoBehaviour, INetworkEventListener {
    private const int MAX_CONNECTIONS = 10;

    private const string LOCALHOST = "127.0.0.1";

    private static NetworkManager _instance;

    public static NetworkManager Instance { get { return _instance; } }

    private int _channelId;
    private int _hostId;

    private int _connectionId;

    [SerializeField]
    private UnityEvent _onConnected;

    [SerializeField]
    private UnityEvent _onDisconnected;

    [SerializeField]
    private InputField _host;

    [SerializeField]
    private InputField _port;

    private Client _client;
    private Server _server;

    public void Awake() {
        _instance = this;
    }

    public void Start() {
        initNetwork();
    }

    public void Host() {
        _server = new Server();
        string host;
        int port;
        getHostAndPort(out host, out port);
        _server.Init(port);
        connect(LOCALHOST, port);
    }

    private void getHostAndPort(out string host, out int port) {
        host = _host.text;
        int.TryParse(_port.text, out port);
    }

    public void Connect() {
        string host;
        int port;
        getHostAndPort(out host, out port);
        connect(host, port);
    }

    public void Update() {
        if (_client != null) {
            _client.Handle();
        }
        if (_server != null) {
            _server.Handle();
        }
    }

    public void OnDestroy() {
        NetworkTransport.Shutdown();
    }

    public void SpawnAvatar(FullState state) {
        if (_client != null) {
            _client.OnSpawn(state);
        }
    }

    public void SyncPlayer(FullState state) {
        if (_client != null) {
            _client.OnSync(state);
        }
    }

    public void Shoot(ShootEvent shoot) {
        if (_client != null) {
            _client.OnShoot(shoot);
        }
    }

    public void Death() {
        if (_client != null) {
            _client.OnDeath();
        }
    }

    public void ShootAnim() {
        if (_client != null) {
            _client.OnShootAnim();
        }
    }

    private void initNetwork() {
        GlobalConfig gconfig = new GlobalConfig();
        gconfig.ReactorModel = ReactorModel.FixRateReactor;
        gconfig.ThreadAwakeTimeout = 10;
        NetworkTransport.Init(gconfig);
    }

    private void connect(string host, int port) {
        _client = new Client(this);
        _client.Init(host, port);
    }

    public void OnConnected() {
        _onConnected.Invoke();
    }

    public void OnDisconnected() {
        _onDisconnected.Invoke();
        _client = null;
    }
}
