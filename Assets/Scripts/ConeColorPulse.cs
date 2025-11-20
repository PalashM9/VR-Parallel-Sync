using UnityEngine;

/// <summary>
/// Smoothly animates the cone's color between two colors over time
/// using a sine wave. This satisfies the "animation of another property"
/// requirement (time-dependent color change).
/// </summary>
[RequireComponent(typeof(Renderer))]
public class ConeColorPulse : MonoBehaviour
{
    [Header("Color Animation")]
    public Color colorA = Color.red;          // start color
    public Color colorB = Color.yellow;       // end color
    public float pulseSpeed = 1.0f;           

    private Renderer _renderer;
    private Material _materialInstance;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();

        
        _materialInstance = _renderer.material;
        _materialInstance.color = colorA;
    }

    private void Update()
    {
       
        float t = 0.5f * (1f + Mathf.Sin(Time.time * Mathf.PI * 2f * pulseSpeed));

        
        Color c = Color.Lerp(colorA, colorB, t);
        _materialInstance.color = c;
    }

    private void OnDestroy()
    {
        
        if (Application.isPlaying && _materialInstance != null)
        {
            Destroy(_materialInstance);
        }
    }
}
