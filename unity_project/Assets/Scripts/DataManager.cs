using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor;
public class DataManager : MonoBehaviour {

	public bool sysOn=false;
	public bool saveData=false;
	public string itemsPath = "";
	public GameObject blockObj;
	public Mesh[] meshTypes;
	public Block[] basicBlocks;
	// Use this for initialization
	void Start () {  

	    }  
	
	// Update is called once per frame
	void Update () {
		
	}

	void init(){
		//本地路径  
		//string itemsPath = System.IO.Path.Combine(Application.Asset.path, "Resources/tables/items.txt");  
		itemsPath=Application.dataPath+ "/Resources/tables/itemsss.txt";
		ArrayList itemsLines= LoadFile(itemsPath);
		basicBlocks=new Block[itemsLines.Count];
		int i=0;
		foreach(string s in itemsLines ){
			// Debug.Log(s);
			if(s.Split('\t').Length>5){
				//GameObject go= createBaicBlock();
				GameObject go=GameObject.Instantiate(blockObj) as GameObject;
				Block block=go.GetComponent<Block>();
				String[] blockStr= s.Split('\t') ;
				//Debug.Log(blockStr);
				block.ID= int.Parse(blockStr[0]);
				block.subID=int.Parse(blockStr[1]);
				block.name=blockStr[2];
				block.typeName=blockStr[3];
				block.meshType=int.Parse(blockStr[4]);
				block.texs=new Texture2D[6];
				//Debug.Log(block);
				basicBlocks[i]=block;
				i+=1;

				go.name=block.ID.ToString()+":"+block.subID.ToString()+"_"+block.name;
				Vector3 pos=new Vector3((block.ID/20),block.subID,block.ID%20);
				go.transform.position=pos;
				


				int tcount=0;
				foreach(string p in blockStr[5].Split('|')){
					string pp=p.Replace(".png","");
					tcount+=1;
					if(tcount==1){
						Material mate=new Material(Shader.Find ("Standard"));
						// Debug.Log(p);
						// Debug.Log(Resources.Load(p));

						if(Resources.Load(pp)){
							Texture2D tex=GameObject.Instantiate( Resources.Load(pp) ) as Texture2D;
							mate.mainTexture=tex;
							block.texs[tcount-1]=tex;
							Debug.Log(pp);
						// Debug.Log(Resources.Load(p));
						}

						go.GetComponent<MeshRenderer>().sharedMaterial=mate;
					}else{
						if(Resources.Load(pp)){
							Texture2D tex=GameObject.Instantiate( Resources.Load(pp) ) as Texture2D;
							block.texs[tcount-1]=tex;
						}

					}

					


				}


				


				// string texPath="";
			
				// if (block.ID>255){
				// 	texPath="textures/items/"+block.name.Replace(" ","_");
				// } else if (block.ID<=255){
				// 	texPath="textures/blocks/"+block.name.Replace(" ","_");
				// }

				// if(Resources.Load(texPath)){
				// 	Texture2D tex=GameObject.Instantiate( Resources.Load(texPath) ) as Texture2D;
				// 	Material mate=new Material(Shader.Find ("Standard"));
				// 	mate.mainTexture=tex;
				// 	go.GetComponent<MeshRenderer>().sharedMaterial=mate;
				// }
				// block.texs=new Texture2D[6];
				// block.texs[0]=Resources.Load(texPath) as Texture2D;


				go.transform.parent=transform.GetChild(0);

				block.updateInfo();
				
		
				}

		}
	}

