using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour, IAgent, IHittable
{
    [SerializeField] 
    private int maxHealth;



    private int health;

    public int Health
    {
        get => health;
        set
        {
            health = Mathf.Clamp(value,0,maxHealth);
            uiHealth.UpdateUI(health);
        }
    }

    private bool dead = false;
    private PlayerWeapon playerWeapon;

    [field: SerializeField]
    public UIHealth uiHealth { get; set; }

    [field: SerializeField]
    public UnityEvent OnGetHit { get; set; }

    [field: SerializeField]
    public UnityEvent OnDeath { get; set; }

    [field: SerializeField]
    public UnityEvent OnDungeonEnter { get; set; }

    private void Awake()
    {
        playerWeapon = GetComponentInChildren<PlayerWeapon>();
    }

    private void Start()
    {
        Health = maxHealth;
        uiHealth.Initialize(Health);
    }

    public void GetHit(int damage, GameObject damageDealer)
    {
        if (dead == false)
        {
            Health = Health - damage;
            OnGetHit?.Invoke();
            if (Health <= 0)
            {
                OnDeath?.Invoke();
                dead = true;
            }
            Debug.Log("Player hit");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Resource"))
        {
            var resource = collision.gameObject.GetComponent<Resource>();

            if (resource != null)
            {
                switch (resource.ResourceData.ResourceType)
                {
                    case ResourceTypeEnum.Health:
                        if (Health >= maxHealth)
                        {
                            return;
                        }
                        Health += resource.ResourceData.GetAmount();
                        resource.PickUpResource();
                        break;
                    case ResourceTypeEnum.Ammo:
                        if (playerWeapon.AmmoFull)
                        {
                            return;
                        }
                        playerWeapon.AddAmmo(resource.ResourceData.GetAmount());
                        resource.PickUpResource();
                        break;
                    default:
                        break;
                }

            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Door"))
        {
            OnDungeonEnter?.Invoke();
            Debug.Log("Entered Dungeon");
        }
    }
}
