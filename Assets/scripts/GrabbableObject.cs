using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    public bool isGrabbed = false;
    private Rigidbody rb;
    private ControllerManager controllerManager;

    private Vector3 prevControllerPos;
    private Vector3 controllerVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controllerManager = FindObjectOfType<ControllerManager>();
    }

    void Update()
    {
        if (isGrabbed)
        {
            Vector3 currentPos = controllerManager.GetPosition();
            controllerVelocity = (currentPos - prevControllerPos) / Time.deltaTime;
            prevControllerPos = currentPos;
        }
    }

    public void Grab(float triggerPress)
    {
        if (triggerPress > 0.1f && !isGrabbed)
        {
            isGrabbed = true;

            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            prevControllerPos = controllerManager.GetPosition();

            transform.SetParent(controllerManager.transform);
        }
        else if (triggerPress <= 0.1f && isGrabbed)
        {
            isGrabbed = false;
            transform.SetParent(null);

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.linearVelocity = controllerVelocity;
            }
        }
    }
}