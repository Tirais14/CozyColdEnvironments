using CCEnvs.Unity.Serialization;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.Unity.EditorC
{
    [CustomPropertyDrawer(typeof(SerializedDictionary<,>))]
    public sealed class SerializedDictionaryDrawer : CCPropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = base.CreatePropertyGUI(property);

            var newItemProp = property.FindPropertyRelative("New Item");
            var itemsProp = property.FindPropertyRelative("Items");

            CreateNewItemView(newItemProp);
            CreateAddButton();
            CreateItemsView(itemsProp);

            return root;
        }

        private void CreateNewItemView(SerializedProperty? newItemProp)
        {
            if (newItemProp == null)
                return;

            var newItemView = new ObjectField
            {
                name = "New Item"
            };

            root.Add(newItemView);
        }

        private void CreateAddButton()
        {
            var addBtn = new Button
            {
                name = "Add"
            };

            root.Add(addBtn);
        }

        private void CreateItemsView(SerializedProperty? itemsProp)
        {
            if (itemsProp == null || !itemsProp.isArray)
                return;

            var items = Enumerable.Range(0, itemsProp.arraySize)
                .Select(i =>
                {
                    return itemsProp.GetArrayElementAtIndex(i);
                })
                .ToArray();

            var itemsView = new ListView
            {
                itemsSource = items,
                reorderable = true, // Включаем перетаскивание
                showAddRemoveFooter = true, // Кнопки +/-
                showFoldoutHeader = true,
                headerTitle = "Items",
                showBoundCollectionSize = false,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                makeItem = () => new PropertyField(), // Шаблон для элемента
                bindItem = (element, index) =>
                {
                    var propField = (PropertyField)element;
                    var elementProp = itemsProp.GetArrayElementAtIndex(index);
                    propField.BindProperty(elementProp);
                    propField.label = $"Element {index}";
                }
            };

            root.TrackPropertyValue(itemsProp, (sp) => itemsView.Rebuild());

            root.Add(itemsView);
        }
    }
}
