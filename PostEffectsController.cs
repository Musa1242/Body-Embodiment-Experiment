using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class PostEffectsController : MonoBehaviour
{
    public Shader postShader;
    Material postEffectMaterial;
    public Color screenTint;
    RenderTexture postRenderTexture;
    // Start is called before the first frame update
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (postEffectMaterial == null)
        {
            postEffectMaterial = new Material(postShader);
        }
        if (postRenderTexture == null)
        {
            postRenderTexture = new RenderTexture(src.width, src.height, 0, src.format);
        }
        postEffectMaterial.SetColor("_ScreenTint", screenTint);

        int width = src.width;
        int height = src.height;
        //first Blit
        RenderTexture endRenderTexture = RenderTexture.GetTemporary(width, height, 0, src.format);
        Graphics.Blit(src, endRenderTexture, postEffectMaterial,1);
        RenderTexture startRenderTexture = endRenderTexture;

       for (int i = 0; i < 4; i++)
       {
            width = width / 2;
            height = height / 2;
            endRenderTexture = RenderTexture.GetTemporary(width, height, 0, src.format);
            Graphics.Blit(startRenderTexture, endRenderTexture, postEffectMaterial, 1);
            RenderTexture.ReleaseTemporary(startRenderTexture);
            startRenderTexture = endRenderTexture;
       }

       for (int i = 4; i >= 0; i--)
       {
            width *= 2;
            height *= 2;
            endRenderTexture = RenderTexture.GetTemporary(width, height, 0, src.format);
            Graphics.Blit(startRenderTexture, endRenderTexture, postEffectMaterial, 2);
            RenderTexture.ReleaseTemporary(startRenderTexture);
            startRenderTexture = endRenderTexture;
       }




        //output
        //Graphics.Blit(src, postRenderTexture, postEffectMaterial,1);
        Shader.SetGlobalTexture("_GlobalRenderTexture", startRenderTexture);
        Graphics.Blit(startRenderTexture, dest);
        RenderTexture.ReleaseTemporary(startRenderTexture);
        
    }

    
}
