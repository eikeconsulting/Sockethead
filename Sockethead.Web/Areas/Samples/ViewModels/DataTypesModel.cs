﻿using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Sockethead.Web.Areas.Samples.ViewModels
{
    public class DataTypesModel
    {
        [HiddenInput]
        public int HiddenId { get; set; } = 123;
        
        [DataType(DataType.Text)]
        public string TextBox { get; set; } = "Single line of text";

        [DataType(DataType.MultilineText)]
        public string TextArea { get; set; } = "Multiple lines of text\nLine two\nLine three";
        
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = "noreply@gmail.com";

        [DataType(DataType.Password)]
        public string Password { get; set; } = "password";

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; } = "555-555-5555";
        
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        
        [DataType(DataType.DateTime)]
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
        
        [DataType(DataType.Time)]
        public DateTime Time { get; set; } = DateTime.UtcNow;

        [DataType(DataType.Currency)]
        public decimal Currency { get; set; } = 123.45m;

        [DataType(DataType.Upload)]
        public string Upload { get; set; } = "Upload";
        
        [DataType(DataType.Html)]
        public string Html { get; set; } = "<p>Some HTML</p>";

        [DataType(DataType.Url)]
        public string Url { get; set; } = "https://www.google.com";

        [DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; } = "https://via.placeholder.com/150";
        
        [DataType(DataType.CreditCard)]
        public string CreditCard { get; set; } = "1234-5678-9012-3456";
        
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; } = "12345";

        public Gender GenderEnum { get; set; } = Gender.Unknown;

        public bool Checkbox { get; set; } = true;
        public bool Checkbox2 { get; set; }
        
        public DateTime DateTimeWithoutDataType { get; set; } = DateTime.UtcNow;
        
        public short Short { get; set; } = 123;
        public int Int { get; set; } = 123;
        public float Float { get; set; } = 123.456f;
        public double Double { get; set; } = 123.456d;
        public decimal Decimal { get; set; } = 123.45m;
    }
}