using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using EvaComponentLibrary.Models;
using EvaComponentLibrary.Models.References;
using EvaComponentLibrary.Models.Streamcontent;
using EvaComponentLibrary.Services;
using EvaComponentLibrary.Views;
using Microsoft.AspNetCore.Components;
using static MudBlazor.CategoryTypes;


namespace EvaComponentLibrary.ViewModels
{
    public class MainViewModel
    {
        public bool Bin;
        public bool Favourite;
        private UploadManagerController _uploadManager;
        public UploadManagerController UploadManger { get { return _uploadManager; } set { _uploadManager = value; } }
        //public List<string> Question = new List<string> { "Wie melde ich mich krank?", "Wie schreibe ich eine Entschuldigung für die Schule/Arbeit?", "Wie beantrage ich Urlaub?", "Wie kann ich meine Produktivität steigern?", "Wie kann ich meine Finanzen besser verwalten?", "Wie kann ich Geld sparen?", "Wie schreibe ich einen effektiven Bericht?", "Wie gehe ich mit Überstunden um?" };
        public HashSet<int> RandomizedQuestion = new HashSet<int>();
        public ObservableCollection<Message> MessageItems = new ObservableCollection<Message>();
        public ObservableCollection<Room> MenuItems = new ObservableCollection<Room>();
        public ObservableCollection<Room> ScheduledMenuItems = new ObservableCollection<Room>();
        public ObservableCollection<Room> AgentMenuItems = new ObservableCollection<Room>();
        public ObservableCollection<Room> ArchivedRooms = new ObservableCollection<Room>();
        public ObservableCollection<Website> SitesSaved = new ObservableCollection<Website>();
        public ObservableCollection<Source> Sources = new ObservableCollection<Source>();
        public IEnumerable<Room> TrashItems =>
            (MenuItems ?? Enumerable.Empty<Room>())
                .Concat(ScheduledMenuItems ?? Enumerable.Empty<Room>())
                .Concat(ArchivedRooms)
                .Concat(AgentMenuItems)
                .Where(r => r.Archived);


        public IEnumerable<Room> FavouriteItems =>
             (MenuItems ?? Enumerable.Empty<Room>())
                .Concat(ScheduledMenuItems ?? Enumerable.Empty<Room>())
                .Where(r => r.TrackContext);

        public IEnumerable<Room> AllChatRoom =>
            (MenuItems ?? Enumerable.Empty<Room>())
             .Concat(ScheduledMenuItems ?? Enumerable.Empty<Room>())
            .Concat(AgentMenuItems);

        public List<string> ChosenSources = new List<string>();
        public event Action<bool> WebsocketConnectionState;
        public event Action? OnMessageReceived;
        public event Action? OnMsgAdded;
        public event Action? UpdateInterface;
        public event Func<Task<string>>? UpdateAgentDash;
        public event Action<bool> IsSourcesSelected;
        public Action<string>? OnNavigation;
        public Action? OnNewRoom;
        public User UserInfo { get; set; }
        public string Role { get; set; }
        public bool TypingAnimation { get; set; }
        private string _roomId = "0";
        public Room CurrentRoom =new();
        public int _counter = 0;
        private string _lastMsgId = "0";
        public string RoomId { get { return _roomId; }  set { _roomId = value; } }
        public string? themeMode = string.Empty;
        public bool LoadingRoom { get; set; }
        public readonly WebServices _ws;
        public readonly Logistic _lg;
        public ScheduledTaskViewModel _stvm;
        private readonly IThemeService _themeHandler;
        private readonly IClipboardService _clipboard;
        private readonly IDownloadPDF _downloadPDF;
        private int _fileContainerMergeCountaier = 0; 
        private int _sitesSavedMergeCountaier = 0; 
        public string URLChatroomID { get; set; }

        public bool DoWebsearch { get; set; } = false;

        public IPushNotificationService _pushService;

