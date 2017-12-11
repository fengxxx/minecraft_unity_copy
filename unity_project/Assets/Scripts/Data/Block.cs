using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block  {//}:MonoBehaviour{

	public string name="noname_block";
	public string typeName="typeName";
	public int ID=0;
	public int  subID=0;


	public int topTexture, bottomTexture, leftTexture, rightTexture, frontTexture, backTexture;
	public Vector2[] topUv, bottomUv, leftUv, rightUv, frontUv, backUv;

	public bool isTransparent;
	public bool transparentBlock;



	public int meshType=0; 
	public Texture2D [] texs;

    //public Tex

	public void updateInfo(){
		////Block block =(Block) target;
		//DataManager dm=this.transform.parent.transform.parent.GetComponent<DataManager>();
		//gameObject.name=ID.ToString()+":"+subID.ToString()+"_"+name;
		//this.GetComponent<MeshFilter>().sharedMesh=dm.meshTypes[meshType];
		//MeshRenderer mr=this.gameObject.GetComponent<MeshRenderer>();
		//if(mr.sharedMaterial.name=="Default-Material"){
		//	Material mate=new Material(Shader.Find ("Standard"));
		//	mate.mainTexture=this.texs[0];
		//	mr.sharedMaterial=mate;
		//}else{
		//	mr.sharedMaterial.mainTexture=this.texs[0];
		//}

		//if(this.meshType==2 || this.meshType==3 || this.meshType==4){
		//	//Debug.Log("sharedMaterial");
		//	mr.sharedMaterial.SetFloat("_Mode", 1);
		//}

	}

    public void registerBlock(int id,string name,int subID){

        ////////


    }
	// // Use this for initialization
	// void Start () {

	// }

	// // Update is called once per frame
	// void Update () {

	// }

	

	/// <summary>
	/// Sets the texture information for the given block.
	/// </summary>
	/// <param name="topTexture">Top texture.</param>
	/// <param name="bottomTexture">Bottom texture.</param>
	/// <param name="leftTexture">Left texture.</param>
	/// <param name="rightTexture">Right texture.</param>
	/// <param name="frontTexture">Front texture.</param>
	/// <param name="backTexture">Back texture.</param>
	/// <param name="transparentBlock">If set to <c>true</c> transparent block.</param>
	public Block SetTextures(int topTexture, int bottomTexture, int leftTexture, int rightTexture, int frontTexture, int backTexture, bool transparentBlock)
	{
		// Save texture ids
		this.topTexture = topTexture;
		this.bottomTexture = bottomTexture;
		this.leftTexture = leftTexture;
		this.rightTexture = rightTexture;
		this.frontTexture = frontTexture;
		this.backTexture = backTexture;

		// Save UV data
        this.topUv = Blocks .GetUvForTexture(topTexture);
        this.bottomUv = Blocks.GetUvForTexture(bottomTexture);
		this.leftUv = Blocks.GetUvForTexture(leftTexture);
		this.rightUv = Blocks.GetUvForTexture(rightTexture);
		this.frontUv = Blocks.GetUvForTexture(frontTexture);
		this.backUv = Blocks.GetUvForTexture(backTexture);
        this.transparentBlock = transparentBlock;

		return this;
	}

	/// <summary>
	/// Gets the uvs for face given face.
	/// </summary>
	/// <returns>The uvs for face.</returns>
	/// <param name="face">Face.</param>
	public Vector2[] GetUvsForFace(BlockFace face)
	{
		switch (face)
		{
			case BlockFace.FRONT:
				return this.frontUv;

			case BlockFace.BACK:
				return this.backUv;

			case BlockFace.LEFT:
				return this.leftUv;

			case BlockFace.RIGHT:
				return this.rightUv;

			case BlockFace.TOP:
				return this.topUv;

			case BlockFace.BOTTOM:
				return this.bottomUv;
		}

		return null;
	}
}
