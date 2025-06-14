using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;
using System.Text;

namespace kopinang_api.Services
{
    public class FirestoreService
    {
        private readonly FirestoreDb _firestoreDb;

        public FirestoreService(IConfiguration configuration)
        {
            // Ambil dari env var FIREBASE_CREDENTIAL_B64
            string base64Credential = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIAL_B64");
            string jsonCredential = Encoding.UTF8.GetString(Convert.FromBase64String(base64Credential));

            // Buat credential dari string JSON
            var credential = GoogleCredential.FromJson(jsonCredential);

            // Ambil ProjectId dari config
            string projectId = configuration["Firestore:ProjectId"];

            // Inisialisasi FirestoreDb
            var dbBuilder = new FirestoreDbBuilder
            {
                ProjectId = projectId,
                Credential = credential
            };
            _firestoreDb = dbBuilder.Build();
        }

        public CollectionReference GetPromoCollection()
        {
            return _firestoreDb.Collection("promo");
        }

        public async Task<List<Dictionary<string, object>>> GetAllPromosAsync()
        {
            var promos = new List<Dictionary<string, object>>();
            QuerySnapshot snapshot = await _firestoreDb.Collection("promo").GetSnapshotAsync();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                if (doc.Exists)
                    promos.Add(doc.ToDictionary());
            }

            return promos;
        }
    }
}
