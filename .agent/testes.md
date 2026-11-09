# Testes

O projeto `testes/EscribaApp.Dominio.Tests` usa NUnit e referencia somente o
projeto Dominio. Não requer PostgreSQL, servidor Web ou mocks de persistência.

Na pasta da solução:

```powershell
dotnet restore EscribaApp.slnx
dotnet test testes/EscribaApp.Dominio.Tests/EscribaApp.Dominio.Tests.csproj --no-restore
dotnet build EscribaApp.slnx --no-restore
```

Verificar regras de criação, atomicidade de falhas, associações entre elementos
do mesmo projeto, bloqueio de remoções, igualdade de objetos de valor e proteção
das coleções. Para ramificações, cobrir convergências, prioridades, rejeição de
ciclos, componentes desconectados e precedência das ligações na ordem topológica.