        public MainViewModel(WebServices service,
            IThemeService themeHandler,
            IClipboardService Clipboard,
            IDownloadPDF downloadPDF,
            Logistic logistic,
            IPushNotificationService push)
        {
            _ws = service ?? throw new ArgumentNullException(nameof(service));
            _downloadPDF = downloadPDF ?? throw new ArgumentNullException(nameof(downloadPDF));
            _themeHandler = themeHandler;
            _clipboard = Clipboard;
            _ws.RoomTitle += AddRoomTitle;
            _lg = logistic;
            _pushService = push;
            _ws.OnStream += SetMsgFromStream;

        }
        private void AddRoomTitle(string roomtitle)
        {
            try
            {
                var content = JsonSerializer.Deserialize<RoomTitle>(roomtitle);
                //(content.Title);
                var roomToBeChanged = MenuItems.FirstOrDefault(x => x.RoomId.Equals(content.RoomId));
                if (roomToBeChanged is null)
                {
                    var taskToBeChanged = ScheduledMenuItems.FirstOrDefault(x => x.RoomId
                    .Equals(content.RoomId));

                    if (taskToBeChanged is null) return;

                    taskToBeChanged.Title = content.Title;
                    taskToBeChanged.ActiveAnimation = true;
                    UpdateInterface?.Invoke();
                    return;
                }

                roomToBeChanged.Title = content.Title;
                roomToBeChanged.ActiveAnimation = true;
                //if (!roomToBeChanged.RoomId.Equals(_roomId))
                //{
                //    roomToBeChanged.UnreadMessage = true;
                //}
                //("called");
                TypingAnimation = true;
                UpdateInterface?.Invoke();
                roomToBeChanged.ActiveAnimation = false;

            }
            catch (Exception e)
            {
                //(e);
            }
        }
        public void cleanTime()
        {
            MessageItems.Clear();
            MenuItems.Clear();
            SitesSaved.Clear();
            Sources.Clear();
            ArchivedRooms.Clear();
            ScheduledMenuItems.Clear();
        }

        public async Task<bool> SavePDF(byte[] pdfByte, string fileName)
        {
            return await _downloadPDF.Save(pdfByte, fileName);
        }

        public async Task CopyToClipboard(string text)
        {
            await _clipboard.CopyToClipboard(text);
        }


        public void ChangeThemeOfSafeArea(string theme)
        {
            _themeHandler.ApplyThemeColor(theme);
        }

        public void MergeFilesContainerToSources()
        {
            var existingIds = new HashSet<string>(Sources.Select(s => s.Id));

            var incoming = _uploadManager.FilesContainer
                        .Where(f => existingIds.Add(f.Id)) 
                        .Select(f => new Source
                        {
                            Id = f.Id,
                            Name = f.Name,
                            IsChecked = false,
                            Category = "file",
							FileType = f.FileType
						})
                        .ToList();

            if (incoming.Count == 0) return;
            if (_fileContainerMergeCountaier == 0)
            {
                foreach (var s in incoming)
                    Sources.Add(s);

                _fileContainerMergeCountaier = 1;
            }
            else
            {
                for (int i = incoming.Count - 1; i >= 0; i--)
                    Sources.Insert(0, incoming[i]);
            }
        }


        public void MergeSitesSavedToSources()
        {
            var existingIds = new HashSet<string>(Sources.Select(s => s.Id));
            var incoming = SitesSaved.Where(s => existingIds.Add(s.Id)).Select(f => new Source
            {
                Name = new Uri(f.Url).Host,
                Id = f.Id,
                IsChecked = false,
                Category = "website"
            })
            .ToList();
            if (incoming.Count == 0) return;
            if (_sitesSavedMergeCountaier == 0)
            {
                foreach (var s in incoming)
                    Sources.Add(s);

                _sitesSavedMergeCountaier = 1;
            }
            else
            {
                for (int i = incoming.Count - 1; i >= 0; i--)
                    Sources.Insert(0, incoming[i]);
            }

        }



