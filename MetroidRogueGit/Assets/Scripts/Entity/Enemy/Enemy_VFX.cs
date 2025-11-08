using UnityEngine;

public class Enemy_VFX : Entity_VFX
{
    [Header("Couter Attack Windows")]
    [SerializeField] private GameObject attackAlert;

    public void EnableAttackAlert(bool enable)
    {
        if(attackAlert == null)
            return;

        attackAlert.SetActive(enable);

    } 


}
