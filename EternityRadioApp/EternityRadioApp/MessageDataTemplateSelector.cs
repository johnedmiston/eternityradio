using EternityRadioApp.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EternityRadioApp
{

        public class MessageDataTemplateSelector : DataTemplateSelector
        {
            public DataTemplate MessageTemplate = new DataTemplate(typeof(MessageCell));
            public DataTemplate EmptyTemplate = new DataTemplate(typeof(EmptyCell));

            protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
            {
                return ((Media)item) == item ? EmptyTemplate: MessageTemplate;
            }
        }
    }