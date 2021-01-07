using Backend.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.FileHandling
{
    interface IAudio
    {
        public static Song Read(string filePath) => throw new NotImplementedException();
        public static bool Save(string filePath, Song song) => throw new NotImplementedException();
    }
}