        public void DeleteFileSource(string id)
        {
            var file = Sources.FirstOrDefault(x => x.Id == id);
            if (file != null)
            {
                Sources.Remove(file);
            }
        }

        public void AddTheChosenSources()
        {
            try
            {
                ChosenSources.Clear();
                foreach (var source in Sources)
                {
                    if (source.IsChecked)
                    {
                        ChosenSources.Add(source.Id);
                    }
                }
                IsSourcesSelected?.Invoke(true);
            }
            catch (Exception ex)
            {

                IsSourcesSelected?.Invoke(false);

            }

        }

        public async void InitRooms()
        {
            if (!HasSession())
                return;

            _ws.WebsocketConnectionState += WebSocketTest;
            _ws.OnMessageReceived += WebSocketMessageReceived;
            _ws.MsgObserver.OnChange += WebSocketMessageReceived;
            await GetAllRooms();
            await GetArchivedRooms();
            await _ws.EstablishSignalRConnection();
            var result = await _ws.GetUserInfoAsync();
            UserInfo = JsonSerializer.Deserialize<User>(result);
            Role = UserInfo.Roles[0].RoleId;
            //("role" + Role);
            UpdateInterface?.Invoke();
            _ = TokenHandler();
        }
        public async Task GetAllRooms()
        {
            string rooms = string.Empty;
            rooms = await _ws.GetAllRooms();
            await GetArchivedRooms();
            if (string.IsNullOrEmpty(rooms))
                return;
            var roomList = JsonSerializer.Deserialize<RoomList>(rooms);
            await TaskChatRoomSpliter(roomList);
        }
        public async Task GetArchivedRooms()
        {
            var archivedRoom = await _ws.GetArchivedRooms();
            if (string.IsNullOrEmpty(archivedRoom))
            {
                return;
            }
            var result = JsonSerializer.Deserialize<RoomList>(archivedRoom);
            ArchivedRooms = new ObservableCollection<Room>(result.Rooms);
        }
        public async Task TaskChatRoomSpliter(RoomList roomlist)
        {
            roomlist.Rooms.Reverse();
            var tasksList = roomlist.Rooms.Where(x => x.RoomTypes == RoomTypes.SCHEDULED).ToList();
            var AgentList = roomlist.Rooms.Where(x => x.RoomTypes == RoomTypes.MONITORING).ToList();
            tasksList.ForEach(x =>
            {
                if (x.Length < 2)
                {
                    x.UnreadMessage = false;
                }
            });
            roomlist.Rooms.RemoveAll(x => x.Pinned);
            roomlist.Rooms.RemoveAll(x=> x.RoomTypes == RoomTypes.MONITORING);
            ScheduledMenuItems = new ObservableCollection<Room>(tasksList.OrderByDescending(x => x.UnixupdatedAt));
            MenuItems = new ObservableCollection<Room>(roomlist.Rooms.OrderByDescending(x => x.UnixupdatedAt));
            AgentMenuItems = new ObservableCollection<Room>(AgentList.OrderByDescending(x => x.UnixupdatedAt));
            await _ws.EstablishSignalRConnection();
            
        }

        public bool HasNoConnection()
        {
            return _ws.HasNoConnectionToHub();
        }


        private async Task TokenHandler()
        {
            var token = await _pushService?.GetDeviceTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _ws?.SendNotificationTokenAsnyc(token);
            }
        }
        private void WebSocketTest(bool test)
        {
            WebsocketConnectionState?.Invoke(test);
        }



