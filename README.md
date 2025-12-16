# BoardGame
2.5D board game with multiple players and characters 

**Todo list:**
- [x] Multiple cursors
- [x] Main menu animations
- [x] Main menu buttons (start, quit, settings, leaderboard)
- [x] Character selection screen with animations
- [x] Settings scene :)
- [x] Board scene with throwable dice
- [x] Game logic with multiple players :)
- [x] Game camera :)
- [x] Leaderboard scene :)

Spēlē jūs spēlējat ar izvēlēto personāžu, un jūsu uzdevums ir uzvarēt botu, savācot vairāk uzvaru kārtās/līmeņos.

<hr>

## Kā spēlēt?
### Sākt spēli
Nospiediet pogu “Play”, izvēlieties personāžu, par kuru spēlēsiet, un dodiet tam vārdu.

### Spēles loģika
<img width="1110" height="538" alt="Screenshot 2025-12-15 202646" src="https://github.com/user-attachments/assets/b3b98ed1-9822-4d46-a2ff-6e33f137bd7b" />

- PIRMĀ spēles sākumā jūs ar botu metat kauliņus, un tas, kuram ir lielāks skaitlis, sāk spēli. (Ja izkrīt vienādi skaitļi, tad metat atkārtoti.).
- Gājienu skaits = izkritušais skaitlis uz kubika.
1) Galvenais ir sasniegt finišu ātrāk nekā bots.
2) Zilās šūnas: pārvieto personāžu par vienu zilo šūnu atpakaļ. Ja aiz personāža nav zilo šūnu, personāžs atgriežas sākuma punktā "<b>3)</b>".
- Kad sasniegsiet finišu (bot / player), spēle tiks restartēta un jūs pāriesiet uz nākamo līmeni.
- līmeņi = 3

<hr>

## Konkurētspēja 
<img width="1293" height="792" alt="Screenshot 2025-12-16 195519" src="https://github.com/user-attachments/assets/676e0409-a268-4e22-b925-2f1d627d7922" />

- Spēlē jūs pārbaudāt savu veiksmi, vai jūs varat uzvarēt botus.
- Katra no spēlēm tiek reģistrēta līderu sadaļā. 
