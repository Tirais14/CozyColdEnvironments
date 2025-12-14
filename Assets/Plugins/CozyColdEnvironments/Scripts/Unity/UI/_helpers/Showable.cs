using CCEnvs.Diagnostics;
using CCEnvs.Linq;
using SuperLinq;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using R3;
using UnityEngine;
using UnityEngine.UI;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.UI
{
    //public static class Showable
    //{
    //    public static void Show(GameObject gameObject,
    //        ICollection<GraphicStateSnaphsot> graphicStates,
    //        ICollection<ShowableStateSnapshot> showableStates)
    //    {
    //        CC.Guard.IsNotNull(gameObject, nameof(gameObject));

    //        if (isTransitioning)
    //            return;

    //        //gameObject.SetActive(true);
    //        //return;

    //        foreach (var state in graphicStates.ZL()
    //            .Where(state => state.Target != null))
    //        {
    //            state.Restore();
    //        }
    //        graphicStates.Clear();

    //        foreach (var state in showableStates)
    //        {
    //            if (state.gameObject.Raw != gameObject)
    //                state.Restore();
    //        }
    //        showableStates.Clear();
    //    }

    //    public static void Hide(GameObject gameObject,
    //        ICollection<GraphicStateSnaphsot> graphicStates,
    //        ICollection<ShowableStateSnapshot> showableStates,
    //        ShowableSettings settings = ShowableSettings.Default)
    //    {
    //        CC.Guard.IsNotNull(gameObject, nameof(gameObject));
    //        CC.Guard.IsNotNull(graphicStates, nameof(graphicStates));

    //        //gameObject.SetActive(false);
    //        //return;

    //        bool hideByColor = settings.IsFlagSetted(ShowableSettings.HideByColor);
    //        bool keepRaycastTargetState = settings.IsFlagSetted(ShowableSettings.KeepRaycastTargetState);
    //        bool hideByState = !hideByColor;

    //        if (graphicStates.IsNotEmpty())
    //            typeof(Showable).PrintWarning($"{nameof(graphicStates)} is not empty.");

    //        //var t_ = gameObject.QueryTo()
    //        //                  .ByChildren()
    //        //                  .IncludeInactive()
    //        //                  .Components<Graphic>().ToArray();

    //        //var t = gameObject.QueryTo()
    //        //                  .ByChildren()
    //        //                  .IncludeInactive()
    //        //                  .DepthLimiter<IShowable>()
    //        //                  .Components<Graphic>().ToArray();

    //        //_ = t;

    //        //var t1 = t.ZL().Where(cmp => cmp.color.a.NotNearlyEquals(0f) && cmp.enabled).ToArray();

    //        //_ = t;

    //        foreach (var graphic in gameObject.QueryTo()
    //                                          .ByChildren()
    //                                          .IncludeInactive()
    //                                          .DepthLimiter<IShowable>()
    //                                          .Components<Graphic>()
    //                                          .ZL()
    //                                          .Where(cmp => cmp.color.a.NotNearlyEquals(0f) && cmp.enabled))
    //        {
    //            graphicStates!.Add(graphic);

    //            if (hideByState)
    //            {
    //                graphic.enabled = false;
    //                graphic.raycastTarget = false;
    //            }
    //            else
    //            {
    //                if (hideByColor)
    //                    graphic.color = graphic.color.WithAlpha(0f);

    //                if (!keepRaycastTargetState)
    //                    graphic.raycastTarget = false;
    //            }
    //        }

    //        if (showableStates.IsNotEmpty())
    //            typeof(Showable).PrintWarning($"{nameof(showableStates)} is not empty.");

    //        //var t2 = gameObject.Q()
    //        //                   .ByChildren()
    //        //                   .IncludeInactive()
    //        //                   .ExcludeSelf()
    //        //                   .OnlyFirst()
    //        //                   .Components<IShowable>()
    //        //                   .ToArray();

    //        //_ = t;

    //        foreach (var showable in gameObject.Q()
    //                                           .ByChildren()
    //                                           .IncludeInactive()
    //                                           .ExcludeSelf()
    //                                           .Nearest()
    //                                           .Components<IShowable>()
    //                                           .ZL()
    //                                           .Where(x => x.IsShown))
    //        {
    //            showableStates.Add(new ShowableStateSnapshot(showable));
    //            showable.Hide();
    //        }
    //    }
    //}
}
