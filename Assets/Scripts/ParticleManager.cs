using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    private Queue<Particle> particleQueue = new Queue<Particle>();
    private static ParticleManager instance = null;

    public static ParticleManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.Log("ParticleManager instance가 null 입니다.");
                return null;
            }

            return instance;
        }
    }

    [SerializeField] private GameObject particleObject;
    [SerializeField] private Transform particleTransform;
    [SerializeField] private float count;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("ParticleManager instance가 중복입니다.");
            Destroy(this);
        }

        instance = this;

        for (int i = 0; i < count; i++)
        {
            particleQueue.Enqueue(CreateParticle());
        }
    }

    private Particle CreateParticle()
    {
        Particle particle = Instantiate(particleObject, particleTransform).GetComponent<Particle>();

        particle.gameObject.SetActive(false);

        return particle;
    }

    private Particle GetParticle()
    {
        Particle particle = null;

        if (particleQueue.Peek().gameObject.activeSelf)
        {
            particle = CreateParticle();
        }
        else
        {
            particle = particleQueue.Dequeue();
        }

        particle.gameObject.SetActive(true);
        particleQueue.Enqueue(particle);

        return particle;
    }

    public static void StartParticle(Vector3 position, Texture2D texture)
    {
        Particle particle = Instance.GetParticle();

        particle.StartParticle(position, texture);
    }
}
