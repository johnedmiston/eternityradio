using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace EternityRadioApp.Content.ControlGallery.ControlTemplates
{
    public partial class StackLayoutControlTemplate : Frame, IControlTemplate
    {
        public StackLayoutControlTemplate()
        {
            InitializeComponent();

            TargetControl = targetControl;
        }

        public View TargetControl { get; set; }
    }
}
