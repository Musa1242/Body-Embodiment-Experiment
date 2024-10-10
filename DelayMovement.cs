using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class DelayMovement : MonoBehaviour
{
    public Transform leftControllerReal, rightControllerReal;//headSetReal;
    public Transform leftControllerLag, rightControllerLag; //headSetLag;
    static public float delayTime;
    [SerializeField] ActionBasedController actionController;
    private InputActionProperty selectAction;
    private InputActionProperty selectActionValue;
    List<Vector3> LTransform, RTransform, HTransform;
    List<Quaternion> LTransformR, RTransformR, HTransformR;
    InteractionState interactionState;
    //ActionBasedController Dcontroller = new ActionBasedController();
    //XRDirectInteractor Dinteractor = new XRDirectInteractor();
    //public static DelayMovement delayer;
    //Thread.Sleep(2000);
    
    // Start is called before the first frame update
    void Start()
    {
        LTransform = new List<Vector3>();
        RTransform = new List<Vector3>();
        //HTransform = new List<Vector3>();
        LTransformR = new List<Quaternion>();
        RTransformR = new List<Quaternion>();
        //HTransformR = new List<Quaternion>();
        
        StartCoroutine(mainloop());
        StartCoroutine(addController());
        //List<InputDelay> inputs = new List<InputDelay>();
        //StartCoroutine(interactionState.DelayedSetFrameState(bool isActive, float newValue));
        
        
        
    
    }
   
    

    private IEnumerator mainloop()
    {

        yield return new WaitForSeconds(delayTime);
        while (true){
            //transform.que[delayTime];
            leftControllerLag.position = LTransform[0];
            LTransform.Remove(LTransform[0]);
            leftControllerLag.rotation = LTransformR[0];
            LTransformR.Remove(LTransformR[0]);
            rightControllerLag.position = RTransform[0];
            RTransform.Remove(RTransform[0]);
            rightControllerLag.rotation = RTransformR[0];
            RTransformR.Remove(RTransformR[0]);
            // headSetLag.position = HTransform[0];
            // HTransform.Remove(HTransform[0]);
            // headSetLag.rotation = HTransformR[0];
            // HTransformR.Remove(HTransformR[0]);
            //actionController.selectAction.EnableDirectAction();
            //actionController.selectActionValue.EnableDirectAction();
            yield return null;
         }
        
    }
    
    IEnumerator addController()
    {
        while (true){
            LTransform.Add(leftControllerReal.position);
            RTransform.Add(rightControllerReal.position);
            //HTransform.Add(headSetReal.position);
            LTransformR.Add(leftControllerReal.rotation);
            RTransformR.Add(rightControllerReal.rotation);
            //HTransformR.Add(headSetReal.rotation);
            // add leftController.transform;
            // add rightController.transform;
            yield return null;
        }
    }
    // IEnumerator DelayedSetFrameState()
    // {
    //     if (interactionState.SetFrameState()) {

    //         yield return new WaitForSeconds(delayTime);
    //     }
        
    // }
    
    
}

