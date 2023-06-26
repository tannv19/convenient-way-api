using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace ship_convenient.Config
{
    public static class FirebaseExtension
    {
        public static void AddFirebaseApp(this IServiceCollection services) {
            FirebaseApp.Create(new AppOptions()
            { 

                Credential = GoogleCredential.FromFile("Config/Firebase/convenient-way-firebase-adminsdk-t8g11-38ee5771a0.json")
            });
        }
    }
}
