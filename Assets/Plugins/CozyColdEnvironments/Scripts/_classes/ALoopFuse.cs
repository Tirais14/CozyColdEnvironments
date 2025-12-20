#nullable enable
using System;

namespace CCEnvs
{
    public abstract class ALoopFuse
    {
        private ulong iterations;

        /// <summary>
        /// Default = 100000
        /// </summary>
        public ulong IterationsLimit { get; set; } = LoopChecker.ITERATIONS_LIMIT_DEFAULT;
        public string? ExceptionMessage { get; set; }

        protected bool MoveNext()
        {
            return iterations++ < IterationsLimit;
        }

        protected InvalidOperationException GetException()
        {
            if (ExceptionMessage.IsNullOrWhiteSpace())
                return new InvalidOperationException($"Prevented endless loop with interation count \"{iterations}\"");
            else
                return new InvalidOperationException($"Prevented endless loop with interation count \"{iterations}\". {ExceptionMessage}");
        }
    }
}
