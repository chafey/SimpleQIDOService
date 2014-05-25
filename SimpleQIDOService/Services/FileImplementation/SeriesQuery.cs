using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Dicom;
using SimpleQIDOService.Lib;
using SimpleQIDOService.Models;
using SimpleQIDOService.Services.Interfaces;

namespace SimpleQIDOService.Services.FileImplementation
{
    public class SeriesQuery : ISeriesQuery
    {
        public List<Dictionary<string, object>> Execute(string studyUid, Query query)
        {
            var series = GetSeries(studyUid, query);
            var returnTags = GetReturnTags(query);
            return GetResponse(series, returnTags);
        }

        private Dictionary<DicomTag, DicomTag> GetReturnTags(Query query)
        {
            // Add in the includeField tags
            var returnTags = DefaultResponseTags.SeriesTags.ToDictionary(tag => tag, null);
            
            foreach (var includeField in query.IncludeFields)
            {
                returnTags[includeField] = null;
            }

            if (query.IncludeAll)
            {
                foreach (var studyField in DefaultResponseTags.AllSeriesTags)
                {
                    returnTags[studyField] = null;
                }
            }
            return returnTags;
        }

        private List<Dictionary<string, object>> GetResponse(IQueryable<Series> seriesQueryable, Dictionary<DicomTag, DicomTag> returnTags)
        {
            var resp = new List<Dictionary<string, object>>();
            foreach (var series in seriesQueryable)
            {
                // Add all tags to the builder
                var builder = new DICOMJSONBuilder(series.Instances.First().Value.DicomFile);
                foreach (var tag in returnTags.Keys)
                {
                    builder.Add(tag.Group, tag.Element);
                }

                // Add calculated tags (those not in the DICOM file)
                builder.Add(0x0008, 0x1190, null); // retreieve URL - update once we support WADO-RS
                builder.Add(0x0008, 0x0056, "ONLINE");
                builder.Add(0x0020, 0x1209, series.NumberOfSeriesRelatedInstances.ToString(CultureInfo.InvariantCulture));

                resp.Add(builder.GetAttributes());
            }
            return resp;
        }

        private IQueryable<Series> GetSeries(string studyUid, Query query)
        {
            Study study = StudyDatabase.Instance().FindByStudyUid(studyUid);
            if (study == null)
            {
                return new List<Series>().AsQueryable();
            }

            var series = study.Series.Values.AsQueryable();

            // apply filters
            series = ApplyModalityFilter(series, query.FindQueryAttribute(0x0008, 0x0060));
            series = ApplySeriesDescriptionFilter(series, query.FindQueryAttribute(0x0008, 0x103E));
            series = ApplySeriesUidFilter(series, query.FindQueryAttribute(0x0020, 0x000E));
            series = ApplySeriesNumberFilter(series, query.FindQueryAttribute(0x0020, 0x0011));
            series = ApplyPerformedProcedureStepStartDateFilter(series, query.FindQueryAttribute(0x0040, 0x0244));
            series = ApplyPerformedProcedureStepStartTimeFilter(series, query.FindQueryAttribute(0x0040, 0x0245));
            series = ApplyInstanceAvailabilityFilter(series, query.FindQueryAttribute(0x0008, 0x0056));

            series = series.Take(query.Limit);
            series = series.Skip(query.Offset);
            return series;
        }

        private IQueryable<Series> ApplyPerformedProcedureStepStartTimeFilter(IQueryable<Series> series, QueryAttribute findQueryAttribute)
        {
            if (findQueryAttribute == null)
            {
                return series;
            }

            // TODO: Handle wildcard matching??? 

            if (findQueryAttribute.RawValue.Length > 8)
            {
                // range query
                var startDate = findQueryAttribute.RawValue.Substring(0, 8);
                var endDate = findQueryAttribute.RawValue.Substring(9, 8);
                return series.Where(x => String.Compare(x.PerformedProcedureStepStartTime, startDate, StringComparison.Ordinal) >= 0
                    && String.Compare(x.PerformedProcedureStepStartTime, endDate, StringComparison.Ordinal) < 0);
            }

            // Exact match
            return series.Where(x => x.PerformedProcedureStepStartTime == findQueryAttribute.RawValue);
        }

