using UnityEngine;
using System.Collections;

public class GlowControl : MonoBehaviour {

    ParticleSystem glowSystem;
    ParticleSystem.Particle[] particles;
    float intensity;
    float noise;
    float burst;
    float flicker;

    Color col;
    float[] noiseArray;

	// Use this for initialization
	void Start () {
        glowSystem = GetComponent<ParticleSystem>();
        col = glowSystem.startColor;
	}
	
	// Update is called once per frame
	void Update () {

		col = glowSystem.startColor;

        glowSystem.GetParticles(particles);

        noise = Random.Range(0f, 1f) * intensity;

        float n = Mathf.Clamp(intensity * 0.6f + noise * 0.4f - (noise * flicker) + burst, 0f, 1f); //Ranging from 0.8 to 1
        col = new Color(col.r, col.g, col.b, 0.2f + n * 0.25f); //Keep alpha low

        glowSystem.startSize = 1f + n * 6f;
        glowSystem.startColor = col;

        if (burst > 0.01f)
        {   burst *= 0.66f;     }

        if (flicker > 0.01f)
        {   flicker *= 0.9f;   }
	}

    public void setIntensity(float n)
    {
        intensity = Mathf.Clamp(n, 0f, 1f);
    }
    public void Burst(float n)
    {
        burst = Mathf.Clamp(n, 0f, 1f);
    }

    public void Flicker(float n)
    {
        flicker = Mathf.Clamp(n, 0f, 1f);
    }
}
