using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.IO;

public class WorldManager : MonoBehaviour {
	public Transform playerTransform;
    public int chunkPreloadRadius = 2;

	public int chunkWidth = 32;
	public int chunkHeight = 16;
	public int chunkDepth = 32;
    public int chunksOnYAxis = 4;
    public bool loadPlayerChunkFirst = false;
    private object generationLockObject = new object();

    public Material terrainMaterial;
    public Material transparentTerrainMaterial;
	public bool smoothChunkLoading;



	public bool serializeTerrain;
	/// <summary>
	/// The chunk generation thread.
	/// </summary>
	private Thread chunkGenerationThread;

	/// <summary>
	/// The chunk game objects.
	/// </summary>
	private List3D<GameObject> chunkObjects;

	/// <summary>
	/// The chunk data.
	/// </summary>
	private List3D<CubicTerrainData> chunkData;

	/// <summary>
	/// The generation jobs.
	/// </summary>
	private List3D<ChunkGenerationJob> generationJobs;

	/// <summary>
	/// Copy of the transform position.
	/// </summary>
	private Vector3 transformPosition;
	public bool createWorld=false;
	public bool meshDebug=false;
	public MeshFilter mf;
	// Use this for initialization
	public DataManager dm;
    public Chunk chunk;


	// This class holds information which gets used in the generation thread for generating chunks.
	class ChunkGenerationJob
	{
		public CubicTerrainData terrainChunkData;
		public bool done = false;
		public Vector3 worldspace;

		public ChunkGenerationJob(CubicTerrainData terrainChunkData, Vector3 worldspace)
		{
			this.terrainChunkData = terrainChunkData;
			this.worldspace = worldspace;
		}
	}
    private static WorldManager instance;

	public static WorldManager GetInstance()
	{
		return instance;
	}

	public Vector3 GetChunkPosition(Vector3 worldspace)
	{
		return new Vector3
		(
			Mathf.FloorToInt(((worldspace.x + this.transformPosition.x) / this.chunkWidth)),
			Mathf.FloorToInt(((worldspace.y + this.transformPosition.y) / this.chunkHeight)),
			Mathf.FloorToInt(((worldspace.z + this.transformPosition.z) / this.chunkDepth))
		);
	}

	public Vector3 GetBlockPosition(Vector3 worldspace)
	{
		return new Vector3
		(
			Mathf.FloorToInt((worldspace.x + this.transformPosition.x)),
			Mathf.FloorToInt((worldspace.y + this.transformPosition.y)),
			Mathf.FloorToInt((worldspace.z + this.transformPosition.z))
		);
	}
	void StartX () {
		this.transform.position = new Vector3(0, 0, 0);

		// Singleton
		if (instance != null)
		{
			Debug.LogError("Multiple CubicTerrain Script GameObject detected! Error! Disabling this instance.");
			this.enabled = false;
			return;
		}
		instance = this;

		// Terrain stream?
		//if (this.serializeTerrain)
			//this.terrainFile = new CubicTerrainFile(this.chunkFilesPath + "table.clt", this.chunkFilesPath + "data.cfd");

		// Initialize lists
		this.chunkObjects = new List3D<GameObject>();
		this.chunkData = new List3D<CubicTerrainData>();
		this.generationJobs = new List3D<ChunkGenerationJob>();

		//this.terrainGenerator = this.GetComponent<ATerrainGenerator>();
		//this.chunkGenerationThread = new Thread(this.ChunkGenerationThread);
		//this.chunkGenerationThread.Start();

		// Init
		this.terrainMaterial.SetTexture("_MainTex", Blocks.textureAtlas);
		this.transparentTerrainMaterial.SetTexture("_MainTex", Blocks.textureAtlas);

		if (!this.loadPlayerChunkFirst)
			return;

		//Vector3 chunkPosition = this.GetChunkPosition(this.playerTransform.position);
		//this.GenerateChunk((int)chunkPosition.x, (int)chunkPosition.z);
		this.transformPosition = this.transform.position;

	}

