using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block  :MonoBehaviour{

	public string name="noname_block";
	public string typeName="typeName";
	public int ID=0;
	public int  subID=0;
	


	public int meshType=0; 
	public Texture2D [] texs;






	public void updateInfo(){
		//Block block =(Block) target;
		DataManager dm=this.transform.parent.transform.parent.GetComponent<DataManager>();
		gameObject.name=ID.ToString()+":"+subID.ToString()+"_"+name;
		this.GetComponent<MeshFilter>().sharedMesh=dm.meshTypes[meshType];
		MeshRenderer mr=this.gameObject.GetComponent<MeshRenderer>();
		if(mr.sharedMaterial.name=="Default-Material"){
			Material mate=new Material(Shader.Find ("Standard"));
			mate.mainTexture=this.texs[0];
			mr.sharedMaterial=mate;
		}else{
			mr.sharedMaterial.mainTexture=this.texs[0];
		}

		if(this.meshType==2 || this.meshType==3 || this.meshType==4){
			Debug.Log("sharedMaterial");
			mr.sharedMaterial.SetFloat("_Mode", 1);
		}

	}

	// // Use this for initialization
	// void Start () {
		
	// }
	
	// // Update is called once per frame
	// void Update () {
		
	// }
	
}
