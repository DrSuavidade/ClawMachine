using System.Collections;
using UnityEngine;

public class ClawVisual : MonoBehaviour
{
    [System.Serializable]
    public class Finger
    {
        public Transform pivot;          // the hinge pivot (child of FingerX)
        public Transform bar;            // optional (for gizmo length)
        [Range(-60f, 0f)] public float closedAngle = -35f;
        [Range(0f, 85f)] public float openAngle = 50f;
    }

    public Finger[] fingers = new Finger[3];
    public float openCloseSpeed = 360f;     // deg/sec
    public bool startOpen = true;

    bool _open;
    Coroutine _anim;

    void Awake()
    {
        _open = startOpen;
        ApplyInstant(_open ? 1f : 0f);
    }

    public void SetOpen(bool open)
    {
        if (_open == open) return;
        _open = open;
        if (_anim != null) StopCoroutine(_anim);
        _anim = StartCoroutine(AnimateTo(_open));
    }

    public bool IsOpen => _open;

    IEnumerator AnimateTo(bool toOpen)
    {
        // We’ll animate by moving each pivot's localRotation.z between angles
        while (true)
        {
            float step = openCloseSpeed * Time.deltaTime;
            bool allDone = true;

            foreach (var f in fingers)
            {
                if (!f.pivot) continue;
                float target = toOpen ? f.openAngle : f.closedAngle;
                // we rotate around local X to “curl” the finger; use Z if you built them differently
                Vector3 e = f.pivot.localEulerAngles;
                float current = NormalizeAngle(e.x);
                float next = Mathf.MoveTowards(current, target, step);
                if (Mathf.Abs(next - target) > 0.001f) allDone = false;
                e.x = next;
                f.pivot.localEulerAngles = e;
            }

            if (allDone) break;
            yield return null;
        }
        _anim = null;
    }

    void ApplyInstant(float t01)
    {
        foreach (var f in fingers)
        {
            if (!f.pivot) continue;
            float ang = Mathf.Lerp(f.closedAngle, f.openAngle, t01);
            var e = f.pivot.localEulerAngles;
            e.x = ang;
            f.pivot.localEulerAngles = e;
        }
    }

    float NormalizeAngle(float a)
    {
        // Convert 0..360 to -180..180 for easier MoveTowards
        a %= 360f;
        if (a > 180f) a -= 360f;
        return a;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0.1f, 0.5f);
        if (fingers == null) return;
        foreach (var f in fingers)
        {
            if (f?.pivot)
                Gizmos.DrawWireSphere(f.pivot.position, 0.03f);
        }
    }
}
