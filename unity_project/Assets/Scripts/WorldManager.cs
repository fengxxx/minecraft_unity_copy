﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {
	public bool createWorld=false;
	public bool meshDebug=false;
	public MeshFilter mf;
	// Use this for initialization
	public DataManager dm;
	// public string chunk="";
	public int minx=1;
	public int maxx=2;
	public int miny=1;
	public int maxy=2;
	public int minz=1;
	public int maxz=2;


    public List<Chunk> chunks=new List<Chunk>() ;
	void Start () {
		
	}

	void OnDrawGizmos() {
			if (createWorld==true) {
				CreateWorld();
		 		createWorld=false;
			}
			if (meshDebug==true) {
				MeshDebug();
		 		meshDebug=false;
			}
			

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
		// if(chunks.Length==0){
		// 	chunks=new Chunk[10];
		// }
		
		string itemsPath=Application.dataPath+ "/Resources/tables/world.1.1.txt";
		int i=0;
		foreach(string s in DataManager.LoadFile(itemsPath)){
			if(s!=""){
				string[] ss=s.Split('|');
				int index=int.Parse(ss[0]);
                Chunk chunk = new Chunk();
                chunk.index = index;
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
                    chunk.blocks_ID[count] = int.Parse(b);
					int x=(count%16%16)+ix*16;
					int z=((count/16)%16)+iz*16;
					int y=(count/(16*16))+iy*16;

					if(iy>=miny && iy<=maxy&&ix>=minx && ix<=maxx&& iz>=minz && iz<=maxz  && int.Parse(b)!=0 && (count/(16*16))>13 ){
						//GameObject go = dm.createBlock(int.Parse(b));
						//go.transform.position=new Vector3(x,  y, z);
						}

				
					count+=1;
				}

                chunks.Add(chunk);
			}
			i+=1;

		}

        foreach (Chunk c in chunks)
        {
            //Debug.Log(c.index);
            if (c.index < 10)
            {


                int ix = c.index % 16 % 16;
                int iz = (c.index / 16) % 16;
                int iy = c.index / (16 * 16);
                int countt = 0;
                foreach (int id in c.blocks_ID)
                {
                    int x = (countt % 16 % 16) + ix * 16;
                    int z = ((countt / 16) % 16) + iz * 16;
                    int y = (countt / (16 * 16)) + iy * 16;

                    if (checkBlockRender(c, countt) && id !=0)
                    {
                        GameObject go = dm.createBlock(id);
                        go.transform.position = new Vector3(x, y, z);

                    }

                    countt += 1;
                }
            }

        }
	} 
	// Update is called once per frame
	void Update () {
		
	}

	void MeshDebug(){
		//Debug.Log(c.blocks_ID[0]);
		//Debug.Log(mf.sharedMesh.vertices[1]);
		// foreach(Vector3 v in  mf.sharedMesh.vertices){
		// 	Debug.Log(v);

		// }
		//Debug.Log(mf.sharedMesh.vertices.Length);
		//Debug.Log(mf.sharedMesh.GetIndexCount(0));
		//Debug.Log(mf.sharedMesh.GetIndexStart(0));	
		//Debug.Log(mf.sharedMesh.GetIndexCount(1));
		//Debug.Log(mf.sharedMesh.GetIndexStart(1));
		//Debug.Log(mf.sharedMesh.subMeshCount);
		//Debug.Log(mf.sharedMesh.uv[3]);
		//Debug.Log(mf.sharedMesh.triangles[1]);
		//Debug.Log(mf.sharedMesh.name);


	}

    Mesh renderChunkMesh(Chunk chunk){
        Mesh mesh = new Mesh();





        return mesh;
    }

    int codeToIndex(int x,int y,int z){

        int  index = z  * 16 + x + 256 * y;

        if (index>4095)
        {

			Debug.Log(index);
			Debug.Log(x);
			Debug.Log(y);
			Debug.Log(z);
        }
        return index;

    }

    bool checkBlockRender(Chunk chunk, int index){
        //int[] bound = new int[6];
        bool isBound = false;
		int x = (index % 16 % 16);
		int z = ((index / 16) % 16);
		int y = (index / (16 * 16));


        if (x + 1 < 16)
        {
            if (chunk.blocks_ID[codeToIndex(x + 1, y, z)] == 0)
            {
                return true;
            }
        }else{
            return true;
        }

        if (x - 1 > 0)
        {
            if (chunk.blocks_ID[codeToIndex(x - 1, y, z)] == 0)
            {
                return true;
            }
        }else{

            return true;
        }

		if (y + 1 < 16)
		{
			if (chunk.blocks_ID[codeToIndex(x , y+1, z)] == 0)
			{
				return true;
			}
		}
		else
		{
			return true;
		}

		if (y - 1 > 0)
		{
			if (chunk.blocks_ID[codeToIndex(x , y- 1, z)] == 0)
			{
				return true;
			}
		}
		else
		{

			return true;
		}

		if (z + 1 < 16)
		{
			if (chunk.blocks_ID[codeToIndex(x, y, z+1)] == 0)
			{
				return true;
			}
		}
		else
		{
			return true;
		}

		if (z - 1 > 0)
		{
			if (chunk.blocks_ID[codeToIndex(x, y , z-1)] == 0)
			{
				return true;
			}
		}
		else
		{

			return true;
		}

		//if (y + 1 <16 && chunk.blocks_ID[codeToIndex(x,y + 1, z)] == 0)
		//{
		//	return true;
		//}
		//if (y - 1 > 0 && chunk.blocks_ID[codeToIndex(x,y - 1, z)] == 0)
		//{
		//	return true;
		//}
		//if (z + 1 < 16 && chunk.blocks_ID[codeToIndex(x, y, z+1)] == 0)
		//{
		//	return true;
		//}
		//if (z - 1 > 0 && chunk.blocks_ID[codeToIndex(x, y, z-1)] == 0)
		//{
		//	return true;
		//}

        return isBound;
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
