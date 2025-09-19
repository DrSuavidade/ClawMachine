#nullable enable
using UnityEngine;
using Project.Data;

namespace Project.Machines
{
    /// <summary>Spawns prize items within a bounded pit volume.</summary>
    public sealed class SpawnGrid : MonoBehaviour
    {
        public Transform pitRoot = default!;
        public Vector3 pitSize = new(4f, 2f, 4f);

        public void FillPit(System.Random rng, LootTable table, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var def = table.Roll(rng);
                if (def == null || def.PrizePrefab == null) continue;
                var pos = RandomPointInBox(rng, pitRoot.position, pitSize);
                var rot = RandomRotationY(rng);
                var go = Object.Instantiate(def.PrizePrefab, pos, rot, pitRoot);
                var rb = go.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                    rb.interpolation = RigidbodyInterpolation.Interpolate;
                }
                var prizeComp = go.GetComponent<PrizeItem>() ?? go.AddComponent<PrizeItem>();
                prizeComp.Definition = def;
            }
        }

        private static Vector3 RandomPointInBox(System.Random rng, Vector3 center, Vector3 size)
        {
            return new Vector3(
                center.x + ((float)rng.NextDouble() - 0.5f) * size.x,
                center.y + size.y * 0.5f, // spawn slightly above to fall in
                center.z + ((float)rng.NextDouble() - 0.5f) * size.z
            );
        }

        private static Quaternion RandomRotationY(System.Random rng)
        {
            float y = (float)rng.NextDouble() * 360f;
            return Quaternion.Euler(0f, y, 0f);
        }
    }
}
