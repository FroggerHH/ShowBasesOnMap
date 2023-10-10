namespace ShowBasesOnMap;

public record WatchObject(string prefabName, Sprite icon, Func<ZDO, bool> check = null)
{
    public static List<WatchObject> all = new();

    public string prefabName = prefabName;
    public Sprite icon = icon;
    public Func<ZDO, bool> check = check;
}