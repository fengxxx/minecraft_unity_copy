using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;


public class FengxTools : EditorWindow
{
 [MenuItem("fengx/UpdateSelected")]
    public static void buildChain()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
        	Block b =obj.GetComponent<Block>();
            if(b!=null){
        		b.updateInfo();
                //Debug.Log(b.gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture.name);
        	}
        }
    }

}