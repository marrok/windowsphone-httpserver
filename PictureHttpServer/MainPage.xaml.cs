using HttpServer;
using HttpServer.Helpers;
using HttpServer.Logger;
using HttpServer.Messages;
using HttpServer.Messages.Request;
using HttpServer.Messages.Response;
using HttpServer.Messages.Response.Content;
using HttpServer.StaticResource;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace PictureHttpServer
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Server server;
        private readonly MediaLibrary lib = new MediaLibrary();
        private readonly static IEnumerable<string> _logLevelsList = Enum.GetNames(typeof(LogLevel));
        private Boolean _serverCreated;
        private Boolean _serverStarted;
        private static readonly string USER_LOGIN_KEY = "userLogin";
        private static readonly string USER_PASSWORD_KEY = "userPassword";
        private static readonly string AUTH_ENABLED_KEY = "authEnabled";
        private static readonly string LOG_LEVEL_KEY = "logLevel";

        public MainPage()
        {
            InitializeComponent();
            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;

            LoggingLevel.ItemsSource = _logLevelsList;
            
            LockGui();
            _serverStarted = false;

            var ipadresses = "192.168.117.136";// IpAddressHelper.FetchIpAddress();
            if (ipadresses == null)
            {
                StartStopServer.IsEnabled = false;
                Notification.Text = "Your Windows Phone is not connected to Wi-Fi.\nPlease connect it to your Wi-Fi router and restart this application";
                MessageBox.Show("Your Windows Phone is not connected to Wi-Fi.\nPlease connect it to your Wi-Fi router and restart this application", "Not connected to Wi-Fi", MessageBoxButton.OK);
                _serverCreated = false;
            }
            else
            {
                server = new Server(LogLevel.INFO, message =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        LogBox.Text = message;
                    });
                }, 50);

                server.AddAllowLocalInterfaceIpConnectionFilter(ipadresses);
                server.EnableConnectionFilter(true);

                if (IsolatedStorageSettings.ApplicationSettings.Contains(USER_LOGIN_KEY) && IsolatedStorageSettings.ApplicationSettings.Contains(USER_PASSWORD_KEY))
                {
                    UserLogin.Text = IsolatedStorageSettings.ApplicationSettings[USER_LOGIN_KEY] as string;
                    UserPassword.Text = IsolatedStorageSettings.ApplicationSettings[USER_PASSWORD_KEY] as string;
                }         
                server.SetAuthCredidentials(UserLogin.Text, UserPassword.Text);

                if (IsolatedStorageSettings.ApplicationSettings.Contains(LOG_LEVEL_KEY))
                {
                    server.ChangeLoggerLevel((LogLevel)IsolatedStorageSettings.ApplicationSettings[LOG_LEVEL_KEY]);
                }

                var authEnabled = IsolatedStorageSettings.ApplicationSettings.Contains(AUTH_ENABLED_KEY) && ((Boolean) IsolatedStorageSettings.ApplicationSettings[AUTH_ENABLED_KEY]);
                SecureWithCredidentials.IsChecked = authEnabled;
                server.EnableAuthFilter(authEnabled);
                
                Notification.Text = "Please type this ip address in your web browser: " + ipadresses;

                RegisterStaticResources();
                RegisterWPPicturesHandlers();

                server.Start();

                StartStopServer.IsEnabled = true;
                StartStopServer.IsChecked = true;
                _serverStarted = true;
                _serverCreated = true;
            }
        }
        
        private void LockGui()
        {
            ChangeGuiState(false);
        }

        private void UnlockGui()
        {
            ChangeGuiState(true);
        }

        private void ChangeGuiState(Boolean locked)
        {
            LoggingLevel.IsEnabled = locked;
            SetLoginPassowrd.IsEnabled = locked;
            AllowMobileConnections.IsEnabled = locked;
            SecureWithCredidentials.IsEnabled = locked;
            UserLogin.IsEnabled = locked;
            UserPassword.IsEnabled = locked;
        }

        private void StartStopServer_Checked(object sender, RoutedEventArgs e)
        {
            if (_serverCreated && !_serverStarted)
            {
                server.Start();
                UnlockGui();
                _serverStarted = true;
                StartStopServer.Content = "running";
            }
        }

        private void StartStopServer_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_serverCreated && _serverStarted)
            {
                server.Stop();
                LockGui();
                _serverStarted = false;
                StartStopServer.Content = "stopped";
            }
        }

        private void LoggingLevel_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (_serverCreated)
            {
                LogLevel newLogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), LoggingLevel.SelectedItem as string);
                server.ChangeLoggerLevel(newLogLevel);
                IsolatedStorageSettings.ApplicationSettings[LOG_LEVEL_KEY] = newLogLevel;
            }
        }

        private void SetLoginPassowrd_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (_serverCreated)
            {
                IsolatedStorageSettings.ApplicationSettings[USER_LOGIN_KEY] = UserLogin.Text;
                IsolatedStorageSettings.ApplicationSettings[USER_PASSWORD_KEY] = UserPassword.Text;
                IsolatedStorageSettings.ApplicationSettings.Save();
                server.SetAuthCredidentials(UserLogin.Text, UserPassword.Text);
            }
        }

        private void CheckBox_Unchecked_1(object sender, RoutedEventArgs e)
        {
            if (_serverCreated)
            {
                server.EnableConnectionFilter(true);
            }
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            if (_serverCreated)
            {
                server.EnableConnectionFilter(false);
            }
        }

        private void SecureWithCredidentials_Checked(object sender, RoutedEventArgs e)
        {
            if (_serverCreated)
            {
                IsolatedStorageSettings.ApplicationSettings[AUTH_ENABLED_KEY] = true;
                IsolatedStorageSettings.ApplicationSettings.Save();
                server.EnableAuthFilter(true);
            }
        }

        private void SecureWithCredidentials_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_serverCreated)
            {
                IsolatedStorageSettings.ApplicationSettings[AUTH_ENABLED_KEY] = false;
                IsolatedStorageSettings.ApplicationSettings.Save();
                server.EnableAuthFilter(false);
            }
        }

        private void RateButton_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var rateTask = new MarketplaceReviewTask();
            rateTask.Show();
        }

        private void EmailUsButton_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var emailComposeTask = new EmailComposeTask() { Subject = "Photo Server Feedback Email", To = "mariusz.rokicki@outlook.com" };
            emailComposeTask.Show();
        }

        private void RegisterStaticResources()
        {
            server.RegisterStaticResource("Assets/Static/JavaScript/jquery.js", "/static/js/jquery.js", StaticResourceType.JAVA_SCRIPT);
            server.RegisterStaticResource("Assets/Static/JavaScript/jquery.lazyload.js", "/static/js/jquery.lazyload.js", StaticResourceType.JAVA_SCRIPT);
            server.RegisterStaticResource("Assets/Static/JavaScript/jquery.prettyPhoto.js", "/static/js/jquery.prettyPhoto.js", StaticResourceType.JAVA_SCRIPT);

            server.RegisterStaticResource("Assets/Static/Css/windowsPhoneGallery.css", "/static/css/windowsPhoneGallery.css", StaticResourceType.CSS);
            server.RegisterStaticResource("Assets/Static/Css/prettyPhoto.css", "/static/css/prettyPhoto.css", StaticResourceType.CSS);

            server.RegisterStaticResource("Assets/Static/Images/bg.jpg", "/static/images/bg.jpg", StaticResourceType.JPG);
            server.RegisterStaticResource("Assets/Static/Images/content-bg.jpg", "/static/images/content-bg.jpg", StaticResourceType.JPG);
            server.RegisterStaticResource("Assets/Static/Images/content-bottom.jpg", "/static/images/content-bottom.jpg", StaticResourceType.JPG);
            server.RegisterStaticResource("Assets/Static/Images/content-top.jpg", "/static/images/content-top.jpg", StaticResourceType.JPG);

            server.RegisterStaticResource("Assets/Static/Images/PrettyPhoto/default_thumb.png", "/static/images/prettyPhoto/default_thumb.png", StaticResourceType.PNG);
            server.RegisterStaticResource("Assets/Static/Images/PrettyPhoto/sprite.png", "/static/images/prettyPhoto/sprite.png", StaticResourceType.PNG);
            server.RegisterStaticResource("Assets/Static/Images/PrettyPhoto/sprite_next.png", "/static/images/prettyPhoto/sprite_next.png", StaticResourceType.PNG);
            server.RegisterStaticResource("Assets/Static/Images/PrettyPhoto/sprite_prev.png", "/static/images/prettyPhoto/sprite_prev.png", StaticResourceType.PNG);
            server.RegisterStaticResource("Assets/Static/Images/PrettyPhoto/sprite_x.png", "/static/images/prettyPhoto/sprite_x.png", StaticResourceType.PNG);
            server.RegisterStaticResource("Assets/Static/Images/PrettyPhoto/sprite_y.png", "/static/images/prettyPhoto/sprite_y.png", StaticResourceType.PNG);

            server.RegisterStaticResource("Assets/Static/Images/PrettyPhoto/loader.gif", "/static/images/prettyPhoto/loader.gif", StaticResourceType.GIF);

            server.RegisterStaticResource("Assets/Static/Images/LazyLoad/grey.png", "/static/images/lazyload/grey.png", StaticResourceType.PNG);

            //server.RegisterStaticResource("Assets/Static/Images/headerImage.png", "/favicon.ico", StaticResourceType.PNG);
        }

        private void RegisterWPPicturesHandlers()
        {
            server.RegisterHandler(HttpRequestType.GET, "/", false, request =>
            {
                var htmlBuilder = new StringBuilder();
                var ipAddress = IpAddressHelper.FetchIpAddress();

                htmlBuilder.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">");
                htmlBuilder.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\" lang=\"en\">");
                htmlBuilder.Append("<head>");
                htmlBuilder.Append("<title>Windows Phone Photo Server</title>");
                htmlBuilder.Append("<meta http-equiv=\"Content-Type\" content=\"text/html;charset=utf-8\" />");
                htmlBuilder.Append("<link rel=\"stylesheet\" href=\"/static/css/prettyPhoto.css\" type=\"text/css\" charset=\"utf-8\" />");
                htmlBuilder.Append("<link rel=\"stylesheet\" href=\"/static/css/windowsPhoneGallery.css\" type=\"text/css\" charset=\"utf-8\" />");
                htmlBuilder.Append("<script type=\"text/javascript\" src=\"/static/js/jquery.js\" charset=\"utf-8\"></script>");
                htmlBuilder.Append("<script type=\"text/javascript\" src=\"/static/js/jquery.prettyPhoto.js\" charset=\"utf-8\"></script>");
                htmlBuilder.Append("<script type=\"text/javascript\" src=\"/static/js/jquery.lazyload.js\" charset=\"utf-8\"></script>");
                htmlBuilder.Append("</head>");
                htmlBuilder.Append("<body>");
                htmlBuilder.Append("<div class=\"wpg_gallery\">");
                htmlBuilder.Append("<div class=\"wpg_top\"></div>");
                htmlBuilder.Append("<div class=\"wpg_images_container\">");
                htmlBuilder.Append("<ul class=\"wpg_thumbnails\">");

                foreach (var pic in lib.Pictures)
                {
                    htmlBuilder.Append("<li><a rel=\"photoGallery[windowsPhoneGallery]\" title=\"title\" href=\"images/large/").Append(pic.Name).Append("\" rel=\"prettyPhoto\">");
                    htmlBuilder.Append("<img class=\"lazy\" data-original=\"images/thumbnails/").Append(pic.Name).Append("\" src=\"static/images/lazyload/grey.png\" width=\"765\" height=\"574\"></a></li>");
                }

                htmlBuilder.Append("</ul>");
                htmlBuilder.Append("</div>");
                htmlBuilder.Append("<div class=\"wpg_bottom\"></div>");
                htmlBuilder.Append("</div>");
                htmlBuilder.Append("<script charset=\"utf-8\" type=\"text/javascript\">");
                htmlBuilder.Append("$(function() {");
                htmlBuilder.Append("$(\"a[rel^='photoGallery']\").prettyPhoto({social_tools:'', gallery_markup:''});");
                htmlBuilder.Append("$(\"img.lazy\").lazyload({");
                htmlBuilder.Append("effect : \"fadeIn\"");
                htmlBuilder.Append("});");
                htmlBuilder.Append("});");
                htmlBuilder.Append("</script>");
                htmlBuilder.Append("</body>");
                htmlBuilder.Append("</html>");

                return HttpResponseFactory.CreateResponse(HttpStatusCode.Ok, htmlBuilder.ToString(), typeof(HtmlResponseContent));
            });

            server.RegisterHandler(HttpRequestType.GET, "/images/large/*", true, request =>
            {
                var pictureName = request.Uri.Replace("/images/large/", string.Empty);

                var picture = lib.Pictures.FirstOrDefault(pic => pic.Name.Equals(pictureName));

                if (picture == null)
                {
                    return HttpResponseFactory.CreateHttpNotFoundResponse();
                }
                byte[] buffer;

                using (var image = picture.GetImage())
                {
                    buffer = new byte[image.Length];
                    image.Read(buffer, 0, buffer.Length);
                }

                return HttpResponseFactory.CreateResponse(HttpStatusCode.Ok, buffer, typeof(PngResponseContent));
            });

            server.RegisterHandler(HttpRequestType.GET, "/images/thumbnails/*", true, request =>
            {
                var pictureName = request.Uri.Replace("/images/thumbnails/", string.Empty);

                var picture = lib.Pictures.FirstOrDefault(pic => pic.Name.Equals(pictureName));

                if (picture == null)
                {
                    return HttpResponseFactory.CreateHttpNotFoundResponse();
                }
                byte[] buffer;

                using (var image = picture.GetThumbnail())
                {
                    buffer = new byte[image.Length];
                    image.Read(buffer, 0, buffer.Length);
                }

                return HttpResponseFactory.CreateResponse(HttpStatusCode.Ok, buffer, typeof(PngResponseContent));
            });
        }
    }
}