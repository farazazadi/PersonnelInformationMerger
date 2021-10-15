using System.Collections.Generic;
using PersonnelInformationMerger.Core.Models;

namespace PersonnelInformationMerger.Core.Providers.EventArgs
{
    public class PersonnelListChangedEventArgs : System.EventArgs
    {
        public List<PersonStandardModel> PersonnelList { get; set; }
        public List<PersonStandardModel> PersonnelListOld { get; set; }
    }
}