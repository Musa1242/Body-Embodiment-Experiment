using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandVisualCopier : MonoBehaviour
{
    // This script pulls hand bone information from the oculus integration in a way that fits more standard hand models, and adjusts it to fit a model for tracking/tutorial purposes
    // Specifically, it pulls three bone points for each finger, including the thumb.
    // the offset field can be used to adjust the rotations of the fingers to better align to the hand, as some hands have strange starting points.
    // The LowPolyAnime model is used for the Nina/Dan model i've imported and, due to some strangeness in how they are rigged, likely won't work out of the box with other models
    // when adding a model, use those functions as a base to copy/paste and tweak to your model. For mine I had to switch up some x/y/z points and do some inversion of points, but 
    // your milage will vary 
    public ExperimentController experimentController; 

   [SerializeField]
   public HandSkeletonReference RHandRef;

   
   [SerializeField]
   public HandSkeletonReference rhandref;
   [SerializeField]
   public HandSkeletonReference RWrist;
   [SerializeField]
   public HandSkeletonReference rwrist;
   public bool ArmScaleToZero = false;
   public Transform SelectedArmScale;
    

    public enum ModelType {NULL, Reverse, StarterRobot5, Generic4}
    [SerializeField]
    ModelType AvatarModel;
    [SerializeField]
    Vector3 BaseRotationalOffset, PinkyRotationalOffset, ThumbRotationalOffset;


    void copyRotationRightGeneric(){
        var RotationOffset = Vector3.zero;
        RotationOffset = BaseRotationalOffset;
        var qRef = Vector3.zero;
        
        for (int f = 0; f<5; f++){
            for (int i = 0; i<4; i++){
                if ((f == 0) && (i==0)){
                    // adjust the thumb rotation as it is sometimes different
                    RotationOffset = ThumbRotationalOffset;
                } else {
                    RotationOffset = BaseRotationalOffset;
                }
                qRef = (RHandRef.ReferenceFingers[f].ReferenceFingerPoints[i].localEulerAngles);
                rhandref.ReferenceFingers[f].ReferenceFingerPoints[i].localRotation = (Quaternion.Euler(qRef.x, qRef.z, qRef.y) * (Quaternion.Euler(RotationOffset)));
            }
        }
        for (int w = 0; w<1; w++){ //Referencing Wrist Bones
            for(int i=0; i<1; i++){
            qRef = (RWrist.ReferenceFingers[w].ReferenceFingerPoints[i].localEulerAngles);
            rwrist.ReferenceFingers[w].ReferenceFingerPoints[i].localRotation = (Quaternion.Euler(qRef.x, qRef.z, qRef.y) * (Quaternion.Euler(RotationOffset)));
         }}
    }

    IEnumerator ScaleDown(float time)
    {
        float i = 0;
        float rate = 1 / time;

        Vector3 fromScale = transform.localScale;
        Vector3 toScale = Vector3.zero;
        while (i<1)
        {
            i += Time.deltaTime * rate;
            SelectedArmScale.transform.localScale = Vector3.Lerp(fromScale, toScale, i);
            yield return 0;
        }
    }


    // Late Update runs at the end of every frame, ensuring we have all positional data before rendering the model.\



    // MUSA:
        // THIS CODE IS SPECIFIC TO A DIFFERENT INTERACTION KIT, SWITCH LHANDREF to a collection of finger points on
        // the hand you want to transform
    void LateUpdate(){
        if (ArmScaleToZero == true){
            StartCoroutine(ScaleDown(0f));
        }
        if (experimentController.CopyRightHandMovement == true)
        {   switch(AvatarModel){
            case ModelType.NULL:   // no model loaded
                Debug.LogWarning("No Model Type Selected");
                ;break;

            case ModelType.Reverse:
               
                copyRotationRightReverse();
                ;break;
            case ModelType.StarterRobot5:
              
                copyRotationRightRobo();
                ;break;

            case ModelType.Generic4:   // generic model
                copyRotationRightGeneric();
                
                ;break;

            default: // ???
                Debug.LogWarning("Unexpected Model Type provided");
                ;break;
        }
        }
        
        
     }
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// specific presets for different models
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 
    void copyRotationRightReverse(){
        var RotationOffset = Vector3.zero;
        RotationOffset = BaseRotationalOffset;
        var qRef = Vector3.zero;
        for (int f = 0; f<5; f++){
            for (int i = 0; i<3; i++){
                if ((f == 0) && (i==0)){
                    // adjust the thumb rotation as it is different
                    RotationOffset = ThumbRotationalOffset;
                } else {
                    RotationOffset = BaseRotationalOffset;
                }
                qRef = (rhandref.ReferenceFingers[f].ReferenceFingerPoints[i].localEulerAngles);
                RHandRef.ReferenceFingers[f].ReferenceFingerPoints[i].localRotation = (Quaternion.Euler(qRef.x*(-1f), qRef.z, qRef.y) * (Quaternion.Euler(RotationOffset)));
            }
        }
        for (int w = 0; w<1; w++){ //Referencing Wrist Bones
            for(int i=0; i<2; i++){
            qRef = (rwrist.ReferenceFingers[w].ReferenceFingerPoints[i].localEulerAngles);
            RWrist.ReferenceFingers[w].ReferenceFingerPoints[i].localRotation = (Quaternion.Euler(qRef.x, qRef.z, qRef.y) * (Quaternion.Euler(RotationOffset)));
         }}
    }

    // for the low poly anime models, the root of each hand had a strange starting rotation that needed specific targeting.
    // these work reasonably well, though due to the long length of the fingers this type of model has some undesireable outcome when gripping or doing actions like a thumbs up.  
    

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public enum UseAxis {X,Y,Z};
    [SerializeField]
    bool invertX, invertY, invertZ;
    [SerializeField]
    UseAxis XUse, YUse, ZUse;

    float InvertCheck(bool b){
        if (b){
            return 1f;
        } else {
            return -1f;
        }
    }

    float qValue(Vector3 v, UseAxis a){
        var tmp = 0f;
        switch (a){
            case UseAxis.X:
                tmp = v.x;
                ;break;
            case UseAxis.Y:
                tmp = v.y;
                ;break;
            case UseAxis.Z:
                tmp = v.z;
                ;break;
        }
        return tmp;
    }

    Vector3 QrefAdjuster(Vector3 v){
        return new Vector3( qValue(v, XUse) * InvertCheck(invertX), 
                            qValue(v, YUse) * InvertCheck(invertY), 
                            qValue(v, ZUse) * InvertCheck(invertZ)  );
    }



    //////////////////////////////////////////////////////////////////////////////////////////////////////////

    void copyRotationRightRobo(){
        var RotationOffset = Vector3.zero;
        var qRef = Vector3.zero;
        for (int f = 0; f<5; f++){
            for (int i = 0; i<5; i++){ 
                qRef = (RHandRef.ReferenceFingers[f].ReferenceFingerPoints[i].localEulerAngles);
                    if (f != 0){
                        if (f == 3){
                            RotationOffset = BaseRotationalOffset + new Vector3(0f, 0f, RingOffset);
                        } else if (f == 4){
                            RotationOffset = BaseRotationalOffset + new Vector3(0f, 0f, PinkyOffset);
                        } else {
                            RotationOffset = BaseRotationalOffset;
                        }
                        if ((i == 0)){
                            rhandref.ReferenceFingers[f].ReferenceFingerPoints[i].localRotation = Quaternion.Euler(new Vector3(qRef.z*(-1f), qRef.y, qRef.x) + RotationOffset );
                        } else {
                            rhandref.ReferenceFingers[f].ReferenceFingerPoints[i].localRotation = Quaternion.Euler(qRef.z*(-1f), qRef.y, qRef.x);
                        }
                    } else {
                        // The thumb stuff
                        RotationOffset = ThumbRotationalOffset;
                        if ((i == 0)){
                            rhandref.ReferenceFingers[f].ReferenceFingerPoints[i].localRotation = Quaternion.Euler(new Vector3(qRef.z*(-1f), qRef.y, qRef.x) + RotationOffset);
                        } else {
                            rhandref.ReferenceFingers[f].ReferenceFingerPoints[i].localRotation = Quaternion.Euler(qRef.z*(-1f), qRef.y, qRef.x);
                        }
                    }
            }    
        }
        for (int w = 0; w<1; w++){ //Referencing Wrist Bones
            for(int i=0; i<3; i++){
            qRef = (RWrist.ReferenceFingers[w].ReferenceFingerPoints[i].localEulerAngles);
            rwrist.ReferenceFingers[w].ReferenceFingerPoints[i].localRotation = (Quaternion.Euler(qRef.x, qRef.z, qRef.y) * (Quaternion.Euler(RotationOffset)));
         }}
    }

    [SerializeField]
    private float RingOffset = 5f;
    [SerializeField]
    private float PinkyOffset = 5f;
 

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


}


// These public classes are used in some other scripts, like HandBoneDetails, to keep track of finger points.
// The order is Thumb, Index, Middle, Ring, Pinky

[System.Serializable]
public class HandSkeletonReference{
    [SerializeField]
    public FingerSkeletonReference[] ReferenceFingers;
}

[System.Serializable]
public class FingerSkeletonReference{
    [SerializeField]
    string fingerName;
    [SerializeField]
    public Transform[] ReferenceFingerPoints;

}
