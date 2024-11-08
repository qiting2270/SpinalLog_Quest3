using System;
using UnityEngine;
using UnityEngine.XR;
using UnityDebug = UnityEngine.Debug;

public class BoneController : MonoBehaviour
{
    public int boneID;
    //private GameObject bone;
    private float leftDepth;
    private float rightDepth;
    public float averageDepth;
    private float initialLeftDepth;
    private float initialRightDepth;

    public Material objectMaterial;
    public Material whiteMaterial;
    public Material redMaterial;
    private float DEPTH_THRESHOLD = 27.0f; // Depth at which color change starts
    private float MAX_DEPTH = 20.0f; // The maximum depth for full color change

    void Start() {
        this.boneID = int.Parse(gameObject.name.Substring(1));
        objectMaterial.color = whiteMaterial.color;

    }

    void Update() {
        //UnityDebug.Log(gameObject.name + " left: " + leftDepth + ", initial left: " + initialLeftDepth);
        //UnityDebug.Log(gameObject.name + " initial left: " + initialLeftDepth);
        if (initialLeftDepth != 0) {
            if (averageDepth != 0) {
                UpDownMove();
            }
            //UnityDebug.Log(boneID + "leftDepth" + leftDepth + "depthThreshold" + DEPTH_THRESHOLD);
            if (averageDepth < DEPTH_THRESHOLD) {

                // Calculate the interpolation factor based on how close averageDepth is to 0
                float t = Mathf.InverseLerp(DEPTH_THRESHOLD, MAX_DEPTH, averageDepth);

                // Interpolate between the white and red colors based on the depth
                Color newColor = Color.Lerp(whiteMaterial.color, redMaterial.color, t);
                //UnityDebug.Log(boneID + " color: " + newColor);

                // Apply the interpolated color to the object's renderer
                objectMaterial.color = newColor;
            } else
            {
                objectMaterial.color = whiteMaterial.color;
            }
        }     
    }

    public void SetInitialDepth(float leftInput, float rightInput) {
        this.initialLeftDepth = leftInput;
        this.initialRightDepth = rightInput;
    }

    public void SetCurDepth(float leftInput, float rightInput) {
        this.leftDepth = leftInput;
        this.rightDepth = rightInput;
        this.averageDepth = (leftDepth + rightDepth) / 2;
    }

    void UpDownMove() {
        float moveDist = 0;
        //int maxDistance = 35;

        if (initialLeftDepth - leftDepth <= 0.02)
        {
            moveDist = 0;
        }
        else
        {
            moveDist = initialLeftDepth - averageDepth;

            Vector3 originalPosition = transform.localPosition;
        
            transform.localPosition = new Vector3(originalPosition.x, originalPosition.y, -moveDist * 0.005f);
        }       
    }

    public float TransverseRotationDegree() { //transverse rotation
        float halfDistance = Math.Abs(leftDepth - rightDepth)/2;
        float rotateAngle = 0;
        int boneLength = 50;
        
        if (initialLeftDepth - leftDepth <= 0.02)
        {
            return rotateAngle;
        } else {
            if (leftDepth == 0 || rightDepth == 0)
            {
                rotateAngle = Mathf.Sin(leftDepth / boneLength);
            } else
            {
                rotateAngle = Mathf.Sin(halfDistance / boneLength);
            } 

            if (leftDepth > rightDepth)
            {
                //bone.transform.localRotation = Quaternion.Euler(0f, rotateAngle *500f, 0f);
                //UnityDebug.Log("----origin: " + originalDegree + ", rotateAngle: " + rotateAngle);
                return rotateAngle;
            } else
            {
                //bone.transform.localRotation = Quaternion.Euler(0f, -rotateAngle *500f, 0f);
                //UnityDebug.Log("----origin: " + originalDegree + ", rotateAngle: " + -rotateAngle);
                return -rotateAngle;
            }
        }
    }

    public float SaggitoRotationDegree(float focusBoneDepth, int focusBoneID) {
        float rotateAngle = 0;
        float boneGap = 40; // change here
        
        
        float difference = averageDepth - focusBoneDepth;
        //rotateAngle = averageDepth - focusBoneDepth;
        rotateAngle = Mathf.Tan(difference/boneGap);
        if (boneID < focusBoneID) {
            //UnityDebug.Log("boneID: " + boneID + "focusbone: " + focusBoneID + " rotateAngle: " + -rotateAngle);
            return -rotateAngle;
        } else {
            //UnityDebug.Log("boneID: " + boneID + "focusbone: " + focusBoneID + " rotateAngle: " + rotateAngle);
            return rotateAngle;
        }
            
        
        
    }

    public void Rotation(float focusBoneDepth, int focusBoneID) {
        float xDegree = SaggitoRotationDegree(focusBoneDepth, focusBoneID);
        float yDegree = TransverseRotationDegree();
        //UnityDebug.Log("xDegree: " + xDegree);
        //UnityDebug.Log("yDegree: " + yDegree);
        Vector3 newRotation = transform.localEulerAngles;
        newRotation.x = xDegree; // Assuming rotation in the sagittal plane is around the x-axis
        newRotation.y = yDegree; 
        //transform.localEulerAngles = newRotation;
        //UnityDebug.Log(boneID + " localEulerAngles: " + transform.localEulerAngles);          
        transform.localRotation = Quaternion.Euler(xDegree*400f, yDegree*500f, 0f);
        //transform.Rotate(xDegree*40000f, 0, 0, Space.Self);
    }

}
