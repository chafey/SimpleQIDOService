using System;
using System.Collections.Generic;
using Dicom;

namespace SimpleQIDOService.Lib
{
    /// <summary>
    /// Handles generation of the JSON response for QIDO queries
    /// </summary>
    public class DICOMJSONBuilder
    {
        private readonly DicomFile _dicomFile;
        private readonly Dictionary<string, object> _attributes = new Dictionary<string, object>();

        public DICOMJSONBuilder(DicomFile dicomFile)
        {
            _dicomFile = dicomFile;
        }

        public Dictionary<string, object> GetAttributes()
        {
            return _attributes;
        }

        public void Add(ushort group, ushort element, string value)
        {
            var tag = new DicomTag(group, element);
            var item = DicomDictionary.Default[tag];
            Add(group, element, item.ValueRepresentations[0], value);
        }

        public void Add(ushort group, ushort element)
        {
            var tag = new DicomTag(group, element);
            var dicomItem = _dicomFile.Dataset.Get<DicomItem>(tag);
            var value = _dicomFile.Dataset.Get<string>(tag);
            if (value == null)
            {
                return;
            }
            Add(group, element, dicomItem.ValueRepresentation, value);
        }

        private void Add(ushort group, ushort element, DicomVR vr, string value)
        {
            var key = String.Format("{0}{1}", group.ToString("X4"), element.ToString("X4"));

            if (IsString(vr))
            {
                var values = new string[1];
                values[0] = value;
                _attributes[key] = new
                {
                    vr = vr.ToString(),
                    Value = values
                };
            }
            else if (IsFloat(vr))
            {
                var values = new double[1];
                values[0] = Double.Parse(value);
                _attributes[key] = new
                {
                    vr = vr.ToString(),
                    Value = values
                };
            }
            else if (IsInteger(vr))
            {
                var values = new long[1];
                values[0] = Int64.Parse(value);
                _attributes[key] = new
                {
                    vr = vr.ToString(),
                    Value = values
                };
            }
            else if (IsName(vr))
            {
                var pnValues = new object[1];
                pnValues[0] = new
                {
                    Alphabetic = value
                };

                _attributes[key] = new
                {
                    vr = vr.ToString(),
                    Value = pnValues
                };
            }
            else if (IsBase64(vr))
            {
                // todo: base 64 encoding
                return;
            }
            else if (IsSequence(vr))
            {
                // todo: sequence encoding
                return;
            }
        }

    private bool IsBase64(DicomVR vr)
        {
            if (vr == DicomVR.OB
                //|| vr == DicomVR.OD
                || vr == DicomVR.OF
                || vr == DicomVR.OW
                || vr == DicomVR.UN
                )
            {
                return true;
            }
            return false;
        }

        private bool IsSequence(DicomVR vr)
        {
            if (vr == DicomVR.SQ)
            {
                return true;
            }
            return false;
        }

        private bool IsString(DicomVR vr)
        {
            if (vr == DicomVR.AE
                || vr == DicomVR.AS
                || vr == DicomVR.AT
                || vr == DicomVR.CS
                || vr == DicomVR.DA
                || vr == DicomVR.DT
                || vr == DicomVR.AS
                || vr == DicomVR.LO
                || vr == DicomVR.LT
                || vr == DicomVR.SH
                || vr == DicomVR.ST
                || vr == DicomVR.TM
                || vr == DicomVR.UI
                || vr == DicomVR.UT
                )
            {
                return true;
            }
            return false;
        }

        private bool IsFloat(DicomVR vr)
        {
            if (vr == DicomVR.DS
                || vr == DicomVR.FL
                || vr == DicomVR.FD
                )
            {
                return true;
            }
            return false;
        }

        private bool IsInteger(DicomVR vr)
        {
            if (vr == DicomVR.IS
                || vr == DicomVR.SL
                || vr == DicomVR.SS
                || vr == DicomVR.UL
                || vr == DicomVR.US
                )
            {
                return true;
            }
            return false;
        }

        private bool IsName(DicomVR vr)
        {
            if (vr == DicomVR.PN
                )
            {
                return true;
            }
            return false;
        }

    }
}