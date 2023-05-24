﻿using APFT.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.Resources;

namespace APFT.Views;

public sealed partial class CustomerDetailsPage : INotifyPropertyChanged
{
    public CustomerDetailsViewModel ViewModel
    {
        get;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private int _nif;
    public int Nif
    {
        get => _nif;
        set
        {
            _nif = value;
            OnPropertyChanged(nameof(Nif));
        }
    }

    private string _firstName;
    public string FirstName
    {
        get => _firstName;
        set
        {
            _firstName = value;
            OnPropertyChanged(nameof(FirstName));
        }
    }

    private string _lastName;
    public string LastName
    {
        get => _lastName;
        set
        {
            _lastName = value;
            OnPropertyChanged(nameof(LastName));
        }
    }

    private string _address;
    public string Address
    {
        get => _address;
        set
        {
            _address = value;
            OnPropertyChanged(nameof(Address));
        }
    }

    private string _email;
    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged(nameof(Email));
        }
    }

    private int _phone;
    public int Phone
    {
        get => _phone;
        set
        {
            _phone = value;
            OnPropertyChanged(nameof(Phone));
        }
    }

    public CustomerDetailsPage()
    {
        ViewModel = App.GetService<CustomerDetailsViewModel>();
        FetchData();
        InitializeComponent();
    }

    private async void FetchData()
    {
        var localSettings = ApplicationData.Current.LocalSettings;

        await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());
        await cn.OpenAsync();

        var cmd = new SqlCommand("SELECT * FROM EMPRESA_CONSTRUCAO.CLIENTE WHERE nif=" + localSettings.Values["CustomerNif"], cn);
        var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        Nif = reader.GetInt32(0);
        FirstName = reader.GetString(1);
        LastName = reader.GetString(2);
        Email = reader.GetString(3);
        Phone = reader.GetInt32(4);
        Address = reader.GetString(5);
    }

    private readonly ResourceLoader _resourceLoader = new("resources.pri", "CustomerDetails");

    private async void EditInfoButton_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new ContentDialog
        {
            XamlRoot = ContentArea.XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = _resourceLoader.GetString("NotImplemented_Title"),
            PrimaryButtonText = _resourceLoader.GetString("NotImplemented_Close"),
            DefaultButton = ContentDialogButton.Primary,
            Content = _resourceLoader.GetString("NotImplemented_Content")
        };
        _ = await dialog.ShowAsync();
    }
}