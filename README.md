# Miniprojekt-Vorlage-WPF
In der Projekt-Vorlage (ch.hsr.wpf.gadgeothek.sln) finden Sie die .NET-Klassen (.dll-Projekt) und je eine Demo-Anwendung (.exe, Konsolenanwendung), um den Gadgeothek-Service ansprechen zu können und um Live Updates mittes WebSockets zu bekommen. Sie können die Solution dabei direkt als Vorlage für Ihr WPF-Projekt verwenden, nur die Zugriffs-Library (ch.hsr.wpf.gadgeothek) in Ihre eigene Solution integrieren oder den Quellcode aus dem Library-Projekt in Ihre eigene Anwendung übernehmen. Welchen Weg Sie wählen, bleibt Ihnen überlassen.

Die Projekte in der Solution:

* __ch.hsr.wpf.gadgeothek__:

   Die Library zur Verwendung in Ihrem Miniprojekt. Enthält die Domain Klassen, die Implementierung des Admin- und des Client-Interface (Service) für den Zugriff auf die Gadgeothek, sowie das WebSocket-Interface zum Erhalten von Live-Updates via WebSockets.

* __ch.hsr.wpf.gadgeothek.sample-runner__:

   Eine Konsolen-App, welche die Verwendung der Admin- und Client-Service-Klassen demonstriert. 


* __ch.hsr.wpf.gadgeothek.websockets-listener__:

  Eine Konsolen-App, welche demonstriert, wie mit dem WebSocket-Interface Live-Updates vom Server erhalten werden können. 

 
Setzen Sie diesen Code in Ihrem Miniprojekt ein, um Zeit und Aufwand zu sparen. Gerne können Sie diesen auch beliebig ergänzen und verändern.
