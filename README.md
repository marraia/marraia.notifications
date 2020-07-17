![.NET Core](https://github.com/marraia/marraia.notifications/workflows/.NET%20Core/badge.svg?branch=master)

# Utilização do Marraia.Notifications em aplicações em .Net Core

Em sua API, notifique suas regras de negócio ao invés de usar exceptions!

## Injetar o uso do SmartNotification da biblioteca Marraia.Notifications em sua aplicação

Faça a instalação da biblioteca via nuget:
**Install-Package Marraia.Notifications -Version 1.0.0**

No arquivo Startup.cs de sua aplicação adicione no método **ConfigureServices** o middleware em específico:
```
        public void ConfigureServices(IServiceCollection services)
        {
            ..
            ..
            services.AddSmartNotification();
        }
```
## Notificar 

Em suas regras de negócio ao invés de usar exceptions para saber, se alguma regra foi violada, notifique suas mensagens.
Para suas API´s, você poderá especificar se as suas regras são de uma má requisição(Bad Request (400)) ou se alguma regra foi violada(Conflict (409))

```
public class UserAppService : IUserAppService
{
    private readonly IUserRepository _userRepository;
    private readonly ISmartNotification _notification;

    public UserAppService(
      ISmartNotification notification,
      IUserRepository userRepository)
    {
        _notification = notification;
        _userRepository = userRepository;
    }
    
    public async Task InsertAsync(UserInput user)
    {
        if (string.IsNullOrEmpty(user.Login) 
                || string.IsNullOrEmpty(user.Password)
                || string.IsNullOrEmpty(user.Name))
        {
             _notification.NewNotificationBadRequest("Dados obrigatórios");
             return;
        }
        
        if (user.Age < 18)
        {
            _notification.NewNotificationConflict("Usuário com menos de 18 anos");
            return;
        }

        var login = new User(user.Login, 
                             user.Password, user.Name);

        await _userRepository
                .InsertAsync(login)
                .ConfigureAwait(false);
    }
}
```  

## Obter as notificações em sua Controller

Depois de notificar suas regras de negócio ou validações, em sua controller utilize os métodos:

-**OkOrNotFound()** => Para métodos GET, se verifica caso tenha algum item, o método retornará o código HTTP Ok(200).  
                   Caso não ter nenhum item, retornará o código HTTP NotFound(404)
                   
-**OkOrNoContent()** => Para métodos GET, se verifica caso tenha alguma lista com itens, o método retornará o código HTTP Ok(200).  
                    Caso não ter nenhum item na lista, retornará o código HTTP NoContent(204)
                    
-**AcceptedContent()** => Para método PUT, caso a alteração foi com sucesso, retornará Accept(202).  
                    Caso o recurso não foi encontrado, retornará NotFound(404). 
                    Caso tenha notificação retornará as notificações específicas BadRequest(404) ou Conflict(409)
                    
-**CreatedContent()** => Para método POST, caso a inserção foi com sucesso, retornará Created(201).  
                     Caso tenha notificação retornará as notificações específicas BadRequest(404) ou Conflict(409)
                     
 Faça a herança da classe **BaseController** em sua controller, e faça a injeção da interface **INotificationHandler<DomainNotification>** no contrutor.
                     
 ```
[Route("api/[controller]")]
[ApiController]
public class UserController : BaseController
{
    private readonly IUserAppService _userAppService;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="notification"></param>
    /// <param name="itemAppService"></param>
    /// <param name="accessor"></param>
    public UserController(
        INotificationHandler<DomainNotification> notification,
        IUserAppService userAppService)
    : base(notification)
    {
        _userAppService = userAppService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Post([FromBody] UserInput input)
    {
        return CreatedContent("", await _userAppService.InsertAsync(input));
    }
}
 ```

