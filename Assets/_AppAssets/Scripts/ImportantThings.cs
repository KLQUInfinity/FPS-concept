using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ImportantThings
{
    #region Data
    public const string MapKey = "Map";
    public const string PlayerTag = "Player";
    public const string TeamIndex = "TeamIndex";
    #endregion

    #region Methods
    public static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
    #endregion
}
