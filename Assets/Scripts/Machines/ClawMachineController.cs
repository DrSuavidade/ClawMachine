#nullable enable
using UnityEngine;
using UnityEngine.Events;
using Project.Core;
using Project.Economy;
using Project.Inventory;
using Project.Data;

namespace Project.Machines
{
    /// <summary>State machine for a single claw machine.</summary>
    public sealed class ClawMachineController : MonoBehaviour
    {
        [Header("Config & References")]
        public MachineConfig Config = default!;
        public Grabber Grabber = default!;
        public SpawnGrid Spawn = default!;
        public ClawInput Input = default!;

        [Header("UI Events")]
        public UnityEvent<string>? OnMachineName;
        public UnityEvent<int>? OnCreditsChanged;
        public UnityEvent<int>? OnTicketsChanged;
        public UnityEvent<string>? OnHint;

        private EconomyService _econ = default!;
        private InventoryService _inv = default!;
        private System.Random _rng = new(1337);
        private ClawState _state = ClawState.Idle;
        private float _driftTimer;

        // Non-alloc buffer
        private readonly Collider[] _overlapBuffer = new Collider[8];

        private void Start()
        {
            _econ = ServiceLocator.Get<EconomyService>();
            _inv = ServiceLocator.Get<InventoryService>();
            OnMachineName?.Invoke(Config.DisplayName);
            OnCreditsChanged?.Invoke(_econ.Credits);
            OnTicketsChanged?.Invoke(_econ.Tickets);

            // Fill pit with initial loot
            Spawn.pitSize = Config.PitBounds;
            Spawn.FillPit(_rng, Config.LootTable, Config.PitInitialItems);

            // Wire input
            Input.OnMove.AddListener(HandleMoveInput);
            Input.OnDrop.AddListener(HandleDrop);
            Input.OnClamp.AddListener(HandleClamp);
            Input.OnCancel.AddListener(HandleCancel);

            ApplyConfigToGrabber();
            SetState(ClawState.Positioning);
            OnHint?.Invoke("Move the claw. Press Space to Drop, LMB to Clamp.");
        }

        private void ApplyConfigToGrabber()
        {
            Grabber.dropSpeed = Config.DropSpeed;
            Grabber.liftSpeed = Config.LiftSpeed;
            Grabber.maxDropDepth = Config.MaxDropDepth;
            Grabber.swinginess = Config.Swinginess;
        }

        private void Update()
        {
            if (_state == ClawState.Positioning)
            {
                // Random drift to make controls clumsy
                _driftTimer -= Time.deltaTime;
                if (_driftTimer <= 0f)
                {
                    _driftTimer = 0.35f;
                    var drift = new Vector2(
                        RandomRangeSigned(Config.Drift * 0.5f),
                        RandomRangeSigned(Config.Drift * 0.5f)
                    );
                    Grabber.SetMove(drift);
                }
            }
        }

        private float RandomRangeSigned(float mag)
        {
            return (float)(_rng.NextDouble() * 2.0 - 1.0) * mag;
        }

        private void HandleMoveInput(Vector2 input)
        {
            if (_state != ClawState.Positioning) return;
            if (input.magnitude < Config.HorizontalDeadZone) input = Vector2.zero;
            // Blend player input on top of drift (grabber FixedUpdate mixes velocity)
            Grabber.SetMove(input);
        }

        private void HandleDrop()
        {
            if (_state != ClawState.Positioning) return;
            if (!CanAffordPlay()) { OnHint?.Invoke("Not enough credits/tickets!"); return; }
            ConsumePlayCost();
            SetState(ClawState.Dropping);
            Grabber.BeginDrop();
            OnHint?.Invoke("Dropping… press Clamp at the right time!");
        }

        private void HandleClamp()
        {
            if (_state != ClawState.Dropping) return;
            SetState(ClawState.Clamping);

            // Check overlap for prizes under claw
            Grabber.OverlapPrizesNonAlloc(_overlapBuffer);
            // Decide capture using ClampStrength with slight random
            bool captured = false;
            PrizeItem? target = null;

            for (int i = 0; i < _overlapBuffer.Length; i++)
            {
                var col = _overlapBuffer[i];
                if (col == null) continue;
                if (!col.TryGetComponent<PrizeItem>(out var prize)) continue;

                float chance = Mathf.Clamp01(Config.ClampStrength + (float)_rng.NextDouble() * 0.2f - 0.1f);
                if ((float)_rng.NextDouble() <= chance)
                {
                    target = prize;
                    captured = true;
                    break;
                }
            }

            if (captured && target != null)
            {
                // Parent the prize to the hook so it lifts
                var rb = target.Rb;
                rb.isKinematic = true;
                target.transform.SetParent(Grabber.hook, true);
                OnHint?.Invoke($"Got: {target.Definition.DisplayName}! Lifting…");
            }
            else
            {
                OnHint?.Invoke("Missed! Lifting…");
            }

            SetState(ClawState.Lifting);
            Grabber.BeginLift();
            Invoke(nameof(ResolvePayout), 1.0f);
        }

        private void HandleCancel()
        {
            // Reserved (pause/back)
        }

        private bool CanAffordPlay()
        {
            if (Config.CreditCost > 0 && _econ.Credits < Config.CreditCost) return false;
            if (Config.TicketCost > 0 && _econ.Tickets < Config.TicketCost) return false;
            return true;
        }

        private void ConsumePlayCost()
        {
            if (Config.CreditCost > 0) _econ.SpendCredits(Config.CreditCost);
            if (Config.TicketCost > 0) _econ.SpendTickets(Config.TicketCost);
            OnCreditsChanged?.Invoke(_econ.Credits);
            OnTicketsChanged?.Invoke(_econ.Tickets);
        }

        private void ResolvePayout()
        {
            if (_state != ClawState.Lifting && _state != ClawState.Clamping) return;

            // If a prize is parented to hook, award and remove from pit
            PrizeItem? carried = null;
            foreach (Transform child in Grabber.hook)
            {
                if (child.TryGetComponent<PrizeItem>(out var prize))
                {
                    carried = prize; break;
                }
            }

            if (carried != null)
            {
                var def = carried.Definition;
                if (_inv.Add(def.Id))
                {
                    if (def.CreditReward > 0) _econ.AddCredits(def.CreditReward);
                    if (def.TicketReward > 0) _econ.AddTickets(def.TicketReward);
                    OnCreditsChanged?.Invoke(_econ.Credits);
                    OnTicketsChanged?.Invoke(_econ.Tickets);
                    OnHint?.Invoke($"Collected {def.DisplayName}! (+{def.CreditReward}C, +{def.TicketReward}T)");
                }
                Destroy(carried.gameObject);
            }

            // Refill the pit lightly to keep density (optional)
            var newDef = Config.LootTable.Roll(_rng);
            if (newDef != null)
            {
                Spawn.FillPit(_rng, Config.LootTable, 1);
            }

            SetState(ClawState.Positioning);
        }

        private void SetState(ClawState s) => _state = s;
    }
}
