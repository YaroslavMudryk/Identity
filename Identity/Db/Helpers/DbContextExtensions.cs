﻿using Identity.Extensions;
using Identity.Helpers;
using Identity.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Identity.Db.Helpers
{
    public static class DbContextExtensions
    {
        public static void ApplyAuditInfo(this DbContext dbContext, IIdentityService identityService)
        {
            var now = DateTime.Now;

            ApplyUpsertInfo(dbContext, now, identityService);
            ApplyDeleteInfo(dbContext, now, identityService);
        }

        private static void ApplyUpsertInfo(DbContext dbContext, DateTime now, IIdentityService identityService)
        {
            var entries = dbContext.ChangeTracker.Entries().Where(s => s.Entity is IAuditEntity && (s.State == EntityState.Added || s.State == EntityState.Modified));

            entries.ForEach(entry =>
            {
                var entity = (IAuditEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = now;
                    entity.CreatedBy = identityService != null ? identityService.GetUserId() : DefaultsAudit.CreatedBy;
                    entity.CreatedIP = identityService != null ? identityService.GetIP() : DefaultsAudit.CreatedFromIP;
                }

                if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = now;
                    entity.UpdatedBy = identityService != null ? identityService.GetUserId() : DefaultsAudit.CreatedBy;
                    entity.UpdatedIP = identityService != null ? identityService.GetIP() : DefaultsAudit.CreatedFromIP;
                }
                entity.Version++;
            });
        }

        private static void ApplyDeleteInfo(DbContext dbContext, DateTime now, IIdentityService identityService)
        {
            var entries = dbContext.ChangeTracker.Entries().Where(s => s.Entity is ISoftDeletableEntity && (s.State == EntityState.Deleted));

            entries.ForEach(entry =>
            {
                var entity = (ISoftDeletableEntity)entry.Entity;

                entity.DeletedAt = now;
                entity.IsDeleted = true;
                entity.DeletedBy = identityService != null ? identityService.GetUserId() : DefaultsAudit.CreatedBy;
                entry.State = EntityState.Modified;
            });
        }
    }
}
