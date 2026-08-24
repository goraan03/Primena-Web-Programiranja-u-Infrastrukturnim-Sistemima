# TravelPlanner

Web aplikacija za planiranje putovanja — mikroservisna arhitektura na Microsoft Service Fabric platformi.

## Opis projekta

Aplikacija omogućava korisnicima kreiranje planova putovanja, upravljanje destinacijama, organizaciju aktivnosti po danima (sa kalendarskim prikazom), evidenciju troškova i budžeta, checklist/packing listu, kao i deljenje planova sa drugim osobama putem linka i QR koda (sa VIEW ili EDIT nivoom pristupa). Sistem podržava dve uloge — Korisnik i Admin, gde Admin ima uvid u sve korisnike i sva putovanja u sistemu.

## Arhitektura sistema

Vidi `architecture-diagram.svg`.

Sistem se sastoji od četiri Service Fabric servisa i React frontend-a. Svaki servis koji poseduje podatke ima **sopstveno, odvojeno skladište** — princip da servisi ne dele bazu:

- **BackendAPI** (Stateless) — gateway, JWT validacija, REST kontroleri, prosleđuje zahteve ka servisima preko Service Fabric Remoting-a (V2_1). Ne poseduje sopstvene podatke.
- **AuthService** (Stateless) — registracija, prijava, JWT izdavanje, BCrypt heširanje lozinki, admin upravljanje korisničkim nalozima. Baza: **UsersDB**.
- **TravelService** (Stateless) — CRUD nad planovima putovanja, destinacijama, aktivnostima, troškovima, budžetom, checklist-om i deljenjem planova (share token, QR kod). Baza: **TravelPlannerDB**.
- **NotificationService** (Stateful) — čuva istoriju poslatih notifikacija (dobrodošlica pri registraciji, potvrda kreiranja putovanja) u Reliable Dictionary — sopstveno skladište unutar Service Fabric klastera, bez potrebe za SQL bazom.

Brisanje korisnika (AuthService) automatski briše i sva njegova putovanja (TravelService) preko međuservisnog Service Fabric Remoting poziva — cross-service cascade delete, pošto fizička FK veza između odvojenih baza nije moguća.

## Use Case dijagram

Vidi `usecase-diagram.svg`. Sistem ima dve uloge — **Korisnik** (upravljanje sopstvenim planovima putovanja) i **Admin** (nasleđuje sve funkcionalnosti Korisnika, uz dodatna ovlašćenja pregleda i brisanja korisničkih naloga i uvida u sva putovanja u sistemu).

## Tehnički stack

**Backend:** ASP.NET Core, Microsoft Service Fabric, Entity Framework Core, Microsoft SQL Server, BCrypt.Net-Next, JWT (Microsoft.AspNetCore.Authentication.JwtBearer), QRCoder.

**Frontend:** React (Vite), React Router, Axios, Context API, jsPDF/jspdf-autotable.

## Pokretanje projekta

### Preduslovi

- Visual Studio 2022 sa Azure development workload-om
- Microsoft Azure Service Fabric SDK i Runtime
- .NET 8 SDK
- SQL Server Express + SSMS
- Node.js (LTS)

### Baza podataka

1. Pokreni lokalnu SQL Server Express instancu (`SQLEXPRESS`), uz omogućen TCP/IP protokol i pokrenut SQL Server Browser servis (potrebno za named instance).
2. Kreiraj dve prazne baze u SSMS-u:
   ```sql
   CREATE DATABASE UsersDB;
   CREATE DATABASE TravelPlannerDB;
   ```
3. Primeni migracije za svaki servis:
   - `cd AuthService` → `dotnet ef database update`
   - `cd TravelService` → `dotnet ef database update`
4. Dodeli `NT AUTHORITY\NETWORK SERVICE` (nalog pod kojim Service Fabric pokreće servise) pristup obema bazama:
   ```sql
   USE UsersDB;
   CREATE USER [NT AUTHORITY\NETWORK SERVICE] FOR LOGIN [NT AUTHORITY\NETWORK SERVICE];
   ALTER ROLE db_owner ADD MEMBER [NT AUTHORITY\NETWORK SERVICE];

   USE TravelPlannerDB;
   CREATE USER [NT AUTHORITY\NETWORK SERVICE] FOR LOGIN [NT AUTHORITY\NETWORK SERVICE];
   ALTER ROLE db_owner ADD MEMBER [NT AUTHORITY\NETWORK SERVICE];
   ```
5. Connection string se čita iz `PackageRoot/Config/Settings.xml` svakog servisa (`AuthService` → `UsersDB`, `TravelService` → `TravelPlannerDB`).

### Backend

1. Otvori `TravelPlannerBackend.sln` u Visual Studio-u.
2. Postavi **TravelPlannerBackend** (Service Fabric aplikacija, ne pojedinačni servis) kao Startup Project.
3. Proveri da je lokalni Service Fabric klaster pokrenut (Service Fabric Local Cluster Manager → Start Local Cluster).
4. Pritisni F5. Otvoriće se Swagger na `http://localhost:8825/swagger`.

### Frontend

```
cd frontend
npm install
npm run dev
```

Otvara se na `http://localhost:5173`. URL backend-a se čita iz `frontend/.env` (`VITE_API_URL`).

### Admin nalog

Nema UI za dodelu admin uloge — prvi admin se postavlja ručno u bazi:

```sql
USE UsersDB;
UPDATE Users SET Role = 'ADMIN' WHERE Email = 'tvoj@mejl.com';
```

## Funkcionalnosti

- Registracija i prijava korisnika (JWT, BCrypt heširane lozinke)
- Uloge: Korisnik i Admin (admin pregleda i briše korisnike, pregleda sva putovanja u sistemu)
- CRUD nad planovima putovanja, destinacijama, aktivnostima (sa statusom i kalendarskim prikazom), troškovima — kreiranje, izmena i brisanje dostupni kroz UI
- Automatski obračun ukupnih troškova i preostalog budžeta, po kategorijama
- Checklist / packing lista po putovanju
- Deljenje plana putem linka i QR koda, sa dva nivoa pristupa (VIEW — samo pregled, EDIT — izmena putem deljenog linka bez prijave)
- Izvoz plana putovanja u PDF
- Validacije: krajnji datum ne može biti pre početnog, budžet ne može biti negativan, email format, dužina lozinke
- Cascade brisanje povezanih entiteta: brisanje putovanja briše destinacije/aktivnosti/troškove/checklist/share tokene (SQL FK cascade); brisanje korisnika briše njegova putovanja preko cross-service Remoting poziva