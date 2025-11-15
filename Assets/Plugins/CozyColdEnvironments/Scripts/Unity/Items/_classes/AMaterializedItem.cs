using UnityEngine;

#nullable enable
#pragma warning disable S4035
namespace CCEnvs.Unity.Items
{
    public abstract class AMaterializedItem : IItem
    {
        protected IItem item { get; set; } = null!;

        public string Name => item.Name;
        public int ID => item.ID;
        public Sprite Icon => item.Icon;
        public int MaxItemCount => item.MaxItemCount;

        public void SetInternalItem(IItem item)
        {
            CC.Guard.IsNotNull(item, nameof(item));

            this.item = item;
            OnSetInternalItem();
        }

        public bool Equals(IItem other)
        {
            return item.Equals(other);
        }

        public override bool Equals(object obj)
        {
            return item.Equals(obj);
        }

        public override int GetHashCode()
        {
            return item.GetHashCode();
        }

        public override string ToString()
        {
            return item.ToString();
        }

        protected abstract void OnSetInternalItem();
    }
}
