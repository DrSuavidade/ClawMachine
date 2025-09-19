#nullable enable
using UnityEngine;

namespace Project.Machines
{
    /// <summary>Physical grabber that moves horizontally and vertically. The "clamp" is a trigger check against prizes.</summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class Grabber : MonoBehaviour
    {
        [Header("References")]
        public Transform hook;           // vertical moving part
        public Transform clampCollider;  // trigger collider area
        public LayerMask prizeMask;

        [Header("Motion")]
        public float horizontalSpeed = 2.0f;  // units/sec
        public float dropSpeed = 1.5f;        // units/sec
        public float liftSpeed = 1.2f;
        public float maxDropDepth = 2.0f;
        public float swinginess = 0.5f;       // sway multiplier

        private Rigidbody _rb = default!;
        private float _baseY;
        private Vector2 _moveInput;
        private bool _isDropping;
        private bool _isLifting;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.mass = 5f;
            _rb.linearDamping = 1f;
            _rb.angularDamping = 2f;
            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            _baseY = hook.localPosition.y;
        }

        public void SetMove(Vector2 move)
        {
            _moveInput = move;
        }

        private void FixedUpdate()
        {
            // Horizontal move with slight sway
            var targetVel = new Vector3(_moveInput.x, 0f, _moveInput.y) * horizontalSpeed;
            _rb.linearVelocity = new Vector3(targetVel.x, _rb.linearVelocity.y, targetVel.z);

            // Add sway torque for clumsy feel
            var sway = new Vector3(0f, 0f, -_moveInput.x) * swinginess;
            _rb.AddTorque(sway, ForceMode.Acceleration);

            // Vertical motion handled on hook local Y
            var lp = hook.localPosition;
            if (_isDropping)
            {
                lp.y = Mathf.Max(_baseY - maxDropDepth, lp.y - dropSpeed * Time.fixedDeltaTime);
                hook.localPosition = lp;
            }
            else if (_isLifting)
            {
                lp.y = Mathf.Min(_baseY, lp.y + liftSpeed * Time.fixedDeltaTime);
                hook.localPosition = lp;
                if (Mathf.Approximately(lp.y, _baseY)) _isLifting = false;
            }
        }

        public void BeginDrop() { _isDropping = true; _isLifting = false; }
        public void BeginLift() { _isDropping = false; _isLifting = true; }

        public Collider[] OverlapPrizesNonAlloc(Collider[] buffer)
        {
            if (buffer.Length == 0) return buffer;
            var pos = clampCollider.position;
            var radius = 0.25f;
            var count = Physics.OverlapSphereNonAlloc(pos, radius, buffer, prizeMask, QueryTriggerInteraction.Collide);
            // Resize manually (we’ll just return the same buffer with results up to count)
            return buffer;
        }
    }
}
