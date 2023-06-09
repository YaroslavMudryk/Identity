﻿using Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XMessenger.Identity.Db.Configurations
{
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.HasKey(x => x.Id);

            builder.OwnsOne(field => field.App, builder =>
            {
                builder.ToJson();
            });

            builder.OwnsOne(field => field.Location, builder =>
            {
                builder.ToJson();
            });

            builder.OwnsOne(field => field.Device, builder =>
            {
                builder.ToJson();

                builder.OwnsOne(obj => obj.Device);
                builder.OwnsOne(obj => obj.Browser);
                builder.OwnsOne(obj => obj.OS);
            });
        }
    }
}