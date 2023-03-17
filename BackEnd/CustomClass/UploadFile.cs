using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.CustomClass
{
    public class UploadFile
    {
        public Action<bool> IsConfirm { get; set; }

        public Action<string, string, bool> UploadCallback { get; set; }

        Func<UploadFile, UploadFile> func = (db) => { return db; };

        
    }
}
