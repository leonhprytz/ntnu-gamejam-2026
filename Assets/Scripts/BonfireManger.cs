using UnityEngine;

public class BonfireManager : MonoBehaviour
{
    public FireController fireController;

    public void increaseFire()
    {
        fireController.increaseState();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fireController.state = FireState.Small;
    }

    // Update is called once per frame
    void Update() { }
}
