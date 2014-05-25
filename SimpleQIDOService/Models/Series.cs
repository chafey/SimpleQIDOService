using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleQIDOService.Models
{
    public class Series
    {
        public Series()
        {
            Instances = new Dictionary<string, Instance>();
        }
        public string SeriesUid { get; set; }
        public Dictionary<string, Instance> Instances { get; set; }
        public string SeriesDescription { get; set; }
        public string Modality { get; set; }
        public string SeriesNumber { get; set; }
        public string PerformedProcedureStepStartDate;
        public string PerformedProcedureStepStartTime;

        public int NumberOfSeriesRelatedInstances
        {
            get { return Instances.Count; }
        }
    }
}