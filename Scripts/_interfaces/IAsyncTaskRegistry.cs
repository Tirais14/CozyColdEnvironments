using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace UTIRLib
{
    public interface IAsyncTaskRegistry
    {
        event Action? OnTasksCompleted;

        int TaskCount { get; }

        void RegisterTask(UniTask task);

        void RegisterTask(Task task);

        void RegisterTask(Func<bool> waitUntilFalse);
    }
}
