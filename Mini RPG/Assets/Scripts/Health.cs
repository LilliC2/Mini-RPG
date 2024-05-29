using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : GameBehaviour
{
    [SerializeField]
    public int currentHealth;
    public int playerIndex;
    public int maxHealth, defence;

    public UnityEvent<GameObject> OnHitWithRef, OnDeathWithRef;

    [SerializeField]
    public bool isDead = false;

    bool isPlayer = false;

    PlayerController playerController;

    public UnityEvent<float> ApplyParalysisEvent;
    public UnityEvent<float,float> ApplySlownessEvent;
    public UnityEvent<float,float> ApplyBurnEvent;

    public void InitilizeHealth(int healthValue, int defenceValue)
    {
        currentHealth = healthValue;
        maxHealth = healthValue;
        defence = defenceValue;
        isDead = false;
        if(gameObject.TryGetComponent<PlayerController>(out PlayerController _playerController))
        {
            isPlayer = true;
            playerIndex = _playerController.playerNum;
            playerController = _playerController;
        }
    }

    private void Update()
    {
        //if(Input.GetKeyUp(KeyCode.P))
        //{
        //    ApplyParalysis(3);
        
        //}
        
        //if(Input.GetKeyUp(KeyCode.B))
        //{
        //    ApplyBurn(5,1);
        
        //}
        
        //if(Input.GetKeyUp(KeyCode.L))
        //{
        //    ApplySlowness(3, 0.5f);
        
        //}
        
    }

    public void ApplyParalysis(float duration)
    {
        ApplyParalysisEvent.Invoke(duration);
    }

    public void ApplyBurn(float duration, float tickDmg)
    {
        print("Apply burn in health scrupt");
        ApplyBurnEvent.Invoke(duration, tickDmg);
    }

    public void ApplySlowness(float duration, float strength)
    {
        ApplySlownessEvent.Invoke(duration, strength);

    }

    public void Die()
    {
        print("DIE");
        Destroy(gameObject);
    }

    public void GetHit(int amount, GameObject sender)
    {
        if (isDead)
            return;
        if (sender.layer == gameObject.layer) //not to hit yourself
            return;

        int dmg = (amount - defence);
        if (dmg < 0) dmg = 0;

        currentHealth -= dmg;
        if(isPlayer) { playerController.OnHit(); }

        if(currentHealth > 0)
        {
            OnHitWithRef?.Invoke(sender);
            
        }
        else
        {
            OnDeathWithRef?.Invoke(sender);
            isDead = true;
        }
    }
}
