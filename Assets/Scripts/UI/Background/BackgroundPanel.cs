using System.Reflection;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine;

public class BackgroundPanel : MonoBehaviour
{
    #region Fields

    [Header("GUI Elements:")]

    [SerializeField] protected Image bgImage;
    [SerializeField] protected AspectRatioFitter imageFitter;

    [Header("Config:")]

    [SerializeField] bool autoRatioFitter;

    #endregion

    void Awake()
    {
        Initialization();
    }

    protected void Initialization()
    {
        if (bgImage == null && TryGetComponent(out Image bg))
        {
            bgImage = bg;
        }
        if (imageFitter == null && TryGetComponent(out AspectRatioFitter fitter))
        {
            imageFitter = fitter;
        }
    }

    public void SetImage(Sprite newImage)
    {
        bgImage.sprite = newImage;
        //CalculateRatioFitter();
    }

    /*protected void CalculateRatioFitter()
    {
        if (autoRatioFitter)
        {
            int imageWidth;
            int imageHeight;
            //GetImageSize(bgImage.sprite.texture, out imageWidth, out imageHeight);
            imageFitter.aspectRatio = (float)imageWidth / (float)imageHeight;
        }
    }*/

    /*public static bool GetImageSize(Texture2D asset, out int width, out int height)
    {
        if (asset != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer != null)
            {
                object[] args = new object[2] { 0, 0 };
                MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                mi.Invoke(importer, args);

                width = (int)args[0];
                height = (int)args[1];

                return true;
            }
        }

        height = width = 0;
        return false;
    }*/
}
