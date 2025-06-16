Tentu, ini dia versi lengkap dan rapi dari `README.md` untuk repositori `kopinang-api` Anda, siap untuk langsung di-copy-paste ke GitHub:

# ☕ KOPI NANG API

**KOPI NANG** adalah *RESTful API* berbasis **ASP.NET Core** dan **PostgreSQL** yang dikembangkan oleh **Kelompok 5** dalam rangka penyelesaian tugas akhir mata kuliah **Pemrograman Antarmuka Aplikasi (PAA) - Kelas B** di **Universitas Jember**.

API ini mendukung aplikasi mobile **KOPI NANG**, sebuah sistem pemesanan kopi berbasis lokasi dengan verifikasi visual dan sistem ulasan pengguna berbasis rating.

-----

## 📌 Fitur Utama

  * 🔐 Autentikasi JWT berbasis **Firebase Auth**
  * 🛒 Manajemen Produk dan Kategori
  * 📦 Sistem Pemesanan (Order & DetailOrder)
  * 🧾 Integrasi Pembayaran menggunakan **Midtrans** (QRIS, e-wallet)
  * 🧑‍🍳 Dashboard Admin (scan QR, ubah status pesanan, lihat statistik)
  * ⭐ Sistem Ulasan Produk dengan Rating & Review
  * 🎁 Sistem Promo: validasi penggunaan dan pengurangan kuota
  * 📈 Statistik Penjualan Harian & Produk Terlaris

-----

## 🛠️ Teknologi yang Digunakan

| Teknologi         | Keterangan                                 |
| :---------------- | :----------------------------------------- |
| ASP.NET Core      | Backend utama                              |
| Entity Framework  | ORM untuk koneksi ke PostgreSQL            |
| PostgreSQL        | Database relasional                        |
| Firebase Auth     | Autentikasi JWT                            |
| Midtrans API      | Integrasi pembayaran (QRIS & E-Wallet)    |
| Swagger UI        | Dokumentasi API                            |
| Railway           | Deployment API ke cloud                    |

-----

## 🚀 Getting Started

### 1\. Clone Repository

```bash
git clone https://github.com/nabilagx/kopinang-api.git
cd kopinang-api
```

### 2\. Konfigurasi `appsettings.Development.json`

Buat file `appsettings.Development.json` di root folder project Anda, lalu isi dengan konfigurasi berikut:

```json
{
  "Jwt": {
    "Key": "your_secret_key",
    "Issuer": "KopiNangAPI"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=kopinangdb;Username=postgres;Password=yourpassword"
  },
  "Firebase": {
    "ProjectId": "your-firebase-project-id"
  },
  "Midtrans": {
    "ServerKey": "your-midtrans-server-key"
  }
}
```

### 3\. Jalankan Migration dan API

```bash
dotnet ef database update
dotnet run
```

Akses Swagger UI di:

📄 `http://localhost:5000/swagger`

-----

## ☁️ Deployment ke Railway

Proyek ini dideploy ke Railway dan dapat diakses melalui:

🔗 [https://kopinang-api-production.up.railway.app](https://kopinang-api-production.up.railway.app)

**Langkah Deploy:**

1.  Push project ke GitHub.
2.  Hubungkan repo ke Railway ([railway.app](https://railway.app/)).
3.  Tambahkan *environment variable* berikut di Railway:
      * `ConnectionStrings__DefaultConnection`
      * `Jwt__Key`
      * `Firebase__ProjectId`
      * `Midtrans__ServerKey`
4.  Railway akan otomatis membangun dan menjalankan proyek Anda.

-----

## 👥 Anggota Kelompok 5

| No. | Nama Lengkap        | NIM          | Kelas   |
| :-- | :------------------ | :----------- | :------ |
| 1   | Nabila Choirunisa   | 232410102059 | PAA B   |
| 2   | Fahmi Son Aji       | 232410102060 | PAA B   |
| 3   | Farhat Auliya Hasan | 232410102094 | PAA A   |

-----

## 📬 Kontak
Tentu, saya akan perbarui bagian **Kontak** di `README.md` Anda agar lebih mengarah ke *link* GitHub aplikasi Flutter.

---

🐙 **GitHub Aplikasi Mobile (Flutter):** [KOPI NANG Mobile App](https://github.com/nabilagx/aplikasi-kopi-nang.git)

---

-----

"Ngopi nggak sekadar ngopi, tapi pengalaman digital yang praktis, visual, dan terintegrasi."

— Tim KOPI NANG ☕

-----

Semoga ini membantu\! Jika Anda menginginkan versi bilingual (Indonesia + English) atau ada penyesuaian lain, jangan ragu untuk memberitahu saya.
