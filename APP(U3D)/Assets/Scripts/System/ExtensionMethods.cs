using System;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    #region LoadData
    public static bool IsReady(this List<LoadData> datas, float progress)
    {
        if (datas.Count == 0)
            return false;
        return progress > datas[0].atPercentage;
    }

    public static void Run(this List<LoadData> datas)
    {
        datas[0].method.Invoke();
        datas.RemoveAt(0);
    }
    #endregion
}