using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataContainer : MonoBehaviour
{
    public static LevelDataContainer instance;

    public int targetPlacements;

    private void Awake()
    {
        instance = this;
    }
}
