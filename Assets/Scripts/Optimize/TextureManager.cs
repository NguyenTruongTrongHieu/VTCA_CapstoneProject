using System.Collections.Generic;
using UnityEngine;

public class TextureManager : MonoBehaviour
{
    // Singleton
    public static TextureManager Instance;

    // Cache các texture đã preload
    private Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();

    // Danh sách tên texture cần preload (đặt trùng với tên file trong Resources)
    [SerializeField]
    private string[] textureNames = {
        "castle-colour-palette",
        "Castle-Emission",
        "Castle-MetallicSmoothpng",
        "T_Coins_CubeVillage",
        "T_VegFruitA_CubeVillage"
    };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        PreloadTextures();
    }

    // Preload và cache texture
    void PreloadTextures()
    {
        foreach (string textureName in textureNames)
        {
            if (!textureCache.ContainsKey(textureName))
            {
                Texture2D texture = Resources.Load<Texture2D>(textureName);

                if (texture != null)
                {
                    if (texture.width > 1024 || texture.height > 1024)
                    {
                        texture = ResizeTexture(texture, 1024, 1024);
                    }

                    CompressTexture(texture);

                    textureCache[textureName] = texture;
                }
                else
                {
                    Debug.LogWarning($"Texture {textureName} not found in Resources.");
                }
            }
        }
    }

    // Resize nếu texture quá lớn
    Texture2D ResizeTexture(Texture2D originalTexture, int targetWidth, int targetHeight)
    {
        RenderTexture rt = new RenderTexture(targetWidth, targetHeight, 24);
        RenderTexture.active = rt;
        Graphics.Blit(originalTexture, rt);

        Texture2D result = new Texture2D(targetWidth, targetHeight);
        result.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        result.Apply();

        RenderTexture.active = null;
        rt.Release();
        Destroy(rt);

        return result;
    }

    // Nén texture
    void CompressTexture(Texture2D texture)
    {
        texture.Compress(true);
    }

    // Truy cập texture từ cache
    public Texture2D GetTexture(string name)
    {
        textureCache.TryGetValue(name, out var tex);
        return tex;
    }

    // Tự động apply texture cho GameObject sau khi Instantiate
    public void ApplyPreloadedTexture(GameObject obj, string textureKey)
    {
        Texture2D tex = GetTexture(textureKey);
        if (tex == null)
        {
            Debug.LogWarning($"Texture {textureKey} not found in cache.");
            return;
        }

        // Apply cho tất cả Renderer trong GameObject
        var renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            if (renderer.material != null)
                renderer.material.mainTexture = tex;
        }
    }
}
