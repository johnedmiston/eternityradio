﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EternityRadioApp.Styles
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DefaultTheme : ResourceDictionary
    {
        public DefaultTheme()
        {
            InitializeComponent();
        }
    }
}