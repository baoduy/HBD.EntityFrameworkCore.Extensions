﻿using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using HBD.EntityFrameworkCore.Extensions.Abstractions;

[assembly: InternalsVisibleTo("HBD.EntityFrameworkCore.Extensions.Tests")]

namespace DataLayer
{
    public class Account : AuditEntity
    {
        //public virtual User User { get; set; }

        #region Public Properties

        [Required] public string Password { get; set; }
        [Required] public string UserName { get; set; }

        #endregion Public Properties
    }
}