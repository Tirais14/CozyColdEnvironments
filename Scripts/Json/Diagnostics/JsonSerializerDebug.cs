#nullable enable
using CCEnvs.Common;
using Newtonsoft.Json.Serialization;
using System;

namespace CCEnvs.Json.Diagnsotics
{
    public static class JsonSerializerDebug
    {
        public static bool IsEnabled { get; private set; }

        internal static EventHandler<ErrorEventArgs>? OnError { get; private set; }

        public static void Enable()
        {
            IsEnabled = true;
            OnError = (sender, e) =>
            {
                CCDebug.PrintException(e.ErrorContext.Error);
            };
        }

        public static void Disable()
        {
            IsEnabled = false;
            OnError = null;
        }
    }
}
