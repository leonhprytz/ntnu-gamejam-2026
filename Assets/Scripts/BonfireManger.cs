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
    private float lightAmount = 1F;
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
            AddLight(0F);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        AddLight(0F);
    }

    // Update is called once per frame
    void Update() { }


    public void AddLight(float amount)
    {
        lightAmount = Mathf.Clamp(lightAmount + amount, 1F, 100F);
        fireController.radii2 = Mathf.Lerp(minRadius, maxRadius, lightAmount / 100F);

        size =
            lightAmount >= largeThreshold ? BonfireSize.Large
            : lightAmount >= mediumThreshold ? BonfireSize.Medium
            : BonfireSize.Small;
    }

    public void IncreaseFire()
    {
        AddLight(10F);
    }

    public void DecreaseFire()
    {
        AddLight(-10F);
    }
}
