using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class DeviceBasedGrabInteractor : XRBaseInteractor
{
    public XRNode inputSource;
    public InputFeatureUsage<bool> grabButton;
    public float grabDelay = 2f;

    private bool isGrabbing = false;
    public XRGrabInteractable currentInteractable;
    public InputDevice inputDevice; //default(InputDevice);

    protected override void OnEnable()
    {
        base.OnEnable();
        inputDevice = InputDevices.GetDeviceAtXRNode(inputSource);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        inputDevice = default(InputDevice);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        currentInteractable = args.interactableObject as XRGrabInteractable;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        currentInteractable = null;
    }

    private IEnumerator DelayedGrab()
    {
        // Wait for the specified delay duration
        yield return new WaitForSeconds(grabDelay);
        Debug.Log("WARNING1");
        // Perform the grab action here
        if (isGrabbing)
        {
            Debug.Log("WARNING2");
            // Check if there is a valid interactable object
            if (currentInteractable != null)
            {
                // Call the GrabInteractable method on the interactable
                //currentInteractable.GrabInteractable(this);
            }
        }
    }

    private void CheckGrabInput()
    {
        // Check if the grab feature is being pressed
        bool isGrabButtonPressed = false;
        if (inputDevice.isValid)
        {
            inputDevice.TryGetFeatureValue(grabButton, out isGrabButtonPressed);
            Debug.Log("WARNING3");
        }

        // Start or stop the delayed grab based on the grab input
        if (isGrabButtonPressed)
        {
            if (!isGrabbing)
            {
                // Start the delayed grab coroutine
                isGrabbing = true;
                StartCoroutine(DelayedGrab());
                
            }
        }
        else
        {
            if (isGrabbing)
            {
                // Stop the delayed grab coroutine
                //StopCoroutine(DelayedGrab());
                isGrabbing = false;
            }
        }
    }

    private void Update()
    {
        CheckGrabInput();
    }
}
