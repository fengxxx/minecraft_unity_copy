using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.IO;

/// <summary>
/// Cubic terrain main class.
/// This class handles chunk generation jobs (not the actual generation), chunk data passing (passes data to the chunk objects), chunk loading / unloading and
/// 
/// IMPORTANT: ATerrainGeneration implementation must be added to the same gameobject this script is added to.
/// </summary>
public class CubicTerrain : MonoBehaviour
{
	/// <summary>
	/// The player transform.
	/// Used to generate the world near the player.
	/// </summary>
	public Transform playerTransform;

	/// <summary>
	/// The chunk preload radius.
	/// </summary>
	public int chunkPreloadRadius = 2;

	// Config
	public int chunkWidth=32;
	public int chunkHeight=16;
	public int chunkDepth=32;

	/// <summary>
	/// How much chunks on y-axis? aka worldheight = chunksOnYAxis * chunkHeight
	/// </summary>
	public int chunksOnYAxis=4;

	/// <summary>
	/// If set to true then the chunk where the player stands on will get loaded first.
	/// </summary>
	public bool loadPlayerChunkFirst = false;

	// Generator
	private ATerrainGenerator terrainGenerator;

	// Lock objects
	private object generationLockObject = new object();

	/// <summary>
	/// The terrain material.
	/// </summary>
	public Material terrainMaterial;

    /// <summary>
    /// The material for transparent blocks rendering.
    /// </summary>
	public Material transparentTerrainMaterial;

	/// <summary>
	/// If smooth chunk loading is activated, lag will get prevented by not loading all chunks at once.
	/// </summary>
	public bool smoothChunkLoading;

	/// <summary>
	/// Determines the value of frames to wait after loading a chunk.
	/// For example if this value is 5, every 5 frames a new chunk will get loaded.
	/// This prevents lags on larger worlds.
	/// </summary>
	public int smoothingFrameDistancePerLoad = 5;

	/// <summary>
	/// If this is set to true chunks will get mesh colliders added to their game objects.
	/// If not the CubicWorld physics system will get used (NOT DONE YET).
	/// However, CubicWorld's physics system is NOT compatible with unity physics and VERY less precise (It is only a simple "block" physics system).
	/// </summary>
	public bool useMeshColliders = true;

	/// <summary>
	/// If this is turned on, the terrain will not load from the given terrain file
	/// </summary>
	public bool serializeTerrain;

    /// <summary>
    /// The path where the chunkfiles will get saved to
    /// </summary>
	public string chunkFilesPath;

    /// <summary>
    /// The terrain file instance.
    /// Gets initialized in the start() function.
    /// </summary>
	[HideInInspector]
	public CubicTerrainFile terrainFile;
	
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
    public List3D<CubicTerrainData> preLoadData;

	/// <summary>
	/// The generation jobs.
	/// </summary>
	private List3D<ChunkGenerationJob> generationJobs;

	/// <summary>
	/// Copy of the transform position.
	/// </summary>
	private Vector3 transformPosition;

	#region Helper functions and classes
	/// <summary>
	/// Gets the chunk position for the given worldspace.
	/// </summary>
	/// <returns>The chunk position.</returns>
	/// <param name="worldspace">Worldspace.</param>
	public Vector3 GetChunkPosition(Vector3 worldspace)
	{
		return new Vector3
		(
			Mathf.FloorToInt(((worldspace.x + this.transformPosition.x) / this.chunkWidth)),
			Mathf.FloorToInt(((worldspace.y + this.transformPosition.y) / this.chunkHeight)),
			Mathf.FloorToInt(((worldspace.z + this.transformPosition.z) / this.chunkDepth))
		);
	}

	/// <summary>
	/// Gets the block position for the given worldspace.
	/// </summary>
	/// <returns>The block position.</returns>
	/// <param name="worldspace">Worldspace.</param>
	public Vector3 GetBlockPosition(Vector3 worldspace)
	{
		return new Vector3
		(
			Mathf.FloorToInt((worldspace.x + this.transformPosition.x)),
			Mathf.FloorToInt((worldspace.y + this.transformPosition.y)),
			Mathf.FloorToInt((worldspace.z + this.transformPosition.z))
		);
	}

	// Chunk generation job
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

	#endregion

	#region Singleton
	private static CubicTerrain instance;

