using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using fNbt;


using NUnit.Framework;

public class ReadChunk : MonoBehaviour {


    public bool readTest= false;
    public string chunkFile = "";
    // Use this for initialization
    void Start () {
		
	}
    void OnDrawGizmos()
    {
        if (readTest == true)
        {
            readChunk(chunkFile);
            //Debug.Log(chunkFile);
            readTest = false;
        }
      



    }


    void readChunk(string fileName)
    {

    using (FileStream fs = File.OpenRead(fileName))
    {
        var reader = new NbtReader(fs);
        Debug.Log(fileName);
        Assert.AreEqual(fs, reader.BaseStream);
        while (reader.ReadToFollowing())
        {
            Debug.Log("@" + reader.TagStartOffset + " ");
            Debug.Log(reader.ToString());
        }
        Assert.AreEqual("Level", reader.RootName);
    }


    }
    // Update is called once per frame
    void Update () {
		
	}
}
