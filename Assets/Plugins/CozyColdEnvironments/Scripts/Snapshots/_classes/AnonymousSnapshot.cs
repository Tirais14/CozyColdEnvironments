#nullable enable
using CCEnvs.Attributes.Serialization;
using CCEnvs.Caching;
using CCEnvs.Pools;
using CCEnvs.Reflection;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CCEnvs.Snapshots
{
    [Serializable, SerializationDescriptor("AnonymousSnapshot", "2aa9b2cb-9292-420f-ad60-ad002ba80efa")]
    public record AnonymousSnapshot : Snapshot<object>
    {
        private readonly SnapshotProperty[] props;

        private readonly ISnapshot[] compositeParts;

        public IReadOnlyList<SnapshotProperty> Properties => props;

        public IReadOnlyList<ISnapshot> CompositeParts => compositeParts;

        public AnonymousSnapshot(object target)
        {
            CC.Guard.IsNotNullTarget(target);

            CaptureFrom(target);
        }

        public AnonymousSnapshot(
            SnapshotProperty[] props,
            ISnapshot[]? compositeParts
            )
        {
            CC.Guard.IsNotNull(props, nameof(props));

            this.props = props;
            this.compositeParts = compositeParts ?? Array.Empty<ISnapshot>();
        }

        protected override void OnCapture(object target)
        {
            base.OnCapture(target);

            for (int i = 0; i < compositeParts.Length; i++)
                compositeParts[i].Reset().CaptureFrom(target);

            for (int i = 0; i < props.Length; i++)
                props[i].CaptureValueFrom(target);
        }

        protected override void OnRestore(ref object target)
        {
            for (int i = 0; i < compositeParts.Length; i++)
            {
                try
                {
                    compositeParts[i].TryRestore(target, out _);
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }
            }

            SnapshotProperty prop;

            for (int i = 0; i < this.props.Length; i++)
            {
                prop = this.props[i];

                if (prop.CapturedValue.IsNull())
                    continue;

                prop.SetValue(target, prop.CapturedValue);
            }
        }

        protected override void OnReset()
        {
            base.OnReset();

            for (int i = 0; i < compositeParts.Length; i++)
                compositeParts[i].Reset();

            for (int i = 0; i < props.Length; i++)
                props[i].ResetCapturedValue();
        }

        public static class PropertyResolver
        {
            private static Cache<Type, (SnapshotProperty[] Props, ISnapshot[] CompositeParts)> resolveMembersCache = new()
            {
                ExpirationScanFrequency = 30.Seconds()
            };

            //public static (SnapshotProperty[] Props, ISnapshot[] CompositeParts) ResolveMembers(object target)
            //{
            //    CC.Guard.IsNotNullTarget(target);

            //    var type = target.GetType();

            //    var members = type.FindMembers(
            //        MemberTypes.Field | MemberTypes.Property,
            //        BindingFlagsDefault.InstanceAll,
            //        (member, _) =>
            //        {
            //            return member.IsDefined<SnapshotPropertyAttribute>(inherit: true);
            //        },
            //        null
            //        );

            //    var props = new SnapshotProperty[members.Length];

            //    MemberInfo member;

            //    for (int i = 0; i < members.Length; i++)
            //    {
            //        member = members[i];

            //        if (member is FieldInfo field)
            //            props[i] = new SnapshotProperty(field);
            //        else
            //            props[i] = new SnapshotProperty((PropertyInfo)member);
            //    }

            //    using var compositeParts = ListPool<ISnapshot>.Shared.Get();

            //    if (type.GetCustomAttribute<SnapshotConvertibleAttribute>(inherit: true)
            //        .IsNotNull(out var snapshotConvertibleAttribute)
            //        )
            //    {
            //        ConstructorInfo ctor;

            //        ISnapshot compositePart;

            //        object[] buffer = new object[1];

            //        foreach (var compositePartType in snapshotConvertibleAttribute.CompositeParts)
            //        {
            //            try
            //            {
            //                ctor = GetConstructor(compositePartType);

            //                buffer[0] = target;

            //                compositePart = (ISnapshot)ctor.Invoke(buffer);
            //            }
            //            catch (Exception ex)
            //            {
            //                typeof(PropertyResolver).PrintException(ex);
            //            }
            //        }
            //    }
            //}
        }
    }
}
