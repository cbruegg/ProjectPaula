﻿using ProjectPaula.DAL;
using ProjectPaula.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectPaula.ViewModel
{
    /// <summary>
    /// Handles loading and unloading of schedules, adding and removing of
    /// users to/from schedules and creation and deletion of appropiate ViewModels.
    /// </summary>
    public class ScheduleManager
    {
        private static readonly Lazy<ScheduleManager> _scheduleManager =
            new Lazy<ScheduleManager>(() => new ScheduleManager());

        /// <summary>
        /// Gets the singleton <see cref="ScheduleManager"/> instance.
        /// </summary>
        public static ScheduleManager Instance => _scheduleManager.Value;

        private readonly Dictionary<string, SharedScheduleViewModel> _loadedSchedules = new Dictionary<string, SharedScheduleViewModel>();
        private readonly Dictionary<string, PaulaClientViewModel> _connectedClients = new Dictionary<string, PaulaClientViewModel>();

        public IReadOnlyDictionary<string, PaulaClientViewModel> Clients => _connectedClients;

        public PaulaClientViewModel AddClient(string connectionId)
        {
            var client = new PaulaClientViewModel(this, connectionId);
            _connectedClients.Add(connectionId, client);
            return client;
        }

        /// <summary>
        /// Makes the user leave the schedule he/she is working on (if applicable)
        /// and removes the client.
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public bool RemoveClient(string connectionId)
        {
            // TODO: Clean-up: Remove user from schedule etc.
            return _connectedClients.Remove(connectionId);
        }

        public PaulaClientViewModel GetClient(string connectionId)
            => _connectedClients[connectionId];

        public SharedScheduleViewModel GetOrLoadSchedule(string scheduleId)
        {
            SharedScheduleViewModel scheduleVM;

            if (_loadedSchedules.TryGetValue(scheduleId, out scheduleVM))
            {
                return scheduleVM;
            }
            else
            {
                // TODO: Load schedule from database and create a SharedScheduleViewModel,
                //       add to _loadedSchedules and return

                // For testing purposes, create a mock VM
                var sampleCourses = PaulRepository.GetLocalCourses("Grundlagen").Select(c => new SelectedCourse() { CourseId = c.Id }).ToList();
                var schedule = new Schedule();
                schedule.AddCourse(sampleCourses[0]);
                schedule.AddCourse(sampleCourses[1]);
                schedule.AddCourse(sampleCourses[2]);
                schedule.AddCourse(sampleCourses[3]);
                schedule.AddCourse(sampleCourses[4]);
                schedule.AddCourse(sampleCourses[5]);
                schedule.AddCourse(sampleCourses[6]);
                var vm = new SharedScheduleViewModel(schedule);
                _loadedSchedules.Add(scheduleId, vm);
                return vm;
            }
        }
    }
}