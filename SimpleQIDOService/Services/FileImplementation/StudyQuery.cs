using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SimpleQIDOService.Lib;
using SimpleQIDOService.Models;
using SimpleQIDOService.Services.Interfaces;
using Dicom;

namespace SimpleQIDOService.Services.FileImplementation
{

    public class StudyQuery : IStudyQuery
    {
        public List<Dictionary<string, object>> Execute(Query query)
        {
            var studies = GetStudies(query);
            var returnTags = GetReturnTags(query);
            return GetResponse(studies, returnTags);
        }

        private static List<Dictionary<string, object>> GetResponse(IQueryable<Study> studies, Dictionary<DicomTag, DicomTag> returnTags)
        {
            var resp = new List<Dictionary<string, object>>();
            foreach (var study in studies)
            {
                // Add all tags to the builder
                var builder = new DICOMJSONBuilder(study.Series.First().Value.Instances.First().Value.DicomFile);
                foreach (var tag in returnTags.Keys)
                {
                    builder.Add(tag.Group, tag.Element);
                }

                // Add calculated tags (those not in the DICOM file)
                builder.Add(0x0008, 0x0061, String.Join(",", study.ModalitiesInStudy));
                builder.Add(0x0020, 0x1206, study.NumberOfStudyRelatedSeries.ToString(CultureInfo.InvariantCulture));
                builder.Add(0x0020, 0x1208, study.NumberOfStudyRelatedInstances.ToString(CultureInfo.InvariantCulture));
                builder.Add(0x0008, 0x0056, "ONLINE");
                builder.Add(0x0008, 0x1190, null); // retreieve URL - update once we support WADO-RS
                resp.Add(builder.GetAttributes());
            }
            return resp;
        }

        private static Dictionary<DicomTag, DicomTag> GetReturnTags(Query query)
        {
            // Add in the includeField tags
            var returnTags = DefaultResponseTags.StudyTags.ToDictionary(tag => tag, null);
            foreach (var includeField in query.IncludeFields)
            {
                returnTags[includeField] = null;
            }

            if (query.IncludeAll)
            {
                foreach (var studyField in DefaultResponseTags.AllStudyTags)
                {
                    returnTags[studyField] = null;
                }
            }
            return returnTags;
        }

        private IQueryable<Study> GetStudies(Query query)
        {
            var studies = StudyDatabase.Instance().GetStudies().Values.AsQueryable();
            
            // apply filters
            studies = ApplyStudyDateFilter(studies, query.FindQueryAttribute(0x0008,0x0020));
            studies = ApplyStudyTimeFilter(studies, query.FindQueryAttribute(0x0008, 0x0030));
            studies = ApplyAccessionNumberFilter(studies, query.FindQueryAttribute(0x0008, 0x0050));
            studies = ApplyModalitiesInStudyFilter(studies, query.FindQueryAttribute(0x0008, 0x0061));
            studies = ApplyReferringPhysiciansNameFilter(studies, query.FindQueryAttribute(0x0008, 0x0090));
            studies = ApplyStudyDescriptionFilter(studies, query.FindQueryAttribute(0x0008, 0x1030));
            studies = ApplyPatientNameFilter(studies, query.FindQueryAttribute(0x0010, 0x0010));
            studies = ApplyPatientIdFilter(studies, query.FindQueryAttribute(0x0010, 0x0020));
            studies = ApplyStudyUidFilter(studies, query.FindQueryAttribute(0x0020, 0x000D));
            studies = ApplyStudyIdFilter(studies, query.FindQueryAttribute(0x0020, 0x0010));
            studies = ApplyInstanceAvailabilityFilter(studies, query.FindQueryAttribute(0x0008, 0x0056));

            studies = studies.Take(query.Limit);
            studies = studies.Skip(query.Offset);
            return studies;
        }

        private IQueryable<Study> ApplyStudyUidFilter(IQueryable<Study> studies, QueryAttribute queryAttribute)
        {
            if (queryAttribute == null)
            {
                return studies;
            }

            // Exact match
            return studies.Where(x => x.StudyUid== queryAttribute.RawValue);
        }

        private IQueryable<Study> ApplyInstanceAvailabilityFilter(IQueryable<Study> studies, QueryAttribute queryAttribute)
        {
            if (queryAttribute == null)
            {
                return studies;
            }

            // everything we have is online so return nothing if they query for anything but onlien
            if (queryAttribute.RawValue != "ONLINE")
            {
                return new List<Study>().AsQueryable();
            }
            return studies;
        }

        private IQueryable<Study> ApplyStudyDescriptionFilter(IQueryable<Study> studies, QueryAttribute queryAttribute)
        {
            if (queryAttribute == null)
            {
                return studies;
            }

            // Handle wildcard matching
            if (queryAttribute.RawValue.EndsWith("*"))
            {
                var firstPart = queryAttribute.RawValue.Substring(0, queryAttribute.RawValue.Length - 1);
                return studies.Where(x => x.StudyDescription != null && x.PatientsName.StartsWith(firstPart));
            }

            // Exact match
            return studies.Where(x => x.StudyDescription == queryAttribute.RawValue);
        }

