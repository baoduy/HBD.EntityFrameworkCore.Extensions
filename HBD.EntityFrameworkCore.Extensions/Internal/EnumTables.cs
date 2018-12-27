﻿using HBD.EntityFrameworkCore.Extensions.Abstractions;

namespace HBD.EntityFrameworkCore.Extensions.Internal
{
    internal sealed class EnumTables<T> : IEntity<int>
    {
        #region Public Properties

        public int Id { get; private set; }

        #endregion Public Properties
    }
}