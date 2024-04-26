using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using JustAFrogger;
using ShowBasesOnMap.Compatibility.WardIsLove;

namespace ShowBasesOnMap;

[BepInPlugin(ModGUID, ModName, ModVersion)]
public class Plugin : BaseUnityPlugin
{
    private const string ModName = "ShowBasesOnMap",
        ModAuthor = "Frogger",
        ModVersion = "1.1.0",
        ModGUID = $"com.{ModAuthor}.{ModName}";

    private static ConfigEntry<bool> isAdminOnly;

    private void Awake()
    {
        CreateMod(this, ModName, ModAuthor, ModVersion, ModGUID);
        isAdminOnly = config("General", "IsAdminOnly", true, "Allows only admins to see this pins");

        StartUpdating();
    }

    static List<PinData> addedPinDatas = new List<PinData>();
    static List<PinData> oldPinDatas = new List<PinData>();

    internal static async void StartUpdating()
    {
        while (true)
        {
            await Task.Delay(1000);
            await UpdateWatchObjectsOnMap();
        }
    }

    private static async Task UpdateWatchObjectsOnMap()
    {
        try
        {
            if (!ZNet.instance) return;
            if (!ZNetScene.instance) return;
            if (!ZoneSystem.instance) return;
            var minimap = Minimap.instance;
            if (!minimap) return;

            if (IsAdmin || !isAdminOnly.Value)
            {
                foreach (var (prefabName, icon, check) in WatchObject.all)
                {
                    var result = await ZoneSystem.instance.GetWorldObjectsAsync(prefabName, check);
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
                        pin.m_NamePinData = new(pin);

                        //TODO: show range of wards 
                        //TODO: show ward owner name
                        addedPinDatas.Add(pin);
                    }
                }
            }

            foreach (var pinData in addedPinDatas) minimap.m_pins.Add(pinData);
            foreach (var pin in oldPinDatas)
            {
                if (pin.m_uiElement != null)
                {
                    Destroy(pin.m_uiElement.gameObject);
                    pin.m_uiElement = null;
                }

                if (pin.m_NamePinData != null)
                {
                    Destroy(pin.m_NamePinData.PinNameGameObject);
                    pin.m_NamePinData.PinNameGameObject = null;
                }

                minimap.m_pins.Remove(pin);
            }

            oldPinDatas.Clear();
            oldPinDatas = new(addedPinDatas);
            addedPinDatas.Clear();
        }
        catch (Exception e)
        {
            if (e.ToString()
                .Contains("System.IndexOutOfRangeException: Index was outside the bounds of the array.")) return;
            DebugError(e);
        }
    }
}