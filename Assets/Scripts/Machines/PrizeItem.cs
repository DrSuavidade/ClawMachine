#nullable enable
using UnityEngine;
using Project.Data;

namespace Project.Machines
{
    /// <summary>Runtime component on prize prefabs to identify prize definition.</summary>
    public sealed class PrizeItem : MonoBehaviour
    {
        public PrizeItemDef Definition = default!;
        public Rigidbody Rb = default!;

        private void Awake()
        {
            if (!Rb) Rb = GetComponent<Rigidbody>();
        }
    }
}
