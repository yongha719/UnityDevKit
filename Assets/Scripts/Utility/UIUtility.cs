using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GrayScale
{
    Off,
    On
}

public static class UIUtility
{
    private static Shader grayScaleShader = Shader.Find("Custom/GrayScale");
    private static Material grayScaleMaterial = new Material(grayScaleShader);

    public static void SetGrayScale(this Image image, GrayScale grayScale = GrayScale.On)
    {
        if (grayScale == GrayScale.Off)
        {
            image.material = null;
        }
        else
        {
            image.material = grayScaleMaterial;
        }
    }
}