	public static CubicTerrain GetInstance()
	{
		return instance;
	}
    #endregion

    public bool test = true;
	/// <summary>
	/// Initializes the chunk file, chunk generation thread and initializes all needed variables.
	/// </summary>
	public void Start()
	{
		// Make sure we are at 0|0|0
		this.transform.position = new Vector3 (0, 0, 0);

		// Singleton
		if (instance != null)
		{
			Debug.LogError ("Multiple CubicTerrain Script GameObject detected! Error! Disabling this instance.");
			this.enabled = false;
			return;
		}
		instance = this;

		// Terrain stream?
		if (this.serializeTerrain)
			this.terrainFile = new CubicTerrainFile(this.chunkFilesPath+"table.clt", this.chunkFilesPath+"data.cfd");

		// Initialize lists
		this.chunkObjects = new List3D<GameObject> ();
		this.chunkData = new List3D<CubicTerrainData>();
        this.preLoadData = new List3D<CubicTerrainData> ();
		this.generationJobs = new List3D<ChunkGenerationJob> ();

	


		this.terrainGenerator = this.GetComponent<ATerrainGenerator> ();
        this.chunkGenerationThread = new Thread (this.ChunkGenerationThread);
        this.chunkGenerationThread.Start ();

		getData();


		// Init
		this.terrainMaterial.SetTexture ("_MainTex", Blocks.textureAtlas);
		this.transparentTerrainMaterial.SetTexture ("_MainTex", Blocks.textureAtlas);

		if (!this.loadPlayerChunkFirst)
			return;

		Vector3 chunkPosition = this.GetChunkPosition(this.playerTransform.position);
		this.GenerateChunk((int)chunkPosition.x,(int)chunkPosition.z);
		this.transformPosition = this.transform.position;




    }

    /// <summary>
    /// Update this instance.
    /// Loads new chunk and deletes old chunks.
    /// 
    /// Chunk generation checks if the chunk is already existing / quened by checking if there is a chunk at x|0|z.
    /// </summary>
    /// 

    // load files  world 
    // load files  world 
    // load files  world 
    // load files  world 
    // load files  world 
    // load files  world 

   


	public void Update()
	{
		// Load needed chunks
		Vector3 playerPosition = this.playerTransform.position;
		playerPosition.y = 0;
		Vector3 chunkPosition = this.GetChunkPosition(playerPosition);

		for (int x = (int)chunkPosition.x - this.chunkPreloadRadius; x <= (int)chunkPosition.x + this.chunkPreloadRadius; x++)
		{
			for (int z = (int)chunkPosition.z - this.chunkPreloadRadius; z <= (int)chunkPosition.z + this.chunkPreloadRadius; z++)
			{
				if (Vector3.Distance(this.GetChunkPosition(playerPosition), new Vector3(x,0,z)) < this.chunkPreloadRadius &&
				    ! this.chunkObjects.ContainsKey(x,0,z))
				{
					this.GenerateChunk(x,z);
				}
			}
		}

		this.UpdateGenerationData ();
		this.CollectGarbage();

        if (test)
        {
			Debug.Log(preLoadData.listSource.Count);
			//this.
			//getData();
            //CubicTerrain.GetInstance().preLoadData.listSource.Count
			this.UpdateGenerationData();
			this.CollectGarbage();
			//foreach (KeyValuePair<ListIndex<int>, ChunkGenerationJob> job in this.generationJobs.listSource)
			//{
			//	//if (job.Value.done)
			//	//{
			//		// Check if chunk object is in the chunk objects list
			//		if (this.chunkObjects.ContainsKey(job.Key.x, job.Key.y, job.Key.z))
			//		{
			//			// Set chunk data
			//			CubicTerrainChunk chunk = this.chunkObjects[job.Key].GetComponent<CubicTerrainChunk>();
   //                 job.Value.terrainChunkData.SetVoxel(0,0,0,1);
			//		chunk.chunkData = job.Value.terrainChunkData;
			//		//chunk.chunkData = this.chunkData[job.Key];
			//		Debug.Log(job.Value.terrainChunkData.GetVoxel(0, 0, 0));
			//		Debug.Log(chunk.chunkData.GetVoxel(0,0,0));
   //                     chunk.isDirty = true;
			//		    //Debug.Log("xxxx");
					
			//		}
   //             //}else{}
			//}
            test = false;
        }
    }

