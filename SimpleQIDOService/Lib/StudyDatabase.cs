using System;
using System.Collections.Generic;
using System.IO;
using SimpleQIDOService.Models;
using Dicom;

namespace SimpleQIDOService.Lib
{
    public class StudyDatabase
    {
        private static StudyDatabase _instance;
        private readonly Dictionary<string, Study> _studies = new Dictionary<string, Study>();

        public static StudyDatabase Instance()
        {
            if (_instance == null)
            {
                _instance = new StudyDatabase();
                _instance.Load();
            }

            return _instance;
        }

        public Dictionary<string, Study> GetStudies()
        {
            return _studies;
        }

        public Study FindByStudyUid(string studyUid)
        {
            Study study;
            _studies.TryGetValue(studyUid, out study);
            return study;
        }

        private void LoadDirectory(string path)
        {
            var files = Directory.EnumerateFiles(path);
            foreach (var fileName in files)
            {
                try
                {
                    var dicomFile = DicomFile.Open(fileName);
                    ProcessDicomFile(dicomFile);
                }
                catch (Exception)
                {
                    // skip
                }
            }
            var directories = Directory.EnumerateDirectories(path);
            foreach (var directoryPath in directories)
            {
                LoadDirectory(directoryPath);
            }
        }

        private void ProcessDicomFile(DicomFile dicomFile)
        {
            var study = GetOrCreateStudy(dicomFile);
            var series = GetOrCreateSeries(study, dicomFile);
            var instance = GetOrCreateInstance(series, dicomFile);
        }

        private Study GetOrCreateStudy(DicomFile dicomFile)
        {
            var studyUid = dicomFile.Dataset.Get<string>(DicomTag.StudyInstanceUID);
            Study study;
            if (_studies.TryGetValue(studyUid, out study) == false)
            {
                // unknown study, add it
                study = new Study();
                study.StudyUid = studyUid;
                study.StudyId = dicomFile.Dataset.Get<string>(DicomTag.StudyID);
                study.StudyDate = dicomFile.Dataset.Get<string>(DicomTag.StudyDate);
                study.StudyTime = dicomFile.Dataset.Get<string>(DicomTag.StudyTime);
                study.AccessionNumber = dicomFile.Dataset.Get<string>(DicomTag.AccessionNumber);
                study.StudyDescription = dicomFile.Dataset.Get<string>(DicomTag.StudyDescription);
                study.PatientsName = dicomFile.Dataset.Get<string>(DicomTag.PatientName);
                study.ReferringPhysiciansName= dicomFile.Dataset.Get<string>(DicomTag.ReferringPhysicianName);
                study.PatientId = dicomFile.Dataset.Get<string>(DicomTag.PatientID);
                study.PatientBirthDate = dicomFile.Dataset.Get<string>(DicomTag.PatientBirthDate);
                study.PatientSex = dicomFile.Dataset.Get<string>(DicomTag.PatientSex);
                _studies[studyUid] = study;
            }

            return study;
        }
        
        private Series GetOrCreateSeries(Study study, DicomFile dicomFile)
        {
            var seriesUid = dicomFile.Dataset.Get<string>(DicomTag.SeriesInstanceUID);
            Series series;
            if (study.Series.TryGetValue(seriesUid, out series) == false)
            {
                series = new Series();
                study.Series[seriesUid] = series;
                series.SeriesUid = seriesUid;
                series.SeriesDescription = dicomFile.Dataset.Get<string>(DicomTag.SeriesDescription);
                series.Modality = dicomFile.Dataset.Get<string>(DicomTag.Modality);
                series.SeriesNumber = dicomFile.Dataset.Get<string>(DicomTag.SeriesNumber);
                series.PerformedProcedureStepStartDate = dicomFile.Dataset.Get<string>(DicomTag.PerformedProcedureStepStartDate);
                series.PerformedProcedureStepStartTime = dicomFile.Dataset.Get<string>(DicomTag.PerformedProcedureStepStartTime);
            }
            return series;
        }
        private Instance GetOrCreateInstance(Series series, DicomFile dicomFile)
        {
            var sopInstanceUid = dicomFile.Dataset.Get<string>(DicomTag.SOPInstanceUID);
            Instance instance;
            if (series.Instances.TryGetValue(sopInstanceUid, out instance) == false)
            {
                instance = new Instance();
                series.Instances[sopInstanceUid] = instance;
                instance.SopInstanceUid = sopInstanceUid;
                instance.SopClassUid = dicomFile.Dataset.Get<string>(DicomTag.SOPClassUID);
                instance.InstanceNumber = dicomFile.Dataset.Get<string>(DicomTag.InstanceNumber);
                instance.Rows = dicomFile.Dataset.Get<string>(DicomTag.Rows);
                instance.Columns = dicomFile.Dataset.Get<string>(DicomTag.Columns);
                instance.BitsAllocated = dicomFile.Dataset.Get<string>(DicomTag.BitsAllocated);
                instance.NumberOfFrames = dicomFile.Dataset.Get<string>(DicomTag.NumberOfFrames);
                instance.DicomFile = dicomFile;
            }
            return instance;
        }


        private void Load()
        {
            LoadDirectory(@"C:\QIDO");
        }

    }
}