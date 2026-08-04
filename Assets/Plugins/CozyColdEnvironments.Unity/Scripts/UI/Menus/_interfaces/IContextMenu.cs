using R3;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace CCEnvs.UnityX.UI.Menus
{
    public interface IContextMenu : ICollection<KeyValuePair<string, IContextMenuItem>>
    {
        IContextMenuItem this[string name] { get; set; }

        IEnumerable<string> Names { get; }

        IEnumerable<IContextMenuItem> Items { get; }

        void Add(IContextMenuItem item);

        T GetValue<T>(string name) where T : IContextMenuItem;

        bool ContainsName(string name);

        bool ContainsItem(IContextMenuItem item);

        bool TryGetValue<T>(string name, [NotNullWhen(true)] out T? result) where T : IContextMenuItem;

        bool TryFind(
            string name,
            [NotNullWhen(true)] out IContextMenuItem? result,
            StringMatchSettings matchSettings = StringMatchSettings.Ordinal
            );

        Observable<IContextMenuItem> ObserveAdd();

        Observable<IContextMenuItem> ObserveRemove();

        Observable<PreviousCurrentPair<IContextMenuItem>> ObserveReplace();

        Observable<Unit> ObserveClear();
    }
}
