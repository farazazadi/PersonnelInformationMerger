using System.Collections.Generic;
using PersonnelInformationMerger.Core.Models;

namespace PersonnelInformationMerger.Core.SavingStrategies
{
    public interface ISavingStrategy
    {
        void Save(List<PersonStandardModel> personnelList);
    }
}