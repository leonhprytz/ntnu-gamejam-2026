using UnityEngine;
using System.Collections.Generic;

public struct Item
{
    public Sprite icon;
    public GameObject gameObject;
}

public class Inventory : MonoBehaviour
{
    public SortedSet<Item> inventory;
}