using UnityEngine;
using System.Collections;

public class GlowControl : MonoBehaviour {

    ParticleSystem glowSystem;
    ParticleSystem.Particle[] particles;
    NoiseGenerator NG;
    float intensity;
    float noise;
    float burst;

    Color col;
    float[] noiseArray;

	// Use this for initialization
	void Start () {
        NG = new NoiseGenerator();
        glowSystem = GetComponent<ParticleSystem>();
        noiseArray = NG.GetPerlinNoise1D(8, 10, 0.8f, 0, 1);
        Debug.Log("NOISE GEN " + NG.ToString());
        Debug.Log("RESULT " + noiseArray.ToString()); 
        col = glowSystem.startColor;
	}
	
	// Update is called once per frame
	void Update () {

        glowSystem.GetParticles(particles);

        //Debug.Log("NOISE ARRAY "+noiseArray.Length.ToString());
        noise = Random.Range(0f, 1f) * intensity;
        //noise = noiseArray[Mathf.FloorToInt(Time.frameCount % noiseArray.Length)];

        float n = Mathf.Clamp(intensity * 0.6f + noise * 0.4f + burst, 0f, 1f); //Ranging from 0.8 to 1
        col = new Color(col.r, col.g, col.b, 0.2f + n * 0.25f); //Keep alpha low

        glowSystem.startSize = 1f + n * 6f;
        glowSystem.startColor = col;

        if (burst > 0.01f)
        {
            burst *= 0.75f;
        }
	}

    public void setIntensity(float n)
    {
        intensity = Mathf.Clamp(n, 0f, 1f);
    }
    public void Burst(float n)
    {
        burst = Mathf.Clamp(n, 0f, 1f);
    }
}
