using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dicom;

namespace SimpleQIDOService.Models
{
    public class Instance
    {
        public string SopInstanceUid { get; set; }
        public string SopClassUid { get; set; }
        public string InstanceNumber { get; set; }
        public string Rows { get; set; }
        public string Columns { get; set; }
        public string BitsAllocated { get; set; }
        public string NumberOfFrames { get; set; }
        public DicomFile DicomFile { get; set; }
    }
}