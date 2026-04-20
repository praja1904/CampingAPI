# 🏕️ Camping Equipment Rental API

## 📌 Deskripsi Project

Project ini merupakan REST API untuk sistem penyewaan alat camping.
API ini memungkinkan pengguna untuk melakukan pengelolaan data alat, transaksi penyewaan, dan autentikasi user menggunakan JWT.

## ⚙️ Teknologi yang Digunakan

* Bahasa: C#
* Framework: ASP.NET Core Web API (.NET 8 / .NET 10)
* Database: PostgreSQL
* Library:

  * Npgsql
  * JWT Authentication
  * Swagger (Swashbuckle)

## 🚀 Cara Menjalankan Project

1. Buka di Visual Studio

2. Pastikan koneksi database di `appsettings.json`

```
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=camping;Username=postgres;Password=123"
}
```

4. Jalankan project

```
F5 / Ctrl + F5
```

---

## 🗄️ Cara Import Database

1. Buka pgAdmin
2. Buat database baru (misal: `camping`)
3. Klik kanan → Query Tool
4. Copy isi file `database.sql`
5. Klik Execute

---

## 🔐 Cara Login

Endpoint:

```
POST /api/Auth/login
```

Contoh:

```
email: budi@mail.com
password: 123
```

Ambil token → gunakan di Swagger Authorize

---

## 📌 Daftar Endpoint

| Method | Endpoint        | Keterangan                |
| ------ | --------------- | ------------------------- |
| POST   | /api/Auth/login | Login & mendapatkan token |
| GET    | /api/Alat       | Ambil semua alat          |
| GET    | /api/Alat/{id}  | Ambil alat berdasarkan ID |
| POST   | /api/Alat       | Tambah alat               |
| PUT    | /api/Alat/{id}  | Update alat               |
| DELETE | /api/Alat/{id}  | Hapus alat                |
| GET    | /api/Transaksi  | Ambil semua transaksi     |
| POST   | /api/Transaksi  | Tambah transaksi          |

---

## 📦 Format Response

Contoh sukses:

```json
{
  "success": true,
  "message": "Data berhasil diambil",
  "data": [...]
}
```

Contoh error:

```json
{
  "success": false,
  "message": "Data tidak ditemukan"
}
```

---

## 🔒 Keamanan

Semua query menggunakan parameterized query untuk mencegah SQL Injection.

---

## 🎥 Video Presentasi

[Link: https://youtube.com/your-video-link](https://youtu.be/S2bBCs7b9lk?si=i_nMMcUhmomCqfM8)

---
