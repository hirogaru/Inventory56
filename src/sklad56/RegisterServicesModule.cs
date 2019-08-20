using System;
using EnCore;
using sklad56.Models;

namespace sklad56
{
    public class RegisterServicesModule : Module
    {
        public override void RegisterServices(ICompositionBatch batch)
        {
            batch.Register<IRepository>(() => new SqlRepository());
        }
    }
}