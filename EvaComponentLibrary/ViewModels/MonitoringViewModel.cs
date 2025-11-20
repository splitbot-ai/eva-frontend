using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EvaComponentLibrary.Models;
using EvaComponentLibrary.Languages;
using Microsoft.Extensions.Localization;
using EvaComponentLibrary.Models.Agent;
using EvaComponentLibrary.Services;
using EvaComponentLibrary.Views.PagesComponents.AgentsComponent;
using static MudBlazor.CategoryTypes;

namespace EvaComponentLibrary.ViewModels
{
    public class MonitoringViewModel
    {

        private ObservableCollection<string>? _websites = new();
        public ObservableCollection<string>? Websites => _websites;
        public HashSet<string> WebseitesSet => AgentsRelatedToTheRoom.Select(x => x.Url).Concat(Websites).ToHashSet();
        public ObservableCollection<MonitoringAgent>? RunningAgent = new();
        public ObservableCollection<MonitoringAgent>? AgentsRelatedToTheRoom = new();
        public List<string> AddedTempLinks = new();
        public List<string> DeletedTempLinks = new();
        public Action? UpdateUI;
        public WebServices _ws;
        public MainViewModel _vm;
        public IStringLocalizer<MyStrings> locals;

        public MonitoringViewModel(WebServices ws, MainViewModel vm, IStringLocalizer<MyStrings> local)
        {
            _vm = vm;
            _ws = ws;
            locals = local;
            _vm.UpdateAgentDash += GetMonitors;
        }

        public string CurrentAgentRoomId { get; set; } = string.Empty;

        public async Task<string> CreateNewMonitoringAgent(string title)
        {
            if (Websites.Any())
            {
                var agent = new CreateAgent
                {
                    Title = title,
                    Urls = Websites.ToList()
                };

                var result = await _ws.PostMonitors(agent);

                if (result.Equals("NotFound") || result.Equals("Unauthorized"))
                {
                    return result;

                }
                var response = JsonSerializer.Deserialize<OnAgentCreate>(result);
                var MonitorRoom = await _vm.GetRoomById(response.RoomId);
                _vm.AddRoomToSideBar(MonitorRoom);
                await GetMonitors();
                return "OK";
                //var theAgent = RunningAgent.FirstOrDefault(x => x.RoomId.Equals(MonitorRoom.RoomId));
                //theAgent.Title = MonitorRoom.Title;
            }
            return "_genError";
        }
        public async Task<string> AddnewLinkToMonitors(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var newLink = new AddLinkToMonitoringAgent
                {
                    RoomId = CurrentAgentRoomId,
                    Urls = new List<string> { url }
                };

                var result = await _ws.PostNewLinkToAgent(newLink);

                if (result.Equals("OK"))
                {
                    _websites?.Clear();
                    await GetMonitors();

                }
                Websites.Clear();
                return result;
            }
            return string.Empty;
        }

        public async Task<string> GetMonitors()
        {
            var result = await _ws.GetMonitors();

            if (result.Equals("Error", StringComparison.OrdinalIgnoreCase))
            {
                return result;
            }

            var webAgents = JsonSerializer.Deserialize<List<MonitoringAgent>>(result);
            var Grouped = webAgents
                    .GroupBy(x => x.RoomId)
                    .Select(g => g.OrderByDescending(x => x.LastCheck).First())
                    .ToList();
            RunningAgent = new ObservableCollection<MonitoringAgent>(Grouped);

            foreach (var item in RunningAgent)
            {
                item.TimeInPast = ToRelativeAgo(item.LastCheck);
            }
            UpdateUI?.Invoke();
            return "OK";
        }


        public async Task<string> GetMonitorsByRoomId(string roomId)
        {
            var result = await _ws.GetMonitorsByRoomID(roomId);

            if (result.Equals("Error", StringComparison.OrdinalIgnoreCase))
            {
                return result;
            }

            var webAgents = JsonSerializer.Deserialize<List<MonitoringAgent>>(result);
            AgentsRelatedToTheRoom = new ObservableCollection<MonitoringAgent>(webAgents);
            CurrentAgentRoomId = roomId;
            return "OK";
        }
        public async Task DeleteLinks()
        {

            foreach (var item in DeletedTempLinks)
            {
                var result = await _ws.DeleteMonitor(item);
                if (result.Equals("OK"))
                {
                    var cart = RunningAgent?.FirstOrDefault(x => x.Url.Equals(item));
                    RunningAgent?.Remove(cart);
                    var agent = AgentsRelatedToTheRoom?.FirstOrDefault(x => x.Url.Equals(item));
                    AgentsRelatedToTheRoom?.Remove(agent);
                }
            }
            DeletedTempLinks.Clear();
        }
        public async Task<string> DeleteMonitor(string url)
        {
            var result = await _ws.DeleteMonitor(url);
            if (result.Equals("OK"))
            {
                var agent = AgentsRelatedToTheRoom?.FirstOrDefault(x => x.Url.Equals(url));
                var roomid = agent?.RoomId;
                AgentsRelatedToTheRoom?.Remove(agent);
                if (!AgentsRelatedToTheRoom.Any())
                {
                    var itemToRemove = RunningAgent?.FirstOrDefault(x => x.RoomId.Equals(roomid));
                    RunningAgent?.Remove(itemToRemove);
                    UpdateUI?.Invoke();
                }
            }
            return result;
        }
        public async Task<string> DeleteAllMonitor(string RoomId)
        {
            var result = await _ws.DeleteAllMonitor(RoomId);
            if (result.Equals("OK"))
            {
                var itemToRemove = RunningAgent?.FirstOrDefault(x => x.RoomId.Equals(RoomId));
                RunningAgent?.Remove(itemToRemove);
            }
            return result;
        }

        public async Task<string> DeleteMonitorAndRoom(string roomId)
        {
            try
            {
                var res = await _ws.DeleteAllMonitor(roomId);
                var result = await _vm.DeleteRoomAsync(roomId);
                if (result)
                {
                    var roomAgent = _vm.AgentMenuItems.FirstOrDefault(x => x.RoomId.Equals(roomId));
                    _vm.AgentMenuItems.Remove(roomAgent);
                }
                return "OK";
            }
            catch (Exception ex)
            {
                return "_genError";
            }
        }

        public void ChangeCheckBoxState(MonitoringAgent agent, bool val)
        {
            agent.CheckBoxState = val;
            if (val == true)
            {
                DeletedTempLinks.Add(agent.Url);
            }
            else
            {
                DeletedTempLinks.Remove(agent.Url);
            }
        }
        public void ChangeAllCheckBoxState(bool val)
        {
            foreach (var agent in AgentsRelatedToTheRoom)
            {
                if (agent.Status == LinkStatus.Deleted || agent.Status == LinkStatus.Added)
                {
                    continue;
                }
                agent.CheckBoxState = val;
                if (val == true)
                {
                    DeletedTempLinks.Add(agent.Url);
                }
                else
                {
                    DeletedTempLinks.Remove(agent.Url);
                }
            }
        }
        public void MarkingAsDeleted()
        {
            var DeletedSet = new HashSet<string>(DeletedTempLinks);

            foreach (var agent in AgentsRelatedToTheRoom)
            {
                if (DeletedSet.Contains(agent.Url))
                {
                    agent.Status = LinkStatus.Deleted;
                }
            }
        }
        public bool TryAddLink(string url)
        {
            return WebseitesSet.Contains(url);
        }
        public string ToRelativeAgo(double unixTimeSeconds)
        {
            var then = DateTimeOffset.FromUnixTimeSeconds((long)Math.Floor(unixTimeSeconds))
                                         .ToLocalTime();

            var now = DateTimeOffset.Now;
            var ts = now - then;

            if (ts.TotalSeconds < 0) return locals["_timStart"];

            return ts.TotalSeconds switch
            {
                < 5 => locals["_timStart"],
                < 60 => string.Format(locals["_timSAgo"], (int)ts.TotalSeconds),
                < 3600 => string.Format(locals["_timMAgo"], (int)ts.TotalMinutes),
                < 86400 => string.Format(locals["_timHAgo"], (int)ts.TotalHours),
                < 604800 => string.Format(locals["_timDAgo"], (int)ts.TotalDays),
                < 2592000 => string.Format(locals["_timWAgo"], (int)(ts.TotalDays / 7)),
                < 31536000 => string.Format(locals["_timMoAgo"], (int)(ts.TotalDays / 30)),
                _ => string.Format(locals["_timYAgo"], (int)(ts.TotalDays / 365))
            };
        }

        public void adjustTheLanguage()
        {
            if (RunningAgent.Any() && RunningAgent != null)
            {
                foreach (var item in RunningAgent)
                {
                    item.TimeInPast = ToRelativeAgo(item.LastCheck);
                }
            }
        }
    }
}
