# Documentation technique C#

Cette documentation est generee a partir des commentaires XML du projet `MediaTekDocuments`.

## Artefacts
- `MediaTekDocuments.xml` : sortie XML de documentation C# (generation MSBuild / Visual Studio).

## Commande de generation
```powershell
& 'C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe' MediaTekDocuments/MediaTekDocuments.sln /t:Build /p:Configuration=Release /p:Platform="Any CPU"
```

## Point d'attention
- Les commentaires XML invalides bloquent partiellement la qualite de la doc: ils doivent etre corriges avant generation.
