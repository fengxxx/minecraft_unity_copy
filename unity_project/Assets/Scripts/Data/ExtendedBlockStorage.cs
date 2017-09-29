using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendedBlockStorage  {
    public int yBase;
    
    public char[] data ;
    public int[] blocks;
    public int[] heightmap;
    public ExtendedBlockStorage()
    {
        data = new char[4096];
        //blocks = new int[4096];
    }
}
