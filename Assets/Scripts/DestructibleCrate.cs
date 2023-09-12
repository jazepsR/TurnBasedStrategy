using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DestructibleCrate : MonoBehaviour
{
    [SerializeField] private Transform crateDestroyedPrefab;
    public static event EventHandler OnAnyDestroyed;
    private GridPosition gridPosition;

    private void Start()
    {
        gridPosition =LevelGrid.Instance.GetGridPosition(transform.position);
    }
    public void Damage()
    {
        OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
        Transform cratePieces = Instantiate(crateDestroyedPrefab, transform.position, transform.rotation);
        ApplyExplosionToCratePieces(cratePieces, 100, transform.position, 10);
        Destroy(gameObject);
    }

    public GridPosition GetGridPosition()
    { 
        return gridPosition;
    }

    private void ApplyExplosionToCratePieces(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }
            ApplyExplosionToCratePieces(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}
