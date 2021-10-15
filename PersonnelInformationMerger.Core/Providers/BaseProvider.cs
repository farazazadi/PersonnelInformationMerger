using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using PersonnelInformationMerger.Core.Models;
using PersonnelInformationMerger.Core.Providers.EventArgs;

namespace PersonnelInformationMerger.Core.Providers
{
    public abstract class BaseProvider
    {
        protected readonly List<PersonStandardModel> Personnel = new();
        public virtual IReadOnlyList<PersonStandardModel> PersonnelList => Personnel;
        private Timer Timer { get; }


        public event EventHandler<PersonnelListChangedEventArgs> PersonnelListChanged;



        protected BaseProvider()
        {

        }

        protected BaseProvider(double autoCheckInterval)
        {
            Timer = new Timer();

            if (autoCheckInterval > 0)
            {
                Timer.Enabled = true;
                Timer.Interval = autoCheckInterval;
            }

            Timer.Elapsed += TimerOnElapsed;
            Timer.AutoReset = true;

        }



        public abstract Task Process();

        protected void DisableAutoCheckTimer() => Timer.Enabled = false;
        protected void EnableAutoCheckTimer() => Timer.Enabled = true;


        protected virtual void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            
        }


        protected virtual void OnPersonnelListChanged(List<PersonStandardModel> personnelList, List<PersonStandardModel> personnelListOld)
        {
            var personStateChangedEventArgs = new PersonnelListChangedEventArgs
            {
                PersonnelList = personnelList,
                PersonnelListOld = personnelListOld
            };
            
            PersonnelListChanged?.Invoke(this, personStateChangedEventArgs);
        }
        
    }
}