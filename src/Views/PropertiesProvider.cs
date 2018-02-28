using System.Collections.Generic;
using Xamarin.Forms;

namespace Flashcards.Views
{
    class PropertiesProvider : SpacedRepetition.Leitner.Algorithm.IProperties
    {
        public object Get(string key)
        {
            return Application.Current.Properties[key];
        }

        public void Set(string key, object value)
        {
            Application.Current.Properties[key] = value;
        }

        public bool ContainsKey(string key)
        {
            return Application.Current.Properties.ContainsKey(key);
        }
    }
}