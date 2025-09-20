using System.Collections;
using UnityEngine;

public class ClawSimpleController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public Vector2 xzBounds = new Vector2(6f, 6f);
    public bool clampInsideBounds = true;

    [Header("Hook")]
    public Transform hook;                 // Hook transform (child of Head)
    public Rigidbody hookRb;               // Hook rigidbody (KINEMATIC)
    public ClawGrabberSimple grabber;      // Hook grabber
    public float dropDistance = 3.0f;      // meters down from home localY
    public float descendSpeed = 3.5f;      // m/s
    public float ascendSpeed = 4.5f;       // m/s
    public float settlePause = 0.10f;

    [Header("Controls")]
    public KeyCode dropKey = KeyCode.Space;

    float _hookHomeY;          // hook local Y at rest
    bool _busy;                // true during the whole drop→close→retract
    const float EPS = 0.0005f; // tiny epsilon for float compares

    bool HookAtHome => Mathf.Abs(hook.localPosition.y - _hookHomeY) <= EPS;

    void Awake()
    {
        if (!hook) Debug.LogError("Hook not set on ClawSimpleController.");
        _hookHomeY = hook.localPosition.y;

        if (hookRb)
        {
            hookRb.useGravity = false;
            hookRb.isKinematic = true;
            hookRb.interpolation = RigidbodyInterpolation.Interpolate;
            hookRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }
    }

    void Update()
    {
        // Lock movement whenever the hook isn't home or we're mid-sequence
        bool lockMovement = _busy || !HookAtHome;

        if (!lockMovement)
            HandleMove();
        else
            KeepHookUnderHeadXZ(); // still keep XZ aligned even when locked

        if (Input.GetKeyDown(dropKey) && !lockMovement)
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
        // Keep hook under the head horizontally; do not change its local Y
        Vector3 hp = hook.position;
        hp.x = transform.position.x;
        hp.z = transform.position.z;
        hook.position = hp;
    }

    IEnumerator DropRoutine()
    {
        _busy = true;

        // Ensure open and not holding anything
        grabber.SetOpen(true);

        // DESCEND
        float targetY = _hookHomeY - dropDistance;
        while (hook.localPosition.y > targetY + EPS)
        {
            float y = hook.localPosition.y - descendSpeed * Time.deltaTime;
            hook.localPosition = new Vector3(hook.localPosition.x, Mathf.Max(y, targetY), hook.localPosition.z);
            KeepHookUnderHeadXZ(); // maintain XZ under head
            yield return null;
        }

        if (settlePause > 0f) yield return new WaitForSeconds(settlePause);

        // CLOSE & try to grab
        grabber.SetOpen(false);
        grabber.TryAttachClosest();

        // ASCEND
        while (hook.localPosition.y < _hookHomeY - EPS)
        {
            float y = hook.localPosition.y + ascendSpeed * Time.deltaTime;
            hook.localPosition = new Vector3(hook.localPosition.x, Mathf.Min(y, _hookHomeY), hook.localPosition.z);
            KeepHookUnderHeadXZ();
            yield return null;
        }

        // Snap exactly home
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
