using ShowBasesOnMap.Compatibility.WardIsLove;
using UnityEngine.SceneManagement;

namespace ShowBasesOnMap.Patch;

[HarmonyPatch] public class InitWardsSettings
{
    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))] [HarmonyPostfix] [HarmonyWrapSafe]
    private static void Init(ZNetScene __instance)
    {
        if (SceneManager.GetActiveScene().name != "main") return;
        if (!ZNet.instance.IsServer()) return;

        WatchObject.all = new();

        AddObj("guard_stone", true);
        AddObj("piece_workbench");
        AddObj("portal_wood");
        AddObj("Cart");
        AddObj("fire_pit");
        AddWardThorward();
    }

    public static void AddObj(string name, bool isPrivateArea = false)
    {
        var prefab = ZNetScene.instance.GetPrefab(name.GetStableHashCode())?.GetComponent<Piece>();
        if (!prefab) return;

        var areaComponent = prefab.GetComponent<PrivateArea>();
        WatchObject.all.Add(new WatchObject(
            name,
            prefab.m_icon,
            areaComponent ? areaComponent.m_radius : 0,
            localizeKey: prefab.m_name,
            isPrivateArea: isPrivateArea));
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
                localizeKey: prefab.m_name,
                isPrivateArea: true));
    }
}