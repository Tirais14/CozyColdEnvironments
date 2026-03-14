# Navigation
- [About](#about)

- [Setup](#setup)

- [Dependecies](#dependecies)

- [Roadmap](#roadmap)

- [Planned](#planned)

- [Games](#games)

- [GameObjectQuery](#gameobjectquery)

- [SavingSystem](#savingsystem)

- [CommandScheduler](#commandscheduler)

- [Command](#command)
  - [CommandBuilder](#commandbuilder)

- [Leaderboard](#leaderboard)
  - [LeaderboardViewModel](#leaderboardviewmodel)
  - [LeaderboardEntryView](#leaderboardentryview)

- [StateMachine](#statemachine)

- [ObjectPool](#objectpool)

- [Factory](#factory)

- [NameFactory](#namefactory)

# About
>[!IMPORTANT]
>Under active development

CozyColdEnvironments is a game development framework that enables developers to focus more on the game itself rather than on the design of discrete, commonly recurring game components

# Setup
Upon installation of all dependencies, the project should compile successfully. Following a successful build, the menu items illustrated in the screenshot will become available. Select Add Define Symbols to register all installed plugins; this action automatically integrates them into the codebase based on their definitions.
Additionally, the Debug: Enable option adds the CC_DEBUG_ENABLED preprocessor symbol. This activates logging mechanisms and supplementary diagnostic utilities designed to facilitate debugging. It is imperative to disable this mode prior to release; failure to do so may result in significant per-frame memory allocations and severe performance degradation.

Within the game's entry point, you must invoke CC.Install(). If necessary, pass the names of specific assemblies (fullname or partially with '*' in the end of name) to which attribute-based functionalities (such as OnInstallExecuteAttribute or SerializationDescriptorAttribute) should be applied. The correct operation of the entire project relies on CC.Install(). This method must be executed on the main thread as early as possible, ideally within the first user-defined script.

<img width="335" height="92" alt="изображение" src="https://github.com/user-attachments/assets/78cf7c28-cc06-433c-b312-fbcf28166c70" />

# Dependecies
- [UniTask](https://github.com/Cysharp/UniTask) [Optional]
- [R3](https://github.com/Cysharp/R3)
- [ZLinq](https://github.com/Cysharp/ZLinq) [Optional] 
- [ObservableCollections](https://github.com/Cysharp/ObservableCollections)
- [ObservableCollections.R3](https://www.nuget.org/packages/ObservableCollections.R3)
- [SuperLinq](https://github.com/viceroypenguin/SuperLinq)
- [Newtonsoft.Json](https://www.newtonsoft.com/json)
- [Humanizer](https://github.com/Humanizr/Humanizer) (Core EN)
- [CommunityToolkit](github.com/CommunityToolkit/dotnet) (Common and Diagnostics modules)
- [ValueTaskSupplement](https://github.com/Cysharp/ValueTaskSupplement)

# Roadmap
- Finalize the new save system.
- Remove obsolete or redundant code segments
- Optimize the GameObjectQuery

# Planned
- Integrate support for various platforms, such as Steam (currently, only partial support for Yandex.Games is implemented).
- Reduce the number of mandatory project dependencies.
- Refactor the framework into a modular architecture.

# Games
## [SlowMoCrusher](tirais14.itch.io/slowmocrusher)
<img width="1600" height="900" alt="GameCover" src="https://github.com/user-attachments/assets/2218be4f-0d53-4e16-8364-c3e82d336e9a" />

# GameObjectQuery
>[!NOTE]
>Currently incurs memory allocations. Plans are in place to minimize these allocations to negligible levels in the near future.

A type-safe, fluent API for querying Unity GameObjects and Components with powerful filtering, scope control, and MVVM-aware helpers.

## Search Scope
```C#
gameObjectQuery.FromSelf()           // Search target itself (default)
gameObjectQuery.FromChildrens()      // Search descendants (recursive by default)
gameObjectQuery.FromParents()        // Search up the hierarchy
gameObjectQuery.NotRecursive()       // Limit to direct children/parents only
gameObjectQuery.ExcludeSelf()        // Don't include the target in results
```

## Filters
```C#
gameObjectQuery.WithName("Player")                    // Partial match by default
gameObjectQuery.WithName("Player", byFullName: true)  // Exact name match
gameObjectQuery.WithName("player", ignoreCase: true)  // Case-insensitive
gameObjectQuery.WithTag("Enemy")                      // Unity tag filter
gameObjectQuery.WithLayerMask(LayerMask.NameToLayer("UI"))
gameObjectQuery.HasComponent<Health>()                // Must have this component
gameObjectQuery.DepthLimiter<RootMarker>()            // Stop search at this component type
```

## Special Queries
```C#
// Get children/parents directly
var childs = gameObject.Q().ChildrenGameObjects();
var parents = gameObject.Q().ParentGameObjects();

// Get root with RootMarker support
var root = gameObject.Q().RootTransform();

// Scene-wide search
var allManagers = GameObjectQuery.Scene
    .HasComponent<GameManager>()
    .Components<GameManager>();
```

## Exmaples
```C#
private void Foo(GameObject go)
{
    go.Q()
        .FromChildrens()
        .ExcludeSelf()
        .IncludeInactive()
        .WithTag("items")
        .WithName("whisky", ignoreCase: true)
        .Component<IItem>()
        .Lax()
        .IfSome(x => x.Drink());
}
```
```C#
private void Foo(GameObject go)
{
    var transforms = go.Q()
        .FromChildrens()
        .IncludeInactive()
        .ExcludeSelf()
        .DepthLimiter<IShowable>()
        .Components<RectTransform>();

    //Locates all RectTransform components within each branch
    //of the hierarchy, terminating the search on branch upon encountering
    //an IShowable interface.

    //This mechanism is particularly useful in scenarios where
    //each IShowable instance requires exclusive control over
    //its descendant RectTransform elements.
}
```

# SavingSystem
>[!WARNING]
>This system is no longer under active development due to fundamental architectural limitations. A more robust, modular save system is being developed as its replacement. Use SavingSystem only for legacy support or
>short-term projects

## Features
- Minimal manual intervention: Register objects and types once; saving/loading is fully automatic. No per-field save logic required in game code
- Auto-restore on registration: When an object is registered, the system checks loaded snapshots and applies matching data immediately
- Unity Inspector integration: Use SavingSystemRegistrationComponent to register objects without code
- Version-tolerant snapshots via SerializationDescriptorAttribute – Snapshots include type descriptors enabling:
  - Polymorphic deserialization (e.g., EnemyBase → BossEnemy)
  - Forward/backward compatibility across save versions
- Scene-aware registration: Objects can be scoped to specific scenes; unloading a scene auto-unregisters its objects
- Reactive state observation: Monitor save/load progress via R3 observables
- Thread-aware async operations: Heavy snapshot work runs on thread pool; Unity interactions marshal back to main thread via UniTask
- In-memory and file-based saves: Support both serialized strings and file I/O.

## Registration API

Register Types (Converters)
Define how objects convert to/from snapshots:
```C#
// Generic overload
SavingSystem.Instance.RegisterType<Player>(
    player => new PlayerSnapshot(player)
);

// Non-generic overload
SavingSystem.Instance.RegisterType(
    typeof(Enemy),
    obj => new EnemySnapshot((Enemy)obj)
);
```

Register Instances
```C#
// Simple string key
SavingSystem.Instance.RegisterObject(player, "Player_01");

// Key selector function
SavingSystem.Instance.RegisterObject(
    enemy, 
    e => $"Enemy_{e.EnemyId}_{e.SpawnPoint}"
);

// With stateful key factory
SavingSystem.Instance.RegisterObject(
    item,
    itemState,
    (it, state) => $"Item_{it.Id}_{state.Rarity}"
);

// Unity objects (auto-key via PersistentGuid/RuntimeId/hierarchy path)
SavingSystem.Instance.RegisterUnityObject(myGameObject);
SavingSystem.Instance.RegisterUnityObject(myComponent);
```

Unregister
```C#
// Single object
SavingSystem.Instance.UnregisterObject(player);

// All objects (e.g., on game reset)
SavingSystem.Instance.UnregisterAll();

// Type + all its instances
SavingSystem.Instance.UnregisterType<Player>();
```

Save/Load Workflow
```C#
// 1. Register types (once, at startup)
SavingSystem.Instance.RegisterType<Player>(p => new PlayerSnapshot(p));

// 2. Register objects (when they spawn)
SavingSystem.Instance.RegisterObject(player, "Player_01");

// 3. Save (anytime)
await SavingSystem.Instance.SaveInFileAsync("saves/latest.json");

// 4. Load (auto-applies to already-registered objects)
await SavingSystem.Instance.LoadFromFileAsync("saves/latest.json");

// 5. Register new object AFTER load → auto-restores if snapshot exists
SavingSystem.Instance.RegisterObject(newEnemy, "Enemy_Boss"); // state applied immediately
```

>[!IMPORTANT]
> - Registration order matters: Objects registered after LoadFromFileAsync will auto-restore if a matching snapshot exists. Objects registered before load will be captured during the next save.
> - Keys must be unique per (Type, SceneInfo): Duplicate keys throw InvalidOperationException.
> - Unity objects require stable identifiers: Use PersistentGuid or RuntimeId components to ensure keys survive scene reloads.
> - Cancellation support: All async methods accept CancellationToken; internally linked to SavingSystem's lifetime.
> - Thread safety: Registration/unregistration is main-thread only (Unity constraint). Snapshot serialization runs off-main-thread where possible.

# CommandScheduler
A frame-based, thread-safe command execution scheduler with reactive state observation and automatic garbage collection.

## Features
- Frame-based execution: Commands are processed on frame ticks via IFrameRunnerWorkItem. Integrates with any FrameProvider or runs manually via OnFrame()
- Thread-safe scheduling: Schedule() can be called from any thread. Internal state uses ConcurrentQueue, ConcurrentDictionary, and lock(SyncRoot) for safe concurrent access
- Signature-based command cancellation: Scheduling a new command with an existing CommandSignature and IsSingle == true automatically cancels pending duplicates, preventing redundant work
- Dual-mode garbage collection: Lazy cleanup on dequeue + optional periodic bulk cleanup. Tunable via GarbageCollectEveryFrame, GarbageThreshold, and GargabeCommandCountThreshold
- Reactive state observation: Monitor scheduling and lifecycle events via R3 observables: ObserveScheduleCommand(), ObserveIsRunningStarted(), ObserveEnabled(), etc
- Sync/async command support: Executes both ICommand.Execute() and ICommandAsync.ExecuteAsync() uniformly
- Lifecycle control: Explicit Enable(), Disable(), Reset(), and Dispose() methods for precise runtime management
- Configurable idle behavior: DelayFrameCountBeforeRunningFinished controls how long IsRunning stays true after the queue empties, useful for UI feedback or transition logic
- Diagnostic logging: Built-in debug output via CCDebug.Instance when enabled, with command names and lifecycle events.

## Properties
- bool IsEnabled - Scheduler active state
- bool IsRunning - Currently executing commands
- bool HasCommands - Queue or current command exists
- string Name - Optional identifier for logging
- object SyncRoot - Lock object for external synchronization
   
GC tuning
- long GarbageCollectEveryFrame - Default: 60
- float GarbageThreshold: Default - 0.35f, range [0.1, 1.1]
- int GargabeCommandCountThreshold: Default - 32, min: 8
   
Delay before IsRunning flips to false after queue empty
- int DelayFrameCountBeforeRunningFinished - Default: 0

>[!IMPORTANT]
> - A command must be valid (IsValid == true) and not completed (IsDone == false) at the time of Schedule().
> - After execution or cancellation, commands are recycled via Utilizable.TryUtilizeOrDispose().
> - QueueCommand is an internal pooled wrapper — do not use directly.
> -For debugging, enable CCDebug.Instance.IsEnabled — the scheduler will log key lifecycle events.

# Command
Implementation of the Command pattern with async/await support, cancellation, status observation, and lifecycle management

Features
- Status Machine - None → Running → Completed / Canceled / Faulted
- Cancellation - Built-in CancellationToken support with automatic linking
- Observability - Reactive status stream via R3.Observable<CommandStatus>
- Safety -ThrowIfDisposed(), Guard.* checks, protection against re-entry
- CRTP - Generic template CommandBase<TThis> for fluent interface

CommandStatus values
- None — Command not started or reset
- Completed — Successfully finished
- Canceled — Cancelled by user or token
- Faulted — Finished with error

## API
- Name: Command identifier (default: type name)
- Status: Current state
- IsDone, IsRunning, IsCancelled: State flags
- CancellationToken: Cancellation token for execution
- Execute/ExecuteAsync: Entry point (overridden in derived classes)
- TryReset: Reset to initial state
- AttachExternalCancellationToken: Link external cancellation token if it CanBeCancelled

>[!IMPORTANT]
> - Single Execution: Re-calling Execute() while IsRunning == true throws InvalidOperationException.
> - Dispose: Command implements IDisposable. All operations are forbidden after Dispose().
> - IsResetable: Set in constructor. If false, Reset() throws an exception.
> - Exceptions: OperationCanceledException and TaskCanceledException are treated as cancellation; all others result in Faulted status.
> - Thread Safety: Status updates are atomic via ReactiveProperty, but OnExecute* logic must be thread-safe if used externally.

## Exapmles

```C#
public class PrintCommand : Command
{
    private readonly string _message;
    
    public PrintCommand(string message) => _message = message;
    
    protected override void OnExecute()
    {
        Console.WriteLine(_message);
    }
}
```
```C#
public class FetchCommand : CommandAsync
{
    protected override async ValueTask OnExecuteAsync(CancellationToken ct)
    {
        await Task.Delay(1000, ct);
    }
}
```
## Command.Builder
Command.Builder returns a structure designed for fluent runtime command construction. It provides strict type safety and supports the implementation of all core methods defined in the Command interface.

Execution Modes
The builder supports two distinct finalization strategies for the constructed command:
1. Pooled Instance

- Behavior: The command instance is retrieved from an internal object pool.
- Lifecycle: Upon completion of execution within the CommandScheduler, the instance is automatically returned to the pool for reuse.
- Use Case: Optimized for high-frequency scenarios to reduce garbage collection overhead.

2. New Instance

- Behavior: A fresh command instance is allocated.
- Lifecycle: Once execution is complete, the Dispose method is explicitly invoked on the command instance.
- Use Case: Suitable for commands requiring unique state isolation or non-poolable resources.

Key Features
- Fluent Interface: Enables readable, chainable command definition at runtime.
- Strict Typing: Ensures compile-time safety for command parameters and return types.
- Resource Management: Automatic handling of instance lifecycle based on the selected mode (pool return vs. disposal).

### Examples
```C#
Command.Builder.WithName("Activate Item Cannon")
    .SetSingle()
    .WithoutState()
    .Asyncronously()
    .WithExecuteAction(
    static async (cancellationToken) =>
    {
        await UniTask.DelayFrame(
            120,
            cancellationToken: cancellationToken
            );

        GameObjectQuery.Scene.IncludeInactive()
            .Component<ItemCannon>()
            .Strict()
            .enabled = true;
    })
    .BuildPooled()
    .Value
    .AttachExternalCancellationToken(destroyCancellationToken)
    .ScheduleBy(G.LoadingCommands);
```
```C#
string cmdName = NameFactory.CreateFromCaller(
    this,
    nameof(GetSaveDataFromFileAsync),
    expirationTimeRelativeToNow: TimeSpan.Zero
    );

var result = new ValueReference<SaveData?>();

await Command.Builder.WithName(cmdName)
    .WithState((@this: this, configureAwait, result))
    .Asynchronously()
    .WithExecuteAction(
    static async (args, cancellationToken) =>
    {
        args.result = await args.@this.GetSaveDataFromFileAsyncCore(
            args.configureAwait,
            cancellationToken
            );
    })
    .BuildPooled()
    .Value
    .AttachExternalCancellationToken(cancellationToken)
    .ScheduleBy(SaveSystem.CommandScheduler)
    .ObserveIsDone()
    .FirstAsync(cancellationToken);
```

# Leaderboard

A reactive, MVVM-based leaderboard system for Unity with automatic sorting, position tracking, multi-score records, and UI synchronization via R3 observables and ObservableCollections

## Features
- Reactive data flow - All state changes (Score, Position, ScoreRecords) propagate via R3 Observable<T>, enabling zero-boilerplate UI updates
- Automatic sorting & position tracking - Entries are sorted on score change; EntryPositions dictionary updates 1-based ranks automatically
- Multi-score records per entry - Each entry tracks named sub-scores ("kills", "deaths", "time") that sum to total Score
- pecial entry highlighting - Pin a IUserProfile to track its entry separately, useful for "player's rank" display
- MVVM UI integration - LeaderboardView/LeaderboardEntryView use ViewModel bindings with automatic prefab instantiation, sorting, and visibility culling
- Frame-throttled updates - Sorting and UI reordering use ThrottleLastFrame(1) and UniTask.WaitForEndOfFrame to batch changes and avoid per-frame allocations
- Memory-safe disposal - All subscriptions, commands, and Unity objects are tracked via CancellationTokenSource and List<IDisposable>, preventing leaks on scene unload
- Customizable sorting – Provide any IComparer<ILeaderboardEntry> for complex ranking logic (e.g., K/D ratio, time-based decay)

### LeaderboardViewModel 
Configure the leaderboard UI behavior and visual elements in the Inspector.

<img width="407" height="147" alt="изображение" src="https://github.com/user-attachments/assets/f884783a-975b-49b7-ac60-ee31bee43a1a" />

- Entry Prefab - The prefab to instantiate for regular leaderboard entries. Must contain a LeaderboardEntryView component
- [Optional] Special Entry Prefab - The prefab to instantiate for the special/highlighted entry (e.g., current player's rank). Must contain a LeaderboardEntryView
component
- Entry Views Root - The parent transform where entry prefabs will be instantiated as children
- Sorting Enabled - Controls whether entries are automatically sorted by score and reordered in the UI
- Max Visible Count - Maximum number of entries to display simultaneously. Entries beyond this limit are hidden (not destroyed).
  
### LeaderboardEntryView
The LeaderboardEntryView component provides Unity Inspector bindings to connect UI elements with leaderboard entry data. Configure these fields in the Inspector to wire up your UI prefab

<img width="824" height="378" alt="изображение" src="https://github.com/user-attachments/assets/a821e355-cbda-419a-8d78-58b97220cbb1" />

Serialized Fields
- Score Record Views - A key-value collection that maps score record names to their UI text components.
  - Key (string): The name of the score record as defined in LeaderboardEntry.ScoreRecords (e.g., "kills", "deaths", "time")
  - Value (TMP_Text): The TextMeshPro component that displays the record's value

- [Optional] Profile Icon View - Unity UI Image component that displays the profile's avatar/icon sprite
- [Optional] Profile Name View - TextMeshPro component that displays the profile's name from IUserProfile.Name.
- [Optional] Position View - TextMeshPro component that displays the entry's current rank/position in the leaderboard

## Example
```C#
private void Foo(LeaderboardView lboardView)
{
    //UserProfile is IDisposable. Already binded to view. Do not call Dispose manually
    var lboard = lboardView.GetModel<Leaderboard>();

    //UserProfile is IDisposable. In real code you must resolve where it is should dispose
    using var playerProfile = new UserProfile("Tirais", "448d3cf7-8133-4326-b7e5-cac543082ebe");

    //Now the special entry will be display a player profile in the SpecialEntry Property
    lboard.SpecialProfile = playerProfile;

    //UserProfile is IDisposable. In real code you must resolve where it is should dispose
    using var entry = new LeaderboardEntry(playerProfile);

    //Thus names will be used in the inspectors LeaderboardEntryView ScoreRecordViews field
    entry.AddScoreRecord("kills").AddScoreRecord("deaths");

    entry.AddScore("kills", 1);
}
```

>[!IMPORTANT]
> - Duplicate record names – AddScoreRecord() throws ArgumentException if name already exists
> - Null profile/entry – Add(null) throws ArgumentNullException via CC.Guard
> - Invalid comparer – Setting Comparer = null is allowed; defaults to IComparable<ILeaderboardEntry>.Default
> - Async cancellation – All UniTask operations respect CancellationToken; ObjectDisposedException thrown if used after Dispose()

# StateMachine 

>[!WARNING]
>Waiting for refactoring

A Unity-integrated, type-safe state machine implementation that manages IState instances with automatic lifecycle callbacks tied to Unity's update loop.

## Features
- Unity lifecycle integration - Active state receives Tick()/FixedTick()/LateTick() automatically mapped from Update()/FixedUpdate()/LateUpdate()
- Idle state fallback - Abstract CreateIdleState() ensures a default state; SetIdle() restores it. Automatically activated on OnDisable()
- Type-safe state queries - IsPlaying<T>(), IsPlaying(Type), and IsPlaying(IState) enable compile-time and runtime state checks
- Transition deduplication - SetState() skips no-op transitions when the new state equals the current one (reference equality)

## IState
```C#
public interface IState
{
    void Enter();        // Called when state becomes active
    void Tick();         // Called every Update frame
    void FixedTick();    // Called every FixedUpdate frame  
    void LateTick();     // Called every LateUpdate frame
    void Exit();         // Called when state is deactivated
}
```
## Example
```C#
using CCEnvs.Collections;
using CCEnvs.Patterns.States;
using CCEnvs.Unity;
using CCEnvs.Unity.UI;
using System.Collections.Generic;
using Zenject;

#nullable enable
namespace Game.States
{
    public sealed class GameStateMachine : AStateMachine
    {
        private GameIdleState? idleState = null!;

        private GameUIState uiState = null!;

        private GameGameplayState? gameplayState = null!;

        private List<IShowable> activeGUITabs = null!;

        [Inject]
        private void Construct(
            [InjectOptional] GameIdleState? idleState,
            GameUIState uiState,
            [InjectOptional] GameGameplayState? gameplayState,
            List<IShowable> activeGUITabs
            )
        {
            this.idleState = idleState;
            this.uiState = uiState;
            this.gameplayState = gameplayState;
            this.activeGUITabs = activeGUITabs;
        }

        protected override void Update()
        {
            base.Update();

            bool hasActiveGUITabs = activeGUITabs.IsNotNullOrEmpty();
            bool isGameplayScene = SceneManagerHelper.ActiveSceneInfo.Name == G.GAMEPLAY_SCENE_NAME;

            if (isGameplayScene && !hasActiveGUITabs)
                SetState(gameplayState);
            else if (hasActiveGUITabs)
                SetState(uiState);
            else
                SetIdle();
        }

        protected override IState? CreateIdleState() => idleState;
    }
}

```

>[!IMPORTANT]
> - Main-thread only - All state transitions and callbacks must occur on Unity's main thread. Do not call SetState() from background threads
> - OnDisable auto-idle - When the MonoBehaviour is disabled, SetIdle() is called automatically. Ensure Exit() cleanup is idempotent
> - Keep Tick()/FixedTick() lightweight – they run every frame. Offload heavy work to coroutines or async methods.

# ObjectPool
A high-performance, thread-safe object pooling system with reactive lifecycle observation, Unity integration, and support for both synchronous and asynchronous object creation.

## Features
- Thread-safe acquisition/return - Uses ConcurrentStack<T> for idle objects and ConcurrentDictionary<T, PooledObject<T>> for active tracking. Safe to call Get()/Return() from multiple threads
- Automatic handle-based return - PooledObject<T> implements IDisposable; returning to pool happens automatically on Dispose()
- IPoolable<T> lifecycle hooks - Objects implementing IPoolable receive OnSpawned()/OnDespawned() callbacks and hold a weak reference to their pool handle
- Unity-aware pooling - When T is a GameObject or Component, pooled objects are automatically deactivated/hidden on return and reactivated on get. Position is reset to (0, -100000) to avoid physics collisions
- Reactive lifecycle observables - Monitor pool activity via R3
- Async pooling support - ObjectPoolAsync<T> supports factories returning ValueTask<T> or UniTask<T> for asynchronous initialization (e.g., loading assets, network setup).

Unity Integration Details
When T is a Unity type (GameObject, Component):
- On Get():
  - gameObject.SetActive(true)
  - Position reset to (0, -100000) (off-screen staging area)
- On Return():
  - gameObject.SetActive(false)
  - Position reset to (0, -100000)
- On Pool Dispose():
  - All inactive objects destroyed via UnityEngine.Object.Destroy()

# Factory
A lightweight, type-safe factory abstraction layer supporting synchronous and asynchronous object creation with variable argument counts, state capture, and conditional async task support (UniTask/ValueTask).

## Features
- Anonymous factory implementation - Factory.Create() returns lightweight AnonymousFactory<T> instances that wrap lambdas, avoiding boilerplate class definitions
- State-captured factories - Bind external state to a factory at creation time for contextual object construction
- Async factory support - Factory.Async.Create() returns factories producing UniTask<T> or ValueTask<T> based on UNITASK_PLUGIN compilation symbol
- Non-generic fallback - IFactory.Create(params object[] args) enables runtime-polymorphic usage (e.g., for DI containers), with explicit casts handled internally

>[!NOTE]
>Prefer strongly-typed generic factories over non-generic params usage

# NameFactory
A lightweight, cached name generation utility that creates unique, human-readable identifiers from objects with automatic memory management via time-based expiration.
Ideal to use with a Command.Builder

## Features
- Automatic caching: Generated names are cached by (Type, CallerHash) to avoid repeated string construction. Cache entries expire after a configurable duration (default: 5 minutes)
- Hash-based disambiguation: When addHashToId is true, the caller's hash code is appended to the identifier, ensuring uniqueness across instances of the same type
- Flexible identifier handling: Accepts Identifier? (a lightweight wrapper for string/number IDs) with optional hash injection
- Null-safe fallback – If caller is null, returns a simple formatted string without caching
- Thread-safe lazy initialization: The internal cache is initialized on first use via Lazy<T>, ensuring safe concurrent access without startup overhead

Return Value: a formatted string: "{CallerType}.{body} - {id}", where {id} may include an injected hash.

>[!NOTE]
> - Cache key: (Type Type, int CallerHash) – ensures names are unique per runtime instance, not just type.
> - Memory safety: Default 5-minute expiration prevents unbounded growth; adjust based on object lifetime.
> - Hash stability: Uses GetHashCode() – ensure overridden types provide consistent hashes during their lifetime.
> - Identifier composition: When addHashToId = true, the hash is appended via Identifier.WithNumber(), preserving original ID semantics
