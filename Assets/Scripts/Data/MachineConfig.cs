#nullable enable
using UnityEngine;

namespace Project.Data
{
    [CreateAssetMenu(menuName = "Data/Claw Machine Config", fileName = "MachineConfig")]
    public sealed class MachineConfig : ScriptableObject
    {
        [Header("Identity")]
        public string MachineId = "free_machine";
        public string DisplayName = "Free Clumsy Machine";

        [Header("Economy")]
        [Min(0)] public int CreditCost = 0;
        [Min(0)] public int TicketCost = 0;

        [Header("Control Fidelity (Free machine = sloppy)")]
        [Range(0f, 1f)] public float HorizontalDeadZone = 0.15f;
        [Range(0f, 1f)] public float Drift = 0.35f; // random drift applied while moving
        [Range(0f, 1f)] public float Swinginess = 0.6f; // pendulum-like sway
        [Range(0f, 1f)] public float ClampStrength = 0.25f; // probability helper in grab

        [Header("Timings (sec)")]
        [Min(0f)] public float DropSpeed = 1.5f; // units/sec
        [Min(0f)] public float LiftSpeed = 1.2f;
        [Min(0f)] public float MaxDropDepth = 2.0f;

        [Header("Loot")]
        public LootTable LootTable = default!;
        [Min(1)] public int PitInitialItems = 15;
        public Vector3 PitBounds = new(4f, 2f, 4f);
    }
}
