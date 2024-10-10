using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class InverseKinematics : MonoBehaviour
{
    public float positionMagnitude = 0.1f;     // Magnitude of the position noise
    public float positionFrequency = 1f;       // Frequency of the position noise
    // public float rotationMagnitude = 10f;      // Magnitude of the rotation noise
    // public float rotationFrequency = 1f;       // Frequency of the rotation noise
    public ExperimentController experimentController;

    public AnimationClip handAnimationClip;    // Reference to your hand animation clip
    public AnimationClip handAnimationClip2;
    public float animationBlendWeight;  // Weight of the hand animation
    public float animationBlendWeight2;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Animator animator;

    private void Start()
    {
        // Store the initial position and rotation of the hand
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;

        // Get the Animator component attached to the hand object
        animator = GetComponent<Animator>();
    }
    private void Awake()
    {
        experimentController = FindObjectOfType<ExperimentController>();
        if (experimentController != null)
        {
            ExperimentController.OnShakingMagnitudeValueChanged += OnShakingMagnitudeValueChangedHandler;
            ExperimentController.OnShakingFrequencyValueChanged += OnShakingFrequencyValueChangedHandler;
        }
    }
    private void OnDestroy()
    {
        if (experimentController != null)
        {
            ExperimentController.OnShakingMagnitudeValueChanged -= OnShakingMagnitudeValueChangedHandler;
            ExperimentController.OnShakingFrequencyValueChanged -= OnShakingFrequencyValueChangedHandler;
        }
    }

    // private void Update()
    // {
    //     if ( experimentController.shakingMovement == true){
    //     // Generate noise values based on time
    //     float positionNoise = Mathf.PerlinNoise(Time.time * positionFrequency, 0f);
    //     float rotationNoise = Mathf.PerlinNoise(Time.time * rotationFrequency, 0f);

    //     // Apply noise to position and rotation
    //     Vector3 newPosition = initialPosition + new Vector3(positionNoise, positionNoise, positionNoise) * positionMagnitude;
    //     Quaternion newRotation = initialRotation * Quaternion.Euler(0f, rotationNoise * rotationMagnitude, 0f);

    //     // Smoothly interpolate to the new position and rotation
    //     transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, Time.deltaTime);
    //     transform.localRotation = Quaternion.Lerp(transform.localRotation, newRotation, Time.deltaTime);

    //     // Blend the noisy movement with the hand animation
    //     animator.SetFloat("RightGrab", 1f - animationBlendWeight);
    //     animator.SetFloat("LeftGrab", 1f - animationBlendWeight2);
    //     }
    // }
    private void Update()
    {
        if (experimentController != null && experimentController.shakingMovement == true)
        {
            // Generate noise values based on time or use the values controlled by shakingMagnitude and shakingFrequency
            float positionNoise = Mathf.PerlinNoise(Time.time * positionFrequency, 0f);
            float rotationNoise = Mathf.PerlinNoise(Time.time * experimentController.shakingFrequency, 0f);

            // Apply noise to position and rotation
            Vector3 newPosition = transform.localPosition + new Vector3(positionNoise, positionNoise, positionNoise) * positionMagnitude;
            Quaternion newRotation = transform.localRotation * Quaternion.Euler(0f, rotationNoise * experimentController.shakingMagnitude, 0f);

            // Smoothly interpolate to the new position and rotation
            transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, newRotation, Time.deltaTime);
        }
    }
    

    private void OnShakingMagnitudeValueChangedHandler(float newValue)
    {
        experimentController.shakingMagnitude = newValue;
    }

    private void OnShakingFrequencyValueChangedHandler(float newValue)
    {
        experimentController.shakingFrequency = newValue;
    }
}

