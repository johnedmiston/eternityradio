﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EternityRadioApp.Controls
{
    public class PdfJsWebView : WebView
    {
        public static readonly BindableProperty UriProperty =
           BindableProperty.Create(nameof(Uri), typeof(string),
               typeof(PdfJsWebView), default(string));

        public string Uri
        {
            get => (string)GetValue(UriProperty);
            set => SetValue(UriProperty, value);
        }
    }
}
