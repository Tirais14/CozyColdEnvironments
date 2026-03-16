#nullable enable
using CCEnvs.Diagnostics;

namespace CCEnvs.Saves
{
    public static class SaveSystemErrorHandler
    {
        internal static SaveEntryDeserializingErrorHandler? _onSaveEntryDeserializingError =
            (_, ex) =>
            {
                CCDebug.Instance.PrintException(ex);
                return null;
            };

        public static event SaveEntryDeserializingErrorHandler? OnSaveEntryDeserializingError {
            add => _onSaveEntryDeserializingError += value;
            remove => _onSaveEntryDeserializingError -= value;
        }
    }
}
