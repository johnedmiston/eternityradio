using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xamarin.Forms.PlatformConfiguration;

namespace EternityRadioApp.Services
{
    
        public interface ILogService
        {
            void Initialize(Assembly assembly, string assemblyName, string path, object activity);
        }
    
}
