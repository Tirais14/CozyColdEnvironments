#nullable enable

using System.Threading;
using System.Threading.Tasks;

namespace CCEnvs.Patterns.Commands
{
    public interface ICommandAsync : ICommandBase<ICommandAsync>
    {
        ValueTask ExecuteAsync(CancellationToken cancellationToken = default);
    }
}