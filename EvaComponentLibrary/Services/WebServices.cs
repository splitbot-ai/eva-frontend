using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Text;
using System.Text.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using EvaComponentLibrary.Models;
using EvaComponentLibrary.Models.Agent;
using EvaComponentLibrary.Models.CloudBase;
using EvaComponentLibrary.Models.CloudBase.NextCloud;
using EvaComponentLibrary.Models.Mail;
using EvaComponentLibrary.ViewModels;
using EvaComponentLibrary.ViewModels;
using EvaComponentLibrary.Views.PagesComponents.ExternalResources.EmailComponents;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EvaComponentLibrary.Services
{
    public class WebServices
    {
        private const string _version = "v6";
        public string Version { get { return _version; } }
        public string UserID { get; set; }


        public Action<string>? OnMessageReceived { get; set; }

        public Action<string>? SafeToken { get; set; }

        public Action<bool>? WebsocketConnectionState { get; set; }

        public Action? ReconectedSuccessfully { get; set; }


        public Action? ConnectionLost { get; set; }

        public Action? ErrorOccurred { get; set; }

        private string _session = string.Empty;

        public Action<string>? RoomTitle { get; set; }

        public string Session { get { return _session; } set { _session = value; } }

        private ClientWebSocket? _ws = new();
        private CancellationToken _wsToken = new();

        private HubConnection _hubConnection;

        CancellationTokenSource StreamToken;
        public event Action<string> OnStream;


        public string Host { get; set; }
        public string Realm { get; set; }
        public string Auth { get; set; }

        public KeycloackHelper KeycloackHelper { get; set; }

        public string UserName = string.Empty;
        public string UserMail = string.Empty;
        public string Sub = string.Empty;

        private HttpClient _client = new HttpClient();
        private HttpClient _clientEmail = new HttpClient();
        public event Action? ConnectivityChanged;
        public string LastMsg = string.Empty;
        public int ReconnectAttempts = 0;
        private bool _isInit = false;


        private bool _shouldRefresh = true;

        public ObservableMessage? MsgObserver = new();

        private readonly SemaphoreSlim _connectionLock = new(1, 1);
        private readonly IThreadInvoker _threadInvoker;
        private EMailViewModel _evm;
        public ScheduledTaskViewModel _stvm;
        private JsonSerializerOptions _jsonSerializerIgnore = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        private IOptions<HostSettings> _hostSettings;
      

       
        public WebServices(IThreadInvoker threadinvoker, EMailViewModel email, ScheduledTaskViewModel scheduledTask, 
             IOptions<HostSettings> hostsettings)
        {
            _threadInvoker = threadinvoker;
            _evm = email;
            _stvm = scheduledTask;
            _hostSettings = hostsettings;

#if DEBUG
            Host = hostsettings.Value.Dev.Host;
            Realm = hostsettings.Value.Dev.Realm!;
            Auth = hostsettings.Value.Dev.Auth!;
#else
            Host = hostsettings.Value.Prod.Host;
            Realm = hostsettings.Value.Prod.Realm!;
            Auth = hostsettings.Value.Prod.Auth!;
#endif
        }
        public void Init()
        {
            if (string.IsNullOrEmpty(Host) || string.IsNullOrEmpty(Realm) || string.IsNullOrEmpty(Auth))
            {
            
                _isInit = true;
                _client.Dispose();
                _client = new HttpClient();
                _client.BaseAddress = new Uri($"{Host}/Eva/api/{_version}/");
                _clientEmail.BaseAddress = new Uri($"{Host}/mail/api/v1/");
            }
            if (_client.BaseAddress == null)
            {
                _client.Dispose();
                _client = new HttpClient();
                _client.BaseAddress = new Uri($"{Host}/Eva/api/{_version}/");
                _clientEmail.BaseAddress = new Uri($"{Host}/mail/api/v1/");
            }
        }

        /*WEBSOCKET START*/


        public async Task EstablishSignalRConnection()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{Host}/Eva/chatHub/", x =>
            {
                x.AccessTokenProvider = () => Task.FromResult(_session);
            }).WithAutomaticReconnect(new SignalRRetryPolicy())
            .Build();

            _hubConnection.On<string>("ReceiveClientMessage", (message) =>
            {
                _threadInvoker.InvokeOnMainThread(() =>
                {
                    //($"ReceiveClientMessage: {message}");
                });
            });

            _hubConnection.On<string>("ReceiveEvaMessage", (message) =>
            {
                _threadInvoker.InvokeOnMainThread(() =>
                {
                    MsgObserver.Property = message;
                    //(message);
                });
            });

            _hubConnection.On<string>("ErrorOccurred", (message) =>
            {
                _threadInvoker.InvokeOnMainThread(() =>
                {
                   //($"Error Occurred {message}");
                    ErrorOccurred?.Invoke();
                });
            });

            _hubConnection.On<string>("SendRoomTitle", (message) =>
            {
                _threadInvoker.InvokeOnMainThread(() =>
                {
                    RoomTitle?.Invoke(message);
                });
            });

            _hubConnection.Closed += ConnectionClosed;
            _hubConnection.Reconnecting += Reconnecting;
            _hubConnection.Reconnected += Reconnected;

            try
            {
                await _hubConnection.StartAsync();
                //("Connection established successfully.");
                _threadInvoker.InvokeOnMainThread(() =>
                {
                    WebsocketConnectionState?.Invoke(false);
                });
            }
            catch (Exception ex)
            {
                //($"Error starting connection: {ex.StackTrace}");
                // Handle connection failure
            }
        }



        private Task Reconnecting(Exception? arg)
        {
            _threadInvoker.InvokeOnMainThread(() =>
            {
               //("Reconnecting...");
            });
            return Task.CompletedTask;
        }

        public bool HasNoConnectionToHub()
        {
            if (_hubConnection is null) return true; 
            return !_hubConnection.State.Equals(HubConnectionState.Connecting);
        }

        private Task Reconnected(string? arg)
        {
            _threadInvoker.InvokeOnMainThread(() =>
            {
                ReconectedSuccessfully?.Invoke();
                WebsocketConnectionState?.Invoke(false);
            });
            return Task.CompletedTask;
        }

        private Task ConnectionClosed(Exception? exception)
        {
            _threadInvoker?.InvokeOnMainThread(() =>
            {
                //ConnectionLost?.Invoke();
                WebsocketConnectionState?.Invoke(true);
            });
            return Task.CompletedTask;
        }

        public async Task SendNotificationTokenAsnyc(string token)
        {
            if (_hubConnection is not null &&
                _hubConnection.State == HubConnectionState.Connected)
            {
                try
                {
                    await _hubConnection.InvokeAsync("ReceiveAppToken", token);
                }
                catch (Exception ex)
                {

					//($"Token Error: {ex.Message}");
				}
            }
        }
        public void CancelStream()
        {
            StreamToken.Cancel();
        }

        public async Task SendMessage(string message)
        {
            if (_hubConnection is not null && _hubConnection.State == HubConnectionState.Connected)
            {
                try
                {
                    StreamToken = new();
                    await foreach (var chunk in _hubConnection
                        .StreamAsync<string>("GenerateEvaStreamMessage", message, StreamToken.Token))
                    {
                        string tmp = chunk;
                        if (chunk.StartsWith("data:"))
                            tmp = chunk.Remove(0, 5);

                        //var test = chunk.Replace("data:", string.Empty);
                        //(test);
                        if (StreamToken.IsCancellationRequested) break;

						await Task.Delay(5);
						OnStream.Invoke(tmp);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //($"Error sending message: {ex.Message}");
                   // Optionally, you could attempt to reconnect or retry sending the message
                }
            }
            else
            {
               //("Connection is not established.");
                _threadInvoker.InvokeOnMainThread(() =>
                {
                    ConnectionLost?.Invoke();
                });
                // Optionally, handle reconnecting logic here
                //await EnsureConnectionStartedAsync();
            }
        }

        private async Task EnsureConnectionStartedAsync()
        {
            // Prevent concurrent access
            await _connectionLock.WaitAsync();
            try
            {
                if (_hubConnection.State == HubConnectionState.Disconnected)
                {
                    await _hubConnection.StartAsync();
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }





        /*WEBSOCKET END*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="file"></param>
        /// <returns></returns>



        public async Task<string> GetAllRooms()
        {
            try
            {
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

                HttpResponseMessage response = await _client
                    .GetAsync($"getAvailableRooms");

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    return content;
                }
            }
            catch (Exception e)
            {
               //("Request exception: " + e.Message);
            }
            return string.Empty;

        }



        public async Task<bool> RefreshTokenAsync(string RefreshToken)
        {
            var resuestData = new Dictionary<string, string>()
            {
                {"client_id", "EvaApp"},
                {"grant_type", "refresh_token"},
                {"refresh_token", RefreshToken}
            };

            try
            {
                var client = new HttpClient();

                var content = new FormUrlEncodedContent(resuestData);
                var res = await client
                    .PostAsync($"https://{Auth}/realms/{Realm}/protocol/openid-connect/token", content,
                    CancellationToken.None);

                if (!res.IsSuccessStatusCode)
                {
                   //($"Error: {res.StatusCode} - {res.ReasonPhrase}");
                    return false;
                }

                if (res.StatusCode != HttpStatusCode.BadRequest)
                {
                    var resultContent = await res.Content.ReadAsStringAsync();
                    KeycloackHelper = JsonSerializer.Deserialize<KeycloackHelper>(resultContent);

                    _session = KeycloackHelper?.access_token;

                    SafeToken?.Invoke(KeycloackHelper?.refresh_token);
                    UserName = RetreaveUserNameFromToken(Session);
                    UserMail = RetreaveUserMailFromToken(Session);
                    Sub = RetreaveUserSubFromToken(Session);

                    _shouldRefresh = true;
                    _ = RefreshTokenHeartbeat().ConfigureAwait(false);
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (HttpRequestException ex)
            {
                // Handle specific HttpRequestException, which includes network errors (like CORS issues)
               //($"Request error: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                // Handle timeout or cancellation
               //($"Request timed out: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle other exceptions if needed
               //($"Unexpected error: {ex}");
            }
            finally
            {
               //("Final Statement");
            }
            return false;
        }


        public string RetreaveUserSubFromToken(string secToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(secToken);

            return token.Claims.First(c => c.Type.Equals("subscription")).Value;
        }

        public string RetreaveUserNameFromToken(string secToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(secToken);

            return token.Claims.First(c => c.Type.Equals("name")).Value;
        }

        public string RetreaveUserMailFromToken(string secToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(secToken);

            return token.Claims.First(c => c.Type.Equals("email")).Value;
        }


        public async Task RefreshTokenHeartbeat()
        {
            //bool isValid = true;
            var httpClient = new HttpClient();


            while (_shouldRefresh)
            {
                await Task.Delay((KeycloackHelper.expires_in * 100) - 300);
                var requestData = new Dictionary<string, string>()
                {
                    {"client_id", "EvaApp"},
                    {"grant_type", "refresh_token"},
                    {"refresh_token", KeycloackHelper.refresh_token}
                };

                try
                {
                    if (!_shouldRefresh)
                    {
                        return;
                    }
                    var content = new FormUrlEncodedContent(requestData);
                    var response = await httpClient
                        .PostAsync($"https://{Auth}/realms/{Realm}/protocol/openid-connect/token", content);

                   //(response.StatusCode + " Status Update");
                    if (response.IsSuccessStatusCode)
                    {
                        var resultConent = await response.Content.ReadAsStringAsync();
                        KeycloackHelper = JsonSerializer.Deserialize<KeycloackHelper>(resultConent);

                        Session = KeycloackHelper?.access_token;

                        if (string.IsNullOrEmpty(UserName))
                        {
                            UserName = RetreaveUserNameFromToken(Session);
                            UserMail = RetreaveUserMailFromToken(Session);
                            Sub = RetreaveUserSubFromToken(Session);
                        }

                        SafeToken?.Invoke(KeycloackHelper?.refresh_token);
                    }
                    else
                    {
                        _shouldRefresh = false;
                    }
                }
                catch (Exception ex)
                {
                   //($"Error: {ex}");
                }
                finally
                {
                   //("Final Statement");
                }
            }
        }

        public async Task<bool> LoginAsync(LoginData login)
        {
            var httpClient = new HttpClient();

            var requestData = new Dictionary<string, string>()
            {
                {"grant_type", "password"},
                {"client_id", "EvaApp"},
                {"username", login.UserName},
                {"password", login.Password}
            };

            httpClient.BaseAddress = new Uri($"https://{Auth}");
            var content = new FormUrlEncodedContent(requestData);
            try
            {
               //($"Hostname: {httpClient.BaseAddress}");

                var response = await httpClient
                    .PostAsync($"/realms/{Realm}/protocol/openid-connect/token", content);

                if (response.IsSuccessStatusCode)
                {
                    var resultConent = await response.Content.ReadAsStringAsync();
                    if (resultConent != null)
                    {
                        KeycloackHelper = JsonSerializer.Deserialize<KeycloackHelper>(resultConent);
                        Session = KeycloackHelper?.access_token;



                        UserName = RetreaveUserNameFromToken(Session);
                        UserMail = RetreaveUserMailFromToken(Session);
                        Sub = RetreaveUserSubFromToken(Session);


                        SafeToken?.Invoke(KeycloackHelper?.refresh_token);
                    }

                    _shouldRefresh = true;
                    RefreshTokenHeartbeat().ConfigureAwait(false);

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }




        /*----------------------------Website Speicher---------------------------------------*/

        public async Task<string> GetWebsitesAsync()
        {
            try
            {
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

                HttpResponseMessage response = await _client
                    .GetAsync($"website");

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                   //(content);
                    return content;
                }
            }
            catch (HttpRequestException e)
            {
               //("Request exception: " + e.Message);
            }


            return string.Empty;
        }


        public async Task<bool> PostNewSite(string url, int depth)
        {
            try
            {
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");


                var response = await _client
                    .PostAsync($"website?url={Uri.EscapeDataString(url)}&depth={depth}&refresh=false", null);


                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch (HttpRequestException e)
            {
                return false;
               //($"{e.Message}");
            }
            return false;
        }

        public async Task<bool> UpdateWebsiteSync(Website site)
        {
            try
            {
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

                HttpResponseMessage response = await _client
                    .PutAsync($"website?id={site.Id}&refresh={site.ShouldRefresh}", null);

                if (response.IsSuccessStatusCode)
                {
                    

                    return true;
                }
            }
            catch (HttpRequestException e)
            {
               //("Request exception: " + e.Message);
            }
            return false;
        }

        public async Task<string> DeleteWebsiteAsync(string id)
        {
            try
            {
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

                HttpResponseMessage response = await _client.DeleteAsync($"website?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
               //(e.Message);
                return string.Empty;
            }
            return string.Empty;

        }

        /*-------------------------Instructions---------------------------------------------*/
        public async Task<string> GetInstAsync()
        {

            try
            {
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

                HttpResponseMessage response = await _client
                    .GetAsync($"inst");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                   //(content);
                    return content;
                }
            }
            catch (HttpRequestException e)
            {
               //("Request exception: " + e.Message);
            }


            return string.Empty;
        }


        public async Task<bool> PutInstAsync(CustomInstruction obj)
        {

            //var requestData = new InstructionHelper()
            //{
            //	custom_instructions = custom_instructions,
            //	response_instructions = response_instructions
            //};


            try
            {
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

                var jsonContent = JsonSerializer.Serialize(obj);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");


                HttpResponseMessage response = await _client
                    .PutAsync($"inst", content);

                if (response.IsSuccessStatusCode)
                {
                   //(response.Content.ReadAsStringAsync());
                    return true;
                }
            }
            catch (HttpRequestException e)
            {
               //("Request exception: " + e.Message);
            }
            return false;
        }

        public async Task<bool> DeleteInstAsync()
        {
            try
            {
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

                HttpResponseMessage response = await _client.DeleteAsync($"inst");
                if (response.IsSuccessStatusCode)
                {
                   //(response.Content.ReadAsStringAsync());
                    return true;
                }
            }
            catch (HttpRequestException e)
            {
               //($"{e.Message}");
            }
            return false;
        }

        /*--------------------------------Chat Room----------------------------------------*/

        public async Task<string> GetChatRoomsAsync(int amt, string roomId)
        {
            try
            {
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

                HttpResponseMessage response = await _client
                    .GetAsync($"messages/{roomId}/{amt}");

                string content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (HttpRequestException e)
            {
               //(e.Message);
            }


            return string.Empty;
        }


        public async Task<string> GetRoomByIdAsync(string roomId)
        {
            try
            {
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
                HttpResponseMessage res = await _client
                    .GetAsync($"getRoom?roomID={roomId}");
                if (res.IsSuccessStatusCode)
                {

                    string room = await res.Content.ReadAsStringAsync();
                    //Room newRoom = JsonSerializer.Deserialize<Room>(room);

                    return room;
                }
            }
            catch (Exception e)
            {
               //("Room not awailable");
            }
            return string.Empty;
        }
        public async Task<bool> DeleteChatRoomAsync(string roomID)
        {
            try
            {
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
                HttpResponseMessage response = await _client
                    .DeleteAsync($"{roomID}/delete");
                if (response.IsSuccessStatusCode)
                    return true;
            }
            catch (HttpRequestException e)
            {
               //(e.Message);
                return false;
            }
            return false;
        }

        public async Task<bool> RenameChatRoomAsync(string roomId, string title)
        {
            try
            {
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

                HttpResponseMessage response = await _client.PutAsync($"setRoomTitle?roomID={roomId}&title={title}", null);

                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
               //(e.Message);
            }
            return false;
        }


        public async Task<string> PostNewUserAsync(Registration data)
        {
            try
            {
                _client.DefaultRequestHeaders.Remove("Authorization");
                var requstData = JsonSerializer.Serialize(data);
                var content = new StringContent(requstData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync($"{Host}/account/api/v1/registerUser", content);
                string responseBody = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
                if (response.IsSuccessStatusCode)
                {
                    return "true";
                }
                if (responseData != null && responseData["message"] == "Email already exists")
                {
                    return "exists";
                }
            }
            catch (HttpRequestException e)
            {
               //($"{e.Message}");
            }
            return "false";
        }

        public async Task<string> SetRoomArchived(string roomId, bool archived)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            HttpResponseMessage response = await _client.PutAsync($"setRoomArchived?roomID={roomId}&archived={archived}",null);

            return response.StatusCode switch
            {
                HttpStatusCode.OK => "OK",
                HttpStatusCode.Unauthorized => string.Empty,
                _ => string.Empty
            };
        }
     
        public async Task<string> SetTrackChatContext(string roomId, bool trackContext)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            HttpResponseMessage response = await _client.PutAsync($"setTrackChatContext?roomID={roomId}&trackContext={trackContext}", null);

            return response.StatusCode switch
            {
                HttpStatusCode.OK => "OK",
                HttpStatusCode.Unauthorized => string.Empty,
                _ => string.Empty
            };
        }

        public async Task<string> GetArchivedRooms()
        {
            try
            {
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
                HttpResponseMessage res = await _client
                    .GetAsync("getArchivedRooms");
                if (res.IsSuccessStatusCode)
                {

                    string room = await res.Content.ReadAsStringAsync();
                    //Room newRoom = JsonSerializer.Deserialize<Room>(room);

                    return room;
                }
            }
            catch (Exception e)
            {
                //("Room not awailable");
            }
            return string.Empty;
        }

        /*-------------------------------Feedback----------------------------*/

        public async Task<bool> PostFeedbackAsync(string msgId, int vote, string feedback)
        {
            try
            {
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
				
                HttpResponseMessage response = await _client.PostAsync($"postFeedback?" +
                    $"messageID={msgId}&vote={vote}&description={feedback}", content);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch (HttpRequestException e)
            {
               //($"{e.Message}");
            }
            return false;
        }


        /*---------------------------File Management-------------------------*/
        public async Task<string> UploadAsync(Dictionary<string, FileUploadInfo> files)
        {
            var content = new MultipartFormDataContent();
            try
            {
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
                
                long MaxAllowedSize = 20 * 1024 * 1024;
                
                foreach(var elem in files)
                {
                    var Stream = new MemoryStream(elem.Value.Data);
              
                    var StreamContent = new StreamContent(Stream);
                    StreamContent.Headers.ContentType = new MediaTypeHeaderValue(elem.Value.ContentType);
                    content.Add(StreamContent, "files", elem.Value.Name);
                }

                var response = await _client.PostAsync($"uploadFiles", content);
                if (response.IsSuccessStatusCode)
                {
                    //_client.Timeout = defTimeout;
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
               //("Request exception: " + e.Message);
            }
            return string.Empty;
        }

        public async Task<string> GetAllFilesAsync()
        {
            try
            {

                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

                var response = await _client.GetAsync($"files");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
               //(e.Message);
                return string.Empty;
            }
            return string.Empty;
        }


        public async Task<bool> DeleteFilesAsync(string id, string roleId)
        {
            try
            {

                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

                var response = await _client.DeleteAsync($"files?fileID={id}&roleID={roleId}");
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
               //(e.Message);
                return false;
            }
            return false;
        }
        public async Task<byte[]> GetPDFBase64(string id)
        {
            try
            {

                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

                var response = await _client.GetAsync($"file?fileID={id}");

                if (response.IsSuccessStatusCode)
                {
                    byte[] test = await response.Content.ReadAsByteArrayAsync();
                   //(Convert.ToBase64String(test));
                   //("TEST DISPOS: " + test.Length);
                    return await response.Content.ReadAsByteArrayAsync();
                }
            }
            catch (Exception e)
            {
               //(e.Message);
                return new byte[0];
            }
            return new byte[0];
        }


        //-------------- New Endpoint ------------>
        public async Task<string> PostFileWithAnnotation(string id,List<string> chunks)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            var chunksList = JsonSerializer.Serialize(chunks);
            var content = new StringContent(chunksList, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"fileWithAnnotations/{id}", content);
            if(response.IsSuccessStatusCode)
            {
                var contentPDF =  await response.Content.ReadAsStringAsync();            
                return contentPDF;
            }
            return string.Empty;
        }



        /*----------------------------User Management-------------------------------------*/

        public async Task<string> GetUserInfoAsync()
        {
            try
            {

                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

                var response = await _client.GetAsync($"getUserResources");
                
                if (response.IsSuccessStatusCode)
                {
                    //(await response.Content.ReadAsStringAsync());
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
               //(e.Message);
                return string.Empty;
            }
            return string.Empty;
        }



        public async Task<bool> LogoutAsync()
        {
            var requestData = new Dictionary<string, string>()
            {
                {"client_id", "EvaApp" },
                {"refresh_token", KeycloackHelper.refresh_token }
            };
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://auth.splitbot.de");
            var content = new FormUrlEncodedContent(requestData);
            try
            {
                var response = await httpClient.PostAsync($"/realms/{Realm}/protocol/openid-connect/logout", content);
                if (response.IsSuccessStatusCode)
                {
                    _shouldRefresh = false;
                    return true;
                }
                return false;
            }
            catch (HttpRequestException e)
            {
               //(e.Message);
            }
            return false;
        }
        /*----------------------------NextCloud-------------------------------------*/

        public async Task<string> PostSelectedItemsAsync(CloudImportLists files, string accountID)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            var requstData = JsonSerializer.Serialize(files, _jsonSerializerIgnore);
            var content = new StringContent(requstData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync($"updateSelectedItems?accountID={accountID}", content);
            return response.StatusCode switch
            {
                HttpStatusCode.OK => "OK",
                HttpStatusCode.Unauthorized => "Unauthorized",
                _ => string.Empty
            };

        }
        public async Task<string> PostauthQrCodeAsync(string qr)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            HttpResponseMessage response = await _client.PostAsync($"authQrCode?qrScan={qr}", null);
            return response.StatusCode switch
            {
                HttpStatusCode.OK => "OK",
                HttpStatusCode.NotFound => "NotFound",
                _ => string.Empty
            };

        }
        public async Task<string> PostNextcloudAccountAsync(NextCloudAccountRegistration data)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            var requstData = JsonSerializer.Serialize(data);
            var content = new StringContent(requstData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("addNextcloudAccount", content);
            return response.StatusCode switch
            {
                HttpStatusCode.OK => "OK",
                HttpStatusCode.NotFound => "NotFound",
                HttpStatusCode.Unauthorized => "Unauthorized",
                HttpStatusCode.Conflict => "Conflict",
                _ => string.Empty
            };

        }
        public async Task<string> StartNextCloudSyncAsync(string accountId)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            HttpResponseMessage response = await _client.GetAsync($"nextcloudSync?accountID={accountId}");
            return response.StatusCode switch
            {
                HttpStatusCode.OK => await response.Content.ReadAsStringAsync(),
                HttpStatusCode.Unauthorized => "Unauthorized",
                _ => string.Empty
            };

        }
        public async Task<string> GetSelectedItemsAsync(string accountId)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            HttpResponseMessage response = await _client.GetAsync($"getSelectedItems?accountID={accountId}");
            return response.StatusCode switch
            {
                HttpStatusCode.OK => await response.Content.ReadAsStringAsync(),
                HttpStatusCode.Unauthorized => "Unauthorized",
                _ => string.Empty
            };
        }
        public async Task<string> GetNextcloudFolderAsync(string accountId, string folder)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            HttpResponseMessage response = await _client.GetAsync($"getNextcloudFolder?accountID={accountId}&folder={folder}");
            return response.StatusCode switch
            {
                HttpStatusCode.OK => await response.Content.ReadAsStringAsync(),
                HttpStatusCode.Unauthorized => "Unauthorized",
                _ => string.Empty
            };
        }

        public async Task<string> GetAllNextCloudAccountsAsync()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            HttpResponseMessage response = await _client
                .GetAsync("getAllNextcloudAccounts");
            return response.StatusCode switch
            {
                HttpStatusCode.OK => await response.Content.ReadAsStringAsync(),
                HttpStatusCode.Unauthorized => "Unauthorized",
                _ => string.Empty
            };

        }
        public async Task<string> DeleteNextCloudAccountAsync(string accountId)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            HttpResponseMessage response = await _client.DeleteAsync($"removeNextcloudAccount?accountID={accountId}");
            return response.StatusCode switch
            {
                HttpStatusCode.OK => "OK",
                HttpStatusCode.Unauthorized => "Unauthorized",
                _ => string.Empty
            };
        }

        /*--------------------------- E-Mail START -------------------------*/

        public async Task<string> IMAPRegistrationAsync(Models.Mail.IMAP data)
        {

            _clientEmail.DefaultRequestHeaders.Remove("Authorization");
            _clientEmail.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            var requstData = JsonSerializer.Serialize(data);
            var content = new StringContent(requstData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _clientEmail.PostAsync($"auth/imap", content);
            var Status = response.StatusCode.ToString();
            return Status;
        }

        public async Task<bool> GoogleOauthAsync()
        {
            _clientEmail.DefaultRequestHeaders.Remove("Authorization");
            _clientEmail.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            HttpResponseMessage response = await _clientEmail.GetAsync($"oauth/google/login");
            if (response.IsSuccessStatusCode)
            {
                _evm.GoogleOauth = await response.Content.ReadAsStringAsync();
               //(_evm.GoogleOauth);
                return true;
            }
            return false;
        }

        public async Task<string> GetMailAccountsAsync()
        {
            _clientEmail.DefaultRequestHeaders.Remove("Authorization");
            _clientEmail.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            HttpResponseMessage response = await _clientEmail.GetAsync($"getMailAccounts");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
               //(content);
                //_evm.EmailList = JsonSerializer.Deserialize<ObservableCollection<Email>>(content);
                return content;
                //return true;
            }
            return string.Empty;
        }

        public async Task<bool> SetCrawlRetentionAsync(string id, int retention)
        {
            _clientEmail.DefaultRequestHeaders.Remove("Authorization");
            _clientEmail.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            HttpResponseMessage response = await _clientEmail.PutAsync($"setCrawlRetention?accountID={id}&crawlRetention={retention}", null);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> ActivateAccountAsync(string id, bool active)
        {
            _clientEmail.DefaultRequestHeaders.Remove("Authorization");
            _clientEmail.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            HttpResponseMessage response = await _clientEmail.PutAsync($"activateAccount?accountID={id}&activate={active}", null);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteAccountAsync(string id)
        {
            _clientEmail.DefaultRequestHeaders.Remove("Authorization");
            _clientEmail.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            HttpResponseMessage response = await _clientEmail.DeleteAsync($"deleteAccount?accountID={id}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> SetAttachment(string id, bool attachment)
        {
            _clientEmail.DefaultRequestHeaders.Remove("Authorization");
            _clientEmail.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            HttpResponseMessage response = await _clientEmail.PutAsync($"setAttachment?accountID={id}&attachment={attachment}",null);
            if (response.IsSuccessStatusCode)
            {
                return true;

            }
            return false;

        }
        /*--------------------------- ScheduledTask -------------------------*/

        public async Task<bool> EditScheduledTaskAsync(ScheduledTask data)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PutAsync($"updateScheduledTask", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> AddScheduledTaskAsync(ScheduledTask data)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync($"addScheduledTask", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<string> GetScheduledTaskAsync(string roomid)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

            HttpResponseMessage response = await _client.GetAsync($"getScheduledTaskByRoomID?roomID={roomid}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;

            }
            return string.Empty;
        }

        public async Task<string> GetAllScheduledTaskAsync()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

            HttpResponseMessage response = await _client.GetAsync($"getAllScheduledTask");
            return response.StatusCode switch
            {
                HttpStatusCode.OK => await response.Content.ReadAsStringAsync(),
                HttpStatusCode.Unauthorized => "Unauthorized",
                _ => string.Empty
            };
        }

        public async Task<string> DeleteScheduledTaskByRoomIdAsync(string roomId)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

            HttpResponseMessage response = await _client.DeleteAsync($"removeScheduledTaskByRoomID?roomID={roomId}");
            return response.StatusCode switch
            {
                HttpStatusCode.OK => "OK",
                HttpStatusCode.Unauthorized => "Unauthorized",
                _ => string.Empty
            };
        }
        public async Task<string> DeleteScheduledTaskByJobIdAsync(string JobId)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

            HttpResponseMessage response = await _client.DeleteAsync($"removeScheduledTask?jobID={JobId}");
            return response.StatusCode switch
            {
                HttpStatusCode.OK =>"OK",
                HttpStatusCode.Unauthorized => "Unauthorized",
                _ => string.Empty
            };
        }
        public async Task<bool> DisableRoomBadgAsync(string roomid)
        {
            try
            {
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
                HttpResponseMessage response = await _client
                    .PutAsync($"disableRoomBadge?roomID={roomid}", null);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
               //("Room not awailable");
            }
            return false;
        }
        /*--------------------------- OneDrive -------------------------*/
        public async Task<string> GetOneDriveAccountsAsync()
        {
			_client.DefaultRequestHeaders.Remove("Authorization");
			_client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
			HttpResponseMessage response = await _client
				.GetAsync("onedrive/info");
			return response.StatusCode switch
			{
				HttpStatusCode.OK => await response.Content.ReadAsStringAsync(),
				HttpStatusCode.Unauthorized => "Unauthorized",
				_ => string.Empty
			};
		}
        
        public async Task<string> GetOneDriveItems()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            HttpResponseMessage response = await _client.GetAsync("onedrive/items");
            return response.StatusCode switch
            {
                HttpStatusCode.OK => await response.Content.ReadAsStringAsync(),
                HttpStatusCode.Unauthorized => "Unauthorized",
                _ => string.Empty
            };
        }
        public async Task<string> PostOneDriveItems(CloudImportLists files)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            var json = JsonSerializer.Serialize(files,_jsonSerializerIgnore);
            var Content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("onedrive/items", Content);
            return response.StatusCode switch
            {
                HttpStatusCode.OK => "OK",
                HttpStatusCode.Unauthorized => "Unauthorized",
                _ => string.Empty
            };
        }
        public async Task<string> StartOneDriveSync()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            HttpResponseMessage response = await _client.GetAsync("onedrive/sync");
            return response.StatusCode switch
            {
                HttpStatusCode.OK => await response.Content.ReadAsStringAsync(),
                HttpStatusCode.Unauthorized => "Unauthorized",
                _ => string.Empty
            };
        }

        public async Task<string> GetOneDriveDirectory(string driveId, string RemoteId)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            HttpResponseMessage response = await _client.GetAsync($"onedrive/folder?drive={driveId}&folderID={RemoteId}");
            //(await response.Content.ReadAsStringAsync());
            return response.StatusCode switch
            {   
                HttpStatusCode.OK => await response.Content.ReadAsStringAsync(),
                _ => "Erorr"
            };
        }

        public async Task<string> GetOneDriveAuth()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            HttpResponseMessage response = await _client.GetAsync("onedrive/auth");
            return response.StatusCode switch
            {
                HttpStatusCode.OK => await response.Content.ReadAsStringAsync(),
                _ => string.Empty
            };
        }

        public async Task<string> DeleteOndriveAsync()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            HttpResponseMessage response = await _client.DeleteAsync("onedrive");
            return response.StatusCode switch
            {
                HttpStatusCode.OK => await response.Content.ReadAsStringAsync(),
                _ => string.Empty
            };
        }

        /*--------------------------- OutLook -------------------------*/


        public async Task<string> PostAuthOutLook(OutLookAuth data)
        {
            try
            {

                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
                var requstData = JsonSerializer.Serialize(data);
                var content = new StringContent(requstData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync($"outlook/auth", content);
                return response.StatusCode switch
                {
                    HttpStatusCode.OK => "OK",
                    HttpStatusCode.NotFound => "NotFound",
                    HttpStatusCode.Unauthorized => "Unauthorized",
                    _ => string.Empty
                };

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return string.Empty;
            }



        }
        public async Task<string> GetKalenderAuth()
        {
            try
            {
                _client.DefaultRequestHeaders.Remove("Authorization");
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
                HttpResponseMessage response = await _client.GetAsync("outlook/calendar");
                return response.StatusCode switch
                {
                    HttpStatusCode.OK => await response.Content.ReadAsStringAsync(),
                    HttpStatusCode.NotFound => "NotFound",
                    HttpStatusCode.Unauthorized => "Unauthorized",
                    _ => string.Empty
                };
            }
            catch
            {
                return string.Empty;
            }

        }
        public async Task<bool> DeleteKalenderAuth()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            HttpResponseMessage response = await _client.DeleteAsync("outlook/auth");

            return response.StatusCode switch
            {
                HttpStatusCode.OK => true,
                _ => false
            };

        }
        /*--------------------------- Monitoring -------------------------*/
        public async Task<string> PostMonitors(CreateAgent data)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");
            var requstData = JsonSerializer.Serialize(data);
            var content = new StringContent(requstData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync($"monitoring/createMonitoringAgent", content);
            return response.StatusCode switch
            {
                HttpStatusCode.OK => await response.Content.ReadAsStringAsync(),
                HttpStatusCode.NotFound => "NotFound",
                HttpStatusCode.Unauthorized => "Unauthorized",
                _ => string.Empty
            };
        }
        public async Task<string> GetMonitors()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

            HttpResponseMessage response = await _client.GetAsync("monitoring/monitors");
            return response.StatusCode switch
            {
                HttpStatusCode.OK => await response.Content.ReadAsStringAsync(),
                _ => "Error"
            };

        }

        public async Task<string> DeleteMonitor(string url)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

            HttpResponseMessage response = await _client.DeleteAsync($"monitoring/monitors?url={url}");
            return response.StatusCode.ToString();

        }
 
        public async Task<string> GetMonitorsByRoomID(string roomid)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

            HttpResponseMessage response = await _client.GetAsync($"monitoring/monitorsbyroomid?roomId={roomid}");
            return response.StatusCode switch
            {
                HttpStatusCode.OK => await response.Content.ReadAsStringAsync(),
                _ => "Error"
            };

        }
        public async Task<string> PostNewLinkToAgent(AddLinkToMonitoringAgent data)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

            var requstData = JsonSerializer.Serialize(data);
            var content = new StringContent(requstData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync($"monitoring/addWebsitesToMonitorAgent", content);
            return response.StatusCode switch
            {
                HttpStatusCode.OK => "OK",
                HttpStatusCode.NotFound => "NotFound",
                HttpStatusCode.Unauthorized => "Unauthorized",
                _ => string.Empty
            };

        }
        public async Task<string> DeleteAllMonitor(string roomId)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Session}");

            HttpResponseMessage response = await _client.DeleteAsync($"monitoring/deleteMonitoringAgent?roomId={roomId}");
            return response.StatusCode switch
            {
                HttpStatusCode.OK => "OK",
                HttpStatusCode.NotFound => "NotFound",
                HttpStatusCode.Unauthorized => "Unauthorized",
                _ => string.Empty
            };

        }

    }
}