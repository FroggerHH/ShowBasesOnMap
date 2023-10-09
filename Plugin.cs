using System.Threading.Tasks;
using BepInEx;

namespace ShowBasesOnMap;

[BepInPlugin(ModGUID, ModName, ModVersion)]
public class Plugin : BaseUnityPlugin
{
    private const string ModName = "ShowBasesOnMap",
        ModAuthor = "Frogger",
        ModVersion = "1.0.0",
        ModGUID = $"com.{ModAuthor}.{ModName}";

    private void Awake()
    {
        CreateMod(this, ModName, ModAuthor, ModVersion);

        StartUpdating();
    }

    private async void StartUpdating()
    {
        while (true)
            await UpdateWatchObjectsOnMap();
    }

    private async Task UpdateWatchObjectsOnMap()
    {
        if (!ZNet.instance) return;
        if (!Minimap.instance) return;
        if (!ZNetScene.instance) return;
        if (!ZoneSystem.instance) return;

        foreach (var (prefabName, icon, radius, localizeKey) in WatchObject.all)
        {
            var result = await ZoneSystem.instance.GetWorldObjectsInAreaAsync(prefabName);
            if (result.Count == 0) continue;
            
            //TODO: display on map as pings
        }

        await Task.Delay(1000);
    }
}