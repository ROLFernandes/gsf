//
// This file was generated by the BinaryNotes compiler.
// See http://bnotes.sourceforge.net 
// Any modifications to this file will be lost upon recompilation of the source ASN.1. 
//

using System.Runtime.CompilerServices;
using GSF.ASN1;
using GSF.ASN1.Attributes;
using GSF.ASN1.Coders;

namespace GSF.MMS.Model
{
    
    [ASN1PreparedElement]
    [ASN1Sequence(Name = "StoreUnitControlToFile_Request", IsSet = false)]
    public class StoreUnitControlToFile_Request : IASN1PreparedElement
    {
        private static readonly IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(StoreUnitControlToFile_Request));
        private FileName fileName_;


        private ApplicationReference thirdParty_;

        private bool thirdParty_present;
        private Identifier unitControlName_;

        [ASN1Element(Name = "unitControlName", IsOptional = false, HasTag = true, Tag = 0, HasDefaultValue = false)]
        public Identifier UnitControlName
        {
            get
            {
                return unitControlName_;
            }
            set
            {
                unitControlName_ = value;
            }
        }

        [ASN1Element(Name = "fileName", IsOptional = false, HasTag = true, Tag = 1, HasDefaultValue = false)]
        public FileName FileName
        {
            get
            {
                return fileName_;
            }
            set
            {
                fileName_ = value;
            }
        }

        [ASN1Element(Name = "thirdParty", IsOptional = true, HasTag = true, Tag = 2, HasDefaultValue = false)]
        public ApplicationReference ThirdParty
        {
            get
            {
                return thirdParty_;
            }
            set
            {
                thirdParty_ = value;
                thirdParty_present = true;
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

        public bool isThirdPartyPresent()
        {
            return thirdParty_present;
        }
    }
}