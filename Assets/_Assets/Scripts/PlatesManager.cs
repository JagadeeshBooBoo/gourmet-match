using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlatesManager : MonoBehaviour
{
    public static PlatesManager instace;

    private List<GameObject> plates = new List<GameObject>();

    private Vector3 _plateOriginalPos;
    public int nextPlateIndex;


    private void Awake()
    {
        instace = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform t in transform)
            plates.Add(t.gameObject);

        plates[0].transform.GetChild(LevelManager.instance.levelIndex - 1).gameObject.SetActive(true);

        _plateOriginalPos = plates[0].transform.position;

        TweenMovePlate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            RotatePlate();
    }

    public void RotatePlate()
    {
        //float targetRot = transform.localRotation.eulerAngles.y + 60;
        //Vector3 rot = transform.localRotation.eulerAngles;
        //transform.DOLocalRotate(new Vector3(rot.x, targetRot, rot.z), 0.5f).SetEase(Ease.InOutBack);
    }

    public void TweenMovePlate()
    {
        TweenCurrentPlate();

        Transform plate = plates[nextPlateIndex].transform;
        plate.gameObject.SetActive(true);
        plate.localPosition = new Vector3(0f, plate.transform.localPosition.y, 0f);
        plate.DOLocalMoveX(-4.88f, 0.5f).SetDelay(2f).SetEase(Ease.OutBack);

        nextPlateIndex++;
    }

    private void TweenCurrentPlate()
    {
        if (nextPlateIndex == 0)
            return;

        Transform plate = plates[nextPlateIndex - 1].transform;
        plate.GetComponent<MeshCollider>().enabled = false;
        plate.DOLocalMoveX(0f, 0.5f).SetDelay(1f).SetEase(Ease.InBack);
    }
}
