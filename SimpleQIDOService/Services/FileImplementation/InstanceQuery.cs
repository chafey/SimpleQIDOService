using System.Collections.Generic;
using System.Linq;
using Dicom;
using SimpleQIDOService.Lib;
using SimpleQIDOService.Models;
using SimpleQIDOService.Services.Interfaces;

namespace SimpleQIDOService.Services.FileImplementation
{
    public class InstanceQuery : IInstanceQuery
    {
        public List<Dictionary<string, object>> Execute(string studyUid, Query query)
        {
            var instances = GetInstances(studyUid, null, query);
            var returnTags = GetReturnTags(query);
            return GetResponse(instances, returnTags);
        }

        public List<Dictionary<string, object>> Execute(string studyUid, string seriesUid, Query query)
        {
            var instances = GetInstances(studyUid, seriesUid, query);
            var returnTags = GetReturnTags(query);
            return GetResponse(instances, returnTags);
        }

        private Dictionary<DicomTag, DicomTag> GetReturnTags(Query query)
        {
            // Add in the includeField tags
            var returnTags = DefaultResponseTags.InstaceTags.ToDictionary(tag => tag, null);

            foreach (var includeField in query.IncludeFields)
            {
                returnTags[includeField] = null;
            }

            if (query.IncludeAll)
            {
                foreach (var studyField in DefaultResponseTags.AllInstanceTags)
                {
                    returnTags[studyField] = null;
                }
            }
            return returnTags;
        }

        private List<Dictionary<string, object>> GetResponse(IQueryable<Instance> instanceQueryable, Dictionary<DicomTag, DicomTag> returnTags)
        {
            var resp = new List<Dictionary<string, object>>();
            foreach (var instance in instanceQueryable)
            {
                // Add all tags to the builder
                var builder = new DICOMJSONBuilder(instance.DicomFile);
                foreach (var tag in returnTags.Keys)
                {
                    builder.Add(tag.Group, tag.Element);
                }

                // Add calculated tags (those not in the DICOM file)
                builder.Add(0x0008, 0x1190, null); // retreieve URL - update once we support WADO-RS
                builder.Add(0x0008, 0x0056, "ONLINE");

                resp.Add(builder.GetAttributes());
            }
            return resp;
        }

        private IQueryable<Instance> GetInstances(Study study, string seriesUid)
        {
            if (seriesUid == null)
            {
                return (from s in study.Series.Values
                                 from i in s.Instances.Values
                                 select i).AsQueryable();
            }
            return (from s in study.Series.Values
                    from i in s.Instances.Values
                    where s.SeriesUid == seriesUid
                    select i).AsQueryable();
        }

        private IQueryable<Instance> GetInstances(string studyUid, string seriesUid, Query query)
        {
            Study study = StudyDatabase.Instance().FindByStudyUid(studyUid);
            if (study == null)
            {
                return new List<Instance>().AsQueryable();
            }

            var instances = GetInstances(study, seriesUid);

            // apply filters
            instances = ApplySOPClassUidFilter(instances, query.FindQueryAttribute(0x0008, 0x0016));
            instances = ApplySOPInstanceUidFilter(instances, query.FindQueryAttribute(0x0008, 0x0018));
            instances = ApplyInstanceNumberFilter(instances, query.FindQueryAttribute(0x0020, 0x0013));
            instances = ApplyInstanceAvailabilityFilter(instances, query.FindQueryAttribute(0x0008, 0x0056));

            instances = instances.Take(query.Limit);
            instances = instances.Skip(query.Offset);
            return instances;
        }

        private IQueryable<Instance> ApplyInstanceAvailabilityFilter(IQueryable<Instance> instances, QueryAttribute findQueryAttribute)
        {
            if (findQueryAttribute == null)
            {
                return instances;
            }

            // everything we have is online so return nothing if they query for anything but onlien
            if (findQueryAttribute.RawValue != "ONLINE")
            {
                return new List<Instance>().AsQueryable();
            }
            return instances;
        }

        private IQueryable<Instance> ApplyInstanceNumberFilter(IQueryable<Instance> instances, QueryAttribute findQueryAttribute)
        {
            if (findQueryAttribute == null)
            {
                return instances;
            }

            // Exact match
            return instances.Where(x => x.InstanceNumber == findQueryAttribute.RawValue);
        }

        private IQueryable<Instance> ApplySOPInstanceUidFilter(IQueryable<Instance> instances, QueryAttribute findQueryAttribute)
        {
            if (findQueryAttribute == null)
            {
                return instances;
            }

            // Exact match
            return instances.Where(x => x.SopInstanceUid == findQueryAttribute.RawValue);
        }

        private IQueryable<Instance> ApplySOPClassUidFilter(IQueryable<Instance> instances, QueryAttribute findQueryAttribute)
        {
            if (findQueryAttribute == null)
            {
                return instances;
            }

            // Exact match
            return instances.Where(x => x.SopClassUid == findQueryAttribute.RawValue);
        }
    }
}