    /// <summary>
    /// Generates the chunk generation job for x|z.
    /// 
    /// One Chunk generation job generates a all chunks on y-axis for x|z.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="z">The z coordinate.</param>
    private void GenerateChunk(int x, int z)
	{
		for (int y = 0; y < this.chunksOnYAxis; y++)
		{
			if (this.generationJobs.ContainsKey (x,y,z) || this.chunkData.ContainsKey(x,y,z))
				return;

			// Create gameobject
			Vector3 chunkPosition = this.transform.position + new Vector3
			(
				x * this.chunkWidth,
				y * this.chunkHeight,
				z * this.chunkDepth
			);
			
			GameObject chunkObject = new GameObject ("Chunk (" + x + "|" + y + "|" + z + ")");
			chunkObject.transform.position = chunkPosition;
			chunkObject.transform.parent = this.transform;
			chunkObject.layer = this.gameObject.layer;

			CubicTerrainChunk terrainChunk = chunkObject.AddComponent<CubicTerrainChunk> ();
			terrainChunk.chunkPosition = new Vector3 (x, y, z);
           
             

			lock (this.generationLockObject)
			{
				this.chunkObjects.Add (x,y,z, chunkObject);
                if (this.preLoadData.listSource.ContainsKey(new ListIndex<int>(x, y, z)))
                {
                    this.generationJobs.Add(x, y, z, new ChunkGenerationJob(this.preLoadData.listSource[new ListIndex<int>(x, y, z)], chunkPosition));
                    Debug.Log("xx");
                }
                else
                {
                    this.generationJobs.Add(x, y, z, new ChunkGenerationJob(new CubicTerrainData(this, this.chunkWidth, this.chunkHeight, this.chunkDepth), chunkPosition));
                }
                terrainChunk.isDirty = true;
                //getData(terrainChunk.chunkData, new ListIndex<int>(x, 0, z));
                //getData(generationJobs.listSource[new ListIndex<int>(x,y,z)].terrainChunkData, new ListIndex<int>(x, 0, z));
			}
		}
	}

	/// <summary>
	/// Generates all chunks in chunkGenerationQuene.
    /// This function will use the ATerrainGenerator class attached to it's gameobject.
	/// </summary>
	private void ChunkGenerationThread()
	{
		while (true)
		{
			lock(this.generationLockObject)
			{
				foreach (KeyValuePair<ListIndex<int>, ChunkGenerationJob> job in this.generationJobs.listSource)
				{
					if (! job.Value.done)
					{
						// If this chunk was already generated load it from the file.
						if (this.terrainFile != null && this.terrainFile.HasChunk(job.Key.x, job.Key.y, job.Key.z))
						{
							job.Value.terrainChunkData = this.terrainFile.GetChunkData(this, job.Key.x, job.Key.y, job.Key.z, this.chunkWidth, this.chunkHeight, this.chunkDepth);
						}
						else
						{
                            this.terrainGenerator.GenerateChunk(job.Value.terrainChunkData, job.Value.worldspace,job.Key);
						}
						this.chunkData.Add (job.Key.x, job.Key.y, job.Key.z, job.Value.terrainChunkData);
						job.Value.done = true;
					}
				}
			}

			Thread.Sleep (10);
		}
	}

	/// <summary>
	/// Updates the generation data.
	/// If there are finished chunk generations their data will get added to the chunk object.
	/// </summary>
	private void UpdateGenerationData()
	{
		lock(this.generationLockObject)
		{
			List<ListIndex<int>> jobsToDelete = new List<ListIndex<int>>();
			foreach (KeyValuePair<ListIndex<int>, ChunkGenerationJob> job in this.generationJobs.listSource)
			{
				
				if (job.Value.done)
				{
					// Check if chunk object is in the chunk objects list
					if (this.chunkObjects.ContainsKey(job.Key.x, job.Key.y, job.Key.z))
					{
                        
						// Set chunk data
						CubicTerrainChunk chunk = this.chunkObjects[job.Key].GetComponent<CubicTerrainChunk>();
						chunk.master=this;

                        //Debug.Log("==========="+chunk.gameObject.name);
						chunk.chunkData = job.Value.terrainChunkData;
                        //chunk.isDirty = true;
                        //Debug.Log("===========::" + chunk.chunkData.GetVoxel(0,0,0).blockId);
						// Mark job for removal
						jobsToDelete.Add (job.Key);
                    }else{
                        //Debug.Log("+++++|"+new Vector3(job.Key.x,job.Key.y,job.Key.z).ToString()); 
                    }
				}
			}

			// Delete jobs marked for removal
			foreach(ListIndex<int> t in jobsToDelete)
			{
				this.generationJobs.Remove (t);
			}
		}
	}

