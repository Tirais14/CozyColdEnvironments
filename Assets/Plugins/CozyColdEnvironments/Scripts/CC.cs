#nullable enable
using CCEnvs.Attributes;
using CCEnvs.Json;
using CCEnvs.Json.Converters;
using CCEnvs.Patterns.Commands;
using CCEnvs.Reflection;
using CCEnvs.Serialization;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using R3;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace CCEnvs
{
    public delegate ValueTask<TOutput> ConverterAsync<in TInput, TOutput>(TInput input);

    public static class CC
    {
        public const string FULL_NAME = "CozyColdEnvironments";
        public const string COPYRIGHT_STAMP = "@Tirais: " + FULL_NAME;

        public static object EmptyObject { get; } = new object();

        public static object[] EmptyArguments { get; } = Array.Empty<object>();

        public static Func<bool> TrueFactory { get; } = static () => true;
        public static Func<bool> FalseFactory { get; } = static () => false;

        public static JsonSerializerSettings JsonSettings { get; } = JsonSerializerSettingsProvider.GetDefault(
            new ByDescriptorJsonConverter(),
            new ObservableDictionaryJsonConverter()
            );

        [field: OnInstallResetable]
        public static CommandScheduler CommandScheduler { get; private set; } = null!;

        [field: OnInstallResetable]
        public static int MainThreadID { get; private set; }

        public static bool IsDebugMode {
            get
            {
#if CC_DEBUG_ENABLED || UNITY_EDITOR || DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        #region Install
        public static void Install(string[]? additionalAssemblyNames = null)
        {
            var domainMembers = CCProjectHelper.GetDomainMembers(
                MemberTypes.NestedType
                |
                MemberTypes.Field
                |
                MemberTypes.Property
                |
                MemberTypes.Method
                , additionalAssemblyNames
                );

            CCProjectHelper.Install(domainMembers);
            TypeSerializationHelper.Install(domainMembers);

            MainThreadID = Thread.CurrentThread.ManagedThreadId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMainThread(this Thread thread)
        {
            CommunityToolkit.Diagnostics.Guard.IsNotNull(thread, nameof(thread));

            return thread.ManagedThreadId == MainThreadID;
        }

        [OnInstallExecutable]
        private static void CreateCommandScheduler()
        {
            CommandScheduler = CommandScheduler.CreateDefaultRegistered();
        }
        #endregion Install

#pragma warning disable S112

        public static class ThrowHelper
        {
            public static InvalidCastException InvalidCastException(Type toType,
                          string? message = null,
                          Exception? innerException = null)
            {
                return new InvalidCastException($"Conversation type = {toType.GetFullName()}. {message}", innerException);
            }

            public static InvalidCastException InvalidCastException(Type fromType,
                                                Type toType,
                                                string? message = null,
                                                Exception? innerException = null)
            {
                return new InvalidCastException($"From {fromType.GetFullName()} to {toType.GetFullName()}. {message}", innerException);
            }

            public static IndexOutOfRangeException IndexOutOfRangeException(long index)
            {
                return new IndexOutOfRangeException($"Index = {index}.");
            }

            public static ArgumentException ArgumentExceptionException(object argument, string argName)
            {
                return new ArgumentException($"Argument: {argName} cannot be {argName}.");
            }

            public static InvalidOperationException InvalidOperationException(object arg, string? argName = null)
            {
                return new InvalidOperationException($"Argument: {argName ?? "value"} cannot be {arg}");
            }

            public static InvalidOperationException MemberNotFoundException(string? name = null, MemberTypes? memberType = null, Type? reflectedType = null, BindingFlags? bindingFlags = null, Type[]? argumentTypes = null, Binder? binder = null)
            {
                string msg = Sentence.Empty.AddIfNotDefault($"name: {name}", name)
                    .AddIfNotDefault($"member type: {memberType}", memberType)
                    .AddIfNotDefault($"reflected type: {reflectedType}", reflectedType)
                    .AddIfNotDefault($"binding flags: {bindingFlags}", bindingFlags)
                    .AddIfNotDefault($"argument types: {argumentTypes?.Select(x => x.ToString()).Aggregate((left, right) => left + right)}", argumentTypes)
                    .AddIfNotDefault($"binder: {binder}", binder)
                    .ToString();

                return new InvalidOperationException($"Member not found. {msg}");
            }

            public static InvalidOperationException EndlessLoopException(long iterationCount, string? msg = null)
            {
                return new InvalidOperationException($"Prevented endless loop. Iteration count: {iterationCount}. {msg}");
            }

            public static InvalidOperationException MetadataNotFound(MemberInfo member)
            {
                Guard.IsNotNull(member, nameof(member));

                return new InvalidOperationException($"Metadata not found. Member info: name: {member}, type: {member.MemberType}");
            }

            public static InvalidOperationException TypeNotFoundException(string? typeName, string? assemblyName = null)
            {
                return new InvalidOperationException($"Type name: {typeName}, assembly name: {assemblyName}");
            }

            public static ArgumentException IsNotTypeException(Type? left, Type? right, string? paramName = null)
            {
                return new ArgumentException($"Invalid argument: {paramName ?? "value"}. Left type: {(left?.ToString() ?? "null")} is not type: {(right?.ToString() ?? "null")}");
            }

            public static NotSupportedException CollectionIsReadOnly()
            {
                return new NotSupportedException("Collection is read only");
            }

            public static NotSupportedException CollectionIsReadOnly(Type? collectionType)
            {
                return new NotSupportedException($"Collection: {collectionType} is read only");
            }

            public static InvalidOperationException CannotCreateInstance(string name, string? msg = null)
            {
                if (msg is not null)
                    msg = ". " + msg;

                return new InvalidOperationException($"Cannot create instance: {name} {msg}");
            }

            public static InvalidOperationException CannotCreateInstance(Type type, string? msg = null)
            {
                return CannotCreateInstance(type.ToString(), msg);
            }

            public static NotSupportedException ReadOnlyCollection<T>(T collection)
            {
                return new NotSupportedException($"Collection {collection} is read only");
            }
        }


#pragma warning restore S112

        public static class Guard
        {
            /// <exception cref="ArgumentNullException"></exception>
            public static void IsNotNull<T>([NotNull] T? obj, string? paramName = null)
            {
                if (obj.IsNull())
                    throw new ArgumentNullException(paramName ?? "value");
            }

            public static void IsNotNullTarget<T>([NotNull] T? obj)
            {
                IsNotNull(obj, "target");
            }

            public static void IsNotNullSource<T>([NotNull] T? obj)
            {
                IsNotNull(obj, "source");
            }

            public static void IsNotNullState<T>([NotNull] T? obj)
            {
                IsNotNull(obj, "state");
            }

            public static void IsNotNullInput<T>([NotNull] T? obj)
            {
                IsNotNull(obj, "input");
            }

            public static void IsNotType(Type left, Type right, string? paramName = null)
            {
                CommunityToolkit.Diagnostics.Guard.IsNotNull(left, nameof(left));
                CommunityToolkit.Diagnostics.Guard.IsNotNull(right, nameof(right));

                if (left.IsNotType(right))
                    throw ThrowHelper.IsNotTypeException(left, right, paramName);
            }

            public static void IsNotType<TRight>(Type left, string? paramName = null)
            {
                IsNotType(left, typeof(TRight), paramName);
            }

            public static void IsNotDefault<T>(T value, string paramName = "value")
                where T : struct
            {
                if (value.IsDefault())
                    throw new ArgumentException($"{paramName} is default");
            }
        }

        public static class Platform
        {
            public static bool IsUnityEditor {
                get
                {
#if UNITY_EDITOR
                    return true;
#else
                    return false;
#endif
                }
            }
        }
    }
}