using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using Cysharp.Threading.Tasks;
using R3;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Interactables
{
    public abstract class InteractionZone<TAgent> : CCBehaviour, IInteractionZone<TAgent>
        where TAgent : Component
    {
        protected readonly C5.HashedArrayList<GameObject> gameObjects = new();
        protected ReactiveCommand<GameObject>? goEnterSubj;
        protected ReactiveCommand<GameObject>? goStaySubj;
        protected ReactiveCommand<GameObject>? goExitSubj;
        protected ReactiveCommand<Unit>? goDestroySubj;

        [field: SerializeField, GetBySelf]
        public TAgent InteractionAgent { get; private set; } = null!;
        public IEnumerable<GameObject> GameObjects => gameObjects;

        public abstract bool Contains(Vector2 point);
        public abstract bool Contains(Vector3 point);

        protected override void Awake()
        {
            base.Awake();
            UniTask.RunOnThreadPool(async () =>
            {
                while (true)
                {
                    for (int i = 0; i < gameObjects.Count; i++)
                    {
                        if (gameObjects[i] == null)
                            gameObjects.RemoveAt(i);
                    }

                    await UniTask.Yield();
                }
            }).AttachExternalCancellation(destroyCancellationToken)
            .SuppressCancellationThrow()
            .Forget();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            goEnterSubj?.Dispose();
            goStaySubj?.Dispose();
            goExitSubj?.Dispose();
            goDestroySubj?.Dispose();
        }

        public bool Contains(GameObject? gameObject)
        {
            if (gameObject == null)
                return false;

            return gameObjects.Contains(gameObject);
        }

        public bool ContainsComponent(object? component)
        {
            if (component.IsNull())
                return false;

            return gameObjects.SelectMany(go => go.QueryTo().Components(component.GetType()))
                .Any(cmp => cmp.Equals(component));
        }

        public Observable<GameObject> ObserveOnEnter()
        {
            if (goEnterSubj is null)
            {
                goEnterSubj = new ReactiveCommand<GameObject>();
                goEnterSubj.AddTo(destroyCancellationToken);
            }

            return goEnterSubj;
        }

        public Observable<GameObject> ObesrveOnStay()
        {
            if (goStaySubj is null)
            {
                goStaySubj = new ReactiveCommand<GameObject>();
                goStaySubj.AddTo(destroyCancellationToken);
            }

            return goStaySubj;
        }

        /// <summary>
        /// Also triggered when object in zone will be destroyed.
        /// </summary>
        /// <returns></returns>
        public Observable<GameObject> ObserveOnExit()
        {
            if (goExitSubj is null)
            {
                goExitSubj = new ReactiveCommand<GameObject>();
                goExitSubj.AddTo(destroyCancellationToken);
            }

            return goExitSubj;
        }

        protected void OnEnter(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            if (gameObjects.Add(gameObject))
                goEnterSubj?.Execute(gameObject);
        }

        protected void OnStay(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            goStaySubj?.Execute(gameObject);
        }

        protected void OnExit(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            if (gameObjects.Remove(gameObject))
                goExitSubj?.Execute(gameObject);
        }
    }
    public class InteractionZone : InteractionZone<Collider>
    {
        protected override void Start()
        {
            base.Start();
            InteractionAgent.isTrigger = true;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            OnEnter(other.gameObject);
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            OnStay(other.gameObject);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            OnExit(other.gameObject);
        }

        public override bool Contains(Vector3 point)
        {
            return InteractionAgent.bounds.Contains(point);
        }

        public override bool Contains(Vector2 point)
        {
            return Contains((Vector3)point);
        }
    }
}
