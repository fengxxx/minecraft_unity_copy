// using UnityEditor;
// using UnityEngine;
 
// [CustomEditor(typeof(Block))]
// public class BlockInspector : Editor {
// 	public int s=1;

// 	//private Block block { get { return target as Block; } };
// 	void OnEnable()
// 	{

// 	}

// 	void OnDisable()
// 	{

// 	}

// 	public override void OnInspectorGUI ()
// 	{
// 		base.OnInspectorGUI ();
// 		Block block =(Block) target;
// 		if (GUILayout.Button("update"))
// 		{
// 			updateInfo();
// 		}
// 		GUILayout.Space(40);
// 		//EditorGUILayout.TextField(blockm.block.name);
// 		GUI.color =Color.red;

// 	}

// 	void updateInfo(){
// 		Block block =(Block) target;
// 		DataManager dm=block.gameObject.transform.parent.transform.parent.GetComponent<DataManager>();
// 		block.gameObject.name=block.name;
// 		block.gameObject.GetComponent<MeshFilter>().sharedMesh=dm.meshTypes[block.meshType];
// 		MeshRenderer mr=block.gameObject.GetComponent<MeshRenderer>();
// 		if(mr.sharedMaterial.name=="Default-Material"){
// 			Material mate=new Material(Shader.Find ("Standard"));
// 			mate.mainTexture=block.texs[0];
// 			mr.sharedMaterial=mate;
// 		}else{
// 			mr.sharedMaterial.mainTexture=block.texs[0];
// 		}

// 		if(block.meshType==2 || block.meshType==3 || block.meshType==4){
// 			Debug.Log("sharedMaterial");
// 			mr.sharedMaterial.SetFloat("_Mode", 1);
// 		}

// 	}



	
// }