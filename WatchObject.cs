namespace ShowBasesOnMap;

public record WatchObject(string prefabName, Sprite icon, float radius, string localizeKey, bool isPrivateArea = false)
{
    public static List<WatchObject> all = new();

    public string prefabName = prefabName;
    public Sprite icon = icon;
    public float radius = radius;
    public string localizeKey = localizeKey;
    public bool isPrivateArea = isPrivateArea;
}