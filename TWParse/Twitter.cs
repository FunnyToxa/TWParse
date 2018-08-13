using Tweetinvi;
using Tweetinvi.Models;

namespace TWParse
{
    public class Twitter
    {
        //ключик для подключения приложения к Twitter
        private string conKey = "XYNpO5yj0shMgH43j4lYKMDfH";
        private string conSecret = "VevyEwrhHxabcQgN2S0KuL1i9Gx9CnPXyM2yVLfQ0LlJSZ7BmF";

        private IAuthenticationContext authenticationContext;

        private IUser user;

        public Twitter()
        {
            //В конструкторе сразу запускаем получение пинкода для авторизации (незнаю насколько это хорошая идея, но пока так
        }

        public string RequestAuth()
        {
            // Create a new set of credentials for the application.
            var appCredentials = new TwitterCredentials(conKey, conSecret);
            // Init the authentication process and store the related `AuthenticationContext`.
            authenticationContext = AuthFlow.InitAuthentication(appCredentials);
            // Go to the URL so that Twitter authenticates the user and gives him a PIN code.
            if (authenticationContext is null)
                return "";
            return authenticationContext.AuthorizationURL;
        }

        //функция авторизации в Twitter по пингоду
        public bool Auth(string pinCode)
        {
            // With this pin code it is now possible to get the credentials back from Twitter
            var userCredentials = AuthFlow.CreateCredentialsFromVerifierCode(pinCode, authenticationContext);
            if (userCredentials is null)
                return false;
            // Use the user credentials in your application
            Tweetinvi.Auth.SetCredentials(userCredentials);
            return true;
        }

        //Ищем пользователя
        private bool SearchUser(string userName)
        {
            user = User.GetUserFromScreenName(userName);
            if (user is null)
                return false;
            return true;
        }

        //получаем maxTweets кол-во твитов пользователя userName
        public string GetTweetsText(string userName, int maxTweets)
        {
            //ищем пользователя 
            if (!SearchUser(userName))
                return ""; //если пользователь не найден - выдаем пустой текст

            //var tweets = Timeline.GetHomeTimeline(homeTimelineParameter);
            var tweets = Timeline.GetUserTimeline(user.Id, maximumTweets: maxTweets);

            //собираем весь текст твитов в одну переменную
            string allText = "";
            foreach (var tweet in tweets)
            {
                allText += tweet.FullText;
            }
            return allText;
        }

        //Публикация твита
        public bool PublishTweet(string tweetText)
        {
            var tweet = Tweet.PublishTweet(tweetText);
            return tweet is null ? false : true;
        }
    }
}