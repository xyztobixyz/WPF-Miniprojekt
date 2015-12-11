using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ch.hsr.wpf.gadgeothek.domain;
using ch.hsr.wpf.gadgeothek.service;
using ch.hsr.wpf.gadgeothek.websocket;
using System.Threading.Tasks;

namespace ch.hsr.wpf.gadgeothek.admin_gui
{
    
    public partial class MainWindow : Window
    {
        private WebSocketClient ClientListener { get; set; }

        public LibraryAdminService Service { get; }
        public string ServerUrl { get; }

        public ObservableCollection<Gadget> Gadgets { get; set; }
        public ObservableCollection<Loan> Loans { get; set; }

        public MainWindow()
        {
            DataContext = this;

            ServerUrl = "http://localhost:8080";
            Service = new LibraryAdminService(ServerUrl);
            ClientListener = new WebSocketClient(ServerUrl);

            Gadgets = new ObservableCollection<Gadget>(Service.GetAllGadgets());
            Loans = new ObservableCollection<Loan>(Service.GetAllLoans());

            ClientListener.NotificationReceived += (o, e) =>
            {
                Gadgets.Clear();
                Loans.Clear();
                foreach (var gadget in Service.GetAllGadgets()){ Gadgets.Add(gadget); }
                foreach (var loan in Service.GetAllLoans()) { Loans.Add(loan); }
            };

            InitializeComponent();
            
            GadgetGrid.DataContext = Gadgets;
            LoanGrid.DataContext = Loans;

            InputComboCondition.ItemsSource = Enum.GetValues(typeof(domain.Condition)).Cast<domain.Condition>();

            var bgTask = ClientListener.ListenAsync();
        }


        private void NewGadgetButton_OnClick(object sender, RoutedEventArgs e)
        {
            var gadget = new Gadget();
            gadget.Condition = (domain.Condition) InputComboCondition.SelectionBoxItem;
            gadget.Manufacturer = InputGadgetName.Text;
            gadget.Price = Double.Parse(InputGadgetPrice.Text);
            gadget.Name = InputGadgetName.Text;
            Service.AddGadget(gadget);
        }

        private void DeleteGadget_OnClick(object sender, RoutedEventArgs e)
        {
            var gadget = (Gadget) GadgetGrid.SelectedItem;
            DeleteSelectedGadget(gadget);
        }

        private void GadgetGrid_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (GadgetGrid.SelectedItem != null)
            {
                var gadget = (Gadget) GadgetGrid.SelectedItem;
                Service.UpdateGadget(gadget);
            }
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
            if (GadgetGrid.SelectedItem != null)
            {
                var gadget = (Gadget)GadgetGrid.SelectedItem;
                DeleteSelectedGadget(gadget);
            }
        }
    }
}
