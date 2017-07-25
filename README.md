### WaveManager

Script zum spawnen von Wellen auf jedem Mesh in Unity.


![alt text](https://thumbs.gfycat.com/KaleidoscopicMajorBluewhale-size_restricted.gif "Wie es aussieht")


Beim spawnen von einer Welle werden alle Vertex Punkte die dafür nötig sind in einem Dictionary gespeichert. Der Schlüssel ist ein Wellen Objekt das bestimmte Variablen enthält (Größe etc).

Da Wellen auch über viele verschiedene Objekte gehen können (Wände zB), muss jedes Objekt und die dazugehörigen Vertex Punkte für jede Welle gespeichert werden. Das ganze wird wieder in einem Dictionary gespeichert jeweils mit dem Object als Schlüssel. 

Wir haben also ein doppeltes Dictionary: Ein Dictionary das ein Dictionary speichert, mit den Wellen als Schlüssel für das zweite. Dieses Dictionary enthält alle Objekte und die dazu benötigten Vertex Punkte die verändert werden müssen.

Wir iterieren dann über das Ganze: Jede Welle wird durchgegangen, für jede Welle wird jedes Objekt durchgegangen und für jedes Object wird jeder Vertex Punkt durchgegangen. Wir ändern nun die Vertex Punkte von dem Objekt nach der Formel

## offset = Sinus(PC-Zeit * Wellengeschwindigkeit + Wellenreichweite * Wellenfrequenz) * WellenZeit) * Wellengröße;

Wichtig ist auch das eine Welle sich aufbauen muss deswegen hat jede Welle ein Distanz Dictionary welches speichert wie weit Fortgeschritten diese Welle ist. 


