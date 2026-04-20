-- USERS
CREATE TABLE users (
    id_user SERIAL PRIMARY KEY,
    nama VARCHAR(100),
    email VARCHAR(100) UNIQUE,
    password VARCHAR(100),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ALAT
CREATE TABLE alat (
    id_alat SERIAL PRIMARY KEY,
    nama_alat VARCHAR(100),
    harga INT,
    stok INT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_users_email ON users(email);
-- TRANSAKSI
CREATE TABLE transaksi (
    id_transaksi SERIAL PRIMARY KEY,
    id_user INT,
    id_alat INT,
    jumlah INT,
    total_harga INT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (id_user) REFERENCES users(id_user),
    FOREIGN KEY (id_alat) REFERENCES alat(id_alat)
);
-- Index untuk pencarian nama alat & stok
CREATE INDEX idx_alat_nama ON alat(nama_alat);
CREATE INDEX idx_alat_stok ON alat(stok);

INSERT INTO alat (nama_alat,harga,stok) VALUES
('Kompor Portable', 50000, 8),
('Lampu Kepala LED', 25000, 20),
('Carrier 60L', 120000, 5),
('Tenda 4 Orang', 150000, 3),
('Sleeping Bag', 40000, 10);
INSERT INTO transaksi (id_user, id_alat, jumlah, total_harga) VALUES
(1, 1, 2, 100000),
(2, 2, 1, 25000),
(3, 3, 1, 120000),
(1, 2, 2, 50000),
(2, 1, 1, 50000);
TRUNCATE transaksi, users, alat RESTART IDENTITY CASCADE;
INSERT INTO users (nama,email,password) VALUES
('Budi', 'budi@mail.com', '123'),
('Siti', 'siti@mail.com', '123'),
('Agus', 'agus@mail.com', '123'),
('Dewi', 'dewi@mail.com', '123'),
('Rudi', 'rudi@mail.com', '123');