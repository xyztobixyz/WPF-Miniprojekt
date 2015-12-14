using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ch.hsr.wpf.gadgeothek.domain;
using ch.hsr.wpf.gadgeothek.service;
using ch.hsr.wpf.gadgeothek.websocket;

namespace ch.hsr.wpf.gadgeothek.admin_gui
{
    
    public partial class MainWindow : Window
    {
        private Gadget EditingGadget;

        private WebSocketClient ClientListener { get; set; }

        public LibraryAdminService Service { get; }
        public string ServerUrl { get; }

        public ObservableCollection<Gadget> Gadgets { get; set; }
        public ObservableCollection<Loan> Loans { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            ServerUrl = "http://localhost:8080";
            Service = new LibraryAdminService(ServerUrl);
            ClientListener = new WebSocketClient(ServerUrl);
            ClientListener.NotificationReceived += ClientListenerNotified;
            ClientListener.ListenAsync();

            Gadgets = new ObservableCollection<Gadget>(Service.GetAllGadgets());
            Loans = new ObservableCollection<Loan>(Service.GetAllLoans());
            
            InputComboCondition.ItemsSource = Enum.GetValues(typeof(domain.Condition)).Cast<domain.Condition>();
        }

        private void ClientListenerNotified(object o, WebSocketClientNotificationEventArgs e)
        {
            Gadgets.Clear();
            Loans.Clear();
            foreach (var gadget in Service.GetAllGadgets()) { Gadgets.Add(gadget); }
            foreach (var loan in Service.GetAllLoans()) { Loans.Add(loan); }
        }

        private void NewGadgetButton_OnClick(object sender, RoutedEventArgs e)
        {
            InputError.Content = "";
            try
            {
                var gadget = new Gadget("");
                gadget.Condition = (domain.Condition) InputComboCondition.SelectionBoxItem;
                gadget.Manufacturer = InputGadgetName.Text;
                gadget.Price = Double.Parse(InputGadgetPrice.Text);
                gadget.Name = InputGadgetName.Text;
                Service.AddGadget(gadget);
                InputError.Content = "";
            }
            catch (InvalidCastException)
            {
                InputError.Content += "Condition wählen\n";
            }
            catch (FormatException)
            {
                InputError.Content += "Price als Nummer angeben\n";
            }
            catch (Exception)
            {
                InputError.Content += "Bitte alle Felder ausfüllen";
            }

        }

        private void DeleteGadget_OnClick(object sender, RoutedEventArgs e)
        {
            var gadget = GadgetGrid.SelectedItem as Gadget;
            if (gadget != null)
            {
                DeleteSelectedGadget(gadget);
            }
        }

        private void GadgetGrid_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            EditingGadget = e.Row.Item as Gadget;
        }



        public void DeleteSelectedGadget(Gadget gadget)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Service.DeleteGadget(gadget);
            }
        }

        private void GadgetGrid_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var gadget = GadgetGrid.SelectedItem as Gadget;
                if (gadget != null)
                {
                    DeleteSelectedGadget(gadget);
                }
            }
        }

        private void GadgetGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (EditingGadget != null)
            {
                Service.UpdateGadget(EditingGadget);
                EditingGadget = null;
            }
        }
    }
}
