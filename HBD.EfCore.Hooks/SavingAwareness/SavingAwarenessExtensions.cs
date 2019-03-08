﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace HBD.EfCore.Hooks.SavingAwareness
{
    public static class SavingAwarenessExtensions
    {
        #region Public Methods

        /// <summary>
        /// Get Property Name of expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        private static string GetPropName<T, TProp>(this Expression<Func<T, TProp>> action) where T : class
        {
            var expression = GetMemberInfo(action);
            return expression.Member.Name;
        }

        /// <summary>
        /// Check whether the property value had been Add or Updated
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="props"></param>
        /// <returns></returns>
        public static bool HasChangeOn<T>(this EntityEntry<T> @this, Expression<Func<T, object>> props) where T : class
        {
            var name = props.GetPropName();
            var prop = @this.Properties.FirstOrDefault(i => i.Metadata.Name == name);

            if (prop != null) return prop.IsModified || @this.State == EntityState.Added;

            var navigation = @this.Navigations.FirstOrDefault(n => n.Metadata.Name == name);

            if (navigation == null) return @this.State == EntityState.Added;

            if (navigation.EntityEntry.State == EntityState.Added)
                return true;

            return navigation.IsLoaded && navigation.IsModified;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Get Expression member
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static MemberExpression GetMemberInfo(Expression method)
        {
            if (!(method is LambdaExpression lambda))
                throw new ArgumentNullException(nameof(method));

            MemberExpression memberExpr;

            switch (lambda.Body.NodeType)
            {
                case ExpressionType.Convert:
                    memberExpr =
                        ((UnaryExpression)lambda.Body).Operand as MemberExpression;
                    break;

                case ExpressionType.MemberAccess:
                    memberExpr = lambda.Body as MemberExpression;
                    break;

                default: throw new NotSupportedException(lambda.Body.NodeType.ToString());
            }

            if (memberExpr == null)
                throw new ArgumentException(nameof(method));

            return memberExpr;
        }

        #endregion Private Methods
    }
}