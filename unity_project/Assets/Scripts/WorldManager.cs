using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {
	public bool createWorld=false;
	// Use this for initialization
	public DataManager dm;
	public string chunk="";
	public int minx=1;
	public int maxx=2;
	public int miny=1;
	public int maxy=2;
	public int minz=1;
	public int maxz=2;

	void Start () {
		
	}
	void OnDrawGizmos() {
			if (createWorld==true) {
				CreateWorld();
		 		createWorld=false;
			}
			// if (saveData==true) {
			// 	saveBlockData();
		 // 		saveData=false;
			// }
			

	}



	//a chunk  1000*1000*256
	public void CreateWorld(){
		// int maxx=50;
		// int maxy=50;
		// int maxz=256;
		// int z=1;
		// for(int x=0;x<maxx;x++){
		// 	for(int y=0;y<maxy;y++){
		// 			float md=Mathf.PerlinNoise(x*0.02f, y*0.02f);
		// 			if(md<0.5){
		// 				GameObject go = dm.createBlock(1);
		// 				go.transform.position=new Vector3(x,(int)md*100,y);
		// 			}
		// 		}
		// }
		string itemsPath=Application.dataPath+ "/Resources/tables/world.1.1.txt";
		foreach(string s in DataManager.LoadFile(itemsPath)){

			if(s!=""){
				string[] ss=s.Split('|');
				int index=int.Parse(ss[0]);
				int ix=index%16%16;
				int iz=(index/16)%16; 
				int iy=index/(16*16);
				//Debug.Log(index);
				//Debug.Log(new Vector3( ix,  iy, iz));
				// if(index==0){
				// 	createObject();
				// }
				int count=0;
				//Debug.Log(ss[1].Split(' '));
				//Debug.Log(ss[1].Split(' ').Length);
				foreach (string b in ss[1].Split(' ')){
					// if(count<10){
					// 	Debug.Log(int.Parse(b));
					// }

					int x=(count%16%16)+ix*16;
					int z=((count/16)%16)+iz*16;
					int y=(count/(16*16))+iy*16;

					if(iy>=miny && iy<=maxy&&ix>=minx && ix<=maxx&& iz>=minz && iz<=maxz  && int.Parse(b)!=0 && (count / (16 * 16)) > 7 ){
						GameObject go = dm.createBlock(int.Parse(b));
						go.transform.position=new Vector3(x,  y, z);
						}

				
					count+=1;
				}


			}

		}
		


		//()
		Debug.Log("xx");

	} 
	// Update is called once per frame
	void Update () {
		
	}

	GameObject createObject(){
		GameObject go = new GameObject("new xx");
		MeshFilter mf= go.AddComponent<MeshFilter>();
		MeshRenderer mr= go.AddComponent<MeshRenderer>();

		mf.sharedMesh=CreateMesh(2,4);
		return go;

	}

    Mesh CreateMesh(float width, float height)
    {
        Mesh m = new Mesh();
        m.name = "ScriptedMesh";
        //note: unity is left-hand system
        m.vertices = new Vector3[] {
            new Vector3(-width/2, -height/2, 0),
            new Vector3(-width/2, height/2, 0),
            new Vector3(width/2, height/2, 0),
            new Vector3(width/2, -height/2, 0)
        };
        m.uv = new Vector2[] {
            new Vector2 (0, 0),
            new Vector2 (0, 1),
            new Vector2 (1, 1),
            new Vector2 (1, 0)
        };
        m.triangles = new int[] { 0, 1, 2, 0, 2, 3};
        m.RecalculateNormals();
        m.RecalculateBounds();
        return m;
    }

}
