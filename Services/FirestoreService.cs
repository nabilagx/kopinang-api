using Google.Cloud.Firestore;

namespace kopinang_api.Services
{
    public class FirestoreService
    {
        private readonly FirestoreDb _firestoreDb;

        public FirestoreService(IConfiguration configuration)
        {
            // Set environment variable for credentials
            string credentialPath = Path.Combine(AppContext.BaseDirectory, "kopinang-460316-71bf05d4e71e.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

            // Create FirestoreDb instance
            string projectId = configuration["Firestore:ProjectId"]; // "kopinang-460316"
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        public CollectionReference GetPromoCollection()
        {
            return _firestoreDb.Collection("promo");
        }

        // Misalnya ambil semua promo
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
