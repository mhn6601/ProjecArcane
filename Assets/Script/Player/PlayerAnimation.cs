using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public Animator Animator => animator;

    [SerializeField] private RuntimeAnimatorController defaultOverride;
    [SerializeField] private RuntimeAnimatorController weaponOverride;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void SetWeaponAnimation(WeaponItemData weaponData)
    {
        if (weaponData == null)
        {

            animator.runtimeAnimatorController =
           defaultOverride;
        }
            
        else
        {
            Debug.Log(weaponData.itemId);
            animator.runtimeAnimatorController =
           weaponData.weaponOverrideController;
        }
       
    }
}