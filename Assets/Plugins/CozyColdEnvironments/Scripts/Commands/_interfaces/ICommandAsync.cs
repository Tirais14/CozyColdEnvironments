#nullable enable

using System.Threading;
using System.Threading.Tasks;

namespace CCEnvs.Patterns.Commands
{
    public interface ICommandAsync : ICommandBase
    {
        ValueTask ExecuteAsync(CancellationToken cancellationToken = default);

        ICommandAsync Reset();
    }
}