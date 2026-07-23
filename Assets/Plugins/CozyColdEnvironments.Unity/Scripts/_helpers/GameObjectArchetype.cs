using CCEnvs.Pools;
using CCEnvs.TypeMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX
{
    public readonly struct GameObjectArchetype
    {
        public string? Tag { get; }

        public int Layer { get; }

        public IReadOnlyList<Type> ComponentTypes { get; }

        public GameObjectArchetype(
            IEnumerable<Type> componentTypes,
            string? tag = null,
            int layer = 0
            )
        {
            Tag = tag;
            Layer = layer;
            ComponentTypes = componentTypes.ToArray();
        }

        public static GameObjectArchetype Create(GameObject gameObject)
        {
            var components = gameObject.GetComponents<Component>();

            using var componentTypes = new PooledList<Type>(components.Length);

            foreach (var component in components)
            {
                if (component.Is<Transform>())
                    continue;

                componentTypes.Add(component.GetType());
            }

            return new GameObjectArchetype(
                componentTypes,
                gameObject.tag,
                gameObject.layer
                );
        }

        //public static IReadOnlyList<Type> OrderComponentTypesByDependencies(IEnumerable<Type> componentTypes)
        //{
        //    CC.Guard.IsNotNull(componentTypes, nameof(componentTypes));

        //    var typesWithoutDependencies = new List<Type>(32);
        //    var typesWithDependecies = new List<(Type Value, IList<Type> Dependencies)>(32);
        //    var componentDependecies = new List<Type>(128);

        //    foreach (var componentType in componentTypes)
        //    {


        //        if (componentType.GetCustomAttribute<RequireComponent>().IsNotNull(out var requireComponentAttribute))
        //            componentDependecies.AddRange(requireComponentAttribute.AsEnumerable());

        //        componentType.FindMembers(
        //            MemberTypes.Field,
        //            BindingFlagsDefault.InstanceAll,
        //            (member, _) =>
        //            {
        //                return member.IsDefined<GetBySelfAttribute>();
        //            },
        //            null
        //            )
        //            .OfType<FieldInfo>()
        //            .Select(field => field.FieldType);

        //        componentDependecies.Clear();
        //    }
        //}
    }
}
