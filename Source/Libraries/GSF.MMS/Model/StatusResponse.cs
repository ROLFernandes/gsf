//
// This file was generated by the BinaryNotes compiler.
// See http://bnotes.sourceforge.net 
// Any modifications to this file will be lost upon recompilation of the source ASN.1. 
//

using System.Runtime.CompilerServices;
using GSF.ASN1;
using GSF.ASN1.Attributes;
using GSF.ASN1.Attributes.Constraints;
using GSF.ASN1.Coders;
using GSF.ASN1.Types;

namespace GSF.MMS.Model
{
    
    [ASN1PreparedElement]
    [ASN1Sequence(Name = "StatusResponse", IsSet = false)]
    public class StatusResponse : IASN1PreparedElement
    {
        private static readonly IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(StatusResponse));
        private BitString localDetail_;

        private bool localDetail_present;
        private long vmdLogicalStatus_;


        private long vmdPhysicalStatus_;

        [ASN1Integer(Name = "")]
        [ASN1Element(Name = "vmdLogicalStatus", IsOptional = false, HasTag = true, Tag = 0, HasDefaultValue = false)]
        public long VmdLogicalStatus
        {
            get
            {
                return vmdLogicalStatus_;
            }
            set
            {
                vmdLogicalStatus_ = value;
            }
        }

        [ASN1Integer(Name = "")]
        [ASN1Element(Name = "vmdPhysicalStatus", IsOptional = false, HasTag = true, Tag = 1, HasDefaultValue = false)]
        public long VmdPhysicalStatus
        {
            get
            {
                return vmdPhysicalStatus_;
            }
            set
            {
                vmdPhysicalStatus_ = value;
            }
        }


        [ASN1BitString(Name = "")]
        [ASN1ValueRangeConstraint(
            Min = 0L,
            Max = 128L
            )]
        [ASN1Element(Name = "localDetail", IsOptional = true, HasTag = true, Tag = 2, HasDefaultValue = false)]
        public BitString LocalDetail
        {
            get
            {
                return localDetail_;
            }
            set
            {
                localDetail_ = value;
                localDetail_present = true;
            }
        }


        public void initWithDefaults()
        {
        }


        public IASN1PreparedElementData PreparedData
        {
            get
            {
                return preparedData;
            }
        }

        public bool isLocalDetailPresent()
        {
            return localDetail_present;
        }
    }
}