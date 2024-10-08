﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace EternityRadioApp.Controls
{
    public class ExtendedListView : ListView, IDisposable
    {
        public static readonly BindableProperty LoadMoreCommandProperty =
                    BindableProperty.Create(nameof(LoadMoreCommand), typeof(ICommand), typeof(ExtendedListView), default(ICommand));

        public ICommand LoadMoreCommand
        {
            get { return (ICommand)GetValue(LoadMoreCommandProperty); }
            set { SetValue(LoadMoreCommandProperty, value); }
        }

        public ExtendedListView() : this(ListViewCachingStrategy.RetainElement)
        {
        }

        public ExtendedListView(ListViewCachingStrategy cachingStrategy) : base(cachingStrategy)
        {
            ItemAppearing += OnItemAppearing;
        }

        private void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (LoadMoreCommand != null)
            {
                if (e.Item == ItemsSource.Cast<object>().Last())
                {
                    LoadMoreCommand?.Execute(null);
                }
            }
        }

        public void Dispose()
        {
            ItemAppearing -= OnItemAppearing;
        }
    }
}