using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk {//: MonoBehaviour  {
	public Vector3 pos;
    public Vector3 size = new Vector3(16,256,16); 
    public int index = 0;
    public int[] blocks_ID=new int[4096];
    public ExtendedBlockStorage[] storageArrays;
    public Chunk()
    {
        storageArrays = new ExtendedBlockStorage[1024];
        for (int i = 0; i < storageArrays.Length; i++)
        {
            storageArrays[i] = new ExtendedBlockStorage();
        }
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
