using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    class LocalInvulnerability
    {
        public float timer;
        public GameObject damageSource;
    }

    [SerializeField] private Faction _faction;
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private bool invulnerableAfterDamage = false;
    [SerializeField] private float invulnerabilityDuration = 0;

    public delegate void OnDamage(float damage, Transform damageSource);
    public event OnDamage onDamage;

    public delegate void OnInvulnerabilityToggle(bool active);
    public event OnInvulnerabilityToggle onInvulnerabilityToggle;
    public delegate void OnDeath();
    public event OnDeath onDeath;

    int _health = 1;
    float invulnerabilityTime = 0;
    bool startInvulnerable = true;
    List<LocalInvulnerability> localInvulnerabilities = new List<LocalInvulnerability>();


    public Faction faction => _faction;
    public int health => _health;
    public bool isDead => _health <= 0;
    // Start is called before the first frame update
    void Awake()
    {
        _health = maxHealth;
    }

    void Start()
    {
        
    }

    private void Update()
    {
        if(startInvulnerable)
        {
            if (invulnerableAfterDamage)
            {
                invulnerabilityTime = invulnerabilityDuration;
                if (onInvulnerabilityToggle != null)
                {
                    onInvulnerabilityToggle(true);
                }
            }
            startInvulnerable = false;
        }
        if (invulnerabilityTime > 0)
        {
            invulnerabilityTime -= Time.deltaTime;
            if(invulnerabilityTime <= 0.0f)
            {
                if (onInvulnerabilityToggle != null)
                {
                    onInvulnerabilityToggle(false);
                }
            }
        }

        localInvulnerabilities.ForEach((item) => item.timer -= Time.deltaTime);
        localInvulnerabilities.RemoveAll((item) => item.timer <= 0);
    }

    public bool DealDamage(int damage, Transform damageSource)
    {
        if (_health <= 0) return false;
        if (invulnerabilityTime > 0) return false;
        if (HasInvulnerability(damageSource.gameObject)) return false;

        _health -= damage;

        onDamage?.Invoke(damage, damageSource);

        if (_health <= 0)
        {
            if(onDeath != null)
            {
                onDeath();
            }
            else
            {
                Destroy(gameObject);
            }
            
        }
        else
        {
            if (invulnerableAfterDamage)
            {
                invulnerabilityTime = invulnerabilityDuration;
                if (onInvulnerabilityToggle != null)
                {
                    onInvulnerabilityToggle(true);
                }
            }
        }

        return true;
    }

    bool HasInvulnerability(GameObject source)
    {
        foreach (var invulnerabilityItem in localInvulnerabilities)
        {
            if (invulnerabilityItem.damageSource == source)
            {
                return true;
            }
        }
        return false;
    }

    public void AddInvulnerability(float duration, GameObject source)
    {
        foreach (var invulnerabilityItem in localInvulnerabilities)
        {
            if(invulnerabilityItem.damageSource == source)
            {
                invulnerabilityItem.timer = Mathf.Max(duration, invulnerabilityItem.timer);
                return;
            }
        }

        localInvulnerabilities.Add(new LocalInvulnerability() { timer = duration, damageSource = source });
    }

}
