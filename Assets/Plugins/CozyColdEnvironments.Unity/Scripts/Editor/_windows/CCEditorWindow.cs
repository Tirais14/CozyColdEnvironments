#if UNITY_EDITOR
using System;
using System.Linq;
using CCEnvs.Reflection;
using SuperLinq;
using UnityEditor;
using UnityEngine.UIElements;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity.Editr
{
    public abstract class CCEditorWindow : EditorWindow
    {
        protected VisualElement root => rootVisualElement;

        public virtual void CreateGUI()
        {
            CreateElements();
            InitElements();
        }

        protected abstract void CreateElements();

        protected virtual IndexValuePair<VisualElement>[] GetSeparators()
        {
            return Array.Empty<IndexValuePair<VisualElement>>();
        }

        private void InitElements()
        {
            foreach (var item in from field in this.Reflect().IncludeNonPublic().Fields()
                                 where field.FieldType.IsType<VisualElement>()
                                 select field.GetValue(this).CastTo<VisualElement>())
            {
                root.Add(item);
            }
        }
    }
}
#endif