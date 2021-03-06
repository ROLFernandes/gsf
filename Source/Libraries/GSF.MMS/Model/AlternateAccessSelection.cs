//
// This file was generated by the BinaryNotes compiler.
// See http://bnotes.sourceforge.net 
// Any modifications to this file will be lost upon recompilation of the source ASN.1. 
//

using System.Runtime.CompilerServices;
using GSF.ASN1;
using GSF.ASN1.Attributes;
using GSF.ASN1.Coders;
using GSF.ASN1.Types;

namespace GSF.MMS.Model
{
    
    [ASN1PreparedElement]
    [ASN1Choice(Name = "AlternateAccessSelection")]
    public class AlternateAccessSelection : IASN1PreparedElement
    {
        private static readonly IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(AlternateAccessSelection));
        private SelectAccessChoiceType selectAccess_;
        private bool selectAccess_selected;
        private SelectAlternateAccessSequenceType selectAlternateAccess_;
        private bool selectAlternateAccess_selected;


        [ASN1Element(Name = "selectAlternateAccess", IsOptional = false, HasTag = true, Tag = 0, HasDefaultValue = false)]
        public SelectAlternateAccessSequenceType SelectAlternateAccess
        {
            get
            {
                return selectAlternateAccess_;
            }
            set
            {
                selectSelectAlternateAccess(value);
            }
        }


