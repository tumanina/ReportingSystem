﻿using System;
using System.Collections.Generic;

namespace ReportingSystem.Shared.Models
{
    public class AccountModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public IEnumerable<ActionModel> Actions { get; set; }
    }
}