        public async Task AddMessage(string content, string websearch)
        {
            _counter++;
            if (!_roomId.Equals("0"))
            {
                if (CurrentRoom.Pinned)
                {
                    var room = ScheduledMenuItems.LastOrDefault(x => x.RoomId.Equals(_roomId));
                    room.Length = _counter;
                    CurrentRoom.Length = _counter;
                }
                else
                {
                    var room = MenuItems.LastOrDefault(x => x.RoomId.Equals(_roomId));
                    room.Length = _counter;
                    CurrentRoom.Length = _counter;
                }
            }
            //(_counter);
            if (_roomId.Equals("0"))
            {
                _roomId = Guid.NewGuid().ToString();

                ///_---------------------------------------------------------
                var newRoom = new Room()
                {
                    Id = "neuId",
                    UserId = _ws.UserID,
                    RoomId = _roomId,
                    Title = "Neuer Chat",
                    Archived = false,
                    Length = _counter + 2,
                    UnixupdatedAt = (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000)
                };
                MenuItems.Insert(0, newRoom);
                CurrentRoom = newRoom;
            }

            Message msg = new Message()
            {
                UserId = UserInfo.UserId,
                Content = content,
                isHuman = true,
                RoomId = _roomId,
                Counter = _counter,
                DoWebsearch = DoWebsearch ? "allow" : "off",
                References = new List<Reference>(),
                SourceIds = ChosenSources,
                Version = _ws.Version,
                Roles = UserInfo.Roles
            };
            Message placeholder = new Message()
            {
                UserId = UserInfo.UserId,
                Content = string.Empty,
                isHuman = false,
                RoomId = _roomId,
                Counter = _counter,
                DoWebsearch = string.Empty,
                References = new List<Reference>(),
                SourceIds = new List<string>(),
                IsFinished = false,
                Roles = UserInfo.Roles
            };

            MessageItems.Add(msg);
            MessageItems.Add(placeholder);
            UpdateInterface?.Invoke();
            await Task.Delay(10); // that is not a Solution
            OnMsgAdded?.Invoke();
            string jsonMessage = JsonSerializer.Serialize<Message>(msg);
            try
            {
                await _ws.SendMessage(jsonMessage);
            }
            catch (Exception ex)
            {
                _ws.ErrorOccurred?.Invoke();
            }

        }

        public bool HasSession()
        {
            return !string.IsNullOrEmpty(_ws.Session);
        }

        private async void WebSocketMessageReceived(string message)
        {

            AddResponse(message);
        }

        public async Task FindAndLoadRoom(string id)
        {
            var result = ScheduledMenuItems.FirstOrDefault(x => x.RoomId.Equals(id));
            if(result == null)
            {
                result = MenuItems.FirstOrDefault(x => x.RoomId.Equals(id));
            }
            if (result != null)
            {
                await LoadRoomAsync(result);
            }

        }



        public async Task LoadRoomAsync(Room room)
        {
            if (room.RoomTypes == RoomTypes.MONITORING || room.RoomTypes == RoomTypes.SCHEDULED)
            {
                var result = await _ws.DisableRoomBadgAsync(room.RoomId);
                if (result)
                {
                    room.UnreadMessage = false;
                    _lg.UpdateSidebarforTask?.Invoke();

                }
            }
            else if (room.UnreadMessage)
            {
                room.UnreadMessage = false;
                _lg.UpdateSidebarforTask?.Invoke();
            }
            CurrentRoom = room;
            LoadingRoom = true;
            MessageItems.Clear();
            ChosenSources.Clear();
            List<Message> roomsList;
            //NewRoom();

            string rooms = await _ws.GetChatRoomsAsync(room.Length, room.RoomId);
            try
            {
                //($"RAUM: {rooms}");

                roomsList = JsonSerializer.Deserialize<List<Message>>(rooms); //______________________________________________________>

            }
            catch (Exception ex)
            {
                //($"Error parsing room: {ex.Message}");
                NewRoom();
                return;
            }
            foreach (var file in Sources)
            {
                file.IsChecked = false;
            }
            UpdateInterface?.Invoke();
            MessageItems = new ObservableCollection<Message>(roomsList?.OrderBy(x=>x.Counter));
            foreach(var i in MessageItems)
            {
                //(i.Content);
            }
            LoadingRoom = false;
            _roomId = room.RoomId;
            normalizer();
            await Task.Delay(100);
            OnMessageReceived?.Invoke();
            //(rooms);
        }

