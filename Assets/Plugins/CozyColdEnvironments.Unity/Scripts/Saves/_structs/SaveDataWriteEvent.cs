using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public readonly struct SaveDataWriteEvent : IEquatable<SaveDataWriteEvent>
    {
        public IReadOnlyList<SaveEntry> WrittenEntries { get; }

        public WriteSaveDataMode WriteMode { get; }

        public SaveDataWriteEvent(
            IReadOnlyList<SaveEntry> changedSaveEntries,
            WriteSaveDataMode writeMode
            ) 
        {
            WrittenEntries = changedSaveEntries;
            WriteMode = writeMode;
        }

        public void Deconstruct(
            out IReadOnlyList<SaveEntry> writtenEntries,
            out WriteSaveDataMode writeMode
            )
        {
            writtenEntries = WrittenEntries;
            writeMode = WriteMode;
        }

        public static bool operator ==(SaveDataWriteEvent left, SaveDataWriteEvent right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SaveDataWriteEvent left, SaveDataWriteEvent right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is SaveDataWriteEvent @event && Equals(@event);
        }

        public bool Equals(SaveDataWriteEvent other)
        {
            return EqualityComparer<IReadOnlyList<SaveEntry>>.Default.Equals(WrittenEntries, other.WrittenEntries)
                   &&
                   WriteMode == other.WriteMode;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(WrittenEntries, WriteMode);
        }

        public override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(WrittenEntries)}: {WrittenEntries}; {nameof(WriteMode)}: {WriteMode})";
        }
    }
}
