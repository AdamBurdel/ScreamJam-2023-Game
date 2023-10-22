using CandiceAIforGames.AI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandiceDamageReceiver : MonoBehaviour
{
    [SerializeField]
    public float hitPoints = 3f;

    [SerializeField]
    public bool isAttacking = true;
    public float HitPoints { get => hitPoints; set => hitPoints = value; }
    public CandiceModuleCombat combatModule;
    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }

    // Update is called once per frame
    void Start() 
    {
        combatModule = new CandiceModuleCombat(transform, onAttackComplete);
    }
    public void CandiceReceiveDamage(float damage)
    {
        HitPoints = combatModule.ReceiveDamage(damage, HitPoints);
        //Animations Manager
        //End
        if(HitPoints <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //if kill cam is attached
        Debug.Log("Die!");
        Destroy(this.gameObject, 0.5f);
    }

    void onAttackComplete(bool success)
    {
        IsAttacking = false;
    }
}
