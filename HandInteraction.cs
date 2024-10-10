using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;


[System.Serializable]public class AnimationInput
{
    public string animationPropertyName;
    public InputActionProperty action;
    
}

public class HandInteraction : MonoBehaviour
{
    public LayerMask grabbableLayer;
    public Transform attachmentPoint;
    public float grabRange = 0.1f;
    public InputActionReference grabAction;
    public InputActionReference fireAction;
    public ExperimentController experimentController;
    //public float grabDelay; //experimentController.GrabDelayValue;
    

    private GameObject grabbedObject;
    private Transform grabbedObjectAttachmentPoint;
    private Rigidbody grabbedObjectRigidbody;

    private List<Transform> grabbedObjectAttachmentPoints = new List<Transform>();
    private List<grabableObject> grabbedObjectComponents = new List<grabableObject>();
    private bool isGrabbing = false;
    public Transform ControllerReal;
    public Transform ControllerLag;
    //public float delayTime;
    [SerializeField] ActionBasedController actionController;
    private InputActionProperty selectAction;
    private InputActionProperty selectActionValue;
    List<Vector3> CTransform;
    List<Quaternion> CTransformR;
    //List<float> grabAnimate;
    InteractionState interactionState;
    //AnimateOnInput animateOnInput;
    //public float animationDelay = 2f;
    public Animator animator;
    public List<AnimationInput> animationInputs;
    //public AnimationInput[] animationInputs;

    //private bool isAnimating = false;
    private List<float> actionValues = new List<float>();

    // public delegate void GrabStartAction();
    // public static event GrabStartAction OnGrabStart;

    // public delegate void GrabEndAction();
    // public static event GrabEndAction OnGrabEnd;
    

    void Start()
    {
        
        CTransform = new List<Vector3>();
        CTransformR = new List<Quaternion>();
        StartCoroutine(mainloop());
        StartCoroutine(addController());
        //ExperimentController.OnGrabDelayValueChanged += OnGrabDelayValueChangedHandler;
        
        
    }
// public void SetGrabDelay(float newGrabDelay)
//     {
//         grabDelay = newGrabDelay;
//     }
// public void SetAnimator(Animator newAnimator)
//     {
//         animator = newAnimator;
//     }
    private void OnEnable()
    {
        grabAction.action.Enable();
        grabAction.action.performed += OnGrabAction;
        grabAction.action.canceled += OnGrabAction;

        //ExperimentController.Instance.OnGrabDelayValueChanged -= OnGrabDelayValueChangedHandler;

        //ExperimentController.OnVRCharacterTypeChanged += OnVRCharacterTypeChangedHandler;
        //OnGrabStart += StartAnimating;
        //OnGrabEnd += StopAnimating;
    }

    private void OnDisable()
    {
        grabAction.action.Disable();
        grabAction.action.performed -= OnGrabAction;
        grabAction.action.canceled -= OnGrabAction;
        //ExperimentController.Instance.OnGrabDelayValueChanged -= OnGrabDelayValueChangedHandler;

        //ExperimentController.OnVRCharacterTypeChanged -= OnVRCharacterTypeChangedHandler;
        //OnGrabStart -= StartAnimating;
        //OnGrabEnd -= StopAnimating;
    }
     private void Awake()
    {
        experimentController = FindObjectOfType<ExperimentController>();
        if (experimentController != null)
        {
            ExperimentController.OnGrabDelayValueChanged += OnGrabDelayValueChangedHandler;
            
        }
    }
    private void OnDestroy()
    {
        if (experimentController != null)
        {
            ExperimentController.OnGrabDelayValueChanged -= OnGrabDelayValueChangedHandler;
            
        }
    }
    //  private void OnVRCharacterTypeChangedHandler(GameObject vrCharacter)
    // {
    //     Animator vrCharacterAnimator = vrCharacter.GetComponent<Animator>();
    //     if (vrCharacterAnimator != null)
    //     {
    //         animator = vrCharacterAnimator;
    //     }
    //     else
    //     {
    //         Debug.LogError("Animator component not found on the selected VR Character Type GameObject.");
    //     }
    // }
    public void OnGrabDelayValueChangedHandler(float newDelay)
    {
        experimentController.GrabDelayValue = newDelay;
    }

