using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using ShowBasesOnMap.Compatibility.WardIsLove;

namespace ShowBasesOnMap;

[BepInPlugin(ModGUID, ModName, ModVersion)]
public class Plugin : BaseUnityPlugin
{
    private const string ModName = "ShowBasesOnMap",
        ModAuthor = "Frogger",
        ModVersion = "1.0.0",
        ModGUID = $"com.{ModAuthor}.{ModName}";

    private ConfigEntry<bool> isAdminOnly;
    public static bool calculating = false;

    private void Awake()
    {
        CreateMod(this, ModName, ModAuthor, ModVersion);
        isAdminOnly = mod.config("General", "IsAdminOnly", true, "Allows only admins to see this pins");

        StartUpdating();
    }

    private async void StartUpdating()
    {
        var oldPins = new List<PinData>();
        while (true)
        {
            await Task.Delay(6000);
            await UpdateWatchObjectsOnMap(oldPins);
        }
    }

    private async Task UpdateWatchObjectsOnMap(List<PinData> oldPins)
    {
        if (!ZNet.instance) return;
        if (!ZNetScene.instance) return;
        if (!ZoneSystem.instance) return;
        var minimap = Minimap.instance;
        if (!minimap) return;
        calculating = true;
        if (mod.IsAdmin || !isAdminOnly.Value)
        {
            var newPins = new List<PinData>();
            foreach (var (prefabName, icon, radius, localizeKey, isPrivateArea) in WatchObject.all)
            {
                var result = await ZoneSystem.instance.GetWorldObjectsInAreaAsync(prefabName);
                if (result.Count == 0) continue;
                foreach (var zdo in result)
                {
                    var position = zdo.GetPosition();
                    var pin = new PinData();
                    pin.m_type = PinType.None;
                    pin.m_animate = false;
                    pin.m_checked = false;
                    pin.m_icon = icon;
                    pin.m_pos = position;
                    pin.m_save = false;
                    pin.m_name = "";
                    // if (isPrivateArea)
                    // {
                    //     if (prefabName == "Thorward")
                    //     {
                    //         pin.m_name = ;
                    //         Piece
                    //     }
                    //
                    // }

                    pin.m_NamePinData = new(pin);
                    //TODO: show range of wards
                    //TODO: show ward owner name

                    newPins.Add(pin);
                }
            }

            for (var i = 0; i < newPins.Count; i++) minimap.m_pins.Add(newPins[i]);
        }

        calculating = false;
        for (var i = 0; i < oldPins.Count; i++) minimap.DestroyPinMarker(oldPins[i]);
        oldPins.Clear();
    }
}