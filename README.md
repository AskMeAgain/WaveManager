### WaveManager

Script zum spawnen von Wellen auf jedem Mesh in Unity.


![alt text](https://thumbs.gfycat.com/KaleidoscopicMajorBluewhale-size_restricted.gif "Wie es aussieht")


#### Wie funktioniert das ganze? 

Beim spawnen von einer Welle werden alle Vertex Punkte die dafür nötig sind in einem Dictionary gespeichert. Der Schlüssel ist ein Wellen Construct das bestimmte Variablen enthält (Größe etc).

Da Wellen auch über viele verschiedene Objekte gehen können, muss jedes Objekt und die dazugehörigen Vertex Punkte gespeichert werden. Das ganze wird wieder in einem Dictionary gespeichert mit dem Object als Schlüssel. 

Wir haben also ein doppeltes Dictionary: Ein Dictionary das ein Dictionary speichert, mit den Wellen als Schlüssel. Dieses Dictionary enthält alle Objekte und die dazu benötigten Vertex Punkte.

Wir iterieren dann über das Ganze: Jede Welle wird durchgegangen und dann für jede Welle wird jedes Objekt durchgegangen. Wir ändern nun die Vertex Punkte von dem Objekt.

