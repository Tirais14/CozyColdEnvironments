using CCEnvs.Diagnostics;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Interactables
{
    public abstract class InteractionZone<TAgent> : CCBehaviour, IInteractionZone<TAgent>
        where TAgent : Component
    {
        protected readonly C5.HashedArrayList<GameObject> gameObjects = new();
        protected Subject<GameObject>? goEnterSubj;
        protected Subject<GameObject>? goStaySubj;
        protected Subject<GameObject>? goExitSubj;
        protected Subject<Unit>? goDestroySubj;

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

            return gameObjects.SelectMany(go => go.AppealTo().Components(component.GetType()))
                .Any(cmp => cmp.Equals(component));
        }

        public IObservable<GameObject> ObserveOnEnter()
        {
            if (goEnterSubj is null)
            {
                goEnterSubj = new Subject<GameObject>();
                goEnterSubj.AddTo(this);
            }
            
            return goEnterSubj;
        }

        public IObservable<GameObject> ObesrveOnStay()
        {
            if (goStaySubj is null)
            {
                goStaySubj = new Subject<GameObject>();
                goStaySubj.AddTo(this);
            }

            return goStaySubj;
        }

        /// <summary>
        /// Also triggered when object in zone will be destroyed.
        /// </summary>
        /// <returns></returns>
        public IObservable<GameObject> ObserveOnExit()
        {
            if (goExitSubj is null)
            {
                goExitSubj = new Subject<GameObject>();
                goExitSubj.AddTo(this);
            }

            return goExitSubj;
        }

        protected void OnEnter(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            if (gameObjects.Add(gameObject))
                goEnterSubj?.OnNext(gameObject);
        }

        protected void OnStay(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            goStaySubj?.OnNext(gameObject);
        }

        protected void OnExit(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            if (gameObjects.Remove(gameObject))
                goExitSubj?.OnNext(gameObject);
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