        [ASN1Element(Name = "selectAccess", IsOptional = false, HasTag = false, HasDefaultValue = false)]
        public SelectAccessChoiceType SelectAccess
        {
            get
            {
                return selectAccess_;
            }
            set
            {
                selectSelectAccess(value);
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


        public bool isSelectAlternateAccessSelected()
        {
            return selectAlternateAccess_selected;
        }


        public void selectSelectAlternateAccess(SelectAlternateAccessSequenceType val)
        {
            selectAlternateAccess_ = val;
            selectAlternateAccess_selected = true;


            selectAccess_selected = false;
        }


        public bool isSelectAccessSelected()
        {
            return selectAccess_selected;
        }


        public void selectSelectAccess(SelectAccessChoiceType val)
        {
            selectAccess_ = val;
            selectAccess_selected = true;


            selectAlternateAccess_selected = false;
        }

        [ASN1PreparedElement]
        [ASN1Choice(Name = "selectAccess")]
        public class SelectAccessChoiceType : IASN1PreparedElement
        {
            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(SelectAccessChoiceType));
            private NullObject allElements_;
            private bool allElements_selected;
            private Identifier component_;
            private bool component_selected;
            private IndexRangeSequenceType indexRange_;
            private bool indexRange_selected;


            private Unsigned32 index_;
            private bool index_selected;

            [ASN1Element(Name = "component", IsOptional = false, HasTag = true, Tag = 1, HasDefaultValue = false)]
            public Identifier Component
            {
                get
                {
                    return component_;
                }
                set
                {
                    selectComponent(value);
                }
            }


            [ASN1Element(Name = "index", IsOptional = false, HasTag = true, Tag = 2, HasDefaultValue = false)]
            public Unsigned32 Index
            {
                get
                {
                    return index_;
                }
                set
                {
                    selectIndex(value);
                }
            }


            [ASN1Element(Name = "indexRange", IsOptional = false, HasTag = true, Tag = 3, HasDefaultValue = false)]
            public IndexRangeSequenceType IndexRange
            {
                get
                {
                    return indexRange_;
                }
                set
                {
                    selectIndexRange(value);
                }
            }


            [ASN1Null(Name = "allElements")]
            [ASN1Element(Name = "allElements", IsOptional = false, HasTag = true, Tag = 4, HasDefaultValue = false)]
            public NullObject AllElements
            {
                get
                {
                    return allElements_;
                }
                set
                {
                    selectAllElements(value);
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


            public bool isComponentSelected()
            {
                return component_selected;
            }


            public void selectComponent(Identifier val)
            {
                component_ = val;
                component_selected = true;


                index_selected = false;

                indexRange_selected = false;

                allElements_selected = false;
            }


            public bool isIndexSelected()
            {
                return index_selected;
            }


            public void selectIndex(Unsigned32 val)
            {
                index_ = val;
                index_selected = true;


                component_selected = false;

                indexRange_selected = false;

                allElements_selected = false;
            }


            public bool isIndexRangeSelected()
            {
                return indexRange_selected;
            }


            public void selectIndexRange(IndexRangeSequenceType val)
            {
                indexRange_ = val;
                indexRange_selected = true;


                component_selected = false;

                index_selected = false;

                allElements_selected = false;
            }


            public bool isAllElementsSelected()
            {
                return allElements_selected;
            }


            public void selectAllElements()
            {
                selectAllElements(new NullObject());
            }


            public void selectAllElements(NullObject val)
            {
                allElements_ = val;
                allElements_selected = true;


                component_selected = false;

                index_selected = false;

                indexRange_selected = false;
            }

            [ASN1PreparedElement]
            [ASN1Sequence(Name = "indexRange", IsSet = false)]
            public class IndexRangeSequenceType : IASN1PreparedElement
            {
                private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(IndexRangeSequenceType));
                private Unsigned32 lowIndex_;


                private Unsigned32 numberOfElements_;

                [ASN1Element(Name = "lowIndex", IsOptional = false, HasTag = true, Tag = 0, HasDefaultValue = false)]
                public Unsigned32 LowIndex
                {
                    get
                    {
                        return lowIndex_;
                    }
                    set
                    {
                        lowIndex_ = value;
                    }
                }

                [ASN1Element(Name = "numberOfElements", IsOptional = false, HasTag = true, Tag = 1, HasDefaultValue = false)]
                public Unsigned32 NumberOfElements
                {
                    get
                    {
                        return numberOfElements_;
                    }
                    set
                    {
                        numberOfElements_ = value;
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

        [ASN1PreparedElement]
        [ASN1Sequence(Name = "selectAlternateAccess", IsSet = false)]
        public class SelectAlternateAccessSequenceType : IASN1PreparedElement
        {
            private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(SelectAlternateAccessSequenceType));
            private AccessSelectionChoiceType accessSelection_;


            private AlternateAccess alternateAccess_;

            [ASN1Element(Name = "accessSelection", IsOptional = false, HasTag = false, HasDefaultValue = false)]
            public AccessSelectionChoiceType AccessSelection
            {
                get
                {
                    return accessSelection_;
                }
                set
                {
                    accessSelection_ = value;
                }
            }

            [ASN1Element(Name = "alternateAccess", IsOptional = false, HasTag = false, HasDefaultValue = false)]
            public AlternateAccess AlternateAccess
            {
                get
                {
                    return alternateAccess_;
                }
                set
                {
                    alternateAccess_ = value;
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

            [ASN1PreparedElement]
            [ASN1Choice(Name = "accessSelection")]
            public class AccessSelectionChoiceType : IASN1PreparedElement
            {
                private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(AccessSelectionChoiceType));
                private NullObject allElements_;
                private bool allElements_selected;
                private Identifier component_;
                private bool component_selected;
                private IndexRangeSequenceType indexRange_;
                private bool indexRange_selected;


                private Unsigned32 index_;
                private bool index_selected;

                [ASN1Element(Name = "component", IsOptional = false, HasTag = true, Tag = 0, HasDefaultValue = false)]
                public Identifier Component
                {
                    get
                    {
                        return component_;
                    }
                    set
                    {
                        selectComponent(value);
                    }
                }


                [ASN1Element(Name = "index", IsOptional = false, HasTag = true, Tag = 1, HasDefaultValue = false)]
                public Unsigned32 Index
                {
                    get
                    {
                        return index_;
                    }
                    set
                    {
                        selectIndex(value);
                    }
                }


                [ASN1Element(Name = "indexRange", IsOptional = false, HasTag = true, Tag = 2, HasDefaultValue = false)]
                public IndexRangeSequenceType IndexRange
                {
                    get
                    {
                        return indexRange_;
                    }
                    set
                    {
                        selectIndexRange(value);
                    }
                }


                [ASN1Null(Name = "allElements")]
                [ASN1Element(Name = "allElements", IsOptional = false, HasTag = true, Tag = 3, HasDefaultValue = false)]
                public NullObject AllElements
                {
                    get
                    {
                        return allElements_;
                    }
                    set
                    {
                        selectAllElements(value);
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


                public bool isComponentSelected()
                {
                    return component_selected;
                }


                public void selectComponent(Identifier val)
                {
                    component_ = val;
                    component_selected = true;


                    index_selected = false;

                    indexRange_selected = false;

                    allElements_selected = false;
                }


                public bool isIndexSelected()
                {
                    return index_selected;
                }


                public void selectIndex(Unsigned32 val)
                {
                    index_ = val;
                    index_selected = true;


                    component_selected = false;

                    indexRange_selected = false;

                    allElements_selected = false;
                }


                public bool isIndexRangeSelected()
                {
                    return indexRange_selected;
                }


                public void selectIndexRange(IndexRangeSequenceType val)
                {
                    indexRange_ = val;
                    indexRange_selected = true;


                    component_selected = false;

                    index_selected = false;

                    allElements_selected = false;
                }


                public bool isAllElementsSelected()
                {
                    return allElements_selected;
                }


                public void selectAllElements()
                {
                    selectAllElements(new NullObject());
                }


                public void selectAllElements(NullObject val)
                {
                    allElements_ = val;
                    allElements_selected = true;


                    component_selected = false;

                    index_selected = false;

                    indexRange_selected = false;
                }

                [ASN1PreparedElement]
                [ASN1Sequence(Name = "indexRange", IsSet = false)]
                public class IndexRangeSequenceType : IASN1PreparedElement
                {
                    private static IASN1PreparedElementData preparedData = CoderFactory.getInstance().newPreparedElementData(typeof(IndexRangeSequenceType));
                    private Unsigned32 lowIndex_;


                    private Unsigned32 numberOfElements_;

                    [ASN1Element(Name = "lowIndex", IsOptional = false, HasTag = true, Tag = 0, HasDefaultValue = false)]
                    public Unsigned32 LowIndex
                    {
                        get
                        {
                            return lowIndex_;
                        }
                        set
                        {
                            lowIndex_ = value;
                        }
                    }

                    [ASN1Element(Name = "numberOfElements", IsOptional = false, HasTag = true, Tag = 1, HasDefaultValue = false)]
                    public Unsigned32 NumberOfElements
                    {
                        get
                        {
                            return numberOfElements_;
                        }
                        set
                        {
                            numberOfElements_ = value;
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
        }
    }
}