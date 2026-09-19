using System;
using UnityEngine;

[RequireComponent(typeof(BonfireManager))]
[RequireComponent(typeof(CircleCollider2D))]
public class IntroBonfireScript : Interactable
{
    private static BonfireManager _bonfireManager;
    private static CircleCollider2D _triggerBox;
    
    void Start()
    {
        _bonfireManager = GetComponent<BonfireManager>();
        _triggerBox = GetComponent<CircleCollider2D>();
    }

    public override bool TryInteract()
    {
        _bonfireManager.size = BonfireSize.Medium;
        return true;
    }
}
