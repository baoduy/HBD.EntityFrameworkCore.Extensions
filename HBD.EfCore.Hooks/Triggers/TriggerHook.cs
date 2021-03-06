﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HBD.EfCore.Hooks.Triggers
{
    public sealed class TriggerHook : Hook
    {
        #region Properties

        public static ITriggerContext Context { get; private set; }

        /// <summary>
        ///     Disable TriggerHook
        /// </summary>
        public static bool Disabled { get; set; }

        #endregion Properties

        #region Methods

        public override Task OnSaved(IReadOnlyCollection<TriggerEntityState> entities, DbContext dbContext,
            CancellationToken cancellationToken = default) =>
            Disabled ? Task.CompletedTask : ExecuteAsync(entities.Where(e => e.State != EntityState.Unchanged).ToList());

        internal static IServiceCollection Initialize(IServiceCollection serviceCollection)
        {
            if (Context != null) return serviceCollection;

            Context = new TriggerContext(serviceCollection);
            return serviceCollection;
        }

        internal static bool IsInitialized() => Context != null;

        private static async Task ExecuteAsync(IReadOnlyCollection<TriggerEntityState> entities)
        {
            if (entities.Count <= 0) return;

            if (Context == null)
                throw new InvalidOperationException(nameof(TriggerContext));

            var groups = from e in entities
                         group e by e.EntityType into g
                         select new { EntityType = g.Key, Entries = g.ToList() };

            var profiles = Context.ServiceProvider.GetServices<ITriggerProfile>().ToList();

            if (!profiles.Any()) return;

            var tasks = from g in groups

                            // ReSharper disable once InconsistentlySynchronizedField
                        from p in profiles
                        where g.EntityType == p.EntityType
                              && p.TriggerType != TriggerTypes.None
                        select ExecuteAsync(g.Entries, p, Context);

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private static Task ExecuteAsync(IReadOnlyCollection<TriggerEntityState> entities, ITriggerProfile rule, ITriggerContext context)
        {
            List<dynamic> final;

            if (rule.TriggerType == TriggerTypes.All)
            {
                final = entities.GetTriggerEntity(rule.EntityType).ToList();
            }
            else
            {
                final = new List<dynamic>();

                if (rule.TriggerType.HasFlag(TriggerTypes.Created))
                    final.AddRange(entities.Where(e => e.State == EntityState.Added).GetTriggerEntity(rule.EntityType));
                if (rule.TriggerType.HasFlag(TriggerTypes.Updated))
                    final.AddRange(entities.Where(e => e.State == EntityState.Modified)
                        .GetTriggerEntity(rule.EntityType));
                if (rule.TriggerType.HasFlag(TriggerTypes.Deleted))
                    final.AddRange(
                        entities.Where(e => e.State == EntityState.Deleted).GetTriggerEntity(rule.EntityType));
            }

            return final.Any() ? (Task)((dynamic)rule).Execute(final, context) : Task.CompletedTask;
        }

        #endregion Methods
    }
}