    private void OnGrabAction(InputAction.CallbackContext context)
    {
    if(experimentController.GrabDelayValue>0)
    {
    if (grabAction.action.ReadValue<float>() > 0.2f)
    {
        if (!isGrabbing)
        {
            StartCoroutine(DelayedGrab());
        }
        
    }
    else
    {
        if (isGrabbing)
        {
            StartCoroutine(DelayedRelease());
        }
        
    }
    }
    else
    {
        if (grabAction.action.ReadValue<float>() > 0.2f)
        {
            if (!isGrabbing)
            {
                TryGrabObject();
            }
           
        }
        else
        {
            if (isGrabbing)
            {
                ReleaseObject();
            }
            
        }
    }
    // Check if the grabbed object has the grabableObject script attached and call FirePistol() if it does
        if (fireAction.action.ReadValue<float>() > 0.2f && grabbedObject != null)
        {
            grabableObject grabObjectScript = grabbedObject.GetComponent<grabableObject>();
            if (grabObjectScript != null)
            {
                bool value = fireAction.action.ReadValue<float>() > 0.2f;
                grabObjectScript.FirePistol(value);
            }
        }
        /////
    }


    private IEnumerator DelayedGrab()
    {
        yield return new WaitForSeconds(experimentController.GrabDelayValue);
       
        if (!isGrabbing)
        {
            TryGrabObject();
            
        }
    }

    private IEnumerator DelayedRelease()
    {
        yield return new WaitForSeconds(experimentController.GrabDelayValue);
        
        if (isGrabbing)
        {
            ReleaseObject();
        }

        
    }



   private void TryGrabObject()
    {
    
        if (experimentController.GrabDelayValue == 0f)
        {
        // If grabDelay is  0, check if the grab action button is pressed
        if (grabAction.action.ReadValue<float>() > 0.2f)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, grabRange, grabbableLayer);
            if (colliders.Length > 0)
            {
                grabbedObject = colliders[0].gameObject;
                grabbedObjectAttachmentPoint = grabbedObject.transform;
                grabbedObjectRigidbody = grabbedObject.GetComponent<Rigidbody>();
                grabbedObjectRigidbody.isKinematic = true;
                isGrabbing = true;
            }
        }
        }
    else
        {
        Collider[] colliders = Physics.OverlapSphere(transform.position, grabRange, grabbableLayer);
        if (colliders.Length > 0)
        {
            grabbedObject = colliders[0].gameObject;
            grabbedObjectAttachmentPoint = grabbedObject.transform;
            grabbedObjectRigidbody = grabbedObject.GetComponent<Rigidbody>();
            grabbedObjectRigidbody.isKinematic = true;
            isGrabbing = true;
        }
        }
    }   

    private void ReleaseObject()
    {
        if (grabbedObject != null)
    {
    grabbedObjectRigidbody.isKinematic = false;
    grabbedObject = null;
    grabbedObjectAttachmentPoint = null;
    grabbedObjectRigidbody = null;
    isGrabbing = false;
    }}

    private void MoveGrabbedObject()
    {
        grabbedObjectAttachmentPoint.position = attachmentPoint.position;
        grabbedObjectAttachmentPoint.rotation = attachmentPoint.rotation;
    }


    private IEnumerator mainloop()
    {

        yield return new WaitForSeconds(experimentController.GrabDelayValue);
        while (true){
            ControllerLag.position = CTransform[0];
            CTransform.Remove(CTransform[0]);
            ControllerLag.rotation = CTransformR[0];
            CTransformR.Remove(CTransformR[0]);
            foreach (var item in animationInputs)
            {
                animator.SetFloat(item.animationPropertyName, actionValues[0]);
                actionValues.Remove(actionValues[0]);
            }
            yield return null;
         }
        
    }


     IEnumerator addController()
    {
        while (true){
            CTransform.Add(ControllerReal.position);
            CTransformR.Add(ControllerReal.rotation);
            float actionValue = 0f;
            
            foreach (var item in animationInputs)
            {

            actionValue = item.action.action.ReadValue<float>();
            //animator.SetFloat(item.animationPropertyName, actionValue);
            actionValues.Add(actionValue);
            }

            


            yield return null;
        }
    }

    


    private void Update()
    {
        if (isGrabbing && grabbedObject != null)
        {
            MoveGrabbedObject();
        }
        
    }
}
