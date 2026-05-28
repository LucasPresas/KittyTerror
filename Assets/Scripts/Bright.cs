using UnityEngine;

public class GlowPulse : MonoBehaviour
{
    private Material mat;

    public float speed = 2f;

    public float minGlow = 0.1f;
    public float maxGlow = 1.2f;

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
            mat = rend.material;
    }

    void Update()
    {
        if (mat == null) return;

        float pulse = Mathf.Lerp(
            minGlow,
            maxGlow,
            (Mathf.Sin(Time.time * speed) + 1f) / 2f
        );

        Color finalColor = Color.white * pulse;

        mat.SetColor("_EmissionColor", finalColor);
    }
}