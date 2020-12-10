using MagicOnion.Server.Hubs;
using Sample.Shared.Hubs;
using Sample.Shared.MessagePackObjects;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

public class SampleHub : StreamingHubBase<ISampleHub, ISampleHubReceiver>, ISampleHub
{
    IGroup room;
    Player me;

    string a = "1";

    public async Task JoinAsync(Player player)
    {
        Console.WriteLine("START");
        //ルームは全員固定
        const string roomName = "SampleRoom";
        Console.WriteLine("Create Room");
        //ルームに参加&ルームを保持
        this.room = await this.Group.AddAsync(roomName);
        Console.WriteLine("Join Room");
        //自分の情報も保持
        me = player;
        Console.WriteLine("Create I");
        //参加したことをルームに参加している全メンバーに通知
        this.Broadcast(room).OnJoin(me.Name);
    }

    public async Task LeaveAsync()
    {
        //ルーム内のメンバーから自分を削除
        await room.RemoveAsync(this.Context);
        //退室したことを全メンバーに通知
        this.Broadcast(room).OnLeave(me.Name);
    }

    public async Task SendMessageAsync(string message)
    {
        //発言した内容を全メンバーに通知
        this.Broadcast(room).OnSendMessage(me.Name, message);

        await Task.CompletedTask;
    }

    public async Task MovePositionAsync(Vector3 position)
    {
        // サーバー上の情報を更新
        me.Position = position;

        //更新したプレイヤーの情報を全メンバーに通知
        this.Broadcast(room).OnMovePosition(me);

        await Task.CompletedTask;
    }

    protected override ValueTask OnConnecting()
    {
        // handle connection if needed.
        Console.WriteLine($"client connected {this.Context.ContextId}");
        return CompletedTask;
    }

    protected override ValueTask OnDisconnected()
    {
        // handle disconnection if needed.
        // on disconnecting, if automatically removed this connection from group.
        return CompletedTask;
    }
}