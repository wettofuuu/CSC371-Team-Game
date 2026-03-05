using System.Collections.Generic;
using UnityEngine;

public class BetterCameraZone : MonoBehaviour
{
    [System.Serializable]
    public class CameraArea{
        public Collider trigger;  
        public Transform targetCam;   
    }

    public Transform player;
    public Transform cameraTransform;

    [Header("Areas")]
    public List<CameraArea> areas = new List<CameraArea>();

    [Header("Smoothing")]
    public float smoothTime = 0.7f;

    private Vector3 velocity = Vector3.zero;
    private Transform currentTarget;

    void Update(){
        Transform newTarget = GetAreaTarget();

        if (newTarget != currentTarget){
            currentTarget = newTarget;
        }

        if (currentTarget != null){
            cameraTransform.position = Vector3.SmoothDamp(
                cameraTransform.position,
                currentTarget.position,
                ref velocity,
                smoothTime
            );

            cameraTransform.rotation = Quaternion.Lerp(
                cameraTransform.rotation,
                currentTarget.rotation,
                Time.deltaTime * 5f
            );
        }
    }

    Transform GetAreaTarget(){
        foreach (var area in areas){
            if (area.trigger != null &&
                area.trigger.bounds.Contains(player.position)){
                return area.targetCam;
            }
        }

        return currentTarget;
    }
}