using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace BackEnd.CustomClass
{
    public class ComboBoxBehavior
    {

        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static CharacterCasing GetCharacterCasing(ComboBox comboBox)
        {
            return (CharacterCasing)comboBox.GetValue(CharacterCasingProperty);
        }

        public static void SetCharacterCasing(ComboBox comboBox, CharacterCasing value)
        {
            comboBox.SetValue(CharacterCasingProperty, value);
        }

        // Using a DependencyProperty as the backing store for CharacterCasing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CharacterCasingProperty =
            DependencyProperty.RegisterAttached(
                "CharacterCasing",
                typeof(CharacterCasing),
                typeof(ComboBoxBehavior),
                new UIPropertyMetadata(
                    CharacterCasing.Normal,
                    OnCharacterCasingChanged));

        private static void OnCharacterCasingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var comboBox = o as ComboBox;
            if (comboBox == null)
                return;

            if (comboBox.IsLoaded)
            {
                ApplyCharacterCasing(comboBox);
            }
            else
            {
                // To avoid multiple event subscription
                comboBox.Loaded -= new RoutedEventHandler(comboBox_Loaded);
                comboBox.Loaded += new RoutedEventHandler(comboBox_Loaded);
            }
        }

        private static void comboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null)
                return;

            ApplyCharacterCasing(comboBox);
            comboBox.Loaded -= comboBox_Loaded;
        }

        private static void ApplyCharacterCasing(ComboBox comboBox)
        {
            var textBox = comboBox.Template.FindName("PART_EditableTextBox", comboBox) as TextBox;
            if (textBox != null)
            {
                textBox.CharacterCasing = CharacterCasing.Upper;
            }
        }
    }
}
