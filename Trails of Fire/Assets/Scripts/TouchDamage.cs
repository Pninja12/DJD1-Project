using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
#if UNITY_EDITOR
    using UnityEditor;
#endif


public class TouchDamage : MonoBehaviour
{
    [SerializeField] private Faction faction;
    [SerializeField] private int damage;
    [SerializeField] private bool destroyOnDamage = false;
    [SerializeField] private bool makeThisInvulnerableToDamage = false;
    [SerializeField] private int framesToDelayDamage = 0;
    [SerializeField] private bool directionalDamage;
    [SerializeField, ShowIf(nameof(directionalDamage))] private Vector2 normalDirection = Vector2.up;
    [SerializeField, ShowIf(nameof(directionalDamage)),Range(0,180)] private float angleTolerance = 45.0f;

    private HealthSystem thisHealthSystem;
    private Rigidbody2D thisRigidbody2D;

    public delegate void OnDamage(int damage, HealthSystem target);
    public event OnDamage onDamage;

    private void Awake()
    {
        thisHealthSystem = GetComponent<HealthSystem>();
        thisRigidbody2D = GetComponent<Rigidbody2D>();
        normalDirection.Normalize();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        HealthSystem otherHealthSystem = collision.GetComponentInParent<HealthSystem>();
        if (otherHealthSystem != null)
        {
            Faction otherFaction = otherHealthSystem.faction;

            if (faction.IsHostile(otherFaction))
            {
                if(directionalDamage)
                {
                    Vector2 damageVector = (transform.position - otherHealthSystem.transform.position).normalized;
                    float dp = Vector2.Dot(damageVector, normalDirection);
                    float angle = Mathf.Acos(dp) * Mathf.Rad2Deg;

                    if(angle >= angleTolerance)
                    {
                        return;
                    }

                    if(thisRigidbody2D)
                    {
                        Vector2 velocity = -thisRigidbody2D.velocity.normalized;
                        dp = Vector2.Dot(velocity, normalDirection);
                        angle = Mathf.Acos(dp) * Mathf.Rad2Deg;

                        if(angle >= angleTolerance)
                        {
                            return;
                        }

                    }
                }

                if(framesToDelayDamage > 0)
                {
                    StartCoroutine(DealDamageCR(framesToDelayDamage, otherHealthSystem, damage));
                }
                else
                {
                    DealDamage(otherHealthSystem, damage);
                }
            }
        }
    }

    IEnumerator DealDamageCR(int nFrames, HealthSystem target, int damage)
    {
        for (int i = 0; i < nFrames; i++)
        {
            yield return null;
        }
        DealDamage(target, damage);
    }

    void DealDamage(HealthSystem target, int damage)
    {
        if(target.DealDamage(damage, transform))
        {
            if(destroyOnDamage)
            {
                Destroy(gameObject);
            }

            if(makeThisInvulnerableToDamage && thisHealthSystem)
            {
                thisHealthSystem.AddInvulnerability(1f, target.gameObject);
            }

            onDamage?.Invoke(damage, target);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(directionalDamage)
        {
            Handles.color = new Color(0.0f, 1.0f, 0.5f, 0.05f);
            Handles.DrawSolidArc(transform.position, Vector3.forward, normalDirection, -angleTolerance, 40.0f);
            Handles.DrawSolidArc(transform.position, Vector3.forward, normalDirection, angleTolerance, 40.0f);
        }
    }
#endif
}
