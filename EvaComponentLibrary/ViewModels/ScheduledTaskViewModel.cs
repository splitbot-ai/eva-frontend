using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EvaComponentLibrary.Models;
using EvaComponentLibrary.Services;


namespace EvaComponentLibrary.ViewModels
{
    public class ScheduledTaskViewModel
    {

        public ScheduledTask? CurrentScheduledTask { get; set; }
        public ObservableCollection<ScheduledTask> AllTasks = new();
        public WebServices _ws { get; set; }
        public MainViewModel _vm;
        private Logistic _lg;
        public string Interval { get; set; }
        public int PreferDayOfWeek { get; set; }
        public int PreferMonth{ get; set; }
        public DateTime LocalDate { get; set; }
        public ScheduledTaskViewModel(Logistic logistic)
        {
            _lg = logistic;
        }

        public async Task<string> GetAllTasks()
        {
            var result = await _ws.GetAllScheduledTaskAsync();

            if (!result.Equals("Unauthorized") && !string.IsNullOrEmpty(result))
            {

                var Tasks = JsonSerializer.Deserialize<List<ScheduledTask>>(result);
                AllTasks = new ObservableCollection<ScheduledTask>(Tasks ?? new());

                return "OK";
            }
            return result;
        }
        public void CronToHuman(string cronExpression)
        {

            var parts = cronExpression.Split(' ');
            if (parts.Length != 6)
                throw new ArgumentException("Cron expression must have 6 fields (sec min hour dom mon dow)");

            string sec = parts[0],
                   min = parts[1],
                   hr = parts[2],
                   dom = parts[3],
                   mon = parts[4],
                   dow = parts[5];

            if (hr == "*" && min != "*" && dom == "*" && mon == "*" && dow == "*")
                Interval = "hourly";
            else if (dom == "*" && dow == "*")
                Interval = "daily";
            else if (dom == "*" && dow != "*")
                Interval = "weekly";
            else if (dom != "*" && dow == "*")
                Interval = "monthly";
            else
                Interval = "yearly";

            if (!Interval.Equals("hourly"))
            {
                int s = int.Parse(sec), m = int.Parse(min), h = int.Parse(hr);
                var utcNow = DateTime.UtcNow;
                var utcDate = new DateTime(
                    utcNow.Year, utcNow.Month, utcNow.Day,
                    h, m, s,
                    DateTimeKind.Utc
                );
                TimeZoneInfo tz;
                tz = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneInfo.Local.Id);
                LocalDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, tz);
            }
            if (Interval.Equals("weekly"))
            {
                PreferDayOfWeek = int.Parse(dow);

            }
            if (Interval.Equals("yearly"))
            {
                PreferMonth = int.Parse(mon);
            }
        }
        public async Task<bool> EditScheduledTaskAsync(ScheduledTask data)
        {
            var result = await _ws.EditScheduledTaskAsync(data);
            if (result)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> AddScheduledTaskAsync(ScheduledTask data)
        {
            var result = await _ws.AddScheduledTaskAsync(data);
            if (result)
            {
                string rooms = string.Empty;
                rooms = await _ws.GetAllRooms();
                var roomList = JsonSerializer.Deserialize<RoomList>(rooms);
                await _vm.TaskChatRoomSpliter(roomList);
                _lg.UpdateSidebarforTask?.Invoke();
                await GetAllTasks();
                return true;
            }
            return false;
        }

        public async Task<bool> GetScheduledTaskAsync(string roomId)
        {
            try
            {

                var json = await _ws.GetScheduledTaskAsync(roomId);
                var result = JsonSerializer.Deserialize<ScheduledTask>(json) ?? null;
                if (result is not null)
                {
                    CurrentScheduledTask = result;
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                //(e);
                return false;
            }
        }

        public string GenerateCronExpression(string? frequency, TimeSpan? time, int preferDayOfWeek, int preferMonth)
        {
            string cronExpression = string.Empty;
            if (time.HasValue)
            {
                DateTime today = DateTime.Now.Date.Add(time.Value);
                DateTime Utc = today.ToUniversalTime();
                TimeSpan utcTime = Utc.TimeOfDay;

                int hour = utcTime.Hours;
                int minute = utcTime.Minutes;

                switch (frequency?.ToLower())
                {
                    case "hourly":
                        cronExpression = $"0 0 * * * *";
                        break;
                    case "daily":
                        cronExpression = $"0 {minute} {hour} * * *";
                        break;
                    case "weekly":
                        cronExpression = $"0 {minute} {hour} * * {preferDayOfWeek}";
                        break;
                    case "monthly":
                        int thisDayOfMonth = today.Day;
                        cronExpression = $"0 {minute} {hour} {thisDayOfMonth} * *";
                        break;
                    case "yearly":
                        int thisMonth = today.Month;
                        thisDayOfMonth = today.Day;
                        cronExpression = $"0 {minute} {hour} {thisDayOfMonth} {preferMonth} *";
                        break;
                    default:
                        cronExpression = $"0 {minute} {hour} * * *";
                        break;
                }
            }
            //(cronExpression);
            return cronExpression;
        }

        public async Task<bool> DeleteScheduledTaskByRoomIdAsync(string roomId)
        {
            var result = await _ws.DeleteScheduledTaskByRoomIdAsync(roomId);
          
            if (!string.IsNullOrEmpty(result))
            {

                if (result.Equals("OK",StringComparison.OrdinalIgnoreCase))
                {
                    var result_room = await _ws.DeleteChatRoomAsync(roomId);

                    if (result_room)
                    {

                        var itemToRemove = _vm.ScheduledMenuItems
                        .FirstOrDefault(x => x.RoomId.Equals(roomId));

                        var remove = AllTasks.FirstOrDefault(x => x.RoomId.Equals(roomId));
                        AllTasks.Remove(remove);

                        if (itemToRemove != null)
                        {
                            _vm.ScheduledMenuItems.Remove(itemToRemove);
                            _lg.UpdateSidebarforTask?.Invoke();
                        }
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public async Task<bool> GetTaskToEdit(string roomId)
        {
            var hasTask = await GetScheduledTaskAsync(roomId);

            if (!hasTask) return hasTask;

            CronToHuman(CurrentScheduledTask.Cron);
            _vm.RoomId = "0";
            _vm.MessageItems = new();
            _lg.EditTask = CurrentScheduledTask;
            _lg.EditTask.RoomId = CurrentScheduledTask.RoomId;
            _lg.SetValueForEdit?.Invoke();

            return true;
        }

    }
}
