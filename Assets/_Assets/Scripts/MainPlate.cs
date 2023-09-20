using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlate : MonoBehaviour
{
    public List<GameObject> levels = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        DataContainer.instance.mainPlate = Instantiate(levels[LevelManager.instance.levelIndex - 1], transform.position, Quaternion.identity, transform);
        LevelManager.instance.targetPlacements = LevelDataContainer.instance.targetPlacements;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
