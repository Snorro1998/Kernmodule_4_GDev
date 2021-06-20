using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilsMono : Singleton<UtilsMono>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public void DestroyAllChildObjects(ref GameObject obj)
    {
        for (int i = obj.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(obj.transform.GetChild(i).gameObject);
        }
    }
}
