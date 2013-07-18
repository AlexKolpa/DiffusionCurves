using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DiffusionCurves.Views
{
    /// <summary>
    /// Class for NumericTextBox.
    /// </summary>
    public class NumericTextBox : TextBox
    {
       /// <summary>
       /// Constructor for NumericTextBox.
       /// </summary>
        public NumericTextBox()
            : base()
        {
            DataObject.AddPastingHandler(this, new DataObjectPastingEventHandler(CheckPasteFormat));
        }

        /// <summary>
        /// Checks the format of input.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private Boolean CheckFormat(string text)
        {
            double val;            
            return Double.TryParse(text, out val);
        }

        /// <summary>
        /// Gets input from user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckPasteFormat(object sender, DataObjectPastingEventArgs e)
        {
            var isText = e.SourceDataObject.GetDataPresent(System.Windows.DataFormats.Text, true);
            if (isText)
            {
                var text = e.SourceDataObject.GetData(DataFormats.Text) as string;
                if (CheckFormat(text))
                {
                    return;
                }
            }
            e.CancelCommand();
        }

        /// <summary>
        /// Handles the preview.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewTextInput(System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!CheckFormat(e.Text))
            {
                e.Handled = true;
            }
            else
            {
                base.OnPreviewTextInput(e);
            }
        }
    }
}