	public void saveBlockData(){
		itemsPath=Application.dataPath+ "/Resources/tables/itemsss.txt";
		string fileDir = Application.dataPath+"/Resources/textures/" ;
		string lines="";
		int i=0;
		foreach(Block b in  basicBlocks ){
			string l=b.ID.ToString()+"\t"+b.subID.ToString()+"\t"+b.name+"\t"+b.typeName+"\t"+b.meshType+"\t";
			string tPath="";
			int tcount=0;
			foreach(Texture2D t in b.texs){
				tcount+=1;

				if(tcount==b.texs.Length){
						tPath+= AssetDatabase.GetAssetPath(t).Replace("Assets/Resources/","");;

					}else{
				 		tPath+= AssetDatabase.GetAssetPath(t).Replace("Assets/Resources/","")+"|";//.Replace("Assets/Resources","");
				 		//Debug.Log(AssetDatabase.GetAssetPath(t).Replace("Assets/Resources/",""));
					}
			}
			//Debug.Log(tPath);
			l+=tPath;
			lines+=l+"\n";
			
		}
		CreateOrOPenFile(itemsPath,lines);




	}


	void OnDrawGizmos() {
			if (sysOn==true) {
				init();
		 		sysOn=false;
			}
			if (saveData==true) {
				saveBlockData();
		 		saveData=false;
			}
			


	}

	public GameObject createBlock(int blockID){
		GameObject go=null;
		foreach(Block b in basicBlocks){
			if(b.ID==blockID && b.subID==0){
				 go = GameObject.Instantiate(b.gameObject);
			}
		}
		return go;
		
	}
	public GameObject createBaicBlocka(Block block){
		GameObject go=GameObject.Instantiate(blockObj) as GameObject;
		go.name=block.ID.ToString()+"_"+block.name;
		//BlockM bm=go.GetComponent<BlockM>();
		//bm.block=block;
		Block b= go.AddComponent<Block>();
		b=block;
		Vector3 pos=new Vector3((block.ID/20),block.subID,block.ID%20);
		go.transform.position=pos;
		string texPath="";
	
		if (block.ID>255){
			texPath="textures/items/"+block.name.Replace(" ","_");
		} else if (block.ID<=255){
			texPath="textures/blocks/"+block.name.Replace(" ","_");
		}

		if(Resources.Load(texPath)){
			Texture2D tex=GameObject.Instantiate( Resources.Load(texPath) ) as Texture2D;
			Material mate=new Material(Shader.Find ("Standard"));
			mate.mainTexture=tex;
			go.GetComponent<MeshRenderer>().sharedMaterial=mate;
		}
		return go; 
	}
	public GameObject createBaicBlockx(Block block){
			GameObject go=GameObject.Instantiate(blockObj) as GameObject;
			go.name=block.ID.ToString()+"_"+block.name;
			//BlockM bm=go.GetComponent<BlockM>();
			//bm.block=block;
			Block b= go.AddComponent<Block>();
			b=block;
			Vector3 pos=new Vector3((block.ID/20),block.subID,block.ID%20);
			go.transform.position=pos;
			string texPath="";
		
			if (block.ID>255){
				texPath="textures/items/"+block.name.Replace(" ","_");
			} else if (block.ID<=255){
				texPath="textures/blocks/"+block.name.Replace(" ","_");
			}

			if(Resources.Load(texPath)){
				Texture2D tex=GameObject.Instantiate( Resources.Load(texPath) ) as Texture2D;
				Material mate=new Material(Shader.Find ("Standard"));
				mate.mainTexture=tex;
				go.GetComponent<MeshRenderer>().sharedMaterial=mate;
			}
			return go; 
		}

	public static	ArrayList LoadFile(string path)  
	{  
		StreamReader sr =null;  
		try{  
			sr = File.OpenText(path);  
		}catch(Exception e){ 
			Debug.Log(e.Message);
			return null;  
		}  
		string line;  
		ArrayList arrlist = new ArrayList();  
		while ((line = sr.ReadLine()) != null) {  
			arrlist.Add(line);
			//arrlist.Add(System.Convert.ToInt32(line));  
			}  
		sr.Close();  
		sr.Dispose();  
		return arrlist;  
	}   
	void CreateOrOPenFile(string path, string info)
	  {

	  	Debug.Log(path);
		StreamWriter sw;
		FileInfo fi = new FileInfo(path);
		if (!fi.Exists)
		{
		sw = fi.CreateText();
		}
		else
		{
		sw = fi.CreateText();
		}
		sw.WriteLine(info);
		sw.Close();
	  }

 

}
