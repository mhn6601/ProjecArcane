using System.Collections.Generic;
using Unity.AppUI.UI;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class HurtBoxGenerator : MonoBehaviour
{
    [SerializeField] private CharacterCombat owner; // 이 파츠가 속한 캐릭터
    [Header("Skinned Mesh")]
    [SerializeField]
    private SkinnedMeshRenderer skinnedMesh;
    [Header("Head")]
    [SerializeField]
    string Head = "Head";
    [SerializeField]
    float HeadRadius;
    [Header("Spine")]
    [SerializeField]
    string Spine= "Spine";
    [SerializeField]
    float SpineRadius;
    [Header("LeftArm")]
    [SerializeField]
    string LeftArm="LeftArm";
    [SerializeField]
    float LeftArmRadius;
    [Header("RightArm")]
    [SerializeField]
    string RightArm = "RightArm";
    [SerializeField]
    float RightArmRadius;
    [Header("LeftLeg")]
    [SerializeField]
    string  LeftLeg= "LeftLeg";
    [SerializeField]
    float LeftLegRadius;
    [Header("RightLeg")]
    [SerializeField]
    string  RightLeg= "RightLeg";
    [SerializeField]
    float RightLegRadius;

    [Header("HurtBox Root")]
    [SerializeField]
    private Transform hurtBoxRoot;

    private readonly Dictionary<string, Transform> boneDictionary = new();

    private void Awake()
    {
        Generate();
    }

    [ContextMenu("Generate HurtBox")]
    public void Generate()
    {
        if (skinnedMesh == null)
        {
            Debug.LogError("SkinnedMeshRenderer가 없습니다.");
            return;
        }

        if (hurtBoxRoot == null)
        {
            GameObject root = new GameObject("HurtBox");
            root.transform.SetParent(transform);
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;

            hurtBoxRoot = root.transform;
        }

        CreateBoneDictionary();

        CreateHurtBox("Head", Head, HeadRadius,HitPart.Head);
        CreateHurtBox("Body", Spine, SpineRadius, HitPart.Body);
        CreateHurtBox("LeftArm", LeftArm, LeftArmRadius, HitPart.Arm);
        CreateHurtBox("RightArm", RightArm, RightArmRadius, HitPart.Arm);
        CreateHurtBox("LeftLeg", LeftLeg, LeftLegRadius, HitPart.Leg);
        CreateHurtBox("RightLeg", RightLeg, RightLegRadius, HitPart.Leg);
    }

    private void CreateBoneDictionary()
    {
        boneDictionary.Clear();

        foreach (Transform bone in skinnedMesh.bones)
        {
            if (bone == null)
                continue;

            if (!boneDictionary.ContainsKey(bone.name))
            {
                boneDictionary.Add(bone.name, bone);
            }
        }
    }

    private void CreateHurtBox(string hurtBoxName,string boneName,float radius,HitPart hitPart)
    {
        if (!boneDictionary.TryGetValue(boneName, out Transform bone))
        {
            Debug.LogWarning(
                $"Bone을 찾을 수 없습니다 : {boneName}"
            );

            return;
        }

        GameObject hurtBox = new GameObject(hurtBoxName);
        if (owner.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            hurtBox.layer= LayerMask.NameToLayer("Player");
        }
        else
        {
            hurtBox.layer = LayerMask.NameToLayer("Enemy");
        }

        hurtBox.transform.SetParent(bone);
        hurtBox.transform.localPosition = Vector3.zero;
        hurtBox.transform.localRotation = Quaternion.identity;
        hurtBox.transform.localScale = Vector3.one;

        CapsuleCollider collider =
            hurtBox.AddComponent<CapsuleCollider>();
        collider.isTrigger = true;

        collider.radius = radius;
        collider.height = radius * 2f;

        HurtBox hurtBoxComponent =
            hurtBox.AddComponent<HurtBox>();
        hurtBoxComponent.SetHitPart(hitPart);
        hurtBoxComponent.owner=owner;
    }
}