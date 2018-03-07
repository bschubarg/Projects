using System;
using System.Windows;
using System.Windows.Documents;
using System.Collections.ObjectModel;
using WPFTaskbarNotifier;

namespace WPFTaskbarNotifierExample
{
    /// <summary>
    /// This is just a mock object to hold something of interest. 
    /// </summary>
    public class NotifyObject
    {
        public NotifyObject(string message, string title)
        {
            this.Message = message;
            this.Title = title;
        }

        public string Title { get; set; }

        public string Message { get; set; }
    }

    /// <summary>
    /// This is a TaskbarNotifier that contains a list of NotifyObjects to be displayed.
    /// </summary>
    public partial class LottoTaskbarNotifier : TaskbarNotifier
    {
        public LottoTaskbarNotifier()
        {
            InitializeComponent();
        }

        private ObservableCollection<NotifyObject> _notifyContent;
        /// <summary>
        /// A collection of NotifyObjects that the main window can add to.
        /// </summary>
        public ObservableCollection<NotifyObject> NotifyContent
        {
            get
            {
                if (this._notifyContent == null)
                {
                    // Not yet created.
                    // Create it.
                    this.NotifyContent = new ObservableCollection<NotifyObject>();
                }

                return this._notifyContent;
            }
            set => this._notifyContent = value;
        }

        private void Item_Click(object sender, EventArgs e)
        {
            if(!(sender is Hyperlink hyperlink))
                return;

            if(hyperlink.Tag is NotifyObject notifyObject)
            {
                MessageBox.Show("\"" + notifyObject.Message + "\"" + " clicked!");
            }
        }        
    }
}