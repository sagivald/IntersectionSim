using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthTextureChecker : MonoBehaviour {

	public bool save,getPixel;
	public int intX,intY;

	public string savedTextureFilename, savePathInAssets = "/Save/";

	public RenderTexture rendTex;
	Texture2D textureToSave;

	Texture2D toTexture2D(RenderTexture rTex)
	{
		Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBAFloat, false);
		RenderTexture.active = rTex;
		tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
		tex.Apply();
		return tex;
	}

	void SaveTextureToFile (Texture2D texture, string filename) { 
		Debug.Log("Saving... " + Application.dataPath + savePathInAssets + filename + ".png");
    	System.IO.File.WriteAllBytes (Application.dataPath + "/Save/" + filename + ".png", texture.EncodeToPNG());
		Debug.Log("Saved");
 	}

	private void OnValidate() {

		textureToSave = toTexture2D(rendTex);

		if (getPixel == true){
			getPixel = false;

			Debug.Log(textureToSave.GetPixel(intX,intY).r.ToString("F8"));
			
		}

		if (save == true){
			save = false;
			
			SaveTextureToFile(textureToSave,savedTextureFilename);
		}
	}

}
