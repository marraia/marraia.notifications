# Marraia.Notifications

![.NET](https://github.com/marraia/marraia.notifications/actions/workflows/dotnet.yml/badge.svg?branch=master)

Notifique as regras de negócio da sua API em vez de lançar exceptions. As notificações viram respostas HTTP padronizadas.

Compatível com **.NET 8, 9 e 10**.

```
dotnet add package Marraia.Notifications
```

## Configuração

```csharp
builder.Services
    .AddControllers()
    .AddSmartNotificationModelStateResponse(); // opcional: erros de binding no mesmo formato

builder.Services.AddSmartNotification();
```

## Notificando nas regras de negócio

Injete `ISmartNotification` e chame o método do tipo de erro:

| Método | Status | Quando usar |
|---|---|---|
| `BadRequest(mensagem, campo?)` | 400 | Dados inválidos ou obrigatórios |
| `Unauthorized(mensagem)` | 401 | Não autenticado, credenciais inválidas, sessão expirada |
| `Forbidden(mensagem)` | 403 | Autenticado, mas sem permissão |
| `NotFound(mensagem)` | 404 | Recurso não existe |
| `Conflict(mensagem)` | 409 | Regra de negócio violada, registro duplicado |
| `Error(mensagem)` | 500 | Falha inesperada |
| `Validation(validationResult)` | 400 | Um BadRequest por erro do FluentValidation, com o campo |

```csharp
public class UserAppService : IUserAppService
{
    private readonly IUserRepository _userRepository;
    private readonly ISmartNotification _notification;

    public UserAppService(IUserRepository userRepository, ISmartNotification notification)
    {
        _userRepository = userRepository;
        _notification = notification;
    }

    public async Task<UserViewModel?> InsertAsync(UserInput input)
    {
        var user = new User(input.Name, input.Email);

        var validation = new UserValidator().Validate(user);
        if (!validation.IsValid)
        {
            _notification.Validation(validation);
            return null;
        }

        if (await _userRepository.ExistsAsync(user.Email))
        {
            _notification.Conflict("Já existe um usuário com este e-mail");
            return null;
        }

        await _userRepository.InsertAsync(user);
        return UserViewModel.From(user);
    }
}
```

Também há as propriedades `IsValid`, `HasNotifications`, `Notifications` e `MostSevereType`.

Serviços que preferirem podem herdar de `EntityValidator` e usar `NotifyValidationErrors(...)`.

## Respondendo na controller

Herde de `BaseController`, que recebe o `ISmartNotification`:

```csharp
[Route("api/[controller]")]
[ApiController]
public class UserController : BaseController
{
    private readonly IUserAppService _userAppService;

    public UserController(ISmartNotification notification, IUserAppService userAppService)
        : base(notification)
    {
        _userAppService = userAppService;
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] UserInput input)
        => CreatedContent("", await _userAppService.InsertAsync(input));
}
```

Se houver notificação, qualquer método abaixo responde com o erro. Sem notificação:

| Método | Resposta |
|---|---|
| `OkOrNotFound(resultado)` | 200 com os dados; 404 se nulo |
| `OkOrNoContent(resultado)` | 200 com os dados; 204 se nulo ou lista vazia |
| `CreatedContent(rota, resultado)` | 201 |
| `AcceptedContent(resultado)` | 202; 404 se nulo |
| `AcceptedOrContent(resultado)` | 202, com ou sem corpo |
| `NoContentOrNotification()` | 204 (comandos sem retorno) |

### Formato das respostas

Sucesso:

```json
{ "success": true, "data": { "id": "..." } }
```

Erro (o status é o da notificação mais grave: 500 > 401 > 403 > 404 > 409 > 400):

```json
{
  "error": {
    "code": "BadRequest",
    "message": [
      { "value": "O nome é obrigatório", "field": "name" },
      { "value": "A senha deve ter pelo menos 8 caracteres", "field": "password" }
    ]
  }
}
```

`field` só aparece quando o erro é de um campo.

## Migrando da 2.x

| 2.x | 3.0 |
|---|---|
| `NewNotificationBadRequest(m)` | `BadRequest(m)` ou `BadRequest(m, campo)` |
| `NewNotificationConflict(m)` | `Conflict(m)` |
| `NewNotificationError(m)` | `Error(m)` |
| — | `Unauthorized(m)`, `Forbidden(m)`, `NotFound(m)`, `Validation(result)` |
| `IsValid()` / `HasNotifications()` | `IsValid` / `HasNotifications` (propriedades) |
| `BaseController(INotificationHandler<DomainNotification> n)` | `BaseController(ISmartNotification n)` |
| `NotificationBusiness()` | `NotificationResult()` |
| `DomainNotification`, `DomainNotificationHandler`, `ExceptionResponse` | `Notification`, `ErrorResponse` |

Outras mudanças:

- Não depende mais de **MediatR** nem de **Newtonsoft.Json**. Se o seu projeto usava algum deles por tabela, adicione a referência direta.
- O JSON de erro não tem mais `domainNotificationType`, `target: null` nem `innerError: null`; o `code` de erros 500 passou a ser `Error` (antes saía `Conflict`).
- `OkOrNoContent` aceita qualquer `IEnumerable` (antes quebrava com resultados de LINQ não materializados).

## Publicação

Veja [PUBLICACAO.md](PUBLICACAO.md).
