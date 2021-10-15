using System.Collections.Generic;
using PersonnelInformationMerger.Core.Models;

namespace PersonnelInformationMerger.Core.MergeStrategies
{
    public interface IMergeStrategy
    {
        List<PersonStandardModel> Merge(IReadOnlyList<PersonStandardModel> firstPersonnelList,
            IReadOnlyList<PersonStandardModel> secondPersonnelList);
    }
}