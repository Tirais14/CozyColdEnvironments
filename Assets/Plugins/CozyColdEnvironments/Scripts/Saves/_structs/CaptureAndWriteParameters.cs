using System;

#nullable enable
namespace CCEnvs.Saves
{
    public readonly struct CaptureAndWriteParameters : IEquatable<CaptureAndWriteParameters>
    {
        public WriteSaveDataMode WriteMode { get; }

        public int? PreferedObjectLimitPerFrame { get; }

        public CaptureAndWriteParameters(
            WriteSaveDataMode writeMode,
            int? preferedObjectLimitPerFrame = null
            )
        {
            WriteMode = writeMode;
            PreferedObjectLimitPerFrame = preferedObjectLimitPerFrame;
        }

        public static implicit operator CaptureAndWriteParameters(WriteSaveDataMode writeMode)
        {
            return new CaptureAndWriteParameters(writeMode);
        }

        public static bool operator ==(CaptureAndWriteParameters left, CaptureAndWriteParameters right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CaptureAndWriteParameters left, CaptureAndWriteParameters right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is CaptureAndWriteParameters parameters && Equals(parameters);
        }

        public bool Equals(CaptureAndWriteParameters other)
        {
            return WriteMode == other.WriteMode 
                   &&
                   PreferedObjectLimitPerFrame == other.PreferedObjectLimitPerFrame;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(WriteMode, PreferedObjectLimitPerFrame);
        }

        public override string ToString()
        {
            return $"({nameof(WriteMode)}: {WriteMode}; {nameof(PreferedObjectLimitPerFrame)})";
        }
    }
}