        private IQueryable<Series> ApplyPerformedProcedureStepStartDateFilter(IQueryable<Series> series, QueryAttribute findQueryAttribute)
        {
            if (findQueryAttribute == null)
            {
                return series;
            }

            // TODO: Handle wildcard matching??? 

            if (findQueryAttribute.RawValue.Length > 8)
            {
                // range query
                var startDate = findQueryAttribute.RawValue.Substring(0, 8);
                var endDate = findQueryAttribute.RawValue.Substring(9, 8);
                return series.Where(x => String.Compare(x.PerformedProcedureStepStartDate, startDate, StringComparison.Ordinal) >= 0
                    && String.Compare(x.PerformedProcedureStepStartDate, endDate, StringComparison.Ordinal) < 0);
            }

            // Exact match
            return series.Where(x => x.PerformedProcedureStepStartDate == findQueryAttribute.RawValue);
        }

        private IQueryable<Series> ApplySeriesNumberFilter(IQueryable<Series> series, QueryAttribute findQueryAttribute)
        {
            if (findQueryAttribute == null)
            {
                return series;
            }

            // Handle wildcard matching
            if (findQueryAttribute.RawValue.EndsWith("*"))
            {
                var firstPart = findQueryAttribute.RawValue.Substring(0, findQueryAttribute.RawValue.Length - 1);
                return series.Where(x => x.SeriesNumber != null && x.Modality.StartsWith(firstPart));
            }

            // Exact match
            return series.Where(x => x.SeriesNumber == findQueryAttribute.RawValue);
        }

        private IQueryable<Series> ApplySeriesUidFilter(IQueryable<Series> series, QueryAttribute findQueryAttribute)
        {
            if (findQueryAttribute == null)
            {
                return series;
            }

            // Exact match
            return series.Where(x => x.SeriesDescription == findQueryAttribute.RawValue);
        }

        private IQueryable<Series> ApplySeriesDescriptionFilter(IQueryable<Series> series, QueryAttribute findQueryAttribute)
        {
            if (findQueryAttribute == null)
            {
                return series;
            }

            // Handle wildcard matching
            if (findQueryAttribute.RawValue.EndsWith("*"))
            {
                var firstPart = findQueryAttribute.RawValue.Substring(0, findQueryAttribute.RawValue.Length - 1);
                return series.Where(x => x.SeriesDescription != null && x.Modality.StartsWith(firstPart));
            }

            // Exact match
            return series.Where(x => x.SeriesDescription == findQueryAttribute.RawValue);
        }

        private IQueryable<Series> ApplyModalityFilter(IQueryable<Series> series, QueryAttribute findQueryAttribute)
        {
            if (findQueryAttribute == null)
            {
                return series;
            }

            // Handle wildcard matching
            if (findQueryAttribute.RawValue.EndsWith("*"))
            {
                var firstPart = findQueryAttribute.RawValue.Substring(0, findQueryAttribute.RawValue.Length - 1);
                return series.Where(x => x.Modality != null && x.Modality.StartsWith(firstPart));
            }

            // Exact match
            return series.Where(x => x.Modality == findQueryAttribute.RawValue);
        }

        private IQueryable<Series> ApplyInstanceAvailabilityFilter(IQueryable<Series> series, QueryAttribute queryAttribute)
        {
            if (queryAttribute == null)
            {
                return series;
            }

            // everything we have is online so return nothing if they query for anything but onlien
            if (queryAttribute.RawValue != "ONLINE")
            {
                return new List<Series>().AsQueryable();
            }
            return series;
        }

    }
}