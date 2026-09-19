using System;
using UnityEngine;

public class Bonfire : MonoBehaviour
{
    public int 初始光照级别 = 2;
    public int 光照级别 { get; private set; }
    
    public Bonfire instance { get; private set; }
    
    private void Awake() => instance = this;
    void Start() => 光照级别 = 初始光照级别;
}
