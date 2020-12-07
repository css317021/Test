using Grpc.Core;
using MagicOnion.Client;
using Sample.Shared.Hubs;
using Sample.Shared.MessagePackObjects;
using Sample.Shared.Services;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class SampleController : MonoBehaviour, ISampleHubReceiver
{
    [SerializeField]
    public GameObject StartPanel;
    [SerializeField]
    public GameObject ChatPanel;
    [SerializeField]
    public GameObject AlertPanel;
    [SerializeField]
    public GameObject MenuPanel;
    //名前の入力
    public InputField NameInputField;
    //
    public Text myNameText;
    [SerializeField]
    //参加者名簿
    public GameObject memList;
    //
    public Text memListPrefab;
    //生成された名前
    Text MemName;
    //
    int myNum;

    int LeaveNum;

    //入退室判定
    int t = 0;

    string myName;

    private Channel channel;
    private ISampleService sampleService;
    private ISampleHub sampleHub;

    void Start()
    {
        //Panelの初期化
        ToStart();

        //InputFieldの準備
        NameInputField = GameObject.Find("InputField").GetComponent<InputField>();

        this.channel = new Channel("18.181.92.51:12345", ChannelCredentials.Insecure);
        this.sampleService = MagicOnionClient.Create<ISampleService>(channel);
        this.sampleHub = StreamingHubClient.Connect<ISampleHub, ISampleHubReceiver>(this.channel, this);

        // 普通の API の呼び出しはコメントアウトしておきます
        // 残しておいても問題はないです（リアルタイム通信と両方動きます）
        //this.SampleServiceTest(1, 2);

        //this.SampleHubTest();
    }

    public void ToChat()
    {
        myName = NameInputField.text;
        //入力の判定
        if (string.IsNullOrWhiteSpace(myName))
        {
            //AlertPanel表示
            StartPanel.SetActive(false);
            ChatPanel.SetActive(false);
            AlertPanel.SetActive(true);
            MenuPanel.SetActive(false);
        }
        else
        {
            //入室していなければ入室する
            if (t == 0)
            {
                this.SampleHubTest();
                //myNum = await this.sampleHub.JoinAsync(player);
                myNameText.text = myName;
            }
            //ChatPanel表示
            StartPanel.SetActive(false);
            ChatPanel.SetActive(true);
            AlertPanel.SetActive(false);
            MenuPanel.SetActive(false);
        }
    }

    //Menu画面
    public void ToMenu()
    {
        //StartPanel表示
        StartPanel.SetActive(false);
        ChatPanel.SetActive(false);
        AlertPanel.SetActive(false);
        MenuPanel.SetActive(true);
    }

    //Start画面
    public void ToStart()
    {
        //入室済みなら退出する
        if (t == 1)
        {
            LeaveChat();
            //名前の初期化
            NameInputField.text = "";
            t = 0;
        }
        //StartPanel表示
        StartPanel.SetActive(true);
        ChatPanel.SetActive(false);
        AlertPanel.SetActive(false);
        MenuPanel.SetActive(false);
    }

    async void OnDestroy()
    {
        await this.sampleHub.DisposeAsync();
        await this.channel.ShutdownAsync();
    }

    /// <summary>
    /// 普通のAPI通信のテスト用のメソッド
    /// </summary>
    async void SampleServiceTest(int x, int y)
    {
        var sumReuslt = await this.sampleService.SumAsync(x, y);
        Debug.Log($"{nameof(sumReuslt)}: {sumReuslt}");

        var productResult = await this.sampleService.ProductAsync(2, 3);
        Debug.Log($"{nameof(productResult)}: {productResult}");
    }

    /// <summary>
    /// リアルタイム通信のテスト用のメソッド
    /// </summary>
    async void SampleHubTest()
    {
        // 自分のプレイヤー情報を作ってみる
        var player = new Player
        {
            Name = myName,
            Position = new Vector3(0, 0, 0),
            Rotation = new Quaternion(0, 0, 0, 0)
        };

        // ゲームに接続する
        await this.sampleHub.JoinAsync(player);

        //入室状態にする
        t = 1;

        // チャットで発言してみる
        await this.sampleHub.SendMessageAsync("こんにちは！");

        // 位置情報を更新してみる
        player.Position = new Vector3(1, 0, 0);
        await this.sampleHub.MovePositionAsync(player.Position);
    }

    //切断する
    async void LeaveChat()
    {
        await this.sampleHub.LeaveAsync();
    }

    #region リアルタイム通信でサーバーから呼ばれるメソッド群

    public void OnJoin(string name)
    {
        Debug.Log($"{name}さんが入室しました");
        this.MemName = Instantiate(memListPrefab);
        this.MemName.transform.SetParent(memList.transform, false);
        string v = $"{name}";
        this.MemName.text = v;
    }

    public void OnLeave(string name)
    {

        Destroy(this.MemName);
        Debug.Log($"{name}さんが退室しました");
    }

    public void OnSendMessage(string name, string message)
    {
        Debug.Log($"{name}: {message}");
    }

    public void OnMovePosition(Player player)
    {
        Debug.Log($"{player.Name}さんが移動しました: {{ x: {player.Position.x}, y: {player.Position.y}, z: {player.Position.z} }}");
    }

    #endregion
}