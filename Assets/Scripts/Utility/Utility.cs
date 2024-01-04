using System;
using UnityEditor;
using UnityEngine;

public static class Utility
{
    [InitializeOnLoadMethod]
    public static void Refresh()
    {
        Debug.Log("컴파일 완료");
    }

    /// <summary>
    /// 문자열의 첫번째 문자를 소문자로 만들어줌
    /// </summary>
    public static string ToLowerFirstChar(this string str)
    {
        return char.ToLower(str[0]) + str.Substring(1);
    }

    /// <summary>
    /// 주어진 value 값을  0과 1 사이의 값으로 반환함
    /// </summary>
    public static float Clamp01(float value, float min, float max)
    {
        float relativePosition = (value - min) / (max - min);

        // 0과 1 사이의 범위로 매핑
        float mappedValue = Mathf.Clamp01(relativePosition);

        return mappedValue;
    }

    /// <summary>
    /// 주어진 value 값을  0과 1 사이의 값으로 반환함
    /// </summary>
    public static double Clamp01(double min, double max, double value)
    {
        double relativePosition = (value - min) / (max - min);

        // 0과 1 사이의 범위로 매핑
        double mappedValue = Mathf.Clamp01((float)relativePosition);

        return mappedValue;
    }
}
