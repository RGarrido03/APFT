﻿using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using Windows.Storage;

namespace APFT.Entities;

public class Employee
{
    public int Nif
    {
        get; private set;
    }

    public string FirstName
    {
        get; private set;
    }

    public string LastName
    {
        get; private set;
    }

    public string Email
    {
        get; private set;
    }

    public int Phone
    {
        get; private set;
    }

    public string? Address
    {
        get; private set;
    }

    public string? Gender
    {
        get; private set;
    }

    public DateTime? BirthDate
    {
        get; private set;
    }

    public decimal Salary
    {
        get; private set;
    }

    public int DepartmentId
    {
        get; private set;
    }

    public string Name => FirstName + " " + LastName;

    public Employee(int nif, string firstName, string lastName, string email, int phone, string address, string gender, DateTime birthDate, decimal salary, int departmentId)
    {
        Nif = nif;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Address = address;
        Gender = gender;
        BirthDate = birthDate;
        Salary = salary;
        DepartmentId = departmentId;
    }

    public static async Task<ObservableCollection<GroupInfoList>> GetEmployeesGroupedAsync()
    {
        // Grab Contact objects from pre-existing list (list is returned from function GetContactsAsync())
        var query = from item in await GetEmployeesAsync()

                    // Group the items returned from the query, sort and select the ones you want to keep
                    group item by item.FirstName[..1].ToUpper() into g
                    orderby g.Key

                    // GroupInfoList is a simple custom class that has an IEnumerable type attribute, and
                    // a key attribute. The IGrouping-typed variable g now holds the Contact objects,
                    // and these objects will be used to create a new GroupInfoList object.
                    select new GroupInfoList(g) { Key = g.Key };

        return new ObservableCollection<GroupInfoList>(query);
    }

    public static async Task<ObservableCollection<Employee>> GetEmployeesAsync()
    {
        var employees = new ObservableCollection<Employee>();
        var localSettings = ApplicationData.Current.LocalSettings;

        await using var cn = new SqlConnection(localSettings.Values["SQLConnectionString"].ToString());
        
        await cn.OpenAsync();
        var cmd = new SqlCommand("SELECT * FROM EMPRESA_CONSTRUCAO.EMPREGADO", cn);

        var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            employees.Add(new Employee(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetInt32(4),
                await reader.IsDBNullAsync(5) ? "" : reader.GetString(5),
                await reader.IsDBNullAsync(6) ? "" : reader.GetString(6),
                await reader.IsDBNullAsync(7) ? new DateTime(1970, 1, 1) : reader.GetDateTime(7),
                reader.GetDecimal(8),
                reader.GetInt32(9))
            );
        }

        return employees;
    }
}