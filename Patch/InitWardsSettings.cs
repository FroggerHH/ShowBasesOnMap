using System.Runtime.CompilerServices;
using ShowBasesOnMap.Compatibility.WardIsLove;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace ShowBasesOnMap.Patch;

[HarmonyPatch] public class InitWardsSettings
{
    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))] [HarmonyPostfix] [HarmonyWrapSafe] [HarmonyPriority(-99)]
    private static void Init(ZNetScene __instance)
    {
        if (SceneManager.GetActiveScene().name != "main") return;
        // if (!ZNet.instance.IsServer()) return;

        WatchObject.all = new();

        AddObj("guard_stone");
        AddObj("piece_workbench");
        AddObj("portal_wood");
        AddObj("Cart");
        AddObj("fire_pit");
        AddObj("Thorward");
        AddObj("Dragon");
        AddObj("Deer");
        AddObj("Eikthyr");
        AddObj("gd_king");
        AddObj("GoblinKing");
        AddObj("SeekerQueen");
        AddObj("TheHive");
        AddObj("Hive");
        AddObj("TrainingDummy");
        AddObj("Bonemass");
        AddObj("Troll");
        AddObj("ChunkLoader_stone");
        AddObj("Troll_Summoned");

        foreach (var obj in ZNetScene.instance.m_prefabs.Where(x => x.GetComponent<Tameable>()))
        {
            AddObj(obj.name, zdo => zdo.GetBool(ZDOVars.s_tamed, false));
        }
    }

    public static void AddObj(string name, Func<ZDO, bool> check = null)
    {
        var prefab = ZNetScene.instance.GetPrefab(name);
        if (!prefab) return;
        Character character = null;
        Piece piece = null;
        if (!prefab.TryGetComponent(out piece) && !prefab.TryGetComponent(out character)) return;
        if (character)
        {
            LoadImageFromWEB($@"https://valheim-modding.github.io/Jotunn/Documentation/images/characters/{name}.png",
                sprite => WatchObject.all.Add(new WatchObject(name, sprite, check)));
        } else if (piece)
        {
            WatchObject.all.Add(new WatchObject(name, piece.m_icon, check));
        } else DebugError("Couldn't find prefab with name " + name);
    }


    public static void LoadImageFromWEB(string url, Action<Sprite> callback)
    {
        if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out _)) return;

        GetPlugin().StartCoroutine(_Internal_LoadImage(url, callback));
    }

    private static IEnumerator _Internal_LoadImage(string url, Action<Sprite> callback)
    {
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.result is UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            if (texture.width == 0 || texture.height == 0) yield break;
            Texture2D temp = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            temp.SetPixels(texture.GetPixels());
            temp.Apply();
            var sprite = Sprite.Create(texture, new(0, 0, texture.width, texture.height), new(0.5f, 0.5f));
            callback?.Invoke(sprite);
        }
    }
}