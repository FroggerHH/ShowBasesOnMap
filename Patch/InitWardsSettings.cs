using ShowBasesOnMap.Compatibility.WardIsLove;
using UnityEngine.SceneManagement;

namespace ShowBasesOnMap.Patch;

[HarmonyPatch] internal class InitWardsSettings
{
    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))] [HarmonyPostfix] [HarmonyWrapSafe]
    internal static void Init(ZNetScene __instance)
    {
        if (SceneManager.GetActiveScene().name != "main") return;
        if (!ZNet.instance.IsServer()) return;

        WatchObject.all = new();

        AddWard("guard_stone");
        AddWard("piece_workbench");
        AddWard("portal_wood");
        AddWard("Cart");
        AddWard("fire_pit");
        AddWardThorward();
    }

    private static void AddWard(string name)
    {
        var prefab = ZNetScene.instance.GetPrefab(name.GetStableHashCode())?.GetComponent<Piece>();
        if (prefab)
        {
            var areaComponent = prefab.GetComponent<PrivateArea>();
            WatchObject.all.Add(new WatchObject(
                name,
                prefab.m_icon,
                areaComponent.m_radius,
                localizeKey: prefab.m_name));
        }
    }

    private static void AddWardThorward()
    {
        var name = "Thorward";
        var prefab = ZNetScene.instance.GetPrefab(name.GetStableHashCode())?.GetComponent<Piece>();
        if (prefab)
            WatchObject.all.Add(new WatchObject(
                name,
                prefab.m_icon,
                radius: WardIsLovePlugin.WardRange().Value,
                localizeKey: prefab.m_name));
    }
}