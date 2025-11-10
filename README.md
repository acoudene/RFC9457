# Objectif

Mettre en avant le respect de la norme RFC9457 en .Net.

# Synthèse de la norme RFC9457

## Référence
https://www.rfc-editor.org/rfc/rfc9457.html

## But

L'objectif principal de la RFC9457 est de normaliser la gestion du détail des "problèmes" rencontrés avec une API en http.
La spécification sert à définir des formats d'erreur communs pour les applications qui en ont besoin afin qu'elles ne soient pas obligées de définir les leurs ou, pire, tentées de redéfinir la sémantique des codes de statut HTTP existants

## Contenu

Type de fichier : `JSON`

Type de media : `application/problem+json`

Format de base : 
  - **type (URI)** : Identifie le type de problème spécifique avec un URI, permettant aux API HTTP d'utiliser des URI sous leur contrôle pour identifier des problèmes qui leur sont spécifiques ou de réutiliser des URI existants pour faciliter l'interopérabilité. 2 options possibles :
    - URL déréférençable : Elle doit retourner de la documentation HTML
    - Tag URI : Simple identifiant unique, non déréférençable (ex: urn:problem-type:product-not-found)
  - **title (string)** : Un résumé court et lisible du type de problème
  - **status (number)** : Le code de statut HTTP généré par le serveur
  - **detail (string)** : Une explication détaillée spécifique à cette occurrence du problème
  - **instance (URI)** : Une URI identifiant l'occurrence spécifique du problème, ce qui peut être utile à des fins de support ou de forensics

## Points importants

- Les détails de problème ne sont pas un outil de débogage pour l'implémentation sous-jacente ; il s'agit plutôt d'un moyen d'exposer plus de détails sur l'interface HTTP elle-même.
- Les URI de type doivent être stables dans le temps et sous le contrôle de l'organisation.
- Le format standard évite la nécessité de créer de nouveaux formats de réponse d'erreur pour chaque API.

## Cas particulier du type déréférençable

Il peut être intéressant d'utiliser un portail web référençant les cas d'erreur avec une documentation.

Exemple avec le cas d'un produit non trouvé : 

<img width="298" height="912" alt="image" src="https://github.com/user-attachments/assets/7080d920-477c-47ae-98bc-83d3b750fd5c" />


# Concrètement en .Net 9

- [ ] Ajouter le mécanisme de `Problem Details`: 

```csharp
// Adds services for using Problem Details format
builder.Services.AddProblemDetails(options =>
{
  options.CustomizeProblemDetails = context =>
  {
    context.ProblemDetails.Instance =
        $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

    context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

    Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
    context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
  };
});
```

- [ ] Personnaliser la conversion des exceptions en `Problem Details`

```csharp
public class CustomExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
  public async ValueTask<bool> TryHandleAsync(
      HttpContext httpContext,
      Exception exception,
      CancellationToken cancellationToken)
  {
    var problemDetails = new ProblemDetails
    {
      Status = exception switch
      {
        ArgumentException => StatusCodes.Status400BadRequest,
        _ => StatusCodes.Status500InternalServerError
      },
      Title = "An error occurred",
      Type = exception.GetType().Name,
      Detail = exception.Message
    };

    return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
    {
      Exception = exception,
      HttpContext = httpContext,
      ProblemDetails = problemDetails
    });
  }
}
```

- [ ] Ajouter la personnalisation des exceptions en `Problem Details`

```csharp
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
```

- [ ] Convertir les exceptions en `Problem Details`

```csharp
// Converts unhandled exceptions into Problem Details responses
app.UseExceptionHandler();

// Returns the Problem Details response for (empty) non-successful responses
app.UseStatusCodePages();
```