        private IQueryable<Study> ApplyStudyTimeFilter(IQueryable<Study> studies, QueryAttribute queryAttribute)
        {
            if (queryAttribute == null)
            {
                return studies;
            }

            // TODO: Handle wildcard matching??? 

            if (queryAttribute.RawValue.Length > 8)
            {
                // range query
                var startTime= queryAttribute.RawValue.Substring(0, 8);
                var endTime = queryAttribute.RawValue.Substring(9, 8);
                return studies.Where(x => String.Compare(x.StudyTime, startTime, StringComparison.Ordinal) >= 0
                    && String.Compare(x.StudyTime, endTime, StringComparison.Ordinal) < 0);
            }

            // Exact match
            return studies.Where(x => x.StudyTime == queryAttribute.RawValue);
        }

        private IQueryable<Study> ApplyModalitiesInStudyFilter(IQueryable<Study> studies, QueryAttribute queryAttribute)
        {
            if (queryAttribute == null)
            {
                return studies;
            }

            // TODO: Handle wildcard??
            // Exact match
            return studies.Where(x => x.ModalitiesInStudy.Contains(queryAttribute.RawValue));
        }

        private IQueryable<Study> ApplyReferringPhysiciansNameFilter(IQueryable<Study> studies, QueryAttribute queryAttribute)
        {
            if (queryAttribute == null)
            {
                return studies;
            }

            // Handle wildcard matching
            if (queryAttribute.RawValue.EndsWith("*"))
            {
                var firstPart = queryAttribute.RawValue.Substring(0, queryAttribute.RawValue.Length - 1);
                return studies.Where(x => x.ReferringPhysiciansName != null && x.PatientsName.StartsWith(firstPart));
            }

            // Exact match
            return studies.Where(x => x.ReferringPhysiciansName == queryAttribute.RawValue);
        }

        private IQueryable<Study> ApplyStudyIdFilter(IQueryable<Study> studies, QueryAttribute queryAttribute)
        {
            if (queryAttribute == null)
            {
                return studies;
            }

            // Handle wildcard matching
            if (queryAttribute.RawValue.EndsWith("*"))
            {
                var firstPart = queryAttribute.RawValue.Substring(0, queryAttribute.RawValue.Length - 1);
                return studies.Where(x => x.StudyId != null && x.PatientsName.StartsWith(firstPart));
            }

            // Exact match
            return studies.Where(x => x.StudyId == queryAttribute.RawValue);
        }

        private IQueryable<Study> ApplyPatientNameFilter(IQueryable<Study> studies, QueryAttribute queryAttribute)
        {
            if (queryAttribute == null)
            {
                return studies;
            }

            // Handle wildcard matching
            if (queryAttribute.RawValue.EndsWith("*"))
            {
                var firstPart = queryAttribute.RawValue.Substring(0, queryAttribute.RawValue.Length - 1);
                return studies.Where(x => x.PatientsName != null && x.PatientsName.StartsWith(firstPart));
            }

            // Exact match
            return studies.Where(x => x.PatientsName == queryAttribute.RawValue);
        }

        private IQueryable<Study> ApplyPatientIdFilter(IQueryable<Study> studies, QueryAttribute queryAttribute)
        {
            if (queryAttribute == null)
            {
                return studies;
            }

            // Handle wildcard matching
            if (queryAttribute.RawValue.EndsWith("*"))
            {
                var firstPart = queryAttribute.RawValue.Substring(0, queryAttribute.RawValue.Length - 1);
                return studies.Where(x => x.PatientId != null && x.PatientId.StartsWith(firstPart));
            }

            // Exact match
            return studies.Where(x => x.PatientId == queryAttribute.RawValue);
        }

        private IQueryable<Study> ApplyAccessionNumberFilter(IQueryable<Study> studies, QueryAttribute queryAttribute)
        {
            if (queryAttribute == null)
            {
                return studies;
            }

            // Handle wildcard matching
            if (queryAttribute.RawValue.EndsWith("*"))
            {
                var firstPart = queryAttribute.RawValue.Substring(0, queryAttribute.RawValue.Length - 1);
                return studies.Where(x => x.AccessionNumber !=  null && x.AccessionNumber.StartsWith(firstPart));
            }

            // Exact match
            return studies.Where(x => x.AccessionNumber == queryAttribute.RawValue);
        }


        private IQueryable<Study> ApplyStudyDateFilter(IQueryable<Study> studies, QueryAttribute queryAttribute)
        {
            if (queryAttribute == null)
            {
                return studies;
            }

            // TODO: Handle wildcard matching??? 

            if (queryAttribute.RawValue.Length > 8)
            {
                // range query
                var startDate = queryAttribute.RawValue.Substring(0, 8);
                var endDate = queryAttribute.RawValue.Substring(9, 8);
                return studies.Where(x => String.Compare(x.StudyDate, startDate, StringComparison.Ordinal) >= 0
                    && String.Compare(x.StudyDate, endDate, StringComparison.Ordinal) < 0);
            }

            // Exact match
            return studies.Where(x => x.StudyDate == queryAttribute.RawValue);
        }
    }
}