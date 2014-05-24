using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dicom;

namespace SimpleQIDOService.Models
{
    public class QueryAttribute
    {
        public string RawValue { get; set; }
        public string RawKey { get; set; }

        public DicomTag Tag { get; set; }

    }
}