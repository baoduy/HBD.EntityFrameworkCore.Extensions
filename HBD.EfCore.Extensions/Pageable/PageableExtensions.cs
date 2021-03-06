﻿using HBD.EfCore.Extensions.Pageable;
using HBD.EfCore.Extensions.Specification;
using HBD.EfCore.Extensions.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    public static class PageableExtensions
    {
        #region Methods

        public static IPageable<TEntity> ToPageable<TEntity>(this IOrderedQueryable<TEntity> query, int pageIndex,
            int pageSize)
        {
            Validate(pageIndex, pageSize);
            var itemIndex = pageIndex * pageSize;

            var totalItems = query.Count();

            if (itemIndex >= totalItems) itemIndex = totalItems - pageSize; //Get last page.

            var items = pageSize >= totalItems ? query : query.Skip(itemIndex).Take(pageSize);

            return new Pageable<TEntity>(pageIndex, pageSize, totalItems, items.ToList());
        }

        public static IPageable<TEntity> ToPageable<TEntity>(this IQueryable<TEntity> query, PageableSpec<TEntity> spec) where TEntity : class
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            Validate(spec.PageIndex, spec.PageSize);
            var itemIndex = spec.PageIndex * spec.PageSize;

            var oQuery = query.ForPageableSpec(spec);

            //Catch to improve the performance
            var totalItems = oQuery.Count();

            if (itemIndex >= totalItems) itemIndex = totalItems - spec.PageSize; //Get last page.

            var items = spec.PageSize >= totalItems ? oQuery : oQuery.Skip(itemIndex).Take(spec.PageSize);

            return new Pageable<TEntity>(spec.PageIndex, spec.PageSize, totalItems, items.ToList());
        }

        public static async Task<IPageable<TEntity>> ToPageableAsync<TEntity>(this IOrderedQueryable<TEntity> query,
            int pageIndex, int pageSize)
        {
            Validate(pageIndex, pageSize);
            var itemIndex = pageIndex * pageSize;

            //Catch to improve the performance
            var totalItems = await query.CountAsync().ConfigureAwait(false);

            if (itemIndex < 0) itemIndex = 0; //Get first Page
            if (itemIndex >= totalItems) itemIndex = totalItems - pageSize; //Get last page.

            var items = pageSize >= totalItems ? query : query.Skip(itemIndex).Take(pageSize);

            return new Pageable<TEntity>(pageIndex, pageSize, totalItems, await items.ToListAsync().ConfigureAwait(false));
        }

        public static async Task<IPageable<TEntity>> ToPageableAsync<TEntity>(this IQueryable<TEntity> query, PageableSpec<TEntity> spec) where TEntity : class
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            Validate(spec.PageIndex, spec.PageSize);

            var oQuery = query.ForPageableSpec(spec);

            //Catch to improve the performance
            var totalItems = await oQuery.CountAsync().ConfigureAwait(false);

            var itemIndex = spec.PageIndex * spec.PageSize;
            if (itemIndex >= totalItems) itemIndex = totalItems - spec.PageSize; //Get last page.

            var items = spec.PageSize >= totalItems ? oQuery : oQuery.Skip(itemIndex).Take(spec.PageSize);

            return new Pageable<TEntity>(spec.PageIndex, spec.PageSize, totalItems, await items.ToListAsync().ConfigureAwait(false));
        }

        private static IOrderedQueryable<TEntity> ForPageableSpec<TEntity>(this IQueryable<TEntity> query, PageableSpec<TEntity> spec) where TEntity : class
        {
            var oQuery = query.Includes(spec.InternalSpec).Wheres(spec);
            return spec.OrderDirection == OrderingDirection.Asc ? oQuery.OrderBy(spec.OrderBy) : oQuery.OrderByDescending(spec.OrderBy);
        }

        private static void Validate(int pageIndex, int pageSize)
        {
            if (pageIndex < 0) throw new ArgumentException($"{nameof(pageIndex)} should be >= 0");
            if (pageSize <= 0) throw new ArgumentException($"{nameof(pageSize)} should be > 0");
        }

        #endregion Methods
    }
}