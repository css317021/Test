using MagicOnion;
using MagicOnion.Server;
using Sample.Shared.Services;

namespace Sample.Server.Services
{
    public class SampleService : ServiceBase<ISampleService>, ISampleService
    {
        public UnaryResult<int> SumAsync(int x, int y)
        {
            return UnaryResult(x + y);
        }

        public UnaryResult<int> ProductAsync(int x, int y)
        {
            return UnaryResult(x * y);
        }
    }
}