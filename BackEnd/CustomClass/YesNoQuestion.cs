using System;
using System.Collections.Generic;
using System.Text;

namespace BackEnd.CustomClass
{
    public class YesNoQuestion
    {
        public Action<bool> YesNoCallback { get; set; }
        public string Question { get; set; }

        public Action<bool> UploadCallback { get; set; }
    }
}
