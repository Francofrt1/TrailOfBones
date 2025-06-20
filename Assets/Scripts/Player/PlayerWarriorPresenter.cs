using System.Linq;
using Assets.Scripts.Interfaces;
using UnityEngine;

public class PlayerWarriorPresenter : PlayerPresenter
{
    private AttackArea attackArea;
    [SerializeField] private GameObject attackAreaGameObject;

    protected override void Start()
    {
        base.Start();
        attackArea = attackAreaGameObject.GetComponent<AttackArea>();
    }
    
    public override void DoAttack()
    {
        attackArea.DamageablesInRange.RemoveAll(x => x == null || (x as MonoBehaviour) == null);
        var damageables = attackArea.DamageablesInRange.Where(x => x.GetTag() == "Enemy");
        if (!damageables.Any()) return;
        foreach (IDamageable damageable in damageables)
        {
            damageable.TakeDamage(playerModel.baseDamage, playerModel.ID);
            Debug.Log($"Player did {playerModel.baseDamage} damage to {damageable.GetTag()}");
        }
    }
}