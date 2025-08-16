using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FintrakBanking.ViewModels.ThridPartyIntegration
{
    public class FCUBSCLServiceViewModel
    {
        // using System.Xml.Serialization;
        // XmlSerializer serializer = new XmlSerializer(typeof(Envelope));
        // using (StringReader reader = new StringReader(xml))
        // {
        //    var test = (Envelope)serializer.Deserialize(reader);
        // }

        [XmlRoot(ElementName = "FCUBS_HEADER")]
        public class FCUBSHEADER
        {

            [XmlElement(ElementName = "SOURCE")]
            public string SOURCE { get; set; }

            [XmlElement(ElementName = "UBSCOMP")]
            public string UBSCOMP { get; set; }

            [XmlElement(ElementName = "MSGID")]
            public object MSGID { get; set; }

            [XmlElement(ElementName = "CORRELID")]
            public object CORRELID { get; set; }

            [XmlElement(ElementName = "USERID")]
            public string USERID { get; set; }

            [XmlElement(ElementName = "BRANCH")]
            public int BRANCH { get; set; }

            [XmlElement(ElementName = "SERVICE")]
            public string SERVICE { get; set; }

            [XmlElement(ElementName = "OPERATION")]
            public string OPERATION { get; set; }
        }

        [XmlRoot(ElementName = "Ude-Vals")]
        public class UdeVals
        {

            [XmlElement(ElementName = "UDEID")]
            public string UDEID { get; set; }

            [XmlElement(ElementName = "UDEVAL")]
            public int UDEVAL { get; set; }

            [XmlElement(ElementName = "RATECODE")]
            public string RATECODE { get; set; }

            [XmlElement(ElementName = "CODEUSAGE")]
            public string CODEUSAGE { get; set; }

            [XmlElement(ElementName = "RESOLVEDVAL")]
            public int RESOLVEDVAL { get; set; }
        }

        [XmlRoot(ElementName = "Effec-Date")]
        public class EffecDate
        {

            [XmlElement(ElementName = "EFFDT")]
            public DateTime EFFDT { get; set; }

            [XmlElement(ElementName = "UdeVals")]
            public List<UdeVals> UdeVals { get; set; }
        }

        [XmlRoot(ElementName = "Misdetails")]
        public class Misdetails
        {

            [XmlElement(ElementName = "CONREFNO")]
            public object CONREFNO { get; set; }

            [XmlElement(ElementName = "COMPMIS1")]
            public string COMPMIS1 { get; set; }

            [XmlElement(ElementName = "COMPMIS10")]
            public string COMPMIS10 { get; set; }

            [XmlElement(ElementName = "COMPMIS2")]
            public string COMPMIS2 { get; set; }

            [XmlElement(ElementName = "COMPMIS3")]
            public string COMPMIS3 { get; set; }

            [XmlElement(ElementName = "COMPMIS4")]
            public string COMPMIS4 { get; set; }

            [XmlElement(ElementName = "COMPMIS5")]
            public string COMPMIS5 { get; set; }

            [XmlElement(ElementName = "COMMIS1")]
            public string COMMIS1 { get; set; }

            [XmlElement(ElementName = "COMMIS2")]
            public string COMMIS2 { get; set; }

            [XmlElement(ElementName = "COMMIS3")]
            public string COMMIS3 { get; set; }

            [XmlElement(ElementName = "COMMIS4")]
            public string COMMIS4 { get; set; }

            [XmlElement(ElementName = "COMMIS5")]
            public string COMMIS5 { get; set; }

            [XmlElement(ElementName = "COMMIS7")]
            public string COMMIS7 { get; set; }

            [XmlElement(ElementName = "COMMIS8")]
            public string COMMIS8 { get; set; }

            [XmlElement(ElementName = "COMMIS9")]
            public string COMMIS9 { get; set; }

            [XmlElement(ElementName = "COMMIS10")]
            public string COMMIS10 { get; set; }
        }

        [XmlRoot(ElementName = "Account-Master-Full")]
        public class AccountMasterFull
        {

            [XmlElement(ElementName = "ACCNO")]
            public object ACCNO { get; set; }

            [XmlElement(ElementName = "BRN")]
            public int BRN { get; set; }

            [XmlElement(ElementName = "APPLNO")]
            public string APPLNO { get; set; }

            [XmlElement(ElementName = "PROD")]
            public string PROD { get; set; }

            [XmlElement(ElementName = "CUSTID")]
            public int CUSTID { get; set; }

            [XmlElement(ElementName = "BOOKDT")]
            public DateTime BOOKDT { get; set; }

            [XmlElement(ElementName = "VALDT")]
            public DateTime VALDT { get; set; }

            [XmlElement(ElementName = "CCY")]
            public string CCY { get; set; }

            [XmlElement(ElementName = "AMTFINANCED")]
            public int AMTFINANCED { get; set; }

            [XmlElement(ElementName = "DRPRODAC")]
            public double DRPRODAC { get; set; }

            [XmlElement(ElementName = "DRACCBRN")]
            public int DRACCBRN { get; set; }

            [XmlElement(ElementName = "Effec-Date")]
            public EffecDate EffecDate { get; set; }

            [XmlElement(ElementName = "Misdetails")]
            public Misdetails Misdetails { get; set; }
        }

        [XmlRoot(ElementName = "FCUBS_BODY")]
        public class FCUBSBODY
        {

            [XmlElement(ElementName = "Account-Master-Full")]
            public AccountMasterFull AccountMasterFull { get; set; }
        }

        [XmlRoot(ElementName = "CREATEACCOUNT_FSFS_REQ")]
        public class CREATEACCOUNTFSFSREQ
        {

            [XmlElement(ElementName = "FCUBS_HEADER")]
            public FCUBSHEADER FCUBSHEADER { get; set; }

            [XmlElement(ElementName = "FCUBS_BODY")]
            public FCUBSBODY FCUBSBODY { get; set; }
        }

        [XmlRoot(ElementName = "Body")]
        public class Body
        {

            [XmlElement(ElementName = "CREATEACCOUNT_FSFS_REQ")]
            public CREATEACCOUNTFSFSREQ CREATEACCOUNTFSFSREQ { get; set; }
        }

        [XmlRoot(ElementName = "Envelope")]
        public class Envelope
        {

            [XmlElement(ElementName = "Header")]
            public object Header { get; set; }

            [XmlElement(ElementName = "Body")]
            public Body Body { get; set; }

            [XmlAttribute(AttributeName = "soapenv")]
            public string Soapenv { get; set; }

            [XmlAttribute(AttributeName = "fcub")]
            public string Fcub { get; set; }

            [XmlText]
            public string Text { get; set; }
        }


    }
}
