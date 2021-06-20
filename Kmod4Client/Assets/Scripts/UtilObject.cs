using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilObject : Singleton<UtilObject>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public void DestroyAllChildObjects(GameObject obj)
    {

    }
}