        public void NewRoom()
        {
            OnNavigation?.Invoke("/chat");
            CurrentRoom = new();
            _roomId = "0";
            MessageItems.Clear();
            ChosenSources.Clear();
            foreach (var file in Sources)
            {
                file.IsChecked = false;
            }
            OnNewRoom?.Invoke();
            UpdateInterface?.Invoke();
            OnMessageReceived?.Invoke();

        }
        private async void AddResponse(string content)
        {
            Message msg = JsonSerializer.Deserialize<Message>(content);

            if (!msg.RoomId.Equals(_roomId))
            {
                //var task_item = ScheduledMenuItems.FirstOrDefault(x => x.RoomId == msg.RoomId);
                //var chat_item = MenuItems.FirstOrDefault(x => x.RoomId == msg.RoomId);
                //var agent_item = AgentMenuItems.FirstOrDefault(x => x.RoomId == msg.RoomId);
                //if (task_item != null)
                //{
                //    task_item.UnreadMessage = true;
                //    task_item.Length += 2;
                //    _lg.UpdateSidebarforTask?.Invoke();

                //    return;
                //}
                //if (chat_item != null)
                //{
                //    chat_item.UnreadMessage = true;
                //}
                //if(agent_item !=null)
                //{
                //    agent_item.UnreadMessage = true;
                //}
                //return;
                await GetAllRooms();
                UpdateAgentDash?.Invoke();
                _lg.UpdateSidebarforTask?.Invoke();
            }



            ////("are you there "+content);


            if (!msg.RoomId.Equals(_roomId) &&
                !_roomId.Equals("0"))
                return;

            _lastMsgId = msg.RoomId;
            MessageItems.Add(msg);
            normalizer();

            //UpdateInterface?.Invoke();
            //OnMessageReceived?.Invoke();

        }



        private void normalizer()
        {
            foreach (var msg in MessageItems)
            {
                foreach (var Reference in msg.References)
                {
                    if (Reference is Links links)
                    {
                        List<string> Title = new List<string>();
                        if (Title.Contains("."))
                        {
                            Title = links.Name.Split(".").ToList();

                            int num = Title.Count();
                            if (num > 0)
                            {
                                if (num == 3)
                                {
                                    links.Name = Title[1];
                                }
                                else
                                {
                                    links.Name = Title[0];
                                }
                            }
                        }
                    }
                }
            }

        }
        public async Task<bool> PostFeedbackAsync(string msgId, int vote, string feedback)
        {
            bool result = await _ws.PostFeedbackAsync(msgId, vote, feedback);
            return result;
        }

        public async Task InitWebSave()
        {
            var websites = await _ws.GetWebsitesAsync();
            if (string.IsNullOrEmpty(websites))
                return;

            var sites = JsonSerializer.Deserialize<List<Website>>(websites);
            if (sites.Count != 0)
            {
                SitesSaved = new ObservableCollection<Website>(sites.Reverse<Website>());
            }
            UpdateInterface?.Invoke();
        }

        public async Task<bool> AddNewSite(string url, int depth)
        {
            var uri = NormalizeUrl(url);
            Uri uriResult;
            if (Uri.IsWellFormedUriString(uri, UriKind.Absolute)
                    && Uri.TryCreate(uri, UriKind.Absolute, out uriResult)
                    && Regex.IsMatch(uri, @"^(https?://)?([\w\.-]+)\.([a-z\.]{2,6})([/\w\.-]*)*/?$"))
            {
                bool hasSend = false;
                try
                {
                    hasSend = await _ws.PostNewSite(uriResult.ToString(), depth);

                }
                catch (Exception ex)
                {
                    return false;
                }

                if (hasSend)
                {
                    await InitWebSave();
                }
                return hasSend;
            }
            return false;
        }


