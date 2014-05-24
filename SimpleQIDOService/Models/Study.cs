using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimpleQIDOService.Models
{
    public class Study
    {
        public Study()
        {
            Series = new Dictionary<string, Series>();
        }

        public string StudyUid { get; set; }
        public string StudyId { get; set; }
        public string StudyDate { get; set; }
        public string StudyTime { get; set; }
        public string AccessionNumber { get; set; }
        public string StudyDescription { get; set; }
        public string PatientsName { get; set; }
        public string ReferringPhysiciansName { get; set; }
        public string PatientId { get; set; }
        public string PatientBirthDate { get; set; }
        public string PatientSex { get; set; }

        public int NumberOfStudyRelatedSeries
        {
            get { return Series.Count; }
        }

        public int NumberOfStudyRelatedInstances
        {
            get { return Series.Sum(x => x.Value.NumberOfSeriesRelatedInstances); }
        }

        // derived
        public IEnumerable<string> ModalitiesInStudy
        {
            get { return Series.Select(x => x.Value.Modality).Distinct(); }
        }

        public Dictionary<string, Series> Series { get; set; }
    }
}