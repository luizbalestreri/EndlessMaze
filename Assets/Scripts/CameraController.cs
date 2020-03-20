using System.Collections;
using UnityEngine.Rendering.PostProcessing;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public IEnumerator CameraRotation(Quaternion targetRotation, float time){
        Quaternion rotation = transform.rotation;
        Camera.main.orthographic = false;
        Camera.main.GetComponent<PostProcessLayer>().enabled = true;
        float i = 0;
        float multiplier = 1/time;
        Transform Cam = transform.parent;
        while(i < time){
            Cam.rotation = Quaternion.Slerp(rotation, targetRotation, i * multiplier);
            i += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Cam.rotation = targetRotation;
        Camera.main.GetComponent<PostProcessLayer>().enabled = false;
        Camera.main.orthographic = true;
        yield return null;
    }
}
