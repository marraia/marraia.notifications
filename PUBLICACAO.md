# Como publicar uma nova versão no NuGet

A publicação no nuget.org é **automática pelo GitHub Actions** e acontece **somente quando uma tag `v*` é enviada** (ex.: `v3.0.1`).
Merge na `master` **não publica**: apenas roda build e testes.

Não existe API key do NuGet para gerenciar: o CI usa **Trusted Publishing** (o GitHub gera uma chave temporária de 1 hora a cada publicação).

---

## Passo a passo

### 1. Alterar a versão

No arquivo `Marraia.Notifications/Marraia.Notifications.csproj`, atualize:

```xml
<AssemblyVersion>3.0.1.0</AssemblyVersion>
<FileVersion>3.0.1.0</FileVersion>
<Version>3.0.1</Version>
```

Atualize também o `<PackageReleaseNotes>` com o que mudou.

> **Qual número usar?** (versionamento semântico)
> - `3.0.0` → `3.0.1`: correção de bug, sem mudar a forma de usar.
> - `3.0.0` → `3.1.0`: funcionalidade nova, sem quebrar quem já usa.
> - `3.0.0` → `4.0.0`: mudança que quebra código de quem usa (breaking change).
> - Versão de teste: `3.1.0-alpha`, `3.1.0-beta` (aparece como pré-lançamento no NuGet).

### 2. Fazer o merge na `master`

Abra o PR, aguarde o CI ficar verde (build + testes) e faça o merge.

### 3. Criar e enviar a tag (é isso que publica)

```bash
git checkout master
git pull
git tag v3.0.1
git push origin v3.0.1
```

A tag **deve ser igual** ao `<Version>` do `.csproj`, com um `v` na frente.

### 4. Acompanhar

- **GitHub → aba Actions**: o workflow da tag deve terminar com todos os passos verdes, incluindo **NuGet login (OIDC)** e **Push to nuget.org**.
- **nuget.org**: a versão aparece em https://www.nuget.org/packages/Marraia.Notifications após **5 a 15 minutos**.

---

## Configuração única no nuget.org (antes da primeira publicação por tag)

nuget.org → clique no seu usuário → **Trusted Publishing** → adicione uma política:

| Campo | Valor |
|---|---|
| Repository Owner | `marraia` |
| Repository | `marraia.notifications` |
| Workflow File | `dotnet.yml` (só o nome do arquivo) |
| Environment | (em branco) |
| Glob pattern / Packages | `Marraia.Notifications` (ou `Marraia.*`) |
| Scope | Push new packages and package versions |

> Se o nome do repositório ou do arquivo `dotnet.yml` mudar, a política precisa ser atualizada no nuget.org.

Depois que a publicação por tag funcionar, o secret antigo `KEY_NUGET` pode ser removido do repositório no GitHub
e a API key correspondente pode ser revogada no nuget.org.