        public string NormalizeUrl(string url)
        {
            url = url.Trim().ToLower();

            if (!url.ToLower().Contains("https://"))
            {
                url = $"https://{url}";
            }

            UriBuilder Url = new UriBuilder(url);
            return Url.Uri.ToString();

        }

        public async Task<bool> UpdateSync(Website site)
        {
            return await _ws.UpdateWebsiteSync(site);
        }

        public async Task<string> DeleteWebsiteAsync(string id)
        {
            string result = await _ws.DeleteWebsiteAsync(id);
            if (!string.IsNullOrEmpty(result))
            {
                var itemToRemove = SitesSaved.SingleOrDefault(s => s.Id.Equals(id));
                var SourceToRemove = Sources.SingleOrDefault(s => s.Id.Equals(itemToRemove?.Id));

                if (itemToRemove != null)
                {
                    SitesSaved.Remove(itemToRemove);
                    Sources.Remove(SourceToRemove);
                    UpdateInterface?.Invoke();
                }
                return itemToRemove != null ? itemToRemove.Url : string.Empty;
            }
            return string.Empty;
        }

        public async Task<bool> DeleteRoomAsync(string id)
        {
            bool result = await _ws.DeleteChatRoomAsync(id);
            if (result)
            {
                Room roomToBeDel = MenuItems.FirstOrDefault(e => e.RoomId.Equals(id));
                if (roomToBeDel != null)
                {
                    MenuItems.Remove(roomToBeDel);
                    if (CurrentRoom is null) return result;
                    if(RoomId.Equals(id))
                    {
                        NewRoom();
                    }
                }
               
            }
            return result;
        }

        public async Task<bool> RenameChatRoomAsync(string roomId, string title)
        {
            bool result = await _ws.RenameChatRoomAsync(roomId, title);
            if (result)
            {
                MenuItems.FirstOrDefault(e => e.RoomId.Equals(roomId)).Title = title;
                return true;
            }
            return false;
        }



        public void RandomizeTheQuestion()
        {
            Random random = new Random();
            while (RandomizedQuestion.Count < 4)
            {
                RandomizedQuestion.Add(random.Next(8));
            }

            foreach (var item in RandomizedQuestion)
            {
                //(item);
            }
        }


        public async Task<bool> LogoutAsync()
        {
            await _pushService.DeleteDeviceTokenAsync();
            return await _ws.LogoutAsync();
        }



        /*-------------------------------- Stream ----------------------------------------*/
        int counter = 0;
        private StringBuilder? _contentBuilder;
        private string _messageRoomID = string.Empty;
        private Message _currentStreamedMessage;

        public event Action? OnStream;
        public event Action? OnEndOfStream;

        private void SetMsgFromStream(string content)
        {
            _ = SetMsgFromStreamAsync(content);
        }

        private async Task SetMsgFromStreamAsync(string content)
        {
            if (string.IsNullOrEmpty(content)) return;

            BaseStreamMsg? msg;

            try
            {
                //Console.WriteLine(content);
                msg = JsonSerializer.Deserialize<BaseStreamMsg>(content);
            }
            catch (Exception ex)
			{ 
				Console.WriteLine($"[Stream Error] Invalid JSON: {ex.Message}");
                OnMessageReceived?.Invoke();
				return;
			}

            if(msg is null)
            {
				OnMessageReceived?.Invoke();
                return;
			}


            try
            {
                switch (msg)
                {
                    case InitStreamMsg info:
                        await OnMessageBeginStream(info);
                        break;
                    case Token info:
                        await OnStreamMessage(info);
                        break;
                    case EndStreamMsg info:
                        await OnStremEnd(info);
                        break;
                    default:
                        OnMessageReceived?.Invoke();
                        break;
                }

            } catch (Exception ex) 
            {
            
				Console.WriteLine($"[Stream Error] {ex.Message}");
			}
        }

