﻿using System;
using System.Linq;
using System.Threading.Tasks;
using HBD.EntityFrameworkCore.Extensions.Pageable;
using HBD.EntityFrameworkCore.Extensions.Specification;
using Remotion.Linq.Clauses;
using Z.EntityFramework.Plus;

// ReSharper disable CheckNamespace
namespace Microsoft.EntityFrameworkCore
{
    public static class PageableExtensions
    {
        #region Public Methods

        public static IPageable<TEntity> ToPageable<TEntity>(this IOrderedQueryable<TEntity> query, int pageIndex,
            int pageSize)
        {
            Validate(pageIndex, pageSize);

            //Catch to improve the performance
            var totalItems = query.DeferredCount().FutureValue();

            var itemIndex = pageIndex * pageSize;
            if (itemIndex >= totalItems) itemIndex = totalItems - pageSize; //Get last page.

            var items = pageSize >= totalItems ? query.Future() : query.Skip(itemIndex).Take(pageSize).Future();

            return new Pageable<TEntity>(pageIndex, pageSize, totalItems, items.ToList());
        }

        public static IPageable<TEntity> ToPageable<TEntity>(this IQueryable<TEntity> query, PageableSpec<TEntity> spec) where TEntity : class
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            Validate(spec.PageIndex, spec.PageSize);

            var oQuery = query.ForPageableSpec(spec);

            //Catch to improve the performance
            var totalItems = oQuery.DeferredCount().FutureValue();

            var itemIndex = spec.PageIndex * spec.PageSize;
            if (itemIndex >= totalItems) itemIndex = totalItems - spec.PageSize; //Get last page.

            var items = spec.PageSize >= totalItems ? oQuery.Future() : oQuery.Skip(itemIndex).Take(spec.PageSize).Future();

            return new Pageable<TEntity>(spec.PageIndex, spec.PageSize, totalItems, items.ToList());
        }

        public static async Task<IPageable<TEntity>> ToPageableAsync<TEntity>(this IOrderedQueryable<TEntity> query,
            int pageIndex, int pageSize)
        {
            Validate(pageIndex, pageSize);

            //Catch to improve the performance
            var totalItems = query.DeferredCount().FutureValue();

            var itemIndex = pageIndex * pageSize;
            if (itemIndex < 0) itemIndex = 0; //Get first Page
            if (itemIndex >= totalItems) itemIndex = totalItems - pageSize; //Get last page.

            var items = pageSize >= totalItems ? query.Future() : query.Skip(itemIndex).Take(pageSize).Future();

            return new Pageable<TEntity>(pageIndex, pageSize, totalItems, await items.ToListAsync());
        }

        public static async Task<IPageable<TEntity>> ToPageableAsync<TEntity>(this IQueryable<TEntity> query, PageableSpec<TEntity> spec) where TEntity : class
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            Validate(spec.PageIndex, spec.PageSize);

            var oQuery = query.ForPageableSpec(spec);

            //Catch to improve the performance
            var totalItems = oQuery.DeferredCount().FutureValue();

            var itemIndex = spec.PageIndex * spec.PageSize;
            if (itemIndex >= totalItems) itemIndex = totalItems - spec.PageSize; //Get last page.

            var items = spec.PageSize >= totalItems ? oQuery.Future() : oQuery.Skip(itemIndex).Take(spec.PageSize).Future();

            return new Pageable<TEntity>(spec.PageIndex, spec.PageSize, totalItems, await items.ToListAsync());
        }

        #endregion Public Methods

        #region Private Methods

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

        #endregion Private Methods
    }
}