	/// <summary>
	/// Collects the garbage (not needed chunks) and deletes it.
	/// </summary>
	private void CollectGarbage()
	{
		List<ListIndex<int>> chunksToDelete = new List<ListIndex<int>>();
		
		Vector3 playerPosition = this.playerTransform.position;
		playerPosition.y = 0;

		// Get chunks to delete
		Vector3 chunkVector = Vector3.zero;
		foreach (KeyValuePair<ListIndex<int>, GameObject> chunk in this.chunkObjects.listSource)
		{
			chunkVector = new Vector3(chunk.Key.x, 0, chunk.Key.z);
			if (Vector3.Distance(chunkVector, this.GetChunkPosition(playerPosition)) > this.chunkPreloadRadius)
			{
				chunksToDelete.Add (chunk.Key);
			}
		}
		
		// Delete chunks
		foreach (ListIndex<int> t in chunksToDelete)
		{
			Destroy (this.chunkObjects[t]);
			this.chunkObjects.Remove (t);
			this.chunkData.Remove (t);
		}
	}

	/// <summary>
	/// Gets the chunk object for the given chunk positions.
	/// </summary>
	/// <returns>The chunk object.</returns>
	/// <param name="chunkX">Chunk x.</param>
	/// <param name="chunkZ">Chunk z.</param>
	public GameObject GetChunkObject(int chunkX, int chunkY, int chunkZ)
	{
		return this.chunkObjects [new ListIndex<int>(chunkX, chunkY, chunkZ)];
	}
	
	/// <summary>
	/// Gets the block at position x|y|z.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	/// <returns>The voxel data for the given coordinates, null in case of an error.</returns>
	public CubicTerrainData.VoxelData GetBlock(int x, int y, int z)
	{
		// Calculate chunk position for calculating relative position
		Vector3 chunk = this.GetChunkPosition(new Vector3(x,y,z));
		
		if (! this.chunkData.ContainsKey ((int)chunk.x, (int)chunk.y, (int)chunk.z))
			return null;
		
		CubicTerrainData cData = this.chunkData [new ListIndex<int> ((int)chunk.x, (int)chunk.y, (int)chunk.z)];

		// Calculate relative position
		x -= (int)(chunk.x * this.chunkWidth);
		y -= (int)(chunk.y * this.chunkHeight);
		z -= (int)(chunk.z * this.chunkDepth);
		
		if (x < 0)
			x *= -1;
		if (z < 0)
			z *= -1;

		return cData.GetVoxel(x,y,z);
	}
	
	/// <summary>
	/// Sets the block id at position x|y|z.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	public void SetBlock(int x, int y, int z, short blockId)
	{
		// Calculate chunk position for calculating relative position
		Vector3 chunk = this.GetChunkPosition(new Vector3(x,y,z));
		
		if (! this.chunkData.ContainsKey ((int)chunk.x, (int)chunk.y, (int)chunk.z))
			return;
		
		CubicTerrainData cData = this.chunkData [new ListIndex<int> ((int)chunk.x, (int)chunk.y, (int)chunk.z)];
		
		// Calculate relative position
		x = x - (int)(chunk.x * this.chunkWidth);
		y = y - (int)(chunk.y * this.chunkHeight);
		z = z - (int)(chunk.z * this.chunkDepth);
		
		if (x < 0)
			x *= -1;
		if (z < 0)
			z *= -1;
		
		if (this.chunkData.ContainsKey ((int)chunk.x, (int)chunk.y, (int)chunk.z))
			cData.SetVoxel (x, y, z, blockId);
		else
			Debug.LogError ("Tried to set block to non existing chunk: " + chunk + " at position " + x + "|" + y + "|" + z);
	}

