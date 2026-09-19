using UnityEngine;

public class Bonfire : MonoBehaviour
{
    public int 初始光照级别 = 2;
    public int 步长 = 3;
    private int _光照级别;
    
    void Start() => _光照级别 = 初始光照级别;
    
    public int GetLightValue() => _光照级别;
    public void AddWood(int count) => _光照级别 += count * 步长;
    // public void increaseFire(int count) => _光照级别 += count;
    public void DecreaseFire(int count) => _光照级别 -= count;
}
