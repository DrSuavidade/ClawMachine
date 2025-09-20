using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class ClawGrabberSimple : MonoBehaviour
{
    [Header("Detection")]
    public float grabRadius = 0.45f;
    public LayerMask prizeMask;

    [Header("Grip")]
    public float maxCarryMass = 5f;

    Rigidbody _hookRb;          // our own RB (kinematic)
    Rigidbody _heldRb;          // prize rigidbody we’re carrying
    Transform _heldTf;
    bool _open = true;

    public bool IsOpen => _open;
    public bool IsHolding => _heldRb != null;

    void Awake()
    {
        _hookRb = GetComponent<Rigidbody>();
    }

    public void SetOpen(bool open)
    {
        _open = open;
        if (_open && IsHolding) Release();
    }

    public void TryAttachClosest()
    {
        if (_open || IsHolding) return;

        // Find nearest eligible prize
        Collider[] hits = Physics.OverlapSphere(transform.position, grabRadius, prizeMask);
        if (hits == null || hits.Length == 0) return;

        Collider best = hits
            .OrderBy(c => (c.ClosestPoint(transform.position) - transform.position).sqrMagnitude)
            .First();

        Rigidbody rb = best.attachedRigidbody;
        if (!rb) return;
        if (rb == _hookRb) return;              // safety: don’t “grab” ourselves
        if (rb.mass > maxCarryMass) return;

        // Parent carry: make prize ride with the hook
        _heldRb = rb;
        _heldTf = rb.transform;

        // Freeze physics so it follows smoothly
        _heldRb.linearVelocity = Vector3.zero;
        _heldRb.angularVelocity = Vector3.zero;
        _heldRb.useGravity = false;
        _heldRb.isKinematic = true;

        // Keep world position/rotation when parenting
        _heldTf.SetParent(transform, true);
    }

    public void Release()
    {
        if (!IsHolding) return;

        // Unparent and restore physics
        _heldTf.SetParent(null, true);
        _heldRb.isKinematic = false;
        _heldRb.useGravity = true;

        _heldRb = null;
        _heldTf = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.7f, 0.1f, 0.25f);
        Gizmos.DrawSphere(transform.position, grabRadius);
    }
}
