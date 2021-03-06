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
    [ASN1Sequence(Name = "DirectoryEntry", IsSet = false)]
    public class DirectoryEntry : IASN1PreparedElement
    {
        private static readonly IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(DirectoryEntry));
        private FileAttributes fileAttributes_;
        private FileName fileName_;

        [ASN1Element(Name = "fileName", IsOptional = false, HasTag = true, Tag = 0, HasDefaultValue = false, IsImplicitTag = true)]
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


        [ASN1Element(Name = "fileAttributes", IsOptional = false, HasTag = true, Tag = 1, HasDefaultValue = false)]
        public FileAttributes FileAttributes
        {
            get
            {
                return fileAttributes_;
            }
            set
            {
                fileAttributes_ = value;
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
    }
}