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
    public float breakForce = 1500f;
    public float breakTorque = 800f;

    FixedJoint _joint;
    Rigidbody _held;
    bool _open = true;

    public bool IsOpen => _open;
    public bool IsHolding => _held != null;

    public void SetOpen(bool open)
    {
        _open = open;
        if (_open && IsHolding) Release();
    }

    public void TryAttachClosest()
    {
        if (_open || IsHolding) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, grabRadius, prizeMask);
        if (hits == null || hits.Length == 0) return;

        Collider best = hits.OrderBy(c => (c.ClosestPoint(transform.position) - transform.position).sqrMagnitude).First();
        Rigidbody rb = best.attachedRigidbody;
        if (!rb || rb.mass > maxCarryMass) return;

        _joint = gameObject.AddComponent<FixedJoint>();
        _joint.connectedBody = rb;
        _joint.breakForce = breakForce;
        _joint.breakTorque = breakTorque;
        _held = rb;
    }

    public void Release()
    {
        if (_joint) Destroy(_joint);
        _joint = null;
        _held = null;
    }

    void OnJointBreak(float force)
    {
        _joint = null;
        _held = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.7f, 0.1f, 0.25f);
        Gizmos.DrawSphere(transform.position, grabRadius);
    }
}
