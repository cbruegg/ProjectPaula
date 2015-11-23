﻿using Newtonsoft.Json;
using System;
using System.Linq;

namespace ProjectPaula.ViewModel
{
    /// <summary>
    /// Represents a connected client identified by a SignalR connection ID.
    /// Stores the current user name and the schedule the client is working on.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class PaulaClientViewModel
    {
        private ScheduleManager _scheduleManager;

        public string ConnectionId { get; }

        [JsonProperty]
        public string Name { get; private set; }

        public CourseSearchViewModel SearchVM { get; private set; }

        /// <summary>
        /// The schedule ViewModel that is shared across all
        /// users who have joined the same schedule.
        /// </summary>
        public SharedScheduleViewModel SharedScheduleVM { get; private set; }

        /// <summary>
        /// The schedule ViewModel that is specific to this client.
        /// </summary>
        public ScheduleViewModel TailoredScheduleVM { get; private set; }

        public PaulaClientViewModel(ScheduleManager scheduleManager, string connectionId)
        {
            _scheduleManager = scheduleManager;
            ConnectionId = connectionId;
        }

        /// <summary>
        /// Loads the schedule with the specified ID and assigns it
        /// to the calling client. After this the client is expected
        /// to choose a user name and then call
        /// <see cref="CompleteJoinSchedule(string)"/> to actually join
        /// the schedule and start collaborating with others.
        /// </summary>
        /// <remarks>
        /// After this call, synchronization of <see cref="SharedScheduleVM"/>
        /// with the calling client should start.
        /// </remarks>
        /// <param name="scheduleID">Schedule ID</param>
        public void BeginJoinSchedule(string scheduleID)
        {
            if (SharedScheduleVM != null)
                throw new InvalidOperationException("The client has already joined a schedule");

            // TODO: Handle null/exception
            var scheduleVM = _scheduleManager.GetOrLoadSchedule(scheduleID);

            SharedScheduleVM = scheduleVM;
        }

        /// <summary>
        /// Joins the schedule specified by <see cref="BeginJoinSchedule(string)"/>
        /// by adding the calling client to the schedule's list of users.
        /// </summary>
        /// <remarks>
        /// After this call, synchronization of <see cref="TailoredScheduleVM"/>
        /// with the calling client should start.
        /// </remarks>
        /// <param name="userName">
        /// User name (either one of the schedule's known but currently unused user names
        /// (see <see cref="SharedScheduleViewModel.AvailableUserNames"/> or a new name).
        /// </param>
        public void CompleteJoinSchedule(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException(nameof(userName));

            if (Name != null)
                throw new InvalidOperationException();

            // Check if user name already in use
            if (SharedScheduleVM.Users.Any(o => o.Name == userName))
                throw new ArgumentException($"The user name '{userName}' is already in use");

            // If user name is known, remove it from the list of available names
            SharedScheduleVM.AvailableUserNames.Remove(userName);

            Name = userName;
            SharedScheduleVM.Users.Add(this);


            // TODO: Properly create TailoredScheduleViewModel
            //       (not yet sure which properties can be shared and which must
            //       be tailored and how to sync changes between both VMs)
            TailoredScheduleVM = ScheduleViewModel.CreateFrom(SharedScheduleVM.Schedule);
            SearchVM = new CourseSearchViewModel();
        }

        /// <summary>
        /// Creates a new schedule with a random identifier
        /// and makes the client join it using the specified user name.
        /// </summary>
        public void CreateSchedule(string userName)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}