	// Update is called once per frame
	void UpdateX()
	{
		//// Load needed chunks
		Vector3 playerPosition = this.playerTransform.position;
		playerPosition.y = 0;
		Vector3 chunkPosition = this.GetChunkPosition(playerPosition);
		for (int x = (int)chunkPosition.x - this.chunkPreloadRadius; x <= (int)chunkPosition.x + this.chunkPreloadRadius; x++)
		{
			for (int z = (int)chunkPosition.z - this.chunkPreloadRadius; z <= (int)chunkPosition.z + this.chunkPreloadRadius; z++)
			{
				if (Vector3.Distance(this.GetChunkPosition(playerPosition), new Vector3(x, 0, z)) < this.chunkPreloadRadius &&
					!this.chunkObjects.ContainsKey(x, 0, z))
				{
					//this.GenerateChunk(x, z);
				}
			}
		}
		//this.UpdateGenerationData();
		//this.CollectGarbage();

        // 
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
        chunk = new Chunk();
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


		//string itemsPath = Application.dataPath + "/Resources/tables/r.-1.-1.txt";
		//string itemsPath = Application.dataPath + "/Resources/tables/r.3.-9.txt";
		//string itemsPath = Application.dataPath + "/Resources/tables/r.3.8.txt";
		//string itemsPath = Application.dataPath + "/Resources/tables/r.14.-5.txt";
		//string itemsPath = Application.dataPath + "/Resources/tables/r.-6.-3.txt";
		string itemsPath=Application.dataPath+ "/Resources/tables/r.0.-1.txt";


        Stream s = new FileStream(itemsPath, FileMode.Open);
        BinaryReader br = new BinaryReader(s);
       
        //int iCount = br.ReadInt64();
        int index = br.ReadInt32();
		
        
        //chunk.storageArrays = new ExtendedBlockStorage[16];
        //chunk.index = index;

        while (index!=-1) {
            if (index <1026&&index>=0)
            {

				int sec = br.ReadInt32();
                Debug.Log(index.ToString()+"::"+sec.ToString());
                for (int iih = 0; iih < 256; iih++)
                {
                    chunk.storageArrays[index].heightmap[iih] = br.ReadInt32();
                }
            
                chunk.storageArrays[index].yBase = index;
                chunk.storageArrays[index].blocks = new int[sec * 4096];
                for (int i = 0; i < sec; i++)
                {
                    for (int ii = 0; ii < 4096; ii++)
                    {
                        chunk.storageArrays[index].blocks[ii + i * 4096] = br.ReadInt32();
                    
                    }
                    //Debug.Log(index);
                    //Debug.Log(chunk.storageArrays[index].blocks[0]);
                }
                //br.ReadChars(4096);
                try
                {
                    index = br.ReadInt32();
                }
                catch (System.Exception)
                {
                    index = -1;
                    Debug.Log("xxxxxxxxxxxxxxxxxxxxxxxxxxxx");
                    s.Close();
                    //throw;
                }
            }
            else {
                index = -1;
            }
        }
        s.Close();

        //Debug.Log(chunk.storageArrays[0].heightmap[0]);
        //Debug.Log(chunk.storageArrays[1].heightmap[1]);
        //Debug.Log(chunk.storageArrays[2].heightmap[2]);
        //Debug.Log(chunk.storageArrays[3].heightmap[3]);
        //Debug.Log(chunk.storageArrays[0].data[4]);
        //Debug.Log(chunk.storageArrays[0].data[5]);
        int countt = 0;
        foreach (ExtendedBlockStorage ebs in chunk.storageArrays)
        {
            if (countt < 1200)
            {
                for (int ih = 0; ih < ebs.heightmap.Length; ih++)
                {
                    int x = ih % 16;// + (countt % 16) * 16;
                    int y = ebs.heightmap[ih] - 1;
                    int z = ih / 16;// + (countt /16) * 16;
                    int ebsIndex = codeToIndex(x, y, z);
                    if (ebsIndex>0)
                    {
                        int id = ebs.blocks[ebsIndex];
                        if (id !=0&&id!=9)
                        {
                            //GameObject go = dm.createBlock(id);
                            //int xx = x + (countt % 32) * 16;
                            //int yy = y;// - 1;
                            //int zz = z+ (countt / 32) * 16;
                            //go.transform.position = new Vector3(xx, yy, zz);

						}
                    }
                }
            }
            countt += 1;
        }
    }


	void MeshDebug(){
	

	}

    Mesh renderChunkMesh(Chunk chunk){
        Mesh mesh = new Mesh();

        return mesh;
    }

   
    int codeToIndex(int x,int y,int z){
        int  index = z  * 16 + x + 256 * y;

        return index;
    }

    bool checkBlockRender(Chunk chunk,int sec, int index){
        //int[] bound = new int[6];
        bool isBound = false;
		int x = (index % 16 % 16);
		int z = ((index / 16) % 16);
		int y = (index / (16 * 16));

        if (x + 1 < 16)
        {
            if (chunk.storageArrays[sec].blocks[codeToIndex(x + 1, y, z)] == 0)
            {
                return true;
            }
        }else{
            return true;
        }
        if (x - 1 > 0)
        {
            if (chunk.storageArrays[sec].blocks[codeToIndex(x - 1, y, z)] == 0)
            {
                return true;
            }
        }else{
            return true;
        }
		if (y + 1 < 256)
		{
			if (chunk.storageArrays[sec].blocks[codeToIndex(x , y+1, z)] == 0)
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
			if (chunk.storageArrays[sec].blocks[codeToIndex(x , y- 1, z)] == 0)
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
			if (chunk.storageArrays[sec].blocks[codeToIndex(x, y, z+1)] == 0)
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
			if (chunk.storageArrays[sec].blocks[codeToIndex(x, y , z-1)] == 0)
			{
				return true;
			}
		}
		else
		{

			return true;
		}

		//if (y + 1 <16 && chunk.storageArrays[sec].blocks[codeToIndex(x,y + 1, z)] == 0)
		//{
		//	return true;
		//}
		//if (y - 1 > 0 && chunk.storageArrays[sec].blocks[codeToIndex(x,y - 1, z)] == 0)
		//{
		//	return true;
		//}
		//if (z + 1 < 16 && chunk.storageArrays[sec].blocks[codeToIndex(x, y, z+1)] == 0)
		//{
		//	return true;
		//}
		//if (z - 1 > 0 && chunk.storageArrays[sec].blocks[codeToIndex(x, y, z-1)] == 0)
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
