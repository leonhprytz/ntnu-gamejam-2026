using UnityEngine;
using static UnityEngine.ParticleSystem;

public class EmberController : MonoBehaviour
{
    private BonfireSize _size;
    public BonfireSize size
    {
        get { return _size; }
        set
        {
            _size = value;
            switch (size)
            {
                case BonfireSize.Small:
                    enableEmbers();
                    break;
                default:
                    disableEmbers();
                    break;
            }
        }
    }

    private void enableEmbers()
    {
        var ps = GetComponent<ParticleSystem>();
        var emission = ps.emission;
        emission.enabled = true;
    }

    private void disableEmbers()
    {
        var ps = GetComponent<ParticleSystem>();
        var emission = ps.emission;
        emission.enabled = false;
        ps.SetParticles(new Particle[0], 0, 0);
    }
}
