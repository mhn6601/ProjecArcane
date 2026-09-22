using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLockOnPointGenerator : MonoBehaviour
{
    [System.Serializable]
    public class BodyPartSetting
    {
        public Transform bone;
    }

    [SerializeField]
    private List<BodyPartSetting> bodyParts = new();

    public List<LockOnPoint> lockOnPointList = new List<LockOnPoint>()  ;
    [SerializeField]
    private Transform root;

    private void Awake()
    {
        Generate();
    }
    public void Generate()
    {
        if (root == null)
            root = transform;



        foreach (BodyPartSetting setting in bodyParts)
        {
            if (setting.bone == null)
            {
                Debug.LogWarning(
                    $"{setting/*.bodyPart*/} Bone이 지정되지 않았습니다.",
                    this
                );

                continue;
            }

            CreateLockOnPoint(setting.bone);
        }
    }


    private void CreateLockOnPoint(Transform bone)
    {
        GameObject obj =
            new GameObject("LockOnPoint");
        obj.layer = LayerMask.NameToLayer("LockOnSystem");
        obj.transform.SetParent(bone);
        //Debug.Log(obj.transform.parent.name);
        obj.transform.position =
            bone.position;

        obj.transform.rotation =
            bone.rotation;

        LockOnPoint point =
            obj.AddComponent<LockOnPoint>();
        lockOnPointList.Add(point);
        point.Initialize(/*setting.bodyPart*/);
    }

}