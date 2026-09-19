using UnityEngine;

public enum BonfireSize
{
    Small,
    Medium,
    Large,
}

public class BonfireManager : MonoBehaviour
{
    public FireController fireController;

    [SerializeField]
    private BonfireSize _size;
    public BonfireSize size
    {
        get { return _size; }
        set
        {
            _size = value;
            print(size);
            fireController.fireSize = size;
        }
    }

    public void increaseBonfireSize()
    {
        switch (size)
        {
            case BonfireSize.Small:
                size = BonfireSize.Medium;
                break;
            case BonfireSize.Medium:
                size = BonfireSize.Large;
                break;
        }
    }

    void OnValidate()
    {
        size = _size;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        size = BonfireSize.Small;
    }

    // Update is called once per frame
    void Update() { }
}
