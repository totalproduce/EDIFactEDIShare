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
            CUX,
            LIN,
            PIA,
            IMD,
            QTY,
            UNS,
            CNT,
            UNT,
            UNZ,
            DTM,
            INV
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
            
            //ftx
             
            public NADSegment BodyNameAndAddressNAD { get; set; }
            
            //CUX

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
            public FTXSegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }
            public int TrailerControlCount { get; set; }
            public string TrailerControlReference { get; set; }
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

        static void Main(string[] args)
        {
            string[] lines = System.IO.File.ReadAllLines(@"C:\EDITESTS\WS843729.MFD"); 
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
        


        private static UNHSegment ProcessUNHSegment(string stringToConvert, FileStatusClass fileStatus )
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

                        if (expectedSenderElementCount != senderSubParts.Length)
                        {
                            throw new Exception("Invalid Number Of Elements");
                        }

                        newUNBSegment.SenderId = senderSubParts[0];
                        newUNBSegment.PartnerIDCodeQualifier = senderSubParts[1];
                        break;

                    case 3:

                        var recipientSubParts = SplitStringBySeparator(allSegments[j], ColonSeparator);

                        if (expectedSenderElementCount != recipientSubParts.Length)
                        {
                            throw new Exception("Invalid Number Of Elements");
                        }

                        newUNBSegment.RecipientId = recipientSubParts[0];
                        newUNBSegment.ParterIDCode = recipientSubParts[1];
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

            if (fileStatus.LineItemStarted)
            {
                throw new Exception("End of transmission segment (UNZ) before the last line has been completed in the last message (CNT segment).");
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
                throw new Exception("The number of segments stated in the message count (UNT) segment does not match the number of segments read in the message (" + newUNTSegment.TrailerMessageSegmentsCount + " vs " + fileStatus.DocumentSegmentCount + ")");
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

                            newQTYSegment.ConsumerUnitInTradedUnit= consumerUnitInTradedUnitParseDecimal;
                        }

                        break;

                    default:
                        break;
                }
            }

            return newQTYSegment;
        }
        private static void ProcessINVSegment(string stringToConvert, FileStatusClass fileStatus)
        {
            //Processes invalid segments
            throw new Exception("WTF is this segment? <" + stringToConvert + ">");
        }

        private static bool ProcessEDILine(string ediLineToProcess, ref EDIFact01MessageStructure theFile, ref EDIFact01MessageBody theMessage, ref EDIFact01MessageLines theLine, ref FileStatusClass fileStatus)
        {
            var lineIdentifiers = string.IsNullOrWhiteSpace(ediLineToProcess) ? string.Empty : ediLineToProcess.Length > 0 ? ediLineToProcess.Substring(0, 3) : ediLineToProcess;

            var thisSegment = ConvertSegmentToEDIFactSegmentEnum(lineIdentifiers);

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
                case EDIFactSegmentEnum.NAD:
                    theMessage.BodyNameAndAddressNAD = ProcessNADSegment(ediLineToProcess, new NADSegment("NAD"), fileStatus);
                    break;
                case EDIFactSegmentEnum.LIN:
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
                case EDIFactSegmentEnum.UNS:
                    var newUNSSegment = ProcessUNSSegment(ediLineToProcess);
                    break;
                case EDIFactSegmentEnum.CNT:
                    var newCNTSegment = ProcessCNTSegment(ediLineToProcess, fileStatus);
                    break;
                case EDIFactSegmentEnum.UNT:
                    var newUNTSegment = ProcessUNTSegment(ediLineToProcess, fileStatus);
                    if (fileStatus.DocumentStarted)
                    {
                        //Document already started, so save the last document before clearing ready for the next one
                        theFile.Documents.Add(theMessage);

                        theMessage = new EDIFact01MessageBody();
                        fileStatus.DocumentStarted = false;
                    }
                    break;
                case EDIFactSegmentEnum.UNZ:
                    var newUNZSegment = ProcessUNZSegment(ediLineToProcess, fileStatus);
                    break;
                case EDIFactSegmentEnum.DTM:
                    var newDTMSegment = ProcessDTMSegment(ediLineToProcess, new DTMSegment("DTM"));
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

        #endregion
    }
}
