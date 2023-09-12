using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;
    [SerializeField] private Transform rifleTransform;
    [SerializeField] private Transform swordTransform;

    private void Awake()
    {
        if(TryGetComponent(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        };

        if (TryGetComponent(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        };

        if(TryGetComponent(out SwordAction swordAction))
        {
            swordAction.OnSwordActionStarted += SwordAction_OnSwordActionStarted;
            swordAction.OnSwordActionCompleted += SwordAction_OnSwordActionCompleted;
        }
    }

    private void Start()
    {
        EquipRifle();
    }

    private void SwordAction_OnSwordActionCompleted(object sender, EventArgs e)
    {
        EquipRifle();
        // anim.SetTrigger("shoot");
    }

    private void SwordAction_OnSwordActionStarted(object sender, EventArgs e)
    {
        anim.SetTrigger("swordSlash");
        EquipSword();
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        anim.SetBool("walking", true);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        anim.SetBool("walking", false);
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        Vector3 targetUnitShootAtPosition = Vector3.zero;
        if (e.hit)
        {
            targetUnitShootAtPosition = e.targetUnit.transform.position;
            targetUnitShootAtPosition.y = shootPointTransform.position.y;
            anim.SetTrigger("shoot");
        }
        else
        {

            targetUnitShootAtPosition = e.targetUnit.transform.position;
            targetUnitShootAtPosition.y = shootPointTransform.position.y +0.5f;
            targetUnitShootAtPosition += new Vector3(UnityEngine.Random.value - 0.5f, 0, UnityEngine.Random.value - 0.5f);
            anim.SetTrigger("shootMiss");
        }
        Transform bulletProjectileTransform=
            Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);
        BulletProjectile bulletProjectile= bulletProjectileTransform.GetComponent<BulletProjectile>();
        bulletProjectile.Setup(targetUnitShootAtPosition);
    }

    private void EquipSword()
    {
        swordTransform.gameObject.SetActive(true);
        rifleTransform.gameObject.SetActive(false);
    }

    private void EquipRifle()
    {
        swordTransform.gameObject.SetActive(false);
        rifleTransform.gameObject.SetActive(true);
    }
}
