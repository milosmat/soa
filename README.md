# SOA implementirano:

##  Implementirane funkcionalnosti

###  1. Kontrolna tačka (KT1)

- Neregistrovani korisnik može da se registruje i odabere ulogu:
  - **Turista**
  - **Vodič**
  - **Administrator** (ubacuje se direktno u bazu)
- Profil korisnika obuhvata:
  - korisničko ime
  - lozinku
  - email
  - ulogu
- Korisnik može da menja informacije sa svog profila.

---

###  Docker

- Napisan je **Dockerfile** za svaki servis (Stakeholders, Tours, Purchase).
- Napisan je **docker-compose.yml** koji podiže sve servise zajedno sa MongoDB bazom.
- Omogućena je **međusobna komunikacija** između izolovanih kontejnera i frontend aplikacije.

---

###  2. Kontrolna tačka (KT2)

- Autor može da kreira **draft turu**:
  - naziv, opis, težina, tagovi
  - cena postavljena na **0**
  - status ture: **draft**
  - autor može da vidi sve svoje ture
- Autor može da dodaje **ključne tačke** (checkpoint-ove):
  - geo. širina i dužina
  - naziv
  - opis
  - slika
- Dužina ture se automatski izračunava na osnovu ključnih tačaka.
- Implementiran **Simulator pozicije**:
  - turista vidi mapu
  - može klikom da definiše trenutnu lokaciju
  - lokacija se koristi za **Tour Execution**

---

###  3. Kontrolna tačka (KT3)

- Turista može da kupi objavljene ture:
  - dodavanje ture u **korpu (ShoppingCart)**
  - svaka stavka u korpi sadrži:
    - naziv ture
    - cenu
    - id ture
  - korpa računa **ukupnu cenu**
- Kada turista uradi **Checkout**:
  - generišu se tokeni (**TourPurchaseToken**) za kupljene ture
  - kupljene ture prikazuju sve ključne tačke
- Pravila:
  - arhivirane ture se ne mogu kupiti
  - nekupljene ture prikazuju samo osnovne informacije
- Frontend omogućava:
  - pregled korpe
  - kupovinu tura
  - pregled kupljenih tura i njihovih detalja

---

###  SAGA obrazac 

Implementiran je **SAGA obrazac** preko orkestracije između dva mikroservisa:

- **Purchase API** (korpa i checkout)
- **Tours API** (ture i rezervacije)

**Proces Checkout-a:**

1. Purchase API pokušava da rezerviše sve ture u Tours API-ju (`/reserve` endpoint).
2. Ako je rezervacija uspešna → kreiraju se tokeni o kupovini.
3. Nakon uspešne kupovine, ture se potvrđuju (`/confirm` endpoint).
4. Ako bilo šta pukne, sve prethodne rezervacije se otkazuju (`/cancel` endpoint).

Ovo obezbeđuje **atomicnost** procesa kupovine i otporan je na delimične greške.

---

##  Frontend

- Napisan u **Angular-u**.
- Omogućava:
  - Registraciju / login
  - Pregled i izmenu profila
  - Pregled svih objavljenih tura
  - Dodavanje tura u korpu
  - Checkout procesa (kupovina)
  - Pregled kupljenih tura i detalja (sa mapom i checkpoint-ima)
  - Simulator pozicije turiste

---

##  Pokretanje

### Preko Dockera:

```bash
docker-compose up --build
```
