using UnityEngine;

public class GlobalReferences : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
public static GlobalReferences Instance { get; set; }

    public GameObject bulletImpactEffectPrefab;
    public GameObject grenadeExplosionEffect;
    public GameObject smokeGrenadeEffect;

    public int waveNumber;

    private void Awake()
    {
        if (Instance !=null && Instance!=this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
