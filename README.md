Draft for Anleitung

# Spellfight

Spellfight ist ein schneller 2D-Multiplayer-Brawler mit Zaubern und Upgrades für das totale Chaos mit dir und deinen Freunden.

## Öffnen des Spiels

Um das Spiel zu starten, musst du Steam offen haben und angemeldet sein. Dann reicht es aus, die .exe zu starten.
Wenn alles funktioniert hat, sollte in Steam auch stehen, dass du dich derzeit im Spiel "Spacewar" befindest.

Falls dies nicht der Fall ist, gibt es mehrere Troubleshooting-Schritte:
1. Füge Steam für die .exe als fremdes Spiel hinzu.
2. Installiere Spacewar.
-> Falls das alles nicht funktioniert, schreibe uns einen Bug-Report, welcher die Ausgabe der Konsole (`F2`) beinhaltet.

## Spielgeschehen und -Ziel

Du kämpfst gegen deine Freunde in einer Arena. Jeder Treffer kostet Leben. Wenn deine Lebenspunkte auf 0 fallen, scheidest du für die laufende Runde aus. Sobald nur noch ein Spieler lebt, hat dieser die Runde gewonnen und das Spiel wechselt in die Upgrade-Phase.

In der Upgrade-Phase können alle Verlierer ein Upgrade in Form eines Zaubers oder eines Stat-Boosts wählen. Danach startet direkt die nächste Runde.

Ziel ist es, so viele Runden wie möglich zu gewinnen und mit den Zaubern und Stat-Boosts die most game breaking mechanics zu finden.

## Spielablauf

### 1. Lobby erstellen und Freunde einladen

Um eine Lobby zu erstellen, musst du im Hauptmenü auf "Host Game" klicken. Dadurch erstellst und hostest du eine eigene Lobby.

Sobald du dich in einer Lobby befindest, kannst du deine Freunde einladen, indem du über die gewohnten Steam-Menüs auf deinen Steam-Freund klickst und "Zum Spiel einladen" wählst.

Freunde können über diese Einladung oder direkt über die Steam-Freundesliste beitreten.

Tipp: In der Lobby kannst du auch die Farbe deines Magiers anpassen.

### 2. Starten eines Matches

Sind alle Mitspieler der Lobby beigetreten, müssen sich alle als bereit erklären, indem sie auf "Ready" klicken.

Sind alle Spieler bereit, kann der Host das Match starten.

### 3. Spielphase

Beim Starten eines Matches startet die Spielphase.

Die Spielphase ist die Phase, in der das Haupt-Spielgeschehen vor sich geht. In dieser kämpfen die verschiedenen Spieler als Magier gegeneinander, bis nur noch einer übrig bleibt. Der letzte Überlebende ist der Gewinner der Runde.

### 4. Upgrade-Phase

Nach jeder Runde beginnt die Upgrade-Phase.
In der Upgrade-Phase kann jeder Spieler außer dem Gewinner der Runde ein Upgrade in Form eines Zaubers oder eines Stat-Boosts aussuchen und danach die Zauber den drei verschiedenen Zauberstäben zuordnen.

Wenn jeder Spieler damit fertig ist, beginnt die nächste Runde in der Spielphase.

## Steuerung

### Tastatur und Maus

- Bewegung: `A` / `D`
- Springen: `Leertaste`
- Zielen: Mausbewegung
- Angreifen / Zauber auslösen: `Linke Maustaste`, `Rechte Maustaste` und `Shift` (für die drei verschiedenen Zauberstäbe)
- Menü / Pause: `Escape`

## Mechaniken

### Bewegung und Kampf

- Das Spiel ist auf schnelle 2D-Bewegung ausgelegt.
- Springen funktioniert am Boden und an Wänden.
- An Wänden kannst du rutschen und mit einem Wall Jump abspringen.
- Treffer und Knockback beeinflussen Position und Gesundheit, deshalb ist Bewegung genauso wichtig wie das Zielen.

### Gesundheit

- Jeder Spieler hat einen Lebensbalken mit standardmäßig 100 Leben.
- Schaden kommt durch gegnerische Projektile.
- Wenn die Lebenspunkte auf 0 fallen, stirbt der Spieler und wird für die Runde als erledigt markiert.

### Zauberstäbe

Zauberstäbe sind die Waffen des Spielers, auf ihnen können Zauber ausgerüstet werden, welche in der Spielphase gewirkt werden können.
Jeder Spieler hat drei Zauberstäbe zur Verfügung.
In der Upgrade-Phase kann sich jeder Spieler, außer dem Gewinner der letzten Runde, ein Upgrade wie z. B. ein Zauber aussuchen.
Ein Zauber kann an einen der drei Zauberstäbe angebracht werden. Jeder Zauberstab kann bis zu zehn Zauber halten.

Beim Benutzen eines Zauberstabs werden dessen Zauber nacheinander von links nach rechts gewirkt, bis alle gewirkt worden sind. Dies kann vom Spieler unterbrochen werden, indem dieser die zugewiesene Taste loslässt.
Nach dem Wirken eines Zaubers gibt es eine kleine Abklingzeit, bis der nächste Zauber gewirkt wird. Nach Benutzung eines Zauberstabs hat dieser genauso eine Abklingzeit, abhängig davon, wie viele Zauber von diesem gewirkt worden sind.

Tipp: Mehrere Zauber von einem Zauberstab nacheinander zu wirken reduziert die Abklingzeit jedes folgenden Zaubers ein wenig. Sind die folgenden Zauber verschieden, so wird dieser Effekt verstärkt.

### Upgrades

Jeder Verlierer einer Runde kann sich nach dieser ein Upgrade aussuchen. Es gibt verschiedene Upgrade-Arten: Zauber und Stat-Boosts.

#### Zauber

Zauber sind auslösbare Fähigkeiten, welche über die Zauberstäbe ausgelöst werden können.

- Firebolt
- Jump
- AcidSpray
- LightningBolt
- AcidSplash
- Snowball
- Force Beam
- Thunderwave

Zauber können sich je nach Typ deutlich unterscheiden. Es würde jedoch den Spielspaß vorwegnehmen, hier alle zu erklären.

#### Stat-Boosts

Stat-Boosts sind Upgrades, welche die Werte eines Spielers stärken.

- Armour of Health
- Heart of the Giant
- Staff of Power
- Spell Sniper
- Hat of Heavy Spells
- Staff of Flowing
- Tome of Balance
- Speed Boots
- Jump Boots
<!-- - Armour of Health
- Heart of the Giant
- Staff of Power
- Spell Sniper
- Hat of Heavy Spells: Erhöht Fähigkeitsstärke und Projektilgröße, verringert Projektilgeschwindigkeit
- Staff of Flowing: Reduziert Abklingzeit erheblich und verringert Fähigkeitsstärke
- Tome of Balance: Erhöht Leben und verringert Abklingzeiten
- Speed Boots: erhöht die Geschwindigkeit und leicht die Sprungkraft
- Jump Boots: erhöht die Sprungkraft stark -->

Durch Stat-Boosts baust du deinen Charakter über mehrere Runden immer stärker aus und passt ihn an deinen Spielstil an.

## Kurzfassung

Spellfight ist ein Arena-Brawler, in dem du dich mit Sprüngen, Wall Jumps und Zaubern durchsetzt, bis nur noch ein Magier übrig ist. Wer gut bewegt, clever aufrüstet und seine Zauber sinnvoll kombiniert, gewinnt die Runde.
