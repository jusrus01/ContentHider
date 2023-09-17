# Content Hider
## Sprendžiamo uždavinio aprašymas
### Sistemos paskirtis
Projekto tikslas – suteikti organizacijoms įrankį apdoroti esamus tekstinio tipo failus, kad patenkintų GDPR (angl. general data protection regulation). Pavyzdžiui, vartotojui ištrynus tam tikrą paskyrą, kompanijos turi per tam tikrą laiką pašalinti vartotojo duomenis. Šis įrankis galėtų išanalizuoti tekstą pagal kompanijos sudarytas taisykles ir paslėpti visą informaciją, kuri sietų vartotoją su tam tikru įrašu. Taip pat, galima išplėsti šią sistema ir panaudoti tam tikrų duomenų validacijai t.y. prieš siunčiant duomenis būtų galima sukonfigūruoti, kad sistema patikrintų tekstą ir užslėptų informaciją prieš siunčiant, tuomet galėtume labiau būti užtikrinti, kad tam tikra informacija nebuvo paskleista (būtų galima integruoti su paštu ir siunčiamais laiškais).
Veikimo principas – sukuriama organizacija, ji sukuria tam tikriems failų tipams ar formatams taisykles pagal, kurias turi būti atrenkami duomenys, kuriuos reikės užslėpti.
Funkciniai reikalavimai
Sistema turės tris dalykinės srities esybes: organizacija, teksto formatai, taisyklės. Jos yra susietos hierarchiniu ryšiu.
Neregistruotas sistemos naudotojas galės:
1.	Registruotis.
2.	Prisijungti.
Registruotas sistemos naudotojas galės:
1.	Atsijungti.
2.	Sukurti organizaciją.
3.	Ištrinti organizaciją.
4.	Redaguoti organizaciją.
5.	Pridėti vartotoją prie organizacijos.
6.	Peržiūrėti organizacijų sąrašą.
7.	Sukurti teksto formato tipą.
8.	Redaguoti teksto formato tipą.
9.	Ištrinti teksto formato tipą.
10.	Peržiūrėti teksto formato tipą.
11.	Peržiūrėti teksto formatų sąrašą.
12.	Sukurti taisyklę.
13.	Ištrinti taisyklę.
14.	Peržiūrėti taisykles tam tikram teksto formato tipui.
15.	Redaguoti taisyklę.
16.	Peržiūrėti taisyklę.
17.	Užslėpti tekstą pagal taisykles.
Administratorius galės:
1.	Peržiūrėti visas sukurtas organizacijas.
 
## Sistemos architektūra
Sistemos sudedamosios dalys:
1.	Klientinė programa, naudojant Angular 16.
2.	Serverinė programa, naudojant .NET 8 ir MySQL duomenų bazę.
Toliau pateikiama loginė diegimo diagrama:
![image](https://github.com/jusrus01/ContentHider/assets/75424325/7aed0ef7-342b-4fc1-bc69-53a75eebd3a8)
Vartotojo sąsaja ir serveris bei duomenų bazė bus diegiami atskirai. Fiziniam diegimui bus naudojamos Microsoft Azure paslaugos: Azure Static Web Pages, Azure App Service ir Azure Database for MySQL. Veikimo principas paprastas, klientas siunčia HTTP užklausą naudodamas vartotojo sąsaja, tuomet serveris gaudamas užklausą ją apdoroja ir grąžina klientui. Prireikus serveris siųs užklausas duomenų bazės serveriui per TCP/IP protokolą.
