﻿using NuGet.ProjectModel;

namespace AddressBook.Models.ViewModels
{
    public class EmailContactViewModel
    {
        public Contact? Contact { get; set; }
        public EmailData EmailData { get; set; }
    }
}
