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
            UNB,
            UNH,
            BGM,
            NAD,
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

        public class EDIFact01MessageStructure
        {
            public UNBSegment Header { get; set; }
            public UNZSegment Footer { get; set; }
            public List<EDIFact01MessageBody> LineItems { get; set; }
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
            public UNHSegment BodyHeader { get; set; }
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

        public class CNTSegment : GenericSegments
        {
            public CNTSegment(string segmentIdentifier)
            {
                SegmentIdentifier = segmentIdentifier;
            }
            public int MessageSegmentsCount { get; set; }
            public string MessageReference { get; set; }
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

            public enum NADSegmentTypeEnum
            {
                BY,
                DP,
                SE
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
            string[] lines = System.IO.File.ReadAllLines(@"C:\EDITESTS\WS837541.MFD");

            if (lines.Length < 2)
            {
                var allLines = SplitStringBySeparator(lines[0], SingleQuoteSeparator);

                for (int i = 0; i < allLines.Length; i++)
                {
                    var lineIdentifiers = string.IsNullOrWhiteSpace(allLines[i]) ? string.Empty : allLines.Length > 0 ? allLines[i].Substring(0, 3) : allLines[i];

                    switch (ConvertSegmentToEDIFactSegmentEnum(lineIdentifiers))
                    {
                        case EDIFactSegmentEnum.UNB:
                            var newUNBSegment = ProcessUNBSegment(allLines[i]);
                            break;
                        case EDIFactSegmentEnum.UNH:
                            var newUNHSegment = ProcessUNHSegment(allLines[i]);
                            break;
                        case EDIFactSegmentEnum.BGM:
                            var newBGMSegment = ProcessBGMSegment(allLines[i]);
                            break;
                        case EDIFactSegmentEnum.NAD:
                            var newNADSegment = ProcessNADSegment(allLines[i], new NADSegment("NAD"));
                            break;
                        case EDIFactSegmentEnum.LIN:
                            var newLINSegment = ProcessLINSegment(allLines[i]);
                            break;
                        case EDIFactSegmentEnum.PIA:
                            var newPIASegment = ProcessPIASegment(allLines[i]);
                            break;
                        case EDIFactSegmentEnum.IMD:
                            break;
                        case EDIFactSegmentEnum.QTY:
                            var newQTYSegment = ProcessQTYSegment(allLines[i], new QTYSegment("QTY"));
                            break;
                        case EDIFactSegmentEnum.UNS:
                            break;
                        case EDIFactSegmentEnum.CNT:
                            break;
                        case EDIFactSegmentEnum.UNT:
                            var newUNTSegment = ProcessUNTSegment(allLines[i]);
                            break;
                        case EDIFactSegmentEnum.UNZ:
                            var newUNZSegment = ProcessUNZSegment(allLines[i]);
                            break;
                        case EDIFactSegmentEnum.INV:
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Contents of writeLines2.txt =:");
                foreach (string line in lines)
                {
                    var lineIdentifiers = string.IsNullOrWhiteSpace(line) ? string.Empty : line.Length > 0 ? line.Substring(0, 3) : line;

                    var thisSegment = ConvertSegmentToEDIFactSegmentEnum(lineIdentifiers);

                    if (thisSegment == EDIFactSegmentEnum.UNB)
                    {

                    }



                    switch (thisSegment)
                    {
                        case EDIFactSegmentEnum.UNB:
                            var newUNBSegment = ProcessUNBSegment(line);
                            break;
                        case EDIFactSegmentEnum.UNH:
                            var newUNHSegment = ProcessUNHSegment(line);
                            break;
                        case EDIFactSegmentEnum.BGM:
                            var newBGMSegment = ProcessBGMSegment(line);
                            break;
                        case EDIFactSegmentEnum.NAD:
                            var newNADSegment = ProcessNADSegment(line, new NADSegment("NAD"));
                            break;
                        case EDIFactSegmentEnum.LIN:
                            var newLINSegment = ProcessLINSegment(line);
                            break;
                        case EDIFactSegmentEnum.PIA:
                            var newPIASegment = ProcessPIASegment(line);
                            break;
                        case EDIFactSegmentEnum.IMD:
                            break;
                        case EDIFactSegmentEnum.QTY:
                            var newQTYSegment = ProcessQTYSegment(line, new QTYSegment("QTY"));
                            break;
                        case EDIFactSegmentEnum.UNS:
                            break;
                        case EDIFactSegmentEnum.CNT:
                            var newCNTSegment = ProcessCNTSegment(line);
                            break;
                        case EDIFactSegmentEnum.UNT:
                            var newUNTSegment = ProcessUNTSegment(line);
                            break;
                        case EDIFactSegmentEnum.UNZ:
                            var newUNZSegment = ProcessUNZSegment(line);
                            break;
                        case EDIFactSegmentEnum.DTM:
                            var newDTMSegment = ProcessDTMSegment(line, new DTMSegment("DTM"));
                            break;
                        case EDIFactSegmentEnum.INV:
                            break;
                        default:
                            break;
                    }
                }
            }

            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();

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

            EDIFactSegmentEnum eDIFactSegmentEnum = EDIFactSegmentEnum.INV;

            switch (passInSegmentAsString)
            {
                case "UNB":
                    eDIFactSegmentEnum = EDIFactSegmentEnum.UNB;
                    break;
                case "UNH":
                    eDIFactSegmentEnum = EDIFactSegmentEnum.UNH;
                    break;
                case "BGM":
                    eDIFactSegmentEnum = EDIFactSegmentEnum.BGM;
                    break;
                case "NAD":
                    eDIFactSegmentEnum = EDIFactSegmentEnum.NAD;
                    break;
                case "LIN":
                    eDIFactSegmentEnum = EDIFactSegmentEnum.LIN;
                    break;
                case "PIA":
                    eDIFactSegmentEnum = EDIFactSegmentEnum.PIA;
                    break;
                case "IMD":
                    eDIFactSegmentEnum = EDIFactSegmentEnum.IMD;
                    break;
                case "QTY":
                    eDIFactSegmentEnum = EDIFactSegmentEnum.QTY;
                    break;
                case "UNS":
                    eDIFactSegmentEnum = EDIFactSegmentEnum.UNS;
                    break;
                case "CNT":
                    eDIFactSegmentEnum = EDIFactSegmentEnum.CNT;
                    break;
                case "UNT":
                    eDIFactSegmentEnum = EDIFactSegmentEnum.UNT;
                    break;
                case "DTM":
                    eDIFactSegmentEnum = EDIFactSegmentEnum.DTM;
                    break;
                case "UNZ":
                    eDIFactSegmentEnum = EDIFactSegmentEnum.UNZ;
                    break;
                default:
                    break;
            }

            return eDIFactSegmentEnum;
        }

        private static UNHSegment ProcessUNHSegment(string stringToConvert)
        {
            var newUNHSegment = new UNHSegment("UNH");

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

            return newUNHSegment;
        }

        private static UNBSegment ProcessUNBSegment(string stringToConvert)
        {
            var newUNBSegment = new UNBSegment("UNB");
            const int expectedSyntaxElementCount = 2;
            const int expectedSenderElementCount = 2;

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
                            int.TryParse(subParts[2].Replace(SingleQuoteSeparator, ' '), out result);
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

        private static BGMSegment ProcessBGMSegment(string stringToConvert)
        {
            var newBGMSegment = new BGMSegment("BGM");

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
        private static UNZSegment ProcessUNZSegment(string stringToConvert)
        {
            var newUNZSegment = new UNZSegment("UNZ");

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

            return newUNZSegment;
        }
        private static PIASegment ProcessPIASegment(string stringToConvert)
        {
            var newPIASegment = new PIASegment("PIA");

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
        private static CNTSegment ProcessCNTSegment(string stringToConvert)
        {
            var newCNTSegment = new CNTSegment("CNT");

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
                            int.TryParse(subParts[1].Replace(SingleQuoteSeparator, ' '), out result);
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

            return newCNTSegment;
        }
        private static LINSegment ProcessLINSegment(string stringToConvert)
        {
            var newLINSegment = new LINSegment("LIN");

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
        private static UNTSegment ProcessUNTSegment(string stringToConvert)
        {
            var newUNTSegment = new UNTSegment("UNT");

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

            return newUNTSegment;
        }
        private static NADSegment ProcessNADSegment(string stringToConvert, NADSegment existingNADSegment)
        {
            NADSegment newNADSegment = null;

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
        private static QTYSegment ProcessQTYSegment(string stringToConvert, QTYSegment existingQTYSegment)
        {
            QTYSegment newQTYSegment = null;

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
                                decimal.TryParse(segmentDetails[1].Replace(SingleQuoteSeparator, ' '), out orderedQtyParseInt);
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
                                decimal.TryParse(segmentDetails[1].Replace(SingleQuoteSeparator, ' '), out consumerUnitInTradedUnitParseDecimal);
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

        #endregion
    }
}
