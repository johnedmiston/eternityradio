﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EternityRadioApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmptyCell : ViewCell
    {
        public EmptyCell()
        {
            InitializeComponent();
            Height = Application.Current.MainPage.Height;
        }
    }

}