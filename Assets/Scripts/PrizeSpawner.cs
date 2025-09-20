using UnityEngine;

public class PrizeSpawner : MonoBehaviour
{
    public int initialCount = 25;
    public Vector3 areaSize = new Vector3(10f, 0.2f, 10f);
    public Vector2 scaleRange = new Vector2(0.3f, 0.8f);
    public Material[] prizeMats; // optional nice colors

    public LayerMask groundMask;

    void Start()
    {
        for (int i = 0; i < initialCount; i++)
            SpawnOne();
    }

    void SpawnOne()
    {
        bool makeCylinder = Random.value > 0.5f;
        GameObject g = makeCylinder
            ? GameObject.CreatePrimitive(PrimitiveType.Cylinder)
            : GameObject.CreatePrimitive(PrimitiveType.Cube);

        float s = Random.Range(scaleRange.x, scaleRange.y);
        g.transform.localScale = makeCylinder ? new Vector3(s, s * 0.6f, s) : Vector3.one * s;

        Vector3 pos = transform.position + new Vector3(
            Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f),
            2f + Random.Range(0f, 1f),
            Random.Range(-areaSize.z * 0.5f, areaSize.z * 0.5f)
        );

        g.transform.position = pos;
        g.transform.rotation = Random.rotation;

        if (prizeMats != null && prizeMats.Length > 0)
            g.GetComponent<Renderer>().material = prizeMats[Random.Range(0, prizeMats.Length)];

        var col = g.GetComponent<Collider>();
        col.material = new PhysicsMaterial { dynamicFriction = 0.6f, staticFriction = 0.7f, bounciness = 0.05f };

        g.AddComponent<Rigidbody>();
        g.AddComponent<Prize>();
        g.layer = LayerMaskToLayer(groundMask);
    }

    int LayerMaskToLayer(LayerMask mask)
    {
        int val = mask.value;
        int layer = 0;
        while (val > 1) { val >>= 1; layer++; }
        return layer;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.6f, 0f, 0.25f);
        Gizmos.DrawCube(transform.position, new Vector3(areaSize.x, 0.1f, areaSize.z));
    }
}
