using System;
using UTIRLib.EnumFlags;

#nullable enable

namespace UTIRLib.Injector
{
    [Flags]
    public enum GetComponentSettings
    {
        None = 0,
        Default = 1,
        GetComponent = 4,
        GetAssignedObject = 8,
        ThrowIfSkipped = 16,
        ErrorIfSkipped = 32,
        WarningIfSkipped = 64,
        MessageIfSkipped = 128,
        ThrowIfNotFound = 256,
        ErrorIfNotFound = 512,
        WarningIfNotFound = 1024,
        MessageIfNotFound = 2048
    }

    public static class GetComponentSettingsExtensions
    {
        public static bool NotFoundErrorEnabled(this GetComponentSettings settings) =>
            settings.HasFlag(GetComponentSettings.ThrowIfNotFound)
            || settings.HasFlag(GetComponentSettings.ErrorIfNotFound)
            || settings.HasFlag(GetComponentSettings.WarningIfNotFound)
            || settings.HasFlag(GetComponentSettings.MessageIfNotFound);

        public static bool SkippedErrorEnabled(this GetComponentSettings settings) =>
            settings.HasFlag(GetComponentSettings.ThrowIfSkipped)
            || settings.HasFlag(GetComponentSettings.ErrorIfSkipped)
            || settings.HasFlag(GetComponentSettings.WarningIfSkipped)
            || settings.HasFlag(GetComponentSettings.MessageIfSkipped);

        public static GetComponentSettings GetDefaultSettings(this GetComponentSettings settings)
        {
            if (settings.HasFlag(GetComponentSettings.GetAssignedObject))
            {
                return GetComponentSettings.GetAssignedObject
                    | GetComponentSettings.WarningIfSkipped
                    | GetComponentSettings.ThrowIfNotFound;
            }
            else if (settings.HasFlag(GetComponentSettings.Default)
                || settings.HasFlag(GetComponentSettings.GetComponent))
            {
                return GetComponentSettings.GetComponent
                    | GetComponentSettings.WarningIfSkipped
                    | GetComponentSettings.ThrowIfNotFound;
            }

            return settings;
        }

        public static GetComponentSettings GetDebugSettings(this GetComponentSettings settings) =>
            settings.ResetFlags(
                GetComponentSettings.Default,
                GetComponentSettings.GetComponent,
                GetComponentSettings.GetAssignedObject);

        public static GetComponentSettings GetNotFoundErrorSettings(this GetComponentSettings settings)
        {
            var cleanedSettings = settings.GetDebugSettings();

            return cleanedSettings.ResetFlags(
                GetComponentSettings.ThrowIfSkipped,
                GetComponentSettings.ErrorIfSkipped,
                GetComponentSettings.WarningIfSkipped,
                GetComponentSettings.MessageIfSkipped);
        }

        public static GetComponentSettings GetSkippedErrorSettings(this GetComponentSettings settings)
        {
            var cleanedSettings = settings.GetDebugSettings();

            return cleanedSettings.ResetFlags(
                GetComponentSettings.ThrowIfNotFound,
                GetComponentSettings.ErrorIfNotFound,
                GetComponentSettings.WarningIfNotFound,
                GetComponentSettings.MessageIfNotFound);
        }

        public static GetComponentSettings ToOptional(this GetComponentSettings settings) =>
            settings.ResetFlags(settings.GetDebugSettings()).
            SetFlags(GetComponentSettings.MessageIfNotFound
                | GetComponentSettings.MessageIfSkipped);

        public static GetComponentSettings ToIfNull(this GetComponentSettings settings) =>
            settings.ResetFlags(settings.GetSkippedErrorSettings());

        public static GetComponentSettings DisableDebug(this GetComponentSettings settings) =>
            settings.ResetFlags(settings.GetDebugSettings());
    }
}