        private async Task OnMessageBeginStream(InitStreamMsg msg)
        {
            _messageRoomID = msg.RoomId;
            if (!CheckRoomID())
            {
                _ws.CancelStream();
                return;
            }

            counter = 0;
            _contentBuilder = new();

            _currentStreamedMessage = new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = msg.UserId,
                RoomId = msg.RoomId,
                isHuman = msg.IsHuman,
                Feedback = Feedback.Nan,
                Counter = msg.Counter,
                IsFinished = false
            };

			if (MessageItems.Count > 0)
				MessageItems[^1] = _currentStreamedMessage;
			else
				MessageItems.Add(_currentStreamedMessage);

			AddNewRoom(msg.RoomId);
           
            UpdateInterface?.Invoke();
            await Task.Yield();


        }


        private async Task OnStreamMessage(Token token)
        {
			if (!CheckRoomID())
			{
				_ws.CancelStream();
				return;
			}

			if (MessageItems.Count == 0)
				return;

			var lastMsg = MessageItems[^1];
			token.Id = Guid.NewGuid().ToString();

            if (token.NextToken.Contains("!["))
            {
                lastMsg.IsGeneratingImage = true;
            }

            if(lastMsg.IsGeneratingImage && token.NextToken.Contains(")"))
            {

                lastMsg.IsGeneratingImage = false;
                //lastMsg.Content = lastMsg.ImageBase64;
            }

       //     if(lastMsg.IsGeneratingImage)
       //     {
			    //lastMsg.ImageBase64 += token.NextToken;
       //     } else
       //     {
       //     }
                lastMsg.Content += token.NextToken;

                OnStream?.Invoke();
			await Task.Yield();
		}


        private async Task OnStremEnd(EndStreamMsg info)
        {
			if (!CheckRoomID())
			{
				_ws.CancelStream();
				return;
			}

			if (MessageItems.Count == 0)
				return;

			var lastMessage = MessageItems[^1];
			lastMessage.Content = info.Message ?? lastMessage.Content;
			lastMessage.Feedback = Feedback.Nan;
			lastMessage.References = info.References;
			lastMessage.IsFinished = true;

			_counter++;

			if (!_roomId.Equals("0"))
			{
				var list = CurrentRoom.Pinned ? ScheduledMenuItems : MenuItems;
				var menuItem = list.LastOrDefault(x => x.RoomId.Equals(_roomId));
				if (menuItem != null)
					menuItem.Length = _counter;
			}

			OnStream?.Invoke();
			OnEndOfStream?.Invoke();
			await Task.Yield();
		}

        public bool CheckRoomID()
            => _messageRoomID.Equals(_roomId);


        private async void AddNewRoom(string roomID)
        {
            _roomId = roomID;
            _lastMsgId = roomID;
            //OnMessageReceived?.Invoke();
        }

        public async Task<Room?> GetRoomById(string room)
        {
            var result = await  _ws.GetRoomByIdAsync(room);
            if (!string.IsNullOrEmpty(result))
            {
               return  JsonSerializer.Deserialize<Room>(result);
            }
            return null;
        }

        public void AddRoomToSideBar(Room? room)
        {
            if(room != null)
            {
                switch (room.RoomTypes)
                {
                    case RoomTypes.SCHEDULED:
                        ScheduledMenuItems.Add(room);
                        break;
                    default:
                        MenuItems.Add(room);
                        break;
                }
                _lg.UpdateSidebarforTask?.Invoke();
            }
            return;
        }

        /*-------------------------------- Stream End ----------------------------------------*/
        public async Task<string> SetRoomArchived(string roomId, bool archived)
        {
            var result = await _ws.SetRoomArchived(roomId, archived);
            return result;
        }
        public async Task<string> SetTrackChatContext(string roomId, bool archived)
        {
            var result = await _ws.SetTrackChatContext(roomId, archived);
            return result;
        }

    }
}
