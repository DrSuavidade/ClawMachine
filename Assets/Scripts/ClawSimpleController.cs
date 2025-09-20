using System.Collections;
using UnityEngine;

public class ClawSimpleController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public Vector2 xzBounds = new Vector2(6f, 6f);
    public bool clampInsideBounds = true;

    [Header("Hook")]
    public Transform hook;                 // kinematic hook
    public Rigidbody hookRb;               // kinematic rigidbody
    public ClawGrabberSimple grabber;      // controls joint + open/close
    public ClawVisual clawVisual;          // finger animation

    [Header("Drop Motion")]
    public float dropDistance = 3.0f;
    public float descendSpeed = 3.5f;
    public float ascendSpeed = 4.5f;
    public float settlePause = 0.10f;

    [Header("Controls")]
    public KeyCode dropKey = KeyCode.Space;
    public KeyCode openKeyPrimary = KeyCode.Z;
    public KeyCode openKeyAlt = KeyCode.Return; // Enter

    float _hookHomeY;
    bool _busy;
    const float EPS = 0.0005f;

    bool HookAtHome => Mathf.Abs(hook.localPosition.y - _hookHomeY) <= EPS;

    void Awake()
    {
        _hookHomeY = hook.localPosition.y;
        if (hookRb)
        {
            hookRb.useGravity = false;
            hookRb.isKinematic = true;
            hookRb.interpolation = RigidbodyInterpolation.Interpolate;
            hookRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }
        // ensure we start OPEN
        grabber.SetOpen(true);
        if (clawVisual) clawVisual.SetOpen(false);
    }

    void Update()
    {
        // Manual OPEN (reset): allowed whenever not mid-sequence
        if (!_busy && (Input.GetKeyDown(openKeyPrimary) || Input.GetKeyDown(openKeyAlt)))
        {
            grabber.SetOpen(true);                 // opens + releases if holding
            if (clawVisual) clawVisual.SetOpen(false);
        }

        bool lockMovement = _busy || !HookAtHome;

        if (!lockMovement)
            HandleMove();
        else
            KeepHookUnderHeadXZ();

        // Start drop only if: not busy, hook is home, and claw is OPEN (armed)
        if (Input.GetKeyDown(dropKey) && !_busy && HookAtHome && grabber.IsOpen)
            StartCoroutine(DropRoutine());
    }

    void HandleMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 delta = new Vector3(h, 0f, v).normalized * moveSpeed * Time.deltaTime;
        transform.position += delta;

        if (clampInsideBounds)
        {
            Vector3 p = transform.position;
            p.x = Mathf.Clamp(p.x, -xzBounds.x, xzBounds.x);
            p.z = Mathf.Clamp(p.z, -xzBounds.y, xzBounds.y);
            transform.position = p;
        }

        KeepHookUnderHeadXZ();
    }

    void KeepHookUnderHeadXZ()
    {
        Vector3 hp = hook.position;
        hp.x = transform.position.x;
        hp.z = transform.position.z;
        hook.position = hp;
    }

    IEnumerator DropRoutine()
    {
        _busy = true;

        // FORCE OPEN for the whole descent
        grabber.SetOpen(true);
        if (clawVisual) clawVisual.SetOpen(false);

        // DESCEND (open)
        float targetY = _hookHomeY - dropDistance;
        while (hook.localPosition.y > targetY + EPS)
        {
            float y = hook.localPosition.y - descendSpeed * Time.deltaTime;
            hook.localPosition = new Vector3(hook.localPosition.x, Mathf.Max(y, targetY), hook.localPosition.z);
            KeepHookUnderHeadXZ();
            yield return null;
        }

        // settle, then CLOSE at bottom and try to grab
        if (settlePause > 0f) yield return new WaitForSeconds(settlePause);
        grabber.SetOpen(false);                   // now CLOSED
        if (clawVisual) clawVisual.SetOpen(true);
        grabber.TryAttachClosest();

        // ASCEND (stay CLOSED)
        while (hook.localPosition.y < _hookHomeY - EPS)
        {
            float y = hook.localPosition.y + ascendSpeed * Time.deltaTime;
            hook.localPosition = new Vector3(hook.localPosition.x, Mathf.Min(y, _hookHomeY), hook.localPosition.z);
            KeepHookUnderHeadXZ();
            yield return null;
        }

        // Snap home — STILL CLOSED. Player must press Z/Enter to reset/open.
        hook.localPosition = new Vector3(hook.localPosition.x, _hookHomeY, hook.localPosition.z);

        _busy = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.7f, 1f, 0.25f);
        Vector3 center = new Vector3(0f, transform.position.y, 0f);
        Gizmos.DrawWireCube(center, new Vector3(xzBounds.x * 2f, 0.2f, xzBounds.y * 2f));
    }
}
