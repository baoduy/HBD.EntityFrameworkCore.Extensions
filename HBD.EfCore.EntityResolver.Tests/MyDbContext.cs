﻿using Microsoft.EntityFrameworkCore;


namespace HBD.EfCore.EntityResolver.Tests
{
    public class MyDbContext : DbContext
    {
        #region Public Constructors

        public MyDbContext(DbContextOptions options) : base(options)
        {
        }

        #endregion Public Constructors
    }
}