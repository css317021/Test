using Grpc.Core;
using MagicOnion.Hosting;
using MagicOnion.Server;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Sample.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // コンソールにログ出力するように設定
            GrpcEnvironment.SetLogger(new Grpc.Core.Logging.ConsoleLogger());

            // MagicOnionを使ってホスト作成、起動
            await MagicOnionHost.CreateDefaultBuilder()
                .UseMagicOnion(
                    new MagicOnionOptions(isReturnExceptionStackTraceInErrorDetail: true),
                    new ServerPort("18.181.92.51", 12345, ServerCredentials.Insecure))
                .RunConsoleAsync();
        }
    }
}