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

    private void Awake()
    {
        CreateMod(this, ModName, ModAuthor, ModVersion);
        isAdminOnly = mod.config("General", "IsAdminOnly", true, "Allows only admins to see this pins");

        StartUpdating();
    }

    List<PinData> addedPinDatas = new List<PinData>();

    private async void StartUpdating()
    {
        while (true)
        {
            await Task.Delay(6000);
            await UpdateWatchObjectsOnMap();
        }
    }

    private async Task UpdateWatchObjectsOnMap()
    {
        if (!ZNet.instance) return;
        if (!ZNetScene.instance) return;
        if (!ZoneSystem.instance) return;
        var minimap = Minimap.instance;
        if (!minimap) return;
        foreach (var pin in addedPinDatas)
        {
            pin.m_checked = true;
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

        addedPinDatas.Clear();

        if (mod.IsAdmin || !isAdminOnly.Value)
        {
            foreach (var (prefabName, icon, check) in WatchObject.all)
            {
                var result = await ZoneSystem.instance.GetWorldObjectsInAreaAsync(prefabName, check);
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
    }
}