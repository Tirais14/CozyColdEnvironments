using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.Unity.EditorC
{
    //[CustomPropertyDrawer(typeof(SerializedDictionary<,>))]
    public sealed class SerializedDictionaryDrawer : CCPropertyDrawer
    {
        private ListView itemsView = null!;
        private PropertyField newItemView = null!;
        private SerializedPropertyListAdapter items = null!;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            base.CreatePropertyGUI(property);

            var newItemProp = property.FindPropertyRelative("newItem");
            var itemsProp = property.FindPropertyRelative("items");

            CreateNewItemView(newItemProp);
            CreateAddButton();
            CreateItemsView(itemsProp);

            return root;
        }

        private void CreateNewItemView(SerializedProperty? newItemProp)
        {
            if (newItemProp == null)
                return;

            newItemView = new PropertyField(newItemProp);

            root.Add(newItemView);
        }

        private void CreateAddButton()
        {
            var addBtn = new Button(() =>
            {
                items.Property.arraySize++;
                items[items.Property.arraySize - 1] = newItemView;
            })
            {
                name = "Add",
                text = "Add",
            };

            root.Add(addBtn);
        }

        private void CreateItemsView(SerializedProperty? itemsProp)
        {
            if (itemsProp == null || !itemsProp.isArray)
                return;

            items = new SerializedPropertyListAdapter(itemsProp);

            itemsView = new ListView
            {
                itemsSource = items,
                reorderable = true,
                showAddRemoveFooter = false,
                showFoldoutHeader = true,
                headerTitle = "Items",
                showBoundCollectionSize = false,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                //makeItem = () => new PropertyField(),
                //bindItem = (element, index) =>
                //{
                //    var propField = (PropertyField)element;
                //    var elementProp = itemsProp.GetArrayElementAtIndex(index);
                //    propField.BindProperty(elementProp);
                //    propField.label = $"Element {index}";
                //}
            };

            root.TrackPropertyValue(itemsProp, (sp) => itemsView.Rebuild());

            root.Add(itemsView);
        }
    }
}
