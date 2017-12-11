using UnityEngine;
using System.Collections;

/// <summary>
/// Demo block bootstrap.
/// </summary>
public class TestBootstrap : ABlockBootstrap
{
	public Texture2D dirtTexture;

	public Texture2D grassSideTexture;
	public Texture2D grassTopTexture;
	public Texture2D stoneTexture;
	public Texture2D glassTexture;
	public Texture2D bedrockTexture;
	public Texture2D treeTexture;
	public Texture2D leavesTexture;

    public DataManager dm;
    
	protected override void Bootstrap()
	{
		//int dirtId = Blocks.AddTexture (this.dirtTexture);
		//int grassSideId = Blocks.AddTexture (this.grassSideTexture);
		//int grassTopId = Blocks.AddTexture (this.grassTopTexture);
		//int stoneId = Blocks.AddTexture (this.stoneTexture);
		//int glassId = Blocks.AddTexture (this.glassTexture);
		//int bedrockId = Blocks.AddTexture (this.bedrockTexture);
		//int leavesId = Blocks.AddTexture (this.leavesTexture);
		//int treeId = Blocks.AddTexture (this.treeTexture);
		

		//Blocks.RegisterBlock (1).SetTextures (grassTopId,dirtId,grassSideId,grassSideId,grassSideId,grassSideId, false);
		//Blocks.RegisterBlock (2).SetTextures (dirtId, dirtId, dirtId, dirtId, dirtId, dirtId, false);
		//Blocks.RegisterBlock (3).SetTextures (stoneId, stoneId, stoneId, stoneId, stoneId, stoneId, false);
		//Blocks.RegisterBlock (4).SetTextures (glassId, glassId, glassId, glassId, glassId, glassId, true);
		//Blocks.RegisterBlock (5).SetTextures (bedrockId, bedrockId, bedrockId, bedrockId, bedrockId, bedrockId, false);
		//Blocks.RegisterBlock (6).SetTextures (treeId, treeId, treeId, treeId, treeId, treeId, false);
		//Blocks.RegisterBlock (7).SetTextures (leavesId, leavesId, leavesId, leavesId, leavesId, leavesId, true);
		//Blocks.RegisterBlock (8).SetTextures (leavesId, treeId, dirtId, grassTopId, grassSideId, glassId, true);


        //dm.init();

        dm.chushihua();


       

  //      foreach (Block b in dm.basicBlocks)
  //      {
  //          Texture2D  tex;
  //          try
  //          {
  //              tex = b.gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture as Texture2D;
  //          }
  //          catch (System.Exception ex)
  //          {
  //              tex=dm.defultTex;
               
  //          }
  //          if (tex==null)
  //          {
		//		tex = dm.defultTex;
  //          }
  // //         Debug.Log(tex);
		//	//Debug.Log(tex.name);
		//	//Debug.Log(tex);
  //          if (tex.width == 32 && tex.height == 32)
  //          {
  //              Blocks.AddTexture(tex as Texture2D);
  //          }
  //      }
  //      Blocks.BuildTextureAtlas();

  //      foreach (Block b in dm.basicBlocks)
  //      {
		//	Texture2D tex;
		//	try
		//	{
		//		tex = b.gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture as Texture2D;
		//	}
		//	catch (System.Exception ex)
		//	{
		//		tex = dm.defultTex;
		//	}
		//	if (tex == null)
		//	{
		//		tex = dm.defultTex;
		//	}
  //          Block block;
		//	if (tex.width == 32 && tex.height == 32)
		//	{
			
  //  			int texid = Blocks.AddTexture(tex as Texture2D);

  //              //Debug.Log(texid);
  //              //Debug.Log(b.ID);
  //              if (b.ID>0)
  //              {
  //                   block = Blocks.RegisterBlock(b.ID);
  //                  if (block)
  //                  {
		//					block.SetTextures(texid, texid, texid, texid, texid, texid, false);
  //                  }
  //              }
  //          }else{

  //               block = Blocks.RegisterBlock(b.ID);
  //              if (block)
  //              {
  //                  block.SetTextures(defultTexID, defultTexID, defultTexID, defultTexID, defultTexID, defultTexID, false);
  //              }
  //          }
		//}

    
    }
}
