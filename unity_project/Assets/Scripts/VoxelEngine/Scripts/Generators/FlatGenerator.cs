using UnityEngine;
using System.Collections;
using System.IO;
/// <summary>
/// Flat world generator.
/// </summary>
public class FlatGenerator : ATerrainGenerator
{
    // Block Ids
    private const int grassId = 1;
    private const int dirtId = 2;
    private const int stoneId = 3;
    private const int bedrockId = 5;
    private const int treeId = 6;
    private const int leavesId = 7;

    private System.Random rand;
    public void getdata(CubicTerrainData terrainDataObject, Vector3 worldspace, ListIndex<int> key){
		string itemsPath = Application.dataPath + "/Resources/tables/r.0.-1.txt";
		Stream s = new FileStream(itemsPath, FileMode.Open);
		BinaryReader br = new BinaryReader(s);
		int index = br.ReadInt32();
		while (index != -1)
		{
		  if (index < 1024 && index >= 0)
		  {
		      int sec = br.ReadInt32();
		      for (int iih = 0; iih < 256; iih++)
		      {
		          br.ReadInt32();
		      }
		      int x = index % 32;
		      int z = index / 32;

		              if (x == worldspace.x && z == worldspace.z)
		              {
		                  for (int i = 0; i < sec; i++)
		                  {
		                      for (int ii = 0; ii < 4096; ii++)
		                      {
		                          int vx = ii % 16 % 16;
		                          int vy = ii / 256;
		                          int vz = ii / 16 % 16;
		                          terrainDataObject.SetVoxel(x, vy, z, (short)br.ReadInt32());
		                      }
		                  }
		              }
		      try
		      {
		          index = br.ReadInt32();
		      }
		      catch (System.Exception)
		      {
		          index = -1;
		          Debug.Log("xxxxxxxxxxxxxxxxxxxxxxxxxxxx");
		          //s.Close();
		      }
		  }
		  else
		  {
		      index = -1;
		  }
		}
		//s.Close();


	}
    protected override void GenerateTerrainData(CubicTerrainData terrainDataObject, Vector3 worldspace,ListIndex<int> key)
    {
        //string itemsPath = Application.dataPath + "/Resources/tables/r.0.-1.txt";
        //Stream s = new FileStream(itemsPath, FileMode.Open);
        //BinaryReader br = new BinaryReader(s);

        //int index = br.ReadInt32();
        //while (index != -1)
        //{
        //	if (index < 1024 && index >= 0)
        //	{
        //		int sec = br.ReadInt32();
        //		for (int iih = 0; iih < 256; iih++)
        //		{
        //			br.ReadInt32();
        //		}
        //		int x = index % 32;
        //		int z = index / 32;

        //              if (x == worldspace.x && z == worldspace.z)
        //              {
        //                  for (int i = 0; i < sec; i++)
        //  				{
        //  					for (int ii = 0; ii < 4096; ii++)
        //  					{
        //  						int vx = ii % 16 % 16;
        //  						int vy = ii / 256;
        //  						int vz = ii / 16 % 16;
        //                          terrainDataObject.SetVoxel(x, vy, z, (short)br.ReadInt32());
        //  					}
        //  				}
        //              }
        //		try
        //		{
        //			index = br.ReadInt32();
        //		}
        //		catch (System.Exception)
        //		{
        //			index = -1;
        //			Debug.Log("xxxxxxxxxxxxxxxxxxxxxxxxxxxx");
        //			//s.Close();
        //		}
        //	}
        //	else
        //	{
        //		index = -1;
        //	}
        //}
        ////s.Close();

        //terrainDataObject.setVoxelData(CubicTerrain.GetInstance().preLoadData.listSource[key]);

		terrainDataObject.SetVoxel(0, 0, 0, 7);
        //getdata(terrainDataObject,worldspace,key );
		for (int x = 0; x < terrainDataObject.width; x++)
		{
		    for (int y = 0; y < terrainDataObject.height; y++)
		    {
		        for (int z = 0; z < terrainDataObject.depth; z++)
		        {
		            // Get absolute positions for noise generation
		            float absX = (float)x + worldspace.x;
		            float absZ = (float)z + worldspace.z;
		            float absY = (float)y + worldspace.y;

                    //if (absY < 4)
                    short id=1;
      
      //              if (CubicTerrain.GetInstance().preLoadData.listSource.ContainsKey(key))
      //              {
						////id = CubicTerrain.GetInstance().preLoadData.listSource[key].GetVoxel(x, y, z).blockId;
						//Debug.LogError(id);
                    //}else{

                    //    id = 1;
                    //}

                 
                    //id = 7;
		            //terrainDataObject.SetVoxel(x, y, z, id);

		        }
		    }
		}
	}

}
