using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UIElements;

public class GrenadeProjectile : MonoBehaviour
{
    public static event EventHandler OnAnyGrenadeExplode;
    private Vector3 targetPosition;
    [SerializeField] private float damageRadius = 4f;
    [SerializeField] private int damage = 30;
    private float totalDistance;
    private Action onGrenadeBehaviourComplete;
    private Vector3 positionXZ;
    [SerializeField] private Transform explodeEffectPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve;
    private void Update()
    {
        Vector3 moveDir = (targetPosition - positionXZ).normalized;
        float moveSpeed = 15f;
        positionXZ += moveDir * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1- distance / totalDistance;
        float grenadeHeight = totalDistance/4f;
        float positionY=  arcYAnimationCurve.Evaluate(distanceNormalized)* grenadeHeight;
        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);

        float reachedTargetDistance = 0.2f;
        if(Vector3.Distance(positionXZ, targetPosition) < reachedTargetDistance)
        {
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);
            foreach(Collider collider in colliderArray)
            {
                if(collider.TryGetComponent(out Unit unit))
                {
                    unit.Damage(damage);
                }
                if(collider.TryGetComponent(out DestructibleCrate crate))
                {
                    crate.Damage();
                }
            }
            OnAnyGrenadeExplode?.Invoke(this, EventArgs.Empty);
            onGrenadeBehaviourComplete();
            Instantiate(explodeEffectPrefab, targetPosition + Vector3.up, Quaternion.identity);
            trailRenderer.transform.parent = null;
            Destroy(gameObject);
        }
    }
    public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviourComplete )
    {
        this.onGrenadeBehaviourComplete = onGrenadeBehaviourComplete;
        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
    }
}
