Für die Entwicklung sollte ein User Secret für den Bottoken hinzugefügt werden: 
```
dotnet user-secrets set "Bot:Token" "<hier den token einfügen>"
``` 

Der docker Container kann mit folgendem Command gebaut werden:
```
docker build -f .\announce\Dockerfile .
```