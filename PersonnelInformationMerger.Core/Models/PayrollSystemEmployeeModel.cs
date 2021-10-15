using System.Collections.Generic;
using System.Xml.Serialization;

namespace PersonnelInformationMerger.Core.Models
{
    [XmlRoot(ElementName = "personnel")]
    internal class PayrollSystemEmployeeModel
    {
        [XmlElement(ElementName = "person")] public List<PersonModel> PersonnelList { get; set; } = new();

        public class PersonModel
        {
            [XmlAttribute("number")]
            public string EmployeeNumber { get; set; }


            [XmlElement(ElementName = "title")]
            public string Title { get; set; }


            [XmlElement(ElementName = "name")]
            public string Name { get; set; }


            [XmlElement(ElementName = "email")]
            public string Email { get; set; }


            [XmlElement(ElementName = "mobile")]
            public string Mobile { get; set; }


            [XmlElement(ElementName = "address")]
            public string Address { get; set; }


            [XmlElement(ElementName = "city")]
            public string City { get; set; }
        }
    }
}