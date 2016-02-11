using UnityEngine;
using System.Collections;

public abstract class Mob : MonoBehaviour
{

    public float moveTime = 0.1f;
    public LayerMask blockingLayer;

    private BoxCollider boxCollider;
    private Rigidbody rBody;
    private float inverseMoveTime;

    // Use this for initialization
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        rBody = GetComponent<Rigidbody>();
        inverseMoveTime = 1f / moveTime;
    }

    protected bool Move(int xDir, int yDir, out RaycastHit hit)
    {
        Vector3 start = transform.position;
        Vector3 end = start + new Vector3(xDir, 0f, yDir);
        // temp disable our own collider and cast a ray to see if we can move
        // in the requested direction

        boxCollider.enabled = false;
        Debug.DrawLine(start, end, Color.red, 1f, false);
        Physics.Linecast(start, end, out hit, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        // something is in the way, we did not move
        return false;
    }

    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rBody.position, end, inverseMoveTime * Time.deltaTime);
            rBody.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }

    protected virtual void OnMove<T>(int xDir, int yDir)
    where T : Component
    {
        //Hit will store whatever our linecast hits when Move is called.

        RaycastHit hit;
        bool moved = Move(xDir, yDir, out hit);

        if (hit.transform == null)
        {
            return;
        }

        // get a reference to what we hit
        T hitComponent = hit.transform.GetComponent<T>();

        if (!moved && hitComponent != null)
        {
            // we are blocked by something we can interact with
            Interact(hitComponent);
        }
    }

    protected abstract void Interact<T>(T component)
    where T : Component;
}
