using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace ReadFile
{
    class Program
    {
        private const char SingleQuoteSeparator = '\'';
        private const char PlusSeparator = '+';
        private const char ColonSeparator = ':';

        public enum EDIFactSegmentEnum
        {
            UNA,
            UNB,
            UNH,
            BGM,
            FTX,
            NAD,
            CTA,
            RFF,
            LOC,
            CUX,
            LIN,
            MOA,
            PRI,
            PIA,
            IMD,
            QTY,
            UNS,
            CNT,
            UNT,
            UNZ,
            DTM,
            INV,
            COM,
            MEA,
        }

        //The types of messages we can read in this upload.  This might be how we separate different sub-formats like Tesco vs Dunnes etc.
        public enum EDIFactMessageTypeEnum
        {
            ORDERS,
            UNKNOWN
        }

        public class FileStatusClass
        {
            public bool TransmissionStarted = false;
            public bool DocumentStarted = false;
            public bool LineItemStarted = false;
            public int DocumentSegmentCount = 0;
            public int TransmissionSegmentCount = 0;
            public bool HasCNTSegment = false;
            public int LineCount = 0;
            public int DocumentCount = 0;
            public EDIFactMessageTypeEnum MessageType = EDIFactMessageTypeEnum.UNKNOWN;
        }

        public class EDIFact01MessageStructure
        {
            public EDIFact01MessageStructure()
            {
                Documents = new List<EDIFact01MessageBody>();
            }

            public UNBSegment MessageHeaderUNB { get; set; }

            public List<EDIFact01MessageBody> Documents { get; set; }

            public UNZSegment MessageFooterUNZ { get; set; }
        }

        public enum DateConvertEnum
        {
            [Description("yyyyMMdd")]
            OneOTwoDate = 102,
            [Description("yyyyMMdd")]
            TwoOTwoDate = 202
        }

        public class TescoEDIFact : EDIFact01MessageStructure
        {

        }



        public class EDIFact01MessageBody
        {
            public EDIFact01MessageBody()
            {
                Lines = new List<EDIFact01MessageLines>();
            }

            public UNHSegment BodyHeaderUNH { get; set; }
            public BGMSegment BodyBeginningofMessageBGM { get; set; }
            public DTMSegment BodyDateAndTimeDTM { get; set; }
            public FTXSegment FreeTextFTX { get; set; }
            public RFFSegment ReferenceRFF { get; set; }
            public NADSegment BodyNameAndAddressNAD { get; set; }
            public CTASegment ContactInformationCTA { get; set; }
            public COMSegment CommunicationContactCOM { get; set; }
            public CUXSegment CurrenciesCUX { get; set; }
            public List<EDIFact01MessageLines> Lines { get; set; }
            public UNSSegment BodySectionControlUNS { get; set; }
            public CNTSegment BodyControlTotalCNT { get; set; }
            public UNTSegment BodyMessageTrailerUNT { get; set; }
        }

        public class EDIFact01MessageLines
        {
            public LINSegment LineItemLIN { get; set; }
            public PIASegment LineAdditionalProduceInformationPIA { get; set; }
            public IMDSegment LineItemDescriptionIMD { get; set; }
            public QTYSegment LineQuantityQTY { get; set; }
            public MOASegment LineMonetaryAmountMOA { get; set; }
            public MEASegment LineMeasurementMEA { get; set; }
            public PRISegment LinePriceInformationPRI { get; set; }
        }

        public class GenericSegments
        {
            public string SegmentIdentifier { get; set; }
        }

        public class UNHSegment : GenericSegments
        {
            public UNHSegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }
            public string AssociationAssignmentCode { get; set; }
            public string CommonAccessRef { get; set; }
            public string ControllingAgency { get; set; }
            public string MessageRef { get; set; }
            public string MessageType { get; set; }
            public string ReleaseNumber { get; set; }
            public string Version { get; set; }
        }

        public class UNBSegment : GenericSegments
        {
            public UNBSegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }

            public string SyntaxIdentifier { get; set; }
            public int SyntaxVersion { get; set; }
            public string SenderId { get; set; }
            public string PartnerIDCodeQualifier { get; set; }
            public string ReverseRoutingAddress { get; set; }
            public string RecipientId { get; set; }
            public string ParterIDCode { get; set; }
            public string RoutingAddress { get; set; }
            public DateTime DateOfPreparation { get; set; }
            public string ControlRef { get; set; }
            public int AckRequest { get; set; }
        }

        public class BGMSegment : GenericSegments
        {
            public BGMSegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }
            public string MessageName { get; set; }
            public string DocumentNumber { get; set; }
            public string MessageFunction { get; set; }
            public string ResponseType { get; set; }
        }

        public class UNZSegment : GenericSegments
        {
            public UNZSegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }
            public int TrailerControlCount { get; set; }
            public string TrailerControlReference { get; set; }
        }

        public class FTXSegment : GenericSegments
        {
            //FTX+AAB+1++30 DAYS AT 0PCT FOLLOWING MONTH OF INVOICE sample 
            public FTXSegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }
            public string TextSubjectQualifier { get; set; }
            public string TextFunction { get; set; }
            public string TextReference { get; set; }
            public string FreeTextLine1 { get; set; }
            public string FreeTextLine2 { get; set; }
        }

        public class RFFSegment : GenericSegments
        {
            //RFF+ON:12 SAMPLE FORMAT
            public RFFSegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }

            public enum RFFSegmentEnum
            {
                ON,// – Buyers order number
                SS,// – Suppliers order number
                CR,// – Suppliers client code for buyer
                API// – Suppliers client code for Buyer (alternative)
            }

            public string BuyerOrderNumber { get; set; } 
            public string SupplierOrderNumber { get; set; } 
            public string SuppliersBuyerCode { get; set; }
            public string SuppliersAlternativeBuyerCode { get; set; }
        }

        public class CUXSegment : GenericSegments
        {
            //CUX+2:EUR
            public CUXSegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }

            public int ReferenceCurrency { get; set; } //mandatory
            public string CurrencyCode { get; set; } //conditional
            public int InvoicingCurrency { get; set; } //conditional
        }

        public class CNTSegment : GenericSegments
        {
            public CNTSegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }
            public int MessageSegmentsCount { get; set; }
            public string MessageReference { get; set; }
        }

        public class UNSSegment : GenericSegments
        {
            public UNSSegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }
            public char? UNS { get; set; }
        }

        public class IMDSegment : GenericSegments
        {
            public IMDSegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }
            public string Code { get; set; }
            public string DescriptionCode { get; set; }
            public string Description { get; set; }
        }

        public class PIASegment : GenericSegments
        {
            public PIASegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }
            public string Code { get; set; }
            public string ItemCode { get; set; }
        }

        public class DTMSegment : GenericSegments
        {
            public DTMSegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }
            public DateTime? DeliveryDate { get; set; }
            public DateTime? MessageDate { get; set; }
            public DateTime? ProcessingStartDate { get; set; }
            public DateTime? ProcessingEndDate { get; set; }
        }

        public class UNTSegment : GenericSegments
        {
            public UNTSegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }
            public int TrailerMessageSegmentsCount { get; set; }
            public string TrailerMessageReference { get; set; }
        }

        public class NADSegment : GenericSegments
        {
            public NADSegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }
            public string Buyer { get; set; }
            public string DeliveryPoint { get; set; }
            public string Seller { get; set; }
            public string Supplier { get; set; }

            public enum NADSegmentTypeEnum
            {
                BY,
                DP,
                SE,
                SU
            }
        }

        public class CTASegment : GenericSegments
        {
            //CTA+OC+:CONTACT BRANCH
            public CTASegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }
            public enum CTASegmentEnum
            {
                AD, //accounting contact
                SU, //supplier contact
                BJ,// Department or person responsible for processing purchase order 
                BO, //After business hours contact 
                IC, //Information contact 
                OC, //= Order contact 
                PD,// Purchasing contact 
                SR, //Sales representative or department
            }
            public string DepartmentEmployee { get; set; }
            public string DepartmentEmployeeName { get; set; }
            
          
        }

        public class COMSegment : GenericSegments
        {
            //COM+STORE8021@LLOYDSPHARMACY.IE:EM
            public COMSegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }
          
            public string Email { get; set; } //email or phone number
            public string Phone { get; set; } //email or phone number
            public enum COMSegmentEnum
            {
               EM, //email
               TE //telephone
            }

        }
        public class LINSegment : GenericSegments
        {
            public LINSegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }
            public int LineNumber { get; set; }
            public string ActionRequest { get; set; }
            public string ProductANANumber { get; set; }
        }

        public class QTYSegment : GenericSegments
        {
            public QTYSegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }
            public decimal OrderedQty { get; set; }
            public decimal ConsumerUnitInTradedUnit { get; set; }

            public enum QuantitySegmentTypeEnum
            {
                ActualQtySegment = 21,
                ConsumerUnitInTradedUnitSegment = 59
            }
        }

        public class MOASegment : GenericSegments
        {
            //MOA+52:735:SEK’
            public MOASegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }
            public int MonetaryTypeQualifier { get; set; } //eg 52  = discount amount
            public decimal Amount { get; set; }
            public string CurrencyCode { get; set; }
        }

        public class MEASegment : GenericSegments
        {
            //'MEA+PD+HT+CMT:18.5
            public MEASegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }

            public string MeasurementQualifier { get; set; } //pd physical dimenstion
            public string MeasureOf { get; set; } //height etc.
            public string UnitOFMeasurement { get; set; }
            public decimal MeasurementValue { get; set; }
        }


        public class PRISegment : GenericSegments
        {
            public PRISegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }
            public decimal Price { get; set; }
            public string PriceEnumValue { get; set; }
            public string PriceTypeCode { get; set; }
            public string PriceTypeQualifier { get; set; }
            public int UnitPriceBasis { get; set; }
            public string UOMQualifier { get; set; }
            public enum PRISegmentEnum
            {
                AAA, //The price stated is the net price including allowances/ charges.Allowances/charges may be stated for information only.
                AAB //The price stated is the gross price to which allowances/charges must be applied
            }
        }

        static void Main(string[] args)
        {
            //insert file name here! 
            //string[] lines = System.IO.File.ReadAllLines(@"i:\WS837541.MFD");// WS843729.MFD WS843729DUNNES.MFD WS836601.MFD
            string[] lines = System.IO.File.ReadAllLines(@"C:\FreshTrade\EDITEST\WS837548LLOYDS.MFD");//sarah WS843729DUNNES.MFD //WS837577MUSGRAVES.MFD //WS837548LLOYDS.MFD
            FileStatusClass fileStatus = new FileStatusClass();
            EDIFact01MessageStructure theFile = new EDIFact01MessageStructure();
            theFile.Documents = new List<EDIFact01MessageBody>();

            EDIFact01MessageBody theMessage = new EDIFact01MessageBody();

            EDIFact01MessageLines theLine = new EDIFact01MessageLines();

            if (lines.Length < 2)
            {
                var allLines = SplitStringBySeparator(lines[0], SingleQuoteSeparator);

                for (int i = 0; i < allLines.Length; i++)
                {
                    ProcessEDILine(allLines[i], ref theFile, ref theMessage, ref theLine, ref fileStatus);
                }
            }
            else
            {
                Console.WriteLine("Contents of writeLines2.txt =:");
                foreach (string line in lines)
                {
                    ProcessEDILine(line, ref theFile, ref theMessage, ref theLine, ref fileStatus);
                }
            }


            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

        }

        public EDIFact01MessageStructure BuildEDIFactMessage()
        {





            return new EDIFact01MessageStructure();
        }

        #region Utilities

        private static string[] SplitStringBySeparator(string stringToSplit, char splitSeparator)
        {
            if (string.IsNullOrWhiteSpace(stringToSplit))
            {
                return new string[0];
            }

            return stringToSplit.Split(splitSeparator);
        }

        public static EDIFactSegmentEnum ConvertSegmentToEDIFactSegmentEnum(string passInSegmentAsString)
        {
            if (string.IsNullOrWhiteSpace(passInSegmentAsString))
            {
                return EDIFactSegmentEnum.INV;
            }

            //Convert String to Enum
            EDIFactSegmentEnum eDIFactSegmentEnum = EDIFactSegmentEnum.INV;
            try
            {
                eDIFactSegmentEnum = (EDIFactSegmentEnum)Enum.Parse(typeof(EDIFactSegmentEnum), passInSegmentAsString);
            }
            catch (Exception)
            {
                eDIFactSegmentEnum = EDIFactSegmentEnum.INV;
            }

            return eDIFactSegmentEnum;
        }

        public static EDIFactMessageTypeEnum ConvertMessageTypeToEDIFactMessageEnum(string passInSegmentAsString)
        {
            if (string.IsNullOrWhiteSpace(passInSegmentAsString))
            {
                return EDIFactMessageTypeEnum.UNKNOWN;
            }

            //Convert String to Enum
            EDIFactMessageTypeEnum eDIFactSegmentEnum = EDIFactMessageTypeEnum.UNKNOWN;
            try
            {
                eDIFactSegmentEnum = (EDIFactMessageTypeEnum)Enum.Parse(typeof(EDIFactMessageTypeEnum), passInSegmentAsString);
            }
            catch (Exception)
            {
                eDIFactSegmentEnum = EDIFactMessageTypeEnum.UNKNOWN;
            }

            return eDIFactSegmentEnum;
        }

        private static UNHSegment ProcessUNHSegment(string stringToConvert, FileStatusClass fileStatus)
        {
            var newUNHSegment = new UNHSegment("UNH");

            if (!fileStatus.TransmissionStarted)
            {
                throw new Exception("New document (UNH) without a transmission header (UNB).");
            }
            else
            {
                fileStatus.DocumentStarted = true;
                fileStatus.LineItemStarted = false;
                fileStatus.DocumentSegmentCount = 0;
                fileStatus.LineCount = 0;
                fileStatus.DocumentCount++;
            }

            var allSegments = SplitStringBySeparator(stringToConvert, PlusSeparator);

            for (int j = 0; j < allSegments.Length; j++)
            {
                switch (j)
                {
                    case 1:
                        newUNHSegment.MessageRef = allSegments[j];
                        break;

                    case 2:

                        var subParts = SplitStringBySeparator(allSegments[j], ColonSeparator);

                        for (int subPart = 0; subPart < subParts.Length; subPart++)
                        {
                            switch (subPart)
                            {
                                case 0:
                                    newUNHSegment.MessageType = subParts[subPart];
                                    break;
                                case 1:
                                    newUNHSegment.Version = subParts[subPart];
                                    break;
                                case 2:
                                    newUNHSegment.ReleaseNumber = subParts[subPart];
                                    break;
                                case 3:
                                    newUNHSegment.ControllingAgency = subParts[subPart];
                                    break;
                                case 4:
                                    newUNHSegment.AssociationAssignmentCode = subParts[subPart];
                                    break;
                                default:
                                    break;
                            }
                        }

                        break;
                    default:
                        break;
                }
            }

            fileStatus.MessageType = ConvertMessageTypeToEDIFactMessageEnum(newUNHSegment.MessageType);

            return newUNHSegment;
        }
        private static UNBSegment ProcessUNBSegment(string stringToConvert, FileStatusClass fileStatus)
        {
            var newUNBSegment = new UNBSegment("UNB");
            const int expectedSyntaxElementCount = 2;
            const int expectedSenderElementCount = 2;

            if (fileStatus.DocumentStarted || fileStatus.LineItemStarted)
            {
                throw new Exception("New transmission (UNB) in the middle of a document.");
            }
            else
            {
                fileStatus.TransmissionStarted = true;
                fileStatus.DocumentStarted = false;
                fileStatus.LineItemStarted = false;
                fileStatus.TransmissionSegmentCount = 0;
            }

            var allSegments = SplitStringBySeparator(stringToConvert, PlusSeparator);

            for (int j = 0; j < allSegments.Length; j++)
            {
                switch (j)
                {
                    case 1:

                        var syntaxSubParts = SplitStringBySeparator(allSegments[j], ColonSeparator);

                        if (expectedSyntaxElementCount != syntaxSubParts.Length)
                        {
                            throw new Exception("Invalid Number Of Elements");
                        }

                        newUNBSegment.SyntaxIdentifier = syntaxSubParts[0];

                        int result = 3;
                        Int32.TryParse(syntaxSubParts[1], out result);
                        newUNBSegment.SyntaxVersion = result;
                        break;

                    case 2:

                        var senderSubParts = SplitStringBySeparator(allSegments[j], ColonSeparator);

                        if (senderSubParts.Length > 0)
                        {
                            newUNBSegment.SenderId = senderSubParts[0];

                            if (senderSubParts.Length > 1) //not always populated in file so check before trying to assign it....
                            {
                                newUNBSegment.PartnerIDCodeQualifier = senderSubParts[1];
                            }
                        }
                        else
                        {
                            throw new Exception("Invalid Number Of Elements - UNB");
                        }
                        break;

                    case 3:

                        var recipientSubParts = SplitStringBySeparator(allSegments[j], ColonSeparator);

                        if (recipientSubParts.Length > 0)
                        {
                            newUNBSegment.RecipientId = recipientSubParts[0];

                            if (recipientSubParts.Length > 1) //not always populated in file so check before trying to assign it....
                            {
                                newUNBSegment.ParterIDCode = recipientSubParts[1];
                            }
                        }
                        else
                        {
                            throw new Exception("Invalid Number Of Elements - UNB");
                        }
                        break;

                    case 4:

                        var dateSubParts = SplitStringBySeparator(allSegments[j], ColonSeparator);

                        if (expectedSenderElementCount != dateSubParts.Length)
                        {
                            throw new Exception("Invalid Number Of Elements");
                        }

                        DateTime newDateTime;

                        DateTime.TryParseExact(string.Concat(dateSubParts[0], dateSubParts[1]), "yyMMddHHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out newDateTime);

                        newUNBSegment.DateOfPreparation = newDateTime;
                        break;

                    case 5:

                        newUNBSegment.ControlRef = allSegments[j];
                        break;

                    case 9:

                        int ackResult = 0;
                        Int32.TryParse(allSegments[j], out result);
                        newUNBSegment.AckRequest = ackResult;
                        break;

                    default:
                        break;
                }
            }

            return newUNBSegment;
        }
        private static DTMSegment ProcessDTMSegment(string stringToConvert, DTMSegment existingDTMSegment)
        {
            DTMSegment newDTMSegment;
            const int DTMSegmentCount = 3;

            if (existingDTMSegment != null)
            {
                newDTMSegment = existingDTMSegment;
            }
            else
            {
                newDTMSegment = new DTMSegment("DTM");
            }

            var allSegments = SplitStringBySeparator(stringToConvert, PlusSeparator);

            for (int j = 0; j < allSegments.Length; j++)
            {
                switch (j)
                {
                    case 1:

                        var subParts = SplitStringBySeparator(allSegments[j], ColonSeparator);

                        if (subParts.Length != DTMSegmentCount)
                        {
                            throw new Exception("Incorrect number of segments");
                        }

                        string dateValueToConvert = subParts[1];
                        DateTime dtmDateTime;

                        int result = 0;

                        if (subParts[2].Contains("'"))
                        {
                            int.TryParse(subParts[2].Replace(SingleQuoteSeparator, ' ').TrimEnd(), out result);
                        }
                        else
                        {
                            int.TryParse(subParts[2], out result);
                        }

                        string enumPicture = string.Empty;

                        if (result == (int)DateConvertEnum.OneOTwoDate)
                        {
                            enumPicture = Utilities.GetDescription(DateConvertEnum.OneOTwoDate);
                        }

                        if (result == (int)DateConvertEnum.TwoOTwoDate)
                        {
                            enumPicture = Utilities.GetDescription(DateConvertEnum.OneOTwoDate);
                        }

                        DateTime.TryParseExact(dateValueToConvert, enumPicture, CultureInfo.InvariantCulture, DateTimeStyles.None, out dtmDateTime);

                        if (subParts[0] == "64")
                        {
                            newDTMSegment.DeliveryDate = dtmDateTime;
                        }

                        if (subParts[0] == "137")
                        {
                            newDTMSegment.MessageDate = dtmDateTime;
                        }

                        break;

                    default:
                        break;
                }
            }

            return newDTMSegment;
        }
        private static BGMSegment ProcessBGMSegment(string stringToConvert, FileStatusClass fileStatus)
        {
            var newBGMSegment = new BGMSegment("BGM");

            if (!fileStatus.TransmissionStarted || !fileStatus.DocumentStarted)
            {
                throw new Exception("Unexpected segment (BGM) before the document start segment (UNH).");
            }

            if (fileStatus.LineItemStarted)
            {
                throw new Exception("Unexpected segment (BGM) before the end of the last document (UNT).");
            }


            var allSegments = SplitStringBySeparator(stringToConvert, PlusSeparator);

            for (int j = 0; j < allSegments.Length; j++)
            {
                switch (j)
                {
                    case 1:

                        newBGMSegment.MessageName = allSegments[j];
                        break;

                    case 2:

                        newBGMSegment.DocumentNumber = allSegments[j];
                        break;

                    case 3:

                        newBGMSegment.MessageFunction = allSegments[j];
                        break;

                    case 4:

                        newBGMSegment.ResponseType = allSegments[j];
                        break;


                    default:
                        break;
                }
            }

            return newBGMSegment;
        }
        private static UNZSegment ProcessUNZSegment(string stringToConvert, FileStatusClass fileStatus)
        {
            var newUNZSegment = new UNZSegment("UNZ");

            if (!fileStatus.TransmissionStarted)
            {
                throw new Exception("End of transmission segment (UNZ) before the message start segment (UNB).");
            }

            if (fileStatus.HasCNTSegment)
            {
                if (fileStatus.LineItemStarted)
                {
                    throw new Exception("End of transmission segment (UNZ) before the last line has been completed in the last message (CNT segment).");
                }
            }

            if (fileStatus.DocumentStarted)
            {
                throw new Exception("End of transmission segment (UNZ) before the last document has been completed (UNT segment).");
            }

            var allSegments = SplitStringBySeparator(stringToConvert, PlusSeparator);

            for (int j = 0; j < allSegments.Length; j++)
            {
                switch (j)
                {
                    case 1:

                        int result = 2;
                        Int32.TryParse(allSegments[j], out result);
                        newUNZSegment.TrailerControlCount = result;
                        break;

                    case 2:

                        newUNZSegment.TrailerControlReference = allSegments[j];
                        break;

                    default:
                        break;
                }
            }

            if (newUNZSegment.TrailerControlCount != fileStatus.DocumentCount)
            {
                throw new Exception("The number of documents stated in the Transmission Trailer (UNZ) segment does not match the number of documents read in the file (" + newUNZSegment.TrailerControlCount + " vs " + fileStatus.DocumentCount + ")");
            }

            return newUNZSegment;
        }
        private static IMDSegment ProcessIMDSegment(string stringToConvert)
        {
            var newIMDSegment = new IMDSegment("IMD");

            var allSegments = SplitStringBySeparator(stringToConvert, PlusSeparator);

            for (int j = 0; j < allSegments.Length; j++)
            {
                switch (j)
                {
                    case 1:

                        newIMDSegment.Code = allSegments[j];
                        break;

                    case 3:

                        var subParts = SplitStringBySeparator(allSegments[j], ColonSeparator);

                        newIMDSegment.DescriptionCode = subParts[0];

                        if (newIMDSegment.Code == "F")
                        {
                            newIMDSegment.Description = subParts[3].Replace('\'', ' ').TrimEnd();
                        }

                        break;

                    default:
                        break;
                }
            }

            return newIMDSegment;
        }
        private static UNSSegment ProcessUNSSegment(string stringToConvert)
        {
            var newUNSSegment = new UNSSegment("UNS");

            var allSegments = SplitStringBySeparator(stringToConvert, PlusSeparator);

            for (int j = 0; j < allSegments.Length; j++)
            {
                switch (j)
                {
                    case 1:

                        char unsChar;
                        char.TryParse(allSegments[j].Replace(SingleQuoteSeparator, ' ').TrimEnd(), out unsChar);
                        newUNSSegment.UNS = unsChar;
                        break;

                    default:
                        break;
                }
            }

            return newUNSSegment;
        }
        private static PIASegment ProcessPIASegment(string stringToConvert, FileStatusClass fileStatus)
        {
            var newPIASegment = new PIASegment("PIA");

            if (!fileStatus.LineItemStarted)
            {
                throw new Exception("Additional Product ID segment (PIA) segment found before the start of a line item (LIN)");
            }

            var allSegments = SplitStringBySeparator(stringToConvert, PlusSeparator);

            for (int j = 0; j < allSegments.Length; j++)
            {
                switch (j)
                {
                    case 1:

                        newPIASegment.Code = allSegments[j];
                        break;

                    case 2:

                        var subItemCodeParts = SplitStringBySeparator(allSegments[j], ColonSeparator);

                        newPIASegment.ItemCode = subItemCodeParts[0];
                        break;

                    default:
                        break;
                }
            }

            return newPIASegment;
        }
        private static CNTSegment ProcessCNTSegment(string stringToConvert, FileStatusClass fileStatus)
        {
            var newCNTSegment = new CNTSegment("CNT");

            if (!fileStatus.LineItemStarted)
            {
                throw new Exception("Document Count (CNT) segment found before the start of a document (UNH)");
            }
            else
            {
                fileStatus.LineItemStarted = false;
            }


            var allSegments = SplitStringBySeparator(stringToConvert, PlusSeparator);

            for (int j = 0; j < allSegments.Length; j++)
            {
                switch (j)
                {
                    case 1:

                        var subParts = SplitStringBySeparator(allSegments[j], ColonSeparator);

                        int result = 2;

                        if (subParts[1].Contains("'"))
                        {
                            int.TryParse(subParts[1].Replace(SingleQuoteSeparator, ' ').TrimEnd(), out result);
                        }
                        else
                        {
                            int.TryParse(subParts[1], out result);
                        }

                        newCNTSegment.MessageSegmentsCount = result;

                        break;

                    default:
                        break;
                }


            }

            if (newCNTSegment.MessageSegmentsCount != fileStatus.LineCount)
            {
                throw new Exception("The number of lines stated in the Count (CNT) segment does not match the number of lines (LIN) read in the file (" + newCNTSegment.MessageSegmentsCount + " vs " + fileStatus.LineCount + ")");
            }

            return newCNTSegment;
        }
        private static LINSegment ProcessLINSegment(string stringToConvert, FileStatusClass fileStatus)
        {
            var newLINSegment = new LINSegment("LIN");

            if (!fileStatus.DocumentStarted)
            {
                throw new Exception("Line segment (LIN) segment found before the start of a document (UNH)");
            }
            else
            {
                if (fileStatus.LineItemStarted)
                {
                    fileStatus.LineCount++;
                }
                else
                {
                    fileStatus.LineItemStarted = true;
                    fileStatus.LineCount = 1;
                }

            }

            var allSegments = SplitStringBySeparator(stringToConvert, PlusSeparator);

            for (int j = 0; j < allSegments.Length; j++)
            {
                switch (j)
                {
                    case 1:

                        int result = 2;
                        Int32.TryParse(allSegments[j], out result);
                        newLINSegment.LineNumber = result;
                        break;

                    case 2:

                        newLINSegment.ActionRequest = allSegments[j];
                        break;

                    case 3:

                        var subParts = SplitStringBySeparator(allSegments[j], ColonSeparator);

                        newLINSegment.ProductANANumber = subParts[0];
                        break;

                    default:
                        break;
                }
            }

            return newLINSegment;
        }
        private static UNTSegment ProcessUNTSegment(string stringToConvert, FileStatusClass fileStatus)
        {
            var newUNTSegment = new UNTSegment("UNT");

            if (!fileStatus.DocumentStarted)
            {
                throw new Exception("Message Trailer (UNT) segment found before the start of a document (UNH)");
            }

            var allSegments = SplitStringBySeparator(stringToConvert, PlusSeparator);

            for (int j = 0; j < allSegments.Length; j++)
            {
                switch (j)
                {
                    case 1:

                        int result = 2;
                        Int32.TryParse(allSegments[j], out result);
                        newUNTSegment.TrailerMessageSegmentsCount = result;
                        break;

                    case 2:

                        newUNTSegment.TrailerMessageReference = allSegments[j];
                        break;

                    default:
                        break;
                }
            }

            //Add 1 to the Document Segement Count to include this segment
            if (newUNTSegment.TrailerMessageSegmentsCount != (fileStatus.DocumentSegmentCount + 1))
            {
                throw new Exception("The number of segments stated in the message count (UNT) segment does not match the number of segments read in the message (" + newUNTSegment.TrailerMessageSegmentsCount + " vs " + (fileStatus.DocumentSegmentCount + 1) + ")");
            }


            return newUNTSegment;
        }
        private static NADSegment ProcessNADSegment(string stringToConvert, NADSegment existingNADSegment, FileStatusClass fileStatus)
        {
            NADSegment newNADSegment = null;

            if (!fileStatus.DocumentStarted)
            {
                throw new Exception("Name and Address (NAD) segment found before the start of a document (UNH)");
            }

            var segmentType = NADSegment.NADSegmentTypeEnum.BY;

            if (existingNADSegment != null)
            {
                newNADSegment = existingNADSegment;
            }
            else
            {
                newNADSegment = new NADSegment("NAD");
            }

            var allSegments = SplitStringBySeparator(stringToConvert, PlusSeparator);

            for (int j = 0; j < allSegments.Length; j++)
            {
                switch (j)
                {
                    case 1:

                        if (string.Equals(allSegments[j], "DP"))
                        {
                            segmentType = NADSegment.NADSegmentTypeEnum.DP;
                        }

                        if (string.Equals(allSegments[j], "SE"))
                        {
                            segmentType = NADSegment.NADSegmentTypeEnum.SE;
                        }

                        if (string.Equals(allSegments[j], "SU"))
                        {
                            segmentType = NADSegment.NADSegmentTypeEnum.SU;
                        }

                        break;

                    case 2:

                        var segmentDetails = SplitStringBySeparator(allSegments[j], ColonSeparator);

                        switch (segmentType)
                        {
                            case NADSegment.NADSegmentTypeEnum.BY:
                                newNADSegment.Buyer = segmentDetails[0];
                                break;
                            case NADSegment.NADSegmentTypeEnum.DP:
                                newNADSegment.DeliveryPoint = segmentDetails[0];
                                break;
                            case NADSegment.NADSegmentTypeEnum.SE:
                                newNADSegment.Seller = segmentDetails[0];
                                break;
                            case NADSegment.NADSegmentTypeEnum.SU:
                                newNADSegment.Supplier = segmentDetails[0];
                                break;
                            default:
                                break;
                        }

                        break;

                    default:
                        break;
                }
            }

            return newNADSegment;
        }
        private static QTYSegment ProcessQTYSegment(string stringToConvert, QTYSegment existingQTYSegment, FileStatusClass fileStatus)
        {
            QTYSegment newQTYSegment = null;

            if (!fileStatus.LineItemStarted)
            {
                throw new Exception("Quantity (QTY) segment found before the start of a line item (LIN)");
            }

            if (existingQTYSegment != null)
            {
                newQTYSegment = existingQTYSegment;
            }
            else
            {
                newQTYSegment = new QTYSegment("QTY");
            }

            var allSegments = SplitStringBySeparator(stringToConvert, PlusSeparator);

            for (int j = 0; j < allSegments.Length; j++)
            {
                switch (j)
                {
                    case 1:

                        var segmentDetails = SplitStringBySeparator(allSegments[j], ColonSeparator);

                        int segmentId = 21;

                        int.TryParse(segmentDetails[0], out segmentId);

                        if ((int)QTYSegment.QuantitySegmentTypeEnum.ActualQtySegment == segmentId)
                        {
                            decimal orderedQtyParseInt = 0;

                            if (segmentDetails[1].Contains("'"))
                            {
                                decimal.TryParse(segmentDetails[1].Replace(SingleQuoteSeparator, ' ').TrimEnd(), out orderedQtyParseInt);
                            }
                            else
                            {
                                decimal.TryParse(segmentDetails[1], out orderedQtyParseInt);
                            }

                            newQTYSegment.OrderedQty = orderedQtyParseInt;
                        }

                        if ((int)QTYSegment.QuantitySegmentTypeEnum.ConsumerUnitInTradedUnitSegment == segmentId)
                        {
                            decimal consumerUnitInTradedUnitParseDecimal = 0;

                            if (segmentDetails[1].Contains("'"))
                            {
                                decimal.TryParse(segmentDetails[1].Replace(SingleQuoteSeparator, ' ').TrimEnd(), out consumerUnitInTradedUnitParseDecimal);
                            }
                            else
                            {
                                decimal.TryParse(segmentDetails[1], out consumerUnitInTradedUnitParseDecimal);
                            }

                            newQTYSegment.ConsumerUnitInTradedUnit = consumerUnitInTradedUnitParseDecimal;
                        }

                        break;

                    default:
                        break;
                }
            }

            return newQTYSegment;
        }
        private static PRISegment ProcessPRISegment(string stringToConvert, PRISegment existingPRISegment, FileStatusClass fileStatus)
        {
            //PRI+AAA:618.56’
            PRISegment newPRISegment = null;

            if (!fileStatus.DocumentStarted)
            {
                throw new Exception("Price Information (PRI) segment found before the start of a document (UNH)");
            }

            var segmentType = PRISegment.PRISegmentEnum.AAA;

            if (existingPRISegment != null)
            {
                newPRISegment = existingPRISegment;
            }
            else
            {
                newPRISegment = new PRISegment("PRI");
            }

            var allSegments = SplitStringBySeparator(stringToConvert, PlusSeparator);

            for (int j = 0; j < allSegments.Length; j++)
            {
                switch (j)
                {
                    case 1:
                        var priDetailsSplit = SplitStringBySeparator(allSegments[j], ColonSeparator);
                        try
                        {
                            segmentType = (PRISegment.PRISegmentEnum)Enum.Parse(typeof(PRISegment.PRISegmentEnum), priDetailsSplit[0]);
                            newPRISegment.PriceEnumValue = segmentType.ToString();
                        }
                        catch (Exception)
                        {
                            throw new Exception("Invalid PRI Segment Type in file");
                        }

                        if(priDetailsSplit.Length == 2)
                        {
                            newPRISegment.Price = Convert.ToDecimal(priDetailsSplit[1]);
                        }

                        if (priDetailsSplit.Length == 3)
                        {
                            newPRISegment.PriceTypeCode = priDetailsSplit[2];
                        }

                        if (priDetailsSplit.Length == 4)
                        {
                            newPRISegment.PriceTypeQualifier = priDetailsSplit[3];
                        }

                        if (priDetailsSplit.Length == 5)
                        {
                            newPRISegment.UnitPriceBasis = Convert.ToInt32(priDetailsSplit[4]);
                        }

                        if (priDetailsSplit.Length == 6)
                        {
                            newPRISegment.UOMQualifier = priDetailsSplit[5];
                        }

                        break;
                    default:
                        break;
                }
            }

            return newPRISegment;
        }


        //list mea segments...todo
        private static MEASegment ProcessMEASegment(string stringToConvert, MEASegment existingMEASegment, FileStatusClass fileStatus)
        {
            // //'MEA+PD+HT+CMT:18.5
            MEASegment newMEASegment = null;

            if (!fileStatus.DocumentStarted)
            {
                throw new Exception("Measurements (MEA) segment found before the start of a document (UNH)");
            }

            var segmentType = COMSegment.COMSegmentEnum.EM;

            if (existingMEASegment != null)
            {
                newMEASegment = existingMEASegment;
            }
            else
            {
                newMEASegment = new MEASegment("MEA");
            }

            //var measurementDetail = SplitStringBySeparator(allSegments[j], ColonSeparator);

            var allSegments = SplitStringBySeparator(stringToConvert, PlusSeparator);

            for (int j = 0; j < allSegments.Length; j++)
            {
                switch (j)
                {
                    case 1:
                        newMEASegment.MeasurementQualifier = allSegments[j];
                        break;
                    case 2:
                        newMEASegment.MeasureOf = allSegments[j];
                        break;
                    case 3:
                        var measurementDetail = SplitStringBySeparator(allSegments[j], ColonSeparator);
                        newMEASegment.UnitOFMeasurement = measurementDetail[0];
                        if (measurementDetail.Length != 0)
                        {
                            newMEASegment.MeasurementValue = Convert.ToDecimal(measurementDetail[1]);
                        }
                        else
                        {
                            throw new Exception("No details in MEA segment");
                        }
                        break;
                    default:
                        break;
                }
            }

            return newMEASegment;
        }
        private static MOASegment ProcessMOASegment(string stringToConvert, MOASegment existingMOASegment, FileStatusClass fileStatus)
        {
            MOASegment newMOASegment = null;

            if (!fileStatus.DocumentStarted)
            {
                throw new Exception("Monetary (MOA) segment found before the start of a document (UNH)");
            }

            var segmentType = COMSegment.COMSegmentEnum.EM;

            if (existingMOASegment != null)
            {
                newMOASegment = existingMOASegment;
            }
            else
            {
                newMOASegment = new MOASegment("MOA");
            }

            var allSegments = SplitStringBySeparator(stringToConvert, PlusSeparator);

            for (int j = 0; j < allSegments.Length; j++)
            {
                switch (j)
                {
                    case 1:
                        var moneyDetailsSplit = SplitStringBySeparator(allSegments[j], ColonSeparator);
                        if (moneyDetailsSplit.Length > 3)
                        {
                            throw new Exception("Too many segments in MOA segment.");
                        }

                        newMOASegment.MonetaryTypeQualifier = Convert.ToInt32(moneyDetailsSplit[0]);

                        if (moneyDetailsSplit.Length >= 1)
                        {
                            newMOASegment.Amount = Convert.ToDecimal(moneyDetailsSplit[1]);
                        }

                        if (moneyDetailsSplit.Length > 2) //not always populated...
                        {
                            newMOASegment.CurrencyCode = moneyDetailsSplit[2];
                        }

                     
                        break;
                    default:
                        break;
                }
            }

            return newMOASegment;
        }
        //list of freetext....TODO
        private static FTXSegment ProcessFTXSegment(string stringToConvert, FTXSegment existingFTXSegment, FileStatusClass fileStatus)
        {
            ////FTX+AAB+1++30 DAYS AT 0PCT FOLLOWING MONTH OF INVOICE sample 
            var newFTXSegment = new FTXSegment("FTX");

            var allSegments = SplitStringBySeparator(stringToConvert, PlusSeparator);

            for (int j = 0; j < allSegments.Length; j++)
            {
                switch (j)
                {
                    case 1:
                        newFTXSegment.TextSubjectQualifier = allSegments[j]; //code
                        break;
                    case 2:
                        newFTXSegment.TextFunction = allSegments[j];
                        break;
                    case 3:
                        newFTXSegment.TextReference = allSegments[j];
                        break;
                    case 4:
                        newFTXSegment.FreeTextLine1 = allSegments[j];
                        break;

                    default:
                        break;
                }
            }

            return newFTXSegment;
            return new FTXSegment(stringToConvert);
        }

        private static RFFSegment ProcessRFFSegment(string stringToConvert, RFFSegment existingRFFSegment, FileStatusClass fileStatus)
        {
            // //RFF+ON:12 SAMPLE FORMAT
            RFFSegment newRFFSegment = null;

            if (!fileStatus.DocumentStarted)
            {
                throw new Exception("Communication Information (COM) segment found before the start of a document (UNH)");
            }

            var segmentType = RFFSegment.RFFSegmentEnum.API;

            if (existingRFFSegment != null)
            {
                newRFFSegment = existingRFFSegment;
            }
            else
            {
                newRFFSegment = new RFFSegment("RFF");
            }

            var allSegments = SplitStringBySeparator(stringToConvert, PlusSeparator);
            for (int j = 0; j < allSegments.Length; j++)
            {
                switch (j)
                {
                    case 1:
                        var contactDetailsSplit = SplitStringBySeparator(allSegments[j], ColonSeparator);
                        try
                        {
                            segmentType = (RFFSegment.RFFSegmentEnum)Enum.Parse(typeof(RFFSegment.RFFSegmentEnum), contactDetailsSplit[1]);

                        }
                        catch (Exception)
                        {
                            throw new Exception("Invalid RFF Segment Type in file");
                        }
                        break;

                    case 2:
                        switch (segmentType)
                        {
                            case RFFSegment.RFFSegmentEnum.API:
                                newRFFSegment.SuppliersAlternativeBuyerCode = allSegments[j];
                                break;

                            case RFFSegment.RFFSegmentEnum.SS:
                                newRFFSegment.SupplierOrderNumber = allSegments[j];
                                break;

                            case RFFSegment.RFFSegmentEnum.CR:
                                newRFFSegment.SuppliersBuyerCode = allSegments[j];
                                break;

                            case RFFSegment.RFFSegmentEnum.ON:
                                newRFFSegment.BuyerOrderNumber = allSegments[j];
                                break;
                            default:
                                break;
                        }

                        break;
                    default:
                        break;
                }
            }

            return newRFFSegment;


        }

        private static CTASegment ProcessCTASegment(string stringToConvert, CTASegment existingCTASegment, FileStatusClass fileStatus)
        {
            CTASegment newCTASegment = null;

            if (!fileStatus.DocumentStarted)
            {
                throw new Exception("Contact Information (NAD) segment found before the start of a document (UNH)");
            }

            var segmentType = CTASegment.CTASegmentEnum.AD;

            if (existingCTASegment != null)
            {
                newCTASegment = existingCTASegment;
            }
            else
            {
                newCTASegment = new CTASegment("CTA");
            }

            var allSegments = SplitStringBySeparator(stringToConvert, PlusSeparator);

            for (int j = 0; j < allSegments.Length; j++)
            {
                switch (j)
                {
                    case 1:
                        var contactDetailsSplit = SplitStringBySeparator(allSegments[j], ColonSeparator);
                        try
                        {
                            segmentType = (CTASegment.CTASegmentEnum)Enum.Parse(typeof(CTASegment.CTASegmentEnum), contactDetailsSplit[0]);
                        }
                        catch (Exception)
                        {
                            throw new Exception("Invalid CTA Segment Type in file.");
                        }


                        break;

                    case 2:

                        var segmentDetails = SplitStringBySeparator(allSegments[j], ColonSeparator);

                        if (segmentDetails.Length > 0)
                        {
                            newCTASegment.DepartmentEmployee = segmentDetails[0];
                            
                            if (segmentDetails.Length > 1)
                            {
                                newCTASegment.DepartmentEmployeeName = segmentDetails[1];
                            }
                        }
                        else
                        {
                            throw new Exception("Invalid Number Of Elements - UNB");
                        }
                        break;
                    default:
                        break;
                }
            }

            return newCTASegment;
        }
        private static CUXSegment ProcessCUXSegment(string stringToConvert, CUXSegment existingCUXSegment, FileStatusClass fileStatus)
        {
            CUXSegment newCUXSegment = null;

            if (!fileStatus.DocumentStarted)
            {
                throw new Exception("Currency (CUX) segment found before the start of a document (UNH)");
            }

            if (existingCUXSegment != null)
            {
                newCUXSegment = existingCUXSegment;
            }
            else
            {
                newCUXSegment = new CUXSegment("CUX");
            }

            var allSegments = SplitStringBySeparator(stringToConvert, PlusSeparator);

            for (int j = 0; j < allSegments.Length; j++)
            {
                switch (j)
                {
                    case 1:
                        var refCurrencySubParts = SplitStringBySeparator(allSegments[j], ColonSeparator);
                        int result = 2;

                        Int32.TryParse(refCurrencySubParts[0], out result);
                        newCUXSegment.ReferenceCurrency = result;

                        if (refCurrencySubParts.Length >= 2)
                        {
                            newCUXSegment.CurrencyCode = refCurrencySubParts[1];
                            if (refCurrencySubParts.Length > 2)
                            {
                                newCUXSegment.InvoicingCurrency = Convert.ToInt32(refCurrencySubParts[2]);
                            }
                        }

                        if (refCurrencySubParts.Length > 3)
                        {
                            throw new Exception("Too many segments supplied in currency segment");
                        }
                        break;

                    default:
                        break;
                }
            }


            return newCUXSegment;
        }
        private static COMSegment ProcessCOMSegment(string stringToConvert, COMSegment existingCOMSegment, FileStatusClass fileStatus)
        {
            COMSegment newCOMSegment = null;

            if (!fileStatus.DocumentStarted)
            {
                throw new Exception("Communication Information (COM) segment found before the start of a document (UNH)");
            }

            var segmentType = COMSegment.COMSegmentEnum.EM;

            if (existingCOMSegment != null)
            {
                newCOMSegment = existingCOMSegment;
            }
            else
            {
                newCOMSegment = new COMSegment("COM");
            }

            var allSegments = SplitStringBySeparator(stringToConvert, PlusSeparator);

            for (int j = 0; j < allSegments.Length; j++)
            {
                switch (j)
                {
                    case 1:
                        var contactDetailsSplit = SplitStringBySeparator(allSegments[j], ColonSeparator);
                        try
                        {
                            segmentType = (COMSegment.COMSegmentEnum)Enum.Parse(typeof(COMSegment.COMSegmentEnum), contactDetailsSplit[1]);

                        }
                        catch (Exception)
                        {
                            throw new Exception("Invalid COM Segment Type in file");
                        }

                        if (contactDetailsSplit.Length > 0)
                        {
                            switch (segmentType)
                            {
                                case COMSegment.COMSegmentEnum.EM:
                                    newCOMSegment.Email = contactDetailsSplit[0];
                                    break;
                                case COMSegment.COMSegmentEnum.TE:
                                    newCOMSegment.Phone = contactDetailsSplit[0];
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            throw new Exception("Invalid Number Of Elements - COM");
                        }


                        break;
                    default:
                        break;
                }
            }

            return newCOMSegment;
        }
        private static void ProcessINVSegment(string stringToConvert, FileStatusClass fileStatus)
        {
            //Processes invalid segments
            throw new Exception("WTF is this segment? <" + stringToConvert + ">");
        }

        private static bool ProcessEDILine(string ediLineToProcess, ref EDIFact01MessageStructure theFile, ref EDIFact01MessageBody theMessage, ref EDIFact01MessageLines theLine, ref FileStatusClass fileStatus)
        {
            bool hasUNTSegment = false;
            var lineIdentifiers = string.IsNullOrWhiteSpace(ediLineToProcess) ? string.Empty : ediLineToProcess.Length > 0 ? ediLineToProcess.Substring(0, 3) : ediLineToProcess;
            var thisSegment = ConvertSegmentToEDIFactSegmentEnum(lineIdentifiers);

            if (lineIdentifiers != null && lineIdentifiers != string.Empty) //if nothing after the quote then its end of file no need to process
            {
                switch (thisSegment)
                {
                    case EDIFactSegmentEnum.UNB:
                        theFile.MessageHeaderUNB = ProcessUNBSegment(ediLineToProcess, fileStatus);
                        break;
                    case EDIFactSegmentEnum.UNH:
                        theMessage.BodyHeaderUNH = ProcessUNHSegment(ediLineToProcess, fileStatus);
                        break;
                    case EDIFactSegmentEnum.BGM:
                        theMessage.BodyBeginningofMessageBGM = ProcessBGMSegment(ediLineToProcess, fileStatus);
                        break;
                    case EDIFactSegmentEnum.DTM:
                        theMessage.BodyDateAndTimeDTM = ProcessDTMSegment(ediLineToProcess, new DTMSegment("DTM"));
                        break;
                    case EDIFactSegmentEnum.NAD:
                        theMessage.BodyNameAndAddressNAD = ProcessNADSegment(ediLineToProcess, new NADSegment("NAD"), fileStatus);
                        break;
                    case EDIFactSegmentEnum.CTA:
                        theMessage.ContactInformationCTA = ProcessCTASegment(ediLineToProcess, new CTASegment("CTA"), fileStatus);
                        break;
                    case EDIFactSegmentEnum.COM:
                        theMessage.CommunicationContactCOM = ProcessCOMSegment(ediLineToProcess, new COMSegment("COM"), fileStatus);
                        break;
                    case EDIFactSegmentEnum.CUX:
                        theMessage.CurrenciesCUX = ProcessCUXSegment(ediLineToProcess, new CUXSegment("CUX"), fileStatus);
                        break;
                    case EDIFactSegmentEnum.RFF:
                        theMessage.ReferenceRFF = ProcessRFFSegment(ediLineToProcess, new RFFSegment("RFF"), fileStatus);
                        break;
                    case EDIFactSegmentEnum.FTX:
                        theMessage.FreeTextFTX = ProcessFTXSegment(ediLineToProcess, new FTXSegment("FTX"), fileStatus);
                        break;
                    case EDIFactSegmentEnum.LIN:
                        if (fileStatus.LineItemStarted)
                        {
                            //write the previous line before starting another
                            theMessage.Lines.Add(theLine);
                            theLine = new EDIFact01MessageLines();
                        }
                        theLine.LineItemLIN = ProcessLINSegment(ediLineToProcess, fileStatus);
                        break;
                    case EDIFactSegmentEnum.PIA:
                        theLine.LineAdditionalProduceInformationPIA = ProcessPIASegment(ediLineToProcess, fileStatus);
                        break;
                    case EDIFactSegmentEnum.IMD:
                        theLine.LineItemDescriptionIMD = ProcessIMDSegment(ediLineToProcess);
                        break;
                    case EDIFactSegmentEnum.QTY:
                        theLine.LineQuantityQTY = ProcessQTYSegment(ediLineToProcess, new QTYSegment("QTY"), fileStatus);
                        break;
                    case EDIFactSegmentEnum.MOA:
                        theLine.LineMonetaryAmountMOA = ProcessMOASegment(ediLineToProcess, new MOASegment("MOA"), fileStatus);
                        break;
                    case EDIFactSegmentEnum.PRI:
                        theLine.LinePriceInformationPRI = ProcessPRISegment(ediLineToProcess, new PRISegment("PRI"), fileStatus);
                        break;
                    case EDIFactSegmentEnum.MEA:
                        theLine.LineMeasurementMEA = ProcessMEASegment(ediLineToProcess, new MEASegment("MEA"), fileStatus);
                        break;

                    case EDIFactSegmentEnum.UNS:
                        if (fileStatus.LineItemStarted)
                        {
                            //write the last line
                            theMessage.Lines.Add(theLine);
                            theLine = new EDIFact01MessageLines();
                        }
                        theMessage.BodySectionControlUNS = ProcessUNSSegment(ediLineToProcess);
                        break;
                    case EDIFactSegmentEnum.CNT:
                        theMessage.BodyControlTotalCNT = ProcessCNTSegment(ediLineToProcess, fileStatus);
                        hasUNTSegment = true;
                        break;
                    case EDIFactSegmentEnum.UNT:
                        theMessage.BodyMessageTrailerUNT = ProcessUNTSegment(ediLineToProcess, fileStatus);
                        if (fileStatus.DocumentStarted)
                        {
                            //Document already started, so save the last document before clearing ready for the next one
                            theFile.Documents.Add(theMessage);

                            theMessage = new EDIFact01MessageBody();
                            fileStatus.DocumentStarted = false;
                        }
                        break;
                    case EDIFactSegmentEnum.UNZ:
                        fileStatus.HasCNTSegment = hasUNTSegment;
                        theFile.MessageFooterUNZ = ProcessUNZSegment(ediLineToProcess, fileStatus);
                        break;

                    case EDIFactSegmentEnum.INV:
                        ProcessINVSegment(ediLineToProcess, fileStatus);
                        break;
                    default:
                        break;
                }


                //Sort out segment counts
                if (fileStatus.DocumentStarted)
                {
                    fileStatus.DocumentSegmentCount++;
                }

                if (fileStatus.TransmissionStarted)
                {
                    fileStatus.TransmissionSegmentCount++;
                }

                return true;
            }
            else return false;
        }

        #endregion
    }
}
