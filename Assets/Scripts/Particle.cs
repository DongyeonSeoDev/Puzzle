using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    private ParticleSystem particle = null;
    private ParticleSystem.ShapeModule particleShape = default;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
        particleShape = particle.shape;
    }

    public void StartParticle(Vector3 position, Texture2D texture)
    {
        transform.position = position;
        particleShape.texture = texture;

        particle.Play();

        Invoke("EndParticle", 2f);
    }

    private void EndParticle()
    {
        gameObject.SetActive(false);
    }
}
