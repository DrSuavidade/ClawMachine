#nullable enable
using UnityEngine;

namespace Project.Data
{
    [CreateAssetMenu(menuName = "Data/Loot Table", fileName = "LootTable")]
    public sealed class LootTable : ScriptableObject
    {
        [System.Serializable]
        public struct Entry
        {
            public PrizeItemDef Prize;
            [Min(0f)] public float Weight;
        }

        [field: SerializeField] public Entry[] Entries { get; private set; } = System.Array.Empty<Entry>();

        public PrizeItemDef? Roll(System.Random rng)
        {
            if (Entries == null || Entries.Length == 0) return null;
            float total = 0f;
            foreach (var e in Entries) total += Mathf.Max(0f, e.Weight);
            if (total <= 0f) return null;

            var pick = (float)rng.NextDouble() * total;
            foreach (var e in Entries)
            {
                var w = Mathf.Max(0f, e.Weight);
                if (pick <= w) return e.Prize;
                pick -= w;
            }
            return Entries[Entries.Length - 1].Prize; // fallback
        }
    }
}
