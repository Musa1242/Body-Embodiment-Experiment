using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExperimentController : MonoBehaviour
{
    //HandVisualCopier handVisualCopier;
    //HandInteraction handInteraction;
    //InverseKinematics inverseKinematics;
    private GameObject activeVRCharacter;
    private InverseKinematics activeInverseKinematics;
    public HandVisualCopier handVisualCopier;
    HandInteraction handInteraction;
    //public static ExperimentController Instance { get; private set; }
    
   
   
    

    public enum VRCharacterType {Normal, ThreeHumanArms, MonsterThirdArm, MonsterProsthesis, RoboticProsthesis, WrongSelfLocalization }
    [Header("VR Character Types")]
    public GameObject normal;
    public GameObject threeHumanArms;
    public GameObject monsterThirdArm;
    public GameObject monsterProsthesis;
    public GameObject roboticProsthesis;
    public GameObject wrongSelfLocalization;
    public GameObject mainCamera;
    public GameObject BlurCamera;
    public GameObject Plane;
    public GameObject xrOriginNormal; // Reference to the XR Origin GameObject for Normal
    
    public GameObject xrOriginWrong; // Reference to the XR Origin GameObject for WrongSelfLocalization
    public HandInteraction rightController;
    public HandInteraction leftController;

    //Right and Left Controller VR Target and Fake Targets reference

    public GameObject RightTarget; 
    public GameObject RightFakeTarget;
    public GameObject LeftTarget;
    public GameObject LeftFakeTarget;
    [HideInInspector]
    public bool changeSelfLocalization = true;
    
    [HideInInspector]
    public float commonRightXPosition;
     [HideInInspector]
    public float commonLeftXPosition;
    [HideInInspector]
    public float offsetRightXPosition = 0.046f;
    [HideInInspector]
    public float offsetLeftXPosition = -0.046f;

    

    
    

    [Header("Presets")]
    public VRCharacterType vrCharacterType;
    public bool delayMovement = false;
    
    public float GrabDelayValue;
    public delegate void GrabDelayValueChangedDelegate(float newDelay);
    public static event GrabDelayValueChangedDelegate OnGrabDelayValueChanged;

    public bool shakingMovement = false;
    public float shakingMagnitude;
    public float shakingFrequency;
    public delegate void FloatValueChangedDelegate(float newValue);
    public delegate void BoolValueChangedDelegate(bool newValue);
    public static event FloatValueChangedDelegate OnShakingMagnitudeValueChanged;
    public static event FloatValueChangedDelegate OnShakingFrequencyValueChanged;
    public static event BoolValueChangedDelegate OnShakingMovementChanged;
    //public static event BoolValueChangedDelegate OnHandShakeChanged;
    public bool CopyRightHandMovement = false;

    public bool closeThirdArm = false;
    public bool blurEffect = false;
    public bool BlurAll = false;
    private int initialMainCameraCullingMask;
    private int initialBlurCameraCullingMask;
    private bool cullingMasksInitialized = false;
   
    private Dictionary<GameObject, int> originalLayers = new Dictionary<GameObject, int>();
    public delegate void VRCharacterTypeChangedDelegate(GameObject vrCharacter);
    public static event VRCharacterTypeChangedDelegate OnVRCharacterTypeChanged;



private void OnValidate()
{
    ApplyChanges();
}




public void ApplyChanges()
{
    UpdateSelectedObject();
    ApplyBlurToAllObjects();
    ApplyBlurEffect();
     // Check if changeSelfLocalization is true before updating the positions
    if (changeSelfLocalization)
    {
        UpdateRightPositions();
        UpdateLeftPositions();
    }

}
[HideInInspector]
public bool hideApplyChanges;



    public void UpdateArmScaleToZero()
    {
        HandVisualCopier[] handVisualCopiers = FindObjectsOfType<HandVisualCopier>();

        foreach (HandVisualCopier handVisualCopier in handVisualCopiers)
        {
            handVisualCopier.ArmScaleToZero = closeThirdArm;
        }
    }
    

    public void UpdateSelectedObject()
    {
        // Deactivate all VR character types
        normal.SetActive(false);
        threeHumanArms.SetActive(false);
        monsterThirdArm.SetActive(false);
        monsterProsthesis.SetActive(false);
        roboticProsthesis.SetActive(false);
        wrongSelfLocalization.SetActive(false);
        // Deactivate all controllers
        xrOriginNormal.SetActive(true);
        xrOriginWrong.SetActive(false);
        if (OnVRCharacterTypeChanged != null)
        {
            OnVRCharacterTypeChanged(activeVRCharacter);
        }
       
         // Activate the selected VR character type
        switch (vrCharacterType)
        {
            case VRCharacterType.Normal:
                normal.SetActive(true);
                xrOriginNormal.SetActive(true);
                rightController.animator = normal.GetComponent<Animator>();
                leftController.animator = normal.GetComponent<Animator>();
                //AssignAnimatorToHandInteraction(normal);
                break;
            case VRCharacterType.ThreeHumanArms:
                threeHumanArms.SetActive(true);
                xrOriginNormal.SetActive(true);
                rightController.animator = threeHumanArms.GetComponent<Animator>();
                leftController.animator = threeHumanArms.GetComponent<Animator>();
                //AssignAnimatorToHandInteraction(threeHumanArms);
                break;
            case VRCharacterType.MonsterThirdArm:
                monsterThirdArm.SetActive(true);
                xrOriginNormal.SetActive(true);
                rightController.animator = monsterThirdArm.GetComponent<Animator>();
                leftController.animator = monsterThirdArm.GetComponent<Animator>();
                //AssignAnimatorToHandInteraction(monsterThirdArm);
                break;
            case VRCharacterType.MonsterProsthesis:
                monsterProsthesis.SetActive(true);
                xrOriginNormal.SetActive(true);
                rightController.animator = monsterProsthesis.GetComponent<Animator>();
                leftController.animator = monsterProsthesis.GetComponent<Animator>();
                //AssignAnimatorToHandInteraction(monsterProsthesis);
                break;
            case VRCharacterType.RoboticProsthesis:
                roboticProsthesis.SetActive(true);
                xrOriginNormal.SetActive(true);
                rightController.animator = roboticProsthesis.GetComponent<Animator>();
                leftController.animator = roboticProsthesis.GetComponent<Animator>();
               // AssignAnimatorToHandInteraction(roboticProsthesis);
                break;
            case VRCharacterType.WrongSelfLocalization:
                wrongSelfLocalization.SetActive(true);
                xrOriginNormal.SetActive(false);
                xrOriginWrong.SetActive(true);
                //AssignAnimatorToHandInteraction(wrongSelfLocalization);
                break;
        }
        if (handVisualCopier != null)
        {
            handVisualCopier.ArmScaleToZero = closeThirdArm;
        }
    // Apply the blur effect based on BlurAll and blurEffect options
        // if (BlurAll == true)
        // {
        //     ApplyBlurToAllObjects();
        // }
        // else if (blurEffect == true)
        // {
        //     ApplyBlurEffect();
        // }
        // else
        // {
        //     // Reset the layer to default for the selected VRCharacterType GameObject
        //     ResetLayerToDefault();
        // }
    }
     private void ApplyBlurToAllObjects()
    {
        // Assuming you have an array of all publicly selected GameObjects to blur or reset the layer.
        // Adjust the array size based on the number of publicly selected GameObjects.

        GameObject[] normalCamera = { mainCamera };
        GameObject[] blurCamera = { BlurCamera};

    if (!cullingMasksInitialized)
    {
        // Store the initial culling masks only if they have not been initialized yet
        Camera mainCameraComponent = mainCamera.GetComponent<Camera>();
        if (mainCameraComponent != null)
        {
            initialMainCameraCullingMask = mainCameraComponent.cullingMask;
        }

        Camera blurCameraComponent = BlurCamera.GetComponent<Camera>();
        if (blurCameraComponent != null)
        {
            initialBlurCameraCullingMask = blurCameraComponent.cullingMask;
        }

        // Mark culling masks as initialized
        cullingMasksInitialized = true;
    }
        
    if (BlurAll == true)
    {
        foreach (GameObject obj in normalCamera)
        {
            // If BlurAll is true, set the Culling Mask of mainCamera to "Nothing".
            Camera cameraComponent = obj.GetComponent<Camera>();
            if (cameraComponent != null)
            {
                cameraComponent.cullingMask = 0; // Set Culling Mask to "Nothing"
            }
        }
        foreach (GameObject obj in blurCamera)
        {
            // If BlurAll is true, set the Culling Mask of blurCamera to "Everything".
            Camera cameraComponent = obj.GetComponent<Camera>();
            if (cameraComponent != null)
            {
                cameraComponent.cullingMask = -1; // Set Culling Mask to "Everything"
            }
        }
    }
    if(BlurAll == false)
    {
        // Reset the Culling Masks of mainCamera and blurCamera to their initial states
        Camera mainCameraComponent = mainCamera.GetComponent<Camera>();
        if (mainCameraComponent != null)
        {
            mainCameraComponent.cullingMask = initialMainCameraCullingMask;
        }

        Camera blurCameraComponent = BlurCamera.GetComponent<Camera>();
        if (blurCameraComponent != null)
        {
            blurCameraComponent.cullingMask = initialBlurCameraCullingMask;
        }
    }
        

    }
    private void ApplyBlurEffect()
    {
        // Assuming the blurring logic here. You should implement your specific blurring logic based on your post-processing setup.
        // You might need to access the main camera or focusCamera and apply the blurring effect to the selected VRCharacterType GameObject.
        // This will depend on your post-processing setup and how you implement the blurring effect.

        // Example: You might need to get the selected VRCharacterType GameObject and change its layer to "Focused" or "BlurLayer" or something similar to trigger the blurring effect in your post-processing stack.
        // For simplicity, let's assume there is a "BlurLayer" layer in your project and you want to assign the selected VRCharacterType GameObject to this layer when blurEffect is true.
         GameObject[] objectsToBlur = { normal, threeHumanArms, monsterThirdArm, monsterProsthesis, roboticProsthesis, wrongSelfLocalization, Plane };

        foreach (GameObject obj in objectsToBlur)
        {
            if (blurEffect == true)
            {
                // If BlurAll is true, assign all publicly selected GameObjects to "BlurLayer".
                obj.layer = LayerMask.NameToLayer("Focused");
            }
            if(blurEffect == false)
            {
                // If BlurAll is false, reset the layer of all publicly selected GameObjects to "Default".
                obj.layer = LayerMask.NameToLayer("Default");
            }

           
        }
    }

    
    public  void UpdateRightPositions()
    {
        //Debug.Log("Updating Right Positions");
        if (RightTarget != null && RightFakeTarget != null)
    {
        Vector3 rightPosition = RightTarget.transform.localPosition;
        rightPosition.x = commonRightXPosition;
        RightTarget.transform.localPosition = rightPosition;

        Vector3 rightFakePosition = RightFakeTarget.transform.localPosition;
        rightFakePosition.x = commonRightXPosition;
        RightFakeTarget.transform.localPosition = rightFakePosition;
    }
    }
     // Add a method to update the X positions of Left and LeftFake targets
    public  void UpdateLeftPositions()
    {
        // Debug.Log("Updating Left Positions");
    if (LeftTarget != null && LeftFakeTarget != null)
    {
        Vector3 leftPosition = LeftTarget.transform.localPosition;
        leftPosition.x = commonLeftXPosition;
        LeftTarget.transform.localPosition = leftPosition;

        Vector3 leftFakePosition = LeftFakeTarget.transform.localPosition;
        leftFakePosition.x = commonLeftXPosition;
        LeftFakeTarget.transform.localPosition = leftFakePosition;
    }
    }
    // Add a method to reset the X positions to their initial values
    public  void ResetXPositions()
    {
        if (RightTarget != null)
        {
            Vector3 rightPosition = RightTarget.transform.localPosition;
            rightPosition.x = offsetRightXPosition;
            RightTarget.transform.localPosition = rightPosition;
        }

        if (RightFakeTarget != null)
        {
            Vector3 rightFakePosition = RightFakeTarget.transform.localPosition;
            rightFakePosition.x = offsetRightXPosition;
            RightFakeTarget.transform.localPosition = rightFakePosition;
        }

        if (LeftTarget != null)
        {
            Vector3 leftPosition = LeftTarget.transform.localPosition;
            leftPosition.x = offsetLeftXPosition;
            LeftTarget.transform.localPosition = leftPosition;
        }

        if (LeftFakeTarget != null)
        {
            Vector3 leftFakePosition = LeftFakeTarget.transform.localPosition;
            leftFakePosition.x = offsetLeftXPosition;
            LeftFakeTarget.transform.localPosition = leftFakePosition;
        }
    }
// public void AssignAnimatorToHandInteraction(GameObject vrCharacter)
//     {
//         Animator vrCharacterAnimator = vrCharacter.GetComponent<Animator>();

//         if (handInteraction != null && vrCharacterAnimator != null)
//         {
//             handInteraction.SetAnimator(vrCharacterAnimator);
//         }
//     }
    // public void UpdateGrabDelay(float newGrabDelay)
    // {
    //     // Find the HandInteraction script or access it from the reference if you already have it
    //     HandInteraction handInteraction = FindObjectOfType<HandInteraction>();

    //     // Update the grabDelay in HandInteraction
    //     if (handInteraction != null)
    //     {
    //         handInteraction.SetGrabDelay(newGrabDelay);
    //     }
    // }

    

    


#region Editor
#if UNITY_EDITOR
    [CustomEditor(typeof(ExperimentController))]
    public class ExperimentControllerEditor : Editor
{
    //private static bool delayTime = false;
    //private static bool handShake = false;
    //private static bool armScaleZero = false;
    //private static bool blurredBody = false;
    //private static bool blurredFull = false;
    SerializedProperty vrCharacterTypeProp;
    SerializedProperty delayMovementProp;
    SerializedProperty GrabDelayValueProp;
    SerializedProperty shakingMovementProp;
    SerializedProperty shakingMagnitudeProp;
    SerializedProperty shakingFrequencyProp;
    SerializedProperty closeThirdArmProp;
    SerializedProperty blurEffectProp;
    SerializedProperty blurAllProp;
    SerializedProperty grabDelayProp;



       


        //??
        private void OnEnable()
        {
        //grabDelayProp = serializedObject.FindProperty("GrabDelayValue");
      
        vrCharacterTypeProp = serializedObject.FindProperty("vrCharacterType");
        delayMovementProp = serializedObject.FindProperty("delayMovement");

        GrabDelayValueProp = serializedObject.FindProperty("GrabDelayValue");
        

        shakingMovementProp = serializedObject.FindProperty("shakingMovement");
        shakingMagnitudeProp = serializedObject.FindProperty("shakingMagnitude");
        shakingFrequencyProp = serializedObject.FindProperty("shakingFrequency");
        closeThirdArmProp = serializedObject.FindProperty("closeThirdArm");
        blurEffectProp = serializedObject.FindProperty("blurEffect");
        blurAllProp = serializedObject.FindProperty("BlurAll");
        
        }
        //??
        
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(vrCharacterTypeProp);

            ExperimentController experimentController = (ExperimentController)target;
            
            float newGrabDelayValue = experimentController.GrabDelayValue;
            float newShakingMagnitudeValue = experimentController.shakingMagnitude; // Cache the new value
            float newShakingFrequencyValue = experimentController.shakingFrequency; // Cache the new value
            bool newShakingMovementValue = experimentController.shakingMovement; // Cache the new value
            //bool newHandShakeValue = experimentController.CopyRightHandMovement; // Cache the new value
        // Add a toggle for CopyRightHandMovement
        experimentController.CopyRightHandMovement = EditorGUILayout.Toggle("Copy Right Hand Movement", experimentController.CopyRightHandMovement);
        
        
        // If closeThirdArm is true, set ArmScaleToZero to true
        if (experimentController.closeThirdArm)
        {
            if (experimentController.handVisualCopier != null)
            {
                experimentController.handVisualCopier.ArmScaleToZero = true;
            }
        }
        else
        {
            // If closeThirdArm is false, set ArmScaleToZero to false
            if (experimentController.handVisualCopier != null)
            {
                experimentController.handVisualCopier.ArmScaleToZero = false;
            }
        }


            
           // SetConditions(experimentController);
            //DrawList(experimentController);
        if (experimentController.vrCharacterType == VRCharacterType.Normal || experimentController.vrCharacterType == VRCharacterType.ThreeHumanArms
            || experimentController.vrCharacterType == VRCharacterType.MonsterThirdArm || experimentController.vrCharacterType == VRCharacterType.MonsterProsthesis || experimentController.vrCharacterType == VRCharacterType.RoboticProsthesis
            || experimentController.vrCharacterType == VRCharacterType.WrongSelfLocalization)
        {
            EditorGUILayout.PropertyField(blurEffectProp);
            EditorGUILayout.PropertyField(delayMovementProp);
        if (experimentController.delayMovement)
        {
            EditorGUILayout.PropertyField(GrabDelayValueProp);
            
            
        }

        EditorGUILayout.PropertyField(shakingMovementProp);
        if (experimentController.shakingMovement)
        {
            EditorGUILayout.PropertyField(shakingMagnitudeProp);
            EditorGUILayout.PropertyField(shakingFrequencyProp);
        }
        }

        

        if (experimentController.vrCharacterType == VRCharacterType.ThreeHumanArms || experimentController.vrCharacterType == VRCharacterType.MonsterThirdArm 
        || experimentController.vrCharacterType == VRCharacterType.MonsterProsthesis || experimentController.vrCharacterType == VRCharacterType.RoboticProsthesis)
        {
            EditorGUILayout.PropertyField(closeThirdArmProp);
        }
         EditorGUILayout.PropertyField(blurAllProp); // Display the BlurAll toggle in the Inspector

        // Check if BlurAll is true and show the additionalObjectsToBlur list if necessary
        if (experimentController.BlurAll)
        {
            EditorGUI.indentLevel++;
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("additionalObjectsToBlur"), true);
            EditorGUI.indentLevel--;
        }
        // Add custom handling for the X positions and the ChangeSelfLocalization bool
        if (experimentController.vrCharacterType == VRCharacterType.WrongSelfLocalization){
        if (experimentController.changeSelfLocalization == true)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Sync X Positions");
            EditorGUILayout.BeginHorizontal();
            experimentController.commonRightXPosition = EditorGUILayout.FloatField("Common Right X Position", experimentController.commonRightXPosition);
            experimentController.commonLeftXPosition = EditorGUILayout.FloatField("Common Left X Position", experimentController.commonLeftXPosition);
            EditorGUILayout.EndHorizontal();

            //experimentController.changeSelfLocalization = EditorGUILayout.Toggle("Change Self Localization", experimentController.changeSelfLocalization);
        }}

        experimentController.changeSelfLocalization = EditorGUILayout.Toggle("Change Self Localization", experimentController.changeSelfLocalization);

            if (GUILayout.Button("Apply Changes")) // Add a button to apply changes manually
        {
            experimentController.ApplyChanges();
            experimentController.UpdateArmScaleToZero();
            //experimentController.UpdateGrabDelay(grabDelayProp.floatValue);
            if (ExperimentController.OnGrabDelayValueChanged != null)
            {
                ExperimentController.OnGrabDelayValueChanged(newGrabDelayValue);
            }

            if (ExperimentController.OnShakingMagnitudeValueChanged != null)
            {
                ExperimentController.OnShakingMagnitudeValueChanged(newShakingMagnitudeValue);
            }

            if (ExperimentController.OnShakingFrequencyValueChanged != null)
            {
                ExperimentController.OnShakingFrequencyValueChanged(newShakingFrequencyValue);
            }

            if (ExperimentController.OnShakingMovementChanged != null)
            {
                ExperimentController.OnShakingMovementChanged(newShakingMovementValue);
            }
            // Check the ChangeSelfLocalization bool before updating X positions
            if (experimentController.changeSelfLocalization == true)
            {
                experimentController.UpdateRightPositions();
                experimentController.UpdateLeftPositions();
            }
            if(experimentController.changeSelfLocalization == false)
            {
                experimentController.ResetXPositions();
            }

            // if (ExperimentController.OnHandShakeChanged != null) ///???
            // {
            //     ExperimentController.OnHandShakeChanged(newHandShakeValue); ///???
            // }
            
           
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Select VRCharacterType GameObjects");
            experimentController.normal = EditorGUILayout.ObjectField("Normal", experimentController.normal, typeof(GameObject), true) as GameObject;
            experimentController.threeHumanArms = EditorGUILayout.ObjectField("ThreeHumanArms", experimentController.threeHumanArms, typeof(GameObject), true) as GameObject;
            experimentController.monsterThirdArm = EditorGUILayout.ObjectField("MonsterThirdArm", experimentController.monsterThirdArm, typeof(GameObject), true) as GameObject;
            experimentController.monsterProsthesis = EditorGUILayout.ObjectField("MonsterProsthesis", experimentController.monsterProsthesis, typeof(GameObject), true) as GameObject;
            experimentController.roboticProsthesis = EditorGUILayout.ObjectField("RoboticProsthesis", experimentController.roboticProsthesis, typeof(GameObject), true) as GameObject;
            experimentController.wrongSelfLocalization = EditorGUILayout.ObjectField("WrongSelfLocalization", experimentController.wrongSelfLocalization, typeof(GameObject), true) as GameObject;
            EditorGUILayout.LabelField("Select Controller Object");
            experimentController.xrOriginNormal = EditorGUILayout.ObjectField("xrOriginNormal", experimentController.xrOriginNormal, typeof(GameObject), true) as GameObject;
            experimentController.xrOriginWrong = EditorGUILayout.ObjectField("xrOriginWrong", experimentController.xrOriginWrong, typeof(GameObject), true) as GameObject;
            EditorGUILayout.LabelField("Select Camara and BlurCamera, blurred Object");
            experimentController.mainCamera = EditorGUILayout.ObjectField("MainCamera", experimentController.mainCamera, typeof(GameObject), true) as GameObject;
            experimentController.BlurCamera = EditorGUILayout.ObjectField("BlurCamera", experimentController.BlurCamera, typeof(GameObject), true) as GameObject;
            experimentController.Plane = EditorGUILayout.ObjectField("Plane", experimentController.Plane, typeof(GameObject), true) as GameObject;
            EditorGUILayout.LabelField("Select Right, and Left Controller actual and fake VR Target");
            experimentController.RightTarget = EditorGUILayout.ObjectField("RightTarget", experimentController.RightTarget, typeof(GameObject), true) as GameObject;
            experimentController.RightFakeTarget = EditorGUILayout.ObjectField("RightFakeTarget", experimentController.RightFakeTarget, typeof(GameObject), true) as GameObject;
            experimentController.LeftTarget = EditorGUILayout.ObjectField("LeftTarget", experimentController.LeftTarget, typeof(GameObject), true) as GameObject;
            experimentController.LeftFakeTarget = EditorGUILayout.ObjectField("LeftFakeTarget", experimentController.LeftFakeTarget, typeof(GameObject), true) as GameObject;
            EditorGUILayout.LabelField("Right and Left Controlle Animator reference");
            experimentController.rightController = EditorGUILayout.ObjectField("rightController", experimentController.rightController, typeof(HandInteraction), true) as HandInteraction;
            experimentController.leftController = EditorGUILayout.ObjectField("leftController", experimentController.leftController, typeof(HandInteraction), true) as HandInteraction;


            serializedObject.ApplyModifiedProperties();

        
           
        }

        
        
        

    
#endif
#endregion
}}
