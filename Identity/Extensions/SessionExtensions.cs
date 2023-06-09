﻿using Identity.Models;
using Identity.ViewModels;

namespace Identity.Extensions
{
    public static class SessionExtensions
    {
        public static List<SessionViewModel> MapToView(this IEnumerable<Session> sessions, Guid? currentSessionId = null)
        {
            return sessions.Select(session => MapToView(session, currentSessionId)).ToList();
        }

        public static SessionViewModel MapToView(this Session session, Guid? currentSessionId = null)
        {
            if (session == null)
                return null;
            return new SessionViewModel
            {
                Id = session.Id,
                CreatedAt = session.CreatedAt,
                IsActive = session.IsActive,
                Status = session.Status,
                Current = currentSessionId.HasValue ? session.Id == currentSessionId : false,
                Language = session.Language,
                App = session.App,
                Location = session.Location,
                Device = session.Device,
                DeactivatedAt = session.DeactivatedAt
            };
        }

        public static IEnumerable<Guid> MapSessionIds(this IEnumerable<Session> sessions)
        {
            return sessions.Select(s => s.Id);
        }
    }
}