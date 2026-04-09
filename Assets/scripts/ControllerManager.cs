/***
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    public GameObject selectedObject;
    public GameObject grabbedObject;

    public Vector3 GetPointingDir()
    {
        return transform.forward;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    void Update()
    {
        GetButtonPress();
        GetTriggerPress();
    }

    public void GetButtonPress()
    {
        if (OVRInput.GetUp(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            CastRay();
        }
    }

    public void CastRay()
    {
        Ray ray = new Ray(GetPosition(), GetPointingDir());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;
            SelectableObject selectable = hitObject.GetComponent<SelectableObject>();

            if (selectable != null)
            {
                if (selectedObject != null && selectedObject != hitObject)
                {
                    SelectableObject prevSelectable = selectedObject.GetComponent<SelectableObject>();
                    if (prevSelectable != null)
                        prevSelectable.Highlight();
                }

                selectedObject = hitObject;
                selectable.Highlight();
            }
            else
            {
                selectedObject = null;
            }
        }
        else
        {
            selectedObject = null;
        }
    }

    public void GetTriggerPress()
    {
        float triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch);

        if (triggerValue > 0.1f)
        {
            GrabObject(triggerValue);
        }
        else
        {
            if (grabbedObject != null)
            {
                GrabbableObject grabbable = grabbedObject.GetComponent<GrabbableObject>();
                if (grabbable != null)
                    grabbable.Grab(0f);

                grabbedObject = null;
            }
        }
    }

    public void GrabObject(float pressedValue)
    {
        if (grabbedObject != null)
        {
            grabbedObject.GetComponent<GrabbableObject>().Grab(pressedValue);
            return;
        }

        float grabRadius = 10f;
        Collider[] nearbyColliders = Physics.OverlapSphere(GetPosition(), grabRadius);

        GameObject closest = null;
        float closestDist = float.MaxValue;

        foreach (Collider col in nearbyColliders)
        {
            GrabbableObject grabbable = col.gameObject.GetComponent<GrabbableObject>();
            if (grabbable != null)
            {
                float dist = Vector3.Distance(GetPosition(), col.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = col.gameObject;
                }
            }
        }

        if (closest != null)
        {
            grabbedObject = closest;
            grabbedObject.GetComponent<GrabbableObject>().Grab(pressedValue);
        }
    }
}
***/
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    public GameObject selectedObject;
    public GameObject grabbedObject;

    // For regular grab
    private Vector3 prevControllerPos;
    private Vector3 controllerVelocity;

    // For ray grab
    private GameObject rayGrabbedObject;
    private float rayGrabDistance;
    private GameObject rayGrabAnchor;

    public Vector3 GetPointingDir()
    {
        return transform.forward;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    void Update()
    {
        GetButtonPress();
        GetTriggerPress();
        HandleRayGrab();
    }

    public void GetButtonPress()
    {
        if (OVRInput.GetUp(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            CastRay();
        }
    }

    public void CastRay()
    {
        Ray ray = new Ray(GetPosition(), GetPointingDir());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;
            SelectableObject selectable = hitObject.GetComponent<SelectableObject>();

            if (selectable != null)
            {
                if (selectedObject != null && selectedObject != hitObject)
                {
                    SelectableObject prevSelectable = selectedObject.GetComponent<SelectableObject>();
                    if (prevSelectable != null)
                        prevSelectable.Highlight();
                }

                selectedObject = hitObject;
                selectable.Highlight();
            }
            else
            {
                selectedObject = null;
            }
        }
        else
        {
            selectedObject = null;
        }
    }

    // ---- Regular grab (trigger) ----

    public void GetTriggerPress()
    {
        float triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch);

        if (triggerValue > 0.1f)
        {
            GrabObject(triggerValue);
        }
        else
        {
            if (grabbedObject != null)
            {
                GrabbableObject grabbable = grabbedObject.GetComponent<GrabbableObject>();
                if (grabbable != null)
                    grabbable.Grab(0f);

                grabbedObject = null;
            }
        }
    }

    public void GrabObject(float pressedValue)
    {
        if (grabbedObject != null)
        {
            grabbedObject.GetComponent<GrabbableObject>().Grab(pressedValue);
            return;
        }

        float grabRadius = 10f;
        Collider[] nearbyColliders = Physics.OverlapSphere(GetPosition(), grabRadius);

        GameObject closest = null;
        float closestDist = float.MaxValue;

        foreach (Collider col in nearbyColliders)
        {
            GrabbableObject grabbable = col.gameObject.GetComponent<GrabbableObject>();
            if (grabbable != null)
            {
                float dist = Vector3.Distance(GetPosition(), col.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = col.gameObject;
                }
            }
        }

        if (closest != null)
        {
            grabbedObject = closest;
            grabbedObject.GetComponent<GrabbableObject>().Grab(pressedValue);
        }
    }

    // ---- Ray grab (index trigger) ----

    void HandleRayGrab()
{
    float indexTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
    bool bButton = OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.RTouch);

    bool rayGrabActive = indexTrigger > 0.1f || bButton;

    if (rayGrabActive)
    {
        if (rayGrabbedObject == null)
        {
            Ray ray = new Ray(GetPosition(), GetPointingDir());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GrabbableObject grabbable = hit.collider.gameObject.GetComponent<GrabbableObject>();
                if (grabbable != null)
                {
                    rayGrabbedObject = hit.collider.gameObject;
                    rayGrabDistance = hit.distance;

                    rayGrabAnchor = new GameObject("RayGrabAnchor");
                    rayGrabAnchor.transform.position = rayGrabbedObject.transform.position;
                    rayGrabAnchor.transform.rotation = rayGrabbedObject.transform.rotation;
                    rayGrabAnchor.transform.SetParent(this.transform);

                    rayGrabbedObject.transform.SetParent(rayGrabAnchor.transform);

                    Rigidbody rb = rayGrabbedObject.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = true;
                        rb.useGravity = false;
                    }
                }
            }
        }
        else
        {
            Vector2 joystick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
            float pushPullSpeed = 3f;

            rayGrabDistance += joystick.y * pushPullSpeed * Time.deltaTime;
            rayGrabDistance = Mathf.Clamp(rayGrabDistance, 0.5f, 20f);

            rayGrabAnchor.transform.position = GetPosition() + GetPointingDir() * rayGrabDistance;
        }
    }
    else
    {
        if (rayGrabbedObject != null)
        {
            Rigidbody rb = rayGrabbedObject.GetComponent<Rigidbody>();

            rayGrabbedObject.transform.SetParent(null);

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.linearVelocity = (rayGrabAnchor.transform.position - rayGrabbedObject.transform.position) / Time.deltaTime;
            }

            Destroy(rayGrabAnchor);
            rayGrabbedObject = null;
            rayGrabAnchor = null;
        }
    }
}
}