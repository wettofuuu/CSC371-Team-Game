using UnityEngine;

public class NavLinkAreaMarker : MonoBehaviour
{
    public Transform startTransform;
    public Transform endTransform;

    public bool GetFarEndpointPosition(Vector3 fromPosition, out Vector3 endpoint)
    {
        endpoint = Vector3.zero;
        if (startTransform == null || endTransform == null) return false;

        float d0 = (fromPosition - startTransform.position).sqrMagnitude;
        float d1 = (fromPosition - endTransform.position).sqrMagnitude;

        endpoint = d0 >= d1 ? startTransform.position : endTransform.position; // FARTHER one
        return true;
    }
}