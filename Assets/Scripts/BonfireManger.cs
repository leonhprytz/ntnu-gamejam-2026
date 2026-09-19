using UnityEngine;

public enum BonfireSize
{
    Small,
    Medium,
    Large,
}

public class BonfireManager : MonoBehaviour
{
    private Animator animator;

    public FireController fireController;
    public BonfireAudioController bonfireAudioController;
    public EmberController emberController;

    [SerializeField]
    private BonfireSize _size;
    public BonfireSize size
    {
        get { return _size; }
        set
        {
            _size = value;
            fireController.fireSize = size;
            bonfireAudioController.size = size;
            emberController.size = size;
            animator?.SetInteger("bonfireSize", (int)size);
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
        animator = GetComponent<Animator>();
        size = BonfireSize.Small;
    }

    // Update is called once per frame
    void Update() { }
}
