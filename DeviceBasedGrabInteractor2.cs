using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using System.Reflection;

// public static class XRGrabInteractableExtensions
// {
//     public static void GrabInteractable(CustomXRGrabInteractable interactable, XRBaseInteractor interactor)
//     {
//         MethodInfo grabMethod = typeof(XRBaseInteractable).GetMethod("Grab", BindingFlags.NonPublic | BindingFlags.Instance);
//         if (grabMethod != null)
//         {
//             grabMethod.Invoke(interactable, null);
//         } //new object[] { interactor }
//         Debug.Log("Grab function");
//     }
// }

public class DeviceBasedGrabInteractor2 : XRBaseInteractor
{
    public XRNode inputSource;
    public InputActionReference grabAction;
    public float grabDelay = 2f;

    private bool isGrabbing = false;
    public grabableObject currentInteractable;
    private InputAction inputAction;
    public LayerMask grabbableLayer;
    public Transform attachmentPoint;

    private GameObject grabbedObject;
    private Transform grabbedObjectAttachmentPoint;
    private Rigidbody grabbedObjectRigidbody;
    public float grabRange = 0.1f;

    protected override void OnEnable()
    {
        // base.OnEnable();
        // inputAction = grabAction.action;
        // inputAction.Enable();
        // inputAction.started += OnGrabAction;
        // inputAction.canceled += OnGrabAction;
        grabAction.action.Enable();
        grabAction.action.performed += OnGrabAction;
        grabAction.action.canceled += OnReleaseAction;
    }

    protected override void OnDisable()
    {
        // inputAction.started -= OnGrabAction;
        // inputAction.canceled -= OnGrabAction;
        // inputAction.Disable();
        // base.OnDisable();
        grabAction.action.Disable();
        grabAction.action.performed -= OnGrabAction;
        grabAction.action.canceled -= OnReleaseAction;
    }
    // private void OnTriggerEnter(Collider other)
    // {
    //     if (grabbableLayer == (grabbableLayer | (1 << other.gameObject.layer)))
    //     {
    //         grabbedObject = other.gameObject;
    //         grabbedObjectAttachmentPoint = grabbedObject.transform;
    //         grabbedObjectRigidbody = grabbedObject.GetComponent<Rigidbody>();
    //         Debug.Log("Collider1");
    //         grabbedObjectRigidbody.isKinematic = true; // Disable physics simulation when grabbed
    //     }
    // }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.gameObject == grabbedObject)
    //     {
    //         grabbedObjectRigidbody.isKinematic = false; // Enable physics simulation when released
    //         grabbedObjectRigidbody = null;
    //         grabbedObject = null;
    //         grabbedObjectAttachmentPoint = null;
    //         Debug.Log("Collider2");
    //     }
    // }
   
    private void OnReleaseAction(InputAction.CallbackContext context)
    {
        if (isGrabbing)
        {
            ReleaseObject();
        }
    }
    private void TryGrabObject()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, grabRange, grabbableLayer);
        if (colliders.Length > 0)
        {
            grabbedObject = colliders[0].gameObject;
            grabbedObjectRigidbody = grabbedObject.GetComponent<Rigidbody>();
            grabbedObjectRigidbody.isKinematic = true;
            isGrabbing = true;
        }
    }
    private void ReleaseObject()
    {
        grabbedObjectRigidbody.isKinematic = false;
        grabbedObjectRigidbody = null;
        grabbedObject = null;
        isGrabbing = false;
    }
    private void MoveGrabbedObject()
    {
        grabbedObject.transform.position = attachmentPoint.position;
        grabbedObject.transform.rotation = attachmentPoint.rotation;
    }


    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        //currentInteractable = args.interactableObject as CustomXRGrabInteractable;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        //currentInteractable = null;
        base.OnSelectExited(args);

        // Perform custom release logic here
        if (!args.isCanceled && !isGrabbing)
        {
            Debug.Log("WrningSelectExited");
            // Perform any necessary release actions
            isGrabbing = true;
        }
    }
   




    private IEnumerator DelayedGrab()
    {
       
        yield return new WaitForSeconds(grabDelay);
        isGrabbing = true;
        Debug.Log("DelayGrab1");
        
        if (isGrabbing && currentInteractable != null)
        {
            Debug.Log("DelayGrab2");
            TryGrabObject();;
          
            
            //XRGrabInteractableExtensions.GrabInteractable(currentInteractable, this);
            //StartCoroutine(DelayedRelease());
        }
    }

    private IEnumerator DelayedRelease()
    {
        Debug.Log("DelayRelease");
        yield return new WaitForSeconds(grabDelay);
        isGrabbing = false;
        if (currentInteractable != null)
        {
            ReleaseObject();
        }
    }
      private void OnGrabAction(InputAction.CallbackContext context)
    {
        if (!isGrabbing)
        {   
            Debug.Log("OnGrabAction1");
            StartCoroutine(DelayedGrab());
            //TryGrabObject();
        }
        else if (isGrabbing)
        {
            Debug.Log("OnGrabAction2");
            StartCoroutine(DelayedRelease());
        }
    }
    // private void OnGrabAction(InputAction.CallbackContext context)
    // {
    //     if (context.started)
    //     {
    //         if (!isGrabbing)
    //         {
    //             Debug.Log("OnGrabAction1");
    //             StartCoroutine(DelayedGrab());
                
    //         }
    //     }
    //     else if (context.canceled)
    //     {
    //         if (isGrabbing)
    //         {
    //             //StopCoroutine(DelayedGrab());
    //             Debug.Log("OnGrabAction2");
    //             StartCoroutine(DelayedRelease());
    //             // if (currentInteractable != null)
    //             // {
                    
    //             // }
    //         }
    //     }
    // }


    private void Update()
    {
        // if (!isGrabbing)
        //     return;

        // if (grabbedObject != null && grabbedObjectAttachmentPoint != null)
        // {
        //     grabbedObjectAttachmentPoint.position = attachmentPoint.position;
        //     grabbedObjectAttachmentPoint.rotation = attachmentPoint.rotation;

            // if (Input.GetButtonDown("Fire1")) // Change "Fire1" to the desired input for dropping
            // {
            //     DropObject();
            // }
        //}
        if (isGrabbing && grabbedObject != null)
        {
            MoveGrabbedObject();
        }
       
    }
}
