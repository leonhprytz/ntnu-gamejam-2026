using UnityEngine;

public enum BonfireSize
{
    Small,
    Medium,
    Large,
}

public class BonfireManager : InteractionCheckpoint
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
            // fireController.fireSize = size;
            bonfireAudioController.size = size;
            emberController.size = size;
            animator?.SetInteger("bonfireSize", (int)size);
        }
    }

    [Header("Light")]
    [SerializeField, Range(1F, 100F)]
    private float _lightAmount = 1F;
    public float lightAmount
    {
        get { return _lightAmount; }
        set { SetLight(value); }
    }
    public float minRadius = 1.85F;
    public float maxRadius = 5.0F;
    public float mediumThreshold = 30F;
    public float largeThreshold = 60F;

    public override void Interact()
    {
        AddLight(10F);
        MarkInteractionComplete();
    }

    void OnValidate()
    {
        if (fireController != null)
        {
            lightAmount = _lightAmount;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() { }

    public void AddLight(float amount)
    {
        lightAmount += amount;
    }

    public void SetLight(float amount)
    {
        Debug.Log("Setting light on bonfire: " + amount);
        _lightAmount = Mathf.Clamp(amount, 1F, 100F);
        fireController.radii2 = Mathf.Lerp(minRadius, maxRadius, _lightAmount / 100F);

        size =
            _lightAmount >= largeThreshold ? BonfireSize.Large
            : _lightAmount >= mediumThreshold ? BonfireSize.Medium
            : BonfireSize.Small;
    }
}
