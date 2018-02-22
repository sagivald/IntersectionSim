using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GPULidar : MonoBehaviour
{
    Material material;
    float depthLevel = 1;
    public Shader Shade;
    public float horizontalFOV, verticalFOV;
    Matrix4x4 projMat;
    float horizontalMval, verticalMval;
     public Camera depthCam;
    RenderTexture depthImage;
	public bool DrawLidar;

    // Use this for initialization
    void Start()
    {
        depthCam = GetComponent<Camera>();
        material = new Material(Shade);
        projMat = depthCam.projectionMatrix;
        autoFOV();
    }

    // Update is called once per frame
	
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (Shade != null)
        {
            material.SetFloat("_DepthLevel", depthLevel);
            Graphics.Blit(src, dest, material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

    public void autoFOV()
    {

        horizontalMval = 1 / (Mathf.Tan((horizontalFOV / 2) * (Mathf.PI / 180f)));
        verticalMval = 1 / (Mathf.Tan((verticalFOV / 2) * (Mathf.PI / 180f)));
        if(depthCam){projMat[0, 0] = horizontalMval;
        projMat[1, 1] = verticalMval;
        depthCam.projectionMatrix = projMat;     }   
    }
    // private void OnValidate()
    // {
        // autoFOV();
        // depthCam.projectionMatrix = projMat;
    // }
}