	/// <summary>
	/// Determines whether this instance has block the specified x y z.
	/// Returns also false if the blockid at the given position is less than 0 (which means no block, air)
	/// </summary>
	/// <returns><c>true</c> if this instance has block the specified x y z; otherwise, <c>false</c>.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	public bool HasBlock(int x, int y, int z)
	{
		// Calculate chunk position for calculating relative position
		Vector3 chunk = this.GetChunkPosition(new Vector3(x,y,z));
		
		// Calculate relative position
		x -= (int)(chunk.x * this.chunkWidth);
		y -= (int)(chunk.y * this.chunkHeight);
		z -= (int)(chunk.z * this.chunkDepth);
		
		if (! this.chunkData.ContainsKey ((int)chunk.x, (int)chunk.y, (int)chunk.z))
			return false;
		
		CubicTerrainData cData = this.chunkData [new ListIndex<int> ((int)chunk.x, (int)chunk.y, (int)chunk.z)];
		
		if (x < 0)
			x *= -1;
		if (z < 0)
			z *= -1;

		return cData.HasVoxel(x,y,z);
	}

	/// <summary>
	/// Determines whether this instance has block the specified blockPos.
	/// <see cref="HasBlock(int,int,int)"/>
	/// </summary>
	/// <returns><c>true</c> if this instance has block the specified blockPos; otherwise, <c>false</c>.</returns>
	/// <param name="blockPos">Block position.</param>
	public bool HasBlock(Vector3 blockPos)
	{
		return this.HasBlock ((int)blockPos.x, (int)blockPos.y, (int)blockPos.z);
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	public void OnDestroy()
	{
		// TODO
		if (this.terrainFile != null)
			this.terrainFile.Close ();
	}
	
	/// <summary>
	/// Gets the center position of the block at the given position in worldspace.
	/// </summary>
	/// <returns>The absolute center position.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	public Vector3 GetWorldspaceCenterPosition(int x, int y, int z)
	{
		return new Vector3
		(
			x + 0.5f,
			y + 0.5f,
			z + 0.5f
		);
	}


    public void getData(){//(CubicTerrainData ctd,ListIndex<int> key){
		string itemsPath = Application.dataPath + "/Resources/tables/r.0.-1.txt";
		Stream s = new FileStream(itemsPath, FileMode.Open);
		BinaryReader br = new BinaryReader(s);

        //CubicTerrainData[][] data ;
		int index = br.ReadInt32();
        int debugCount = 0;
		while (index != -1)
		{
			if (index < 32 && index >= 0)
			{
				

				int sec = br.ReadInt32();
				for (int iih = 0; iih < 256; iih++)
				{
					br.ReadInt32();
				}
				int x = index % 32;
				int z = index / 32;
                
                Debug.Log(new Vector3(x, index, z).ToString());
				for (int i = 0; i < sec; i++)
				{
                    //CubicTerrainData ctd = new CubicTerrainData(this, 16, 16, 16);
                    for (int ii = 0; ii < 4096; ii++)
                    {
                        int vx = ii % 16 % 16;
                        int vy = (ii + i * 4096) / 256;
                        int vz = ii / 16 % 16;
                        //ctd.SetVoxel(vx, vy, vz, (short)br.ReadInt32());
                        //                 if (this.generationJobs.listSource.ContainsKey(new ListIndex<int>(x, sec, z) ))
                        //                 {
                        //this.generationJobs.listSource[new ListIndex<int>(x, sec, z)].terrainChunkData.SetVoxel(vx, vy, vz, (short)br.ReadInt32());
                        //CubicTerrain.GetInstance().SetBlock(vx+x*16, vy+sec*16, vz+z*16, (short)br.ReadInt32());
                        //}
                        short id = (short)br.ReadInt32();
                        //this.generationJobs.Add(x, y, z, new ChunkGenerationJob(new CubicTerrainData(this, this.chunkWidth, this.chunkHeight, this.chunkDepth), chunkPosition));
                        //lock (this.generationLockObject)
                        //{

                        //if (key==new ListIndex<int>(x,0,z) )
                        //{
                        //    ctd.SetVoxel(vx, vy, vz, id);
                        //}
                        //CubicTerrainChunk[] ctcs=this.transform.GetComponentsInChildren<CubicTerrainChunk>();
                        ////foreach (CubicTerrainChunk ct in ctcs)
                        ////{
                        ////    if (ct.)
                        ////    {

                        ////    }
                        ////}
                        //ctcs[0].chunkData.SetVoxel(vx, vy, vz,id);
                        //chunkObject.GetComponent<CubicTerrainChunk>().chunkData.SetVoxel(x, y, z, (short)this.blockId);

                         if (!this.preLoadData.listSource.ContainsKey(new ListIndex<int>(x, 0, z)))
                        {

                        this.preLoadData.listSource[new ListIndex<int>(x, 0, z)] = new CubicTerrainData(this, this.chunkWidth, this.chunkHeight, this.chunkDepth);
                        }else{

                            this.preLoadData.listSource[new ListIndex<int>(x, 0, z)].SetVoxel(vx, vy, vz, id);
                        }



                        //                      if (!this.generationJobs.listSource.ContainsKey(new ListIndex<int>(x, 0, z)))
                        //                      {
                        //                          Vector3 chunkPosition = this.transform.position + new Vector3
                        //                          (
                        //                              x * this.chunkWidth,
                        //                              0 * this.chunkHeight,
                        //                              z * this.chunkDepth
                        //                          );
                        //                          this.generationJobs.listSource[new ListIndex<int>(x, 0, z)] = new ChunkGenerationJob(new CubicTerrainData(this, this.chunkWidth, this.chunkHeight, this.chunkDepth), chunkPosition);
                        //                      }
                        //                      ChunkGenerationJob job = this.generationJobs.listSource[new ListIndex<int>(x, 0, z)];
                        //                      if (!job.done)
                        //                      {
                        //                          // If this chunk was already generated load it from the file.
                        //                          if (this.terrainFile != null && this.terrainFile.HasChunk(x, 0, z))
                        //                          {
                        //                              job.terrainChunkData = this.terrainFile.GetChunkData(this, x, 0, z, this.chunkWidth, this.chunkHeight, this.chunkDepth);
                        //                          }
                        //                          else
                        //                          {
                        //                              //this.terrainGenerator.GenerateChunk(job.terrainChunkData, job.worldspace);
                        //                              //job.terrainChunkData.SetVoxel(vx, vy, vz, id);
                        //                              //this.chunkData.listSource[new ListIndex<int>(x, 0, z)].SetVoxel(vx, vy, vz, id);
                        //                          }
                        //                          this.chunkData.Add(x, 0, z, job.terrainChunkData);
                        //                          job.done = true;
                        //                      }
                        //                  //}
                        ////if (index == 0)
                        ////{

                        ////	Debug.Log(new Vector3(x, sec, z).ToString());
                        ////	Debug.Log(new Vector3(vx, vy, vz).ToString());
                        ////	Debug.Log(new Vector3(i, ii, ii / 256 + sec * 4096).ToString());
                        ////	Debug.Log(id);

                        ////	Debug.Log(this.chunkData.listSource.Keys.Count);
                        ////}
                        //ChunkGenerationJob ctj = this.generationJobs.listSource[new ListIndex<int>(x, 0, z)];
                        //                  CubicTerrainData ctd = this.chunkData.listSource[new ListIndex<int>(x, 0, z)];
                        //                  ctj.terrainChunkData.SetVoxel(vx, vy, vz, id);
                        //                  ctd.SetVoxel(vx, vy, vz, id);
                        //                  if (this.chunkObjects.listSource.ContainsKey(new ListIndex<int>(x, 0, z)))
                        //                  {
                        //                      CubicTerrainChunk ctc = this.chunkObjects.listSource[new ListIndex<int>(x, 0, z)].GetComponent<CubicTerrainChunk>();
                        //                      //Debug.Log(ctc.chunkData);
                        //                      if (ctc.chunkData==null)
                        //                      {

                        //                          ctc.chunkData = ctd;
                        //                      }
                        //                      ctc.chunkData.SetVoxel(vx, vy, vz, id);
                        //                  }  

                        //if (debugCount<100)
                        //                 {
                        //Debug.Log(new Vector3(vx, vy, vz).ToString());
                        //Debug.Log(new Vector3(x, sec, z).ToString());
                        //    Debug.Log(id);
                        //    debugCount += 1;
                        //}




                    }
                   //x,sec,z

				}
				try
				{
					index = br.ReadInt32();
				}
				catch (System.Exception)
				{
					index = -1;
					Debug.Log("xxxxxxxxxxxxxxxxxxxxxxxxxxxx");
					s.Close();
				}
                //ctd
			}
			else
			{
				index = -1;
			}
		}
		s.Close();



	}

}
