namespace MyApi;

public static class ErrorEndpointsExtensions
{

  public const string BaseUrl = @"https://localhost:7229";
  public const string ProductNotFoundType = $"{BaseUrl}/errors/product-not-found";
  public const string ValidationErrorType = $"{BaseUrl}/errors/validation-error";

  public static void MapErrorEndpoints(this IEndpointRouteBuilder app)
  {
    // ===== DOCUMENTATION DES TYPES D'ERREURS (RFC 9457) =====

    // Page d'index des erreurs
    app.MapGet("/errors", () => Results.Content(@$"
<!DOCTYPE html>
<html lang='fr'>
<head>
    <meta charset='UTF-8'>
    <title>Types d'erreurs API</title>
    <style>
        body {{ font-family: sans-serif; max-width: 900px; margin: 50px auto; padding: 20px; }}
        h1 {{ color: #2c3e50; }}
        .error-type {{ background: #f8f9fa; padding: 20px; margin: 15px 0; border-left: 4px solid #3498db; }}
        .error-type h3 {{ margin-top: 0; }}
        a {{ color: #3498db; text-decoration: none; }}
        a:hover {{ text-decoration: underline; }}
        code {{ background: #ecf0f1; padding: 2px 6px; border-radius: 3px; }}
    </style>
</head>
<body>
    <h1>📚 Documentation des types d'erreurs</h1>
    <p>Cette page liste tous les types d'erreurs possibles de l'API, conformément à la RFC 9457.</p>
    
    <div class='error-type'>
        <h3><a href='/errors/product-not-found'>🔍 Product Not Found</a></h3>
        <p><strong>Type:</strong> <code>{ProductNotFoundType}</code></p>
        <p><strong>Status:</strong> 404 Not Found</p>
        <p>Retourné quand un produit demandé n'existe pas dans la base de données.</p>
    </div>
    
    <div class='error-type'>
        <h3><a href='/errors/validation-error'>⚠️ Validation Error</a></h3>
        <p><strong>Type:</strong> <code>{ValidationErrorType}</code></p>
        <p><strong>Status:</strong> 400 Bad Request</p>
        <p>Retourné quand les données soumises ne respectent pas les règles de validation.</p>
    </div>
    
    <hr style='margin: 40px 0;'>
    <p><a href='/'>← Retour à l'API</a> | <a href='https://www.rfc-editor.org/rfc/rfc9457.html'>RFC 9457</a></p>
</body>
</html>
", "text/html"))
    .WithName("ErrorTypesIndex")
    .ExcludeFromDescription();

    // Documentation pour product-not-found
    app.MapGet("/errors/product-not-found", () => Results.Content($@"
<!DOCTYPE html>
<html lang='fr'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Erreur : Produit non trouvé</title>
    <style>
        body {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; max-width: 800px; margin: 50px auto; padding: 20px; line-height: 1.6; color: #333; }}
        .header {{ border-bottom: 3px solid #e74c3c; padding-bottom: 20px; margin-bottom: 30px; }}
        h1 {{ color: #e74c3c; margin: 0; }}
        .type-uri {{ color: #7f8c8d; font-size: 0.9em; font-family: monospace; background: #ecf0f1; padding: 5px 10px; border-radius: 3px; display: inline-block; margin-top: 10px; }}
        .section {{ margin: 30px 0; padding: 20px; background: #f8f9fa; border-left: 4px solid #3498db; border-radius: 4px; }}
        .section h2 {{ color: #2c3e50; margin-top: 0; }}
        code {{ background: #2c3e50; color: #ecf0f1; padding: 2px 6px; border-radius: 3px; font-family: monospace; }}
        .example {{ background: #2c3e50; color: #ecf0f1; padding: 15px; border-radius: 5px; overflow-x: auto; }}
        .example pre {{ margin: 0; }}
        .status-badge {{ display: inline-block; background: #e74c3c; color: white; padding: 5px 10px; border-radius: 3px; font-weight: bold; font-size: 0.9em; }}
        ul {{ padding-left: 20px; }}
        li {{ margin: 10px 0; }}
        a {{ color: #3498db; text-decoration: none; }}
        a:hover {{ text-decoration: underline; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>🔍 Produit non trouvé</h1>
        <div class='type-uri'>{ProductNotFoundType}</div>
        <p><span class='status-badge'>HTTP 404 Not Found</span></p>
    </div>

    <div class='section'>
        <h2>📋 Description</h2>
        <p>Cette erreur se produit lorsque vous tentez d'accéder à un produit qui n'existe pas dans la base de données ou qui a été supprimé.</p>
    </div>

    <div class='section'>
        <h2>🎯 Causes possibles</h2>
        <ul>
            <li>L'ID du produit spécifié n'existe pas</li>
            <li>Le produit a été supprimé récemment</li>
            <li>Erreur de frappe dans l'ID du produit</li>
        </ul>
    </div>

    <div class='section'>
        <h2>💡 Solutions recommandées</h2>
        <ul>
            <li><strong>Vérifier l'ID :</strong> Assurez-vous que l'ID du produit est correct</li>
            <li><strong>Lister les produits :</strong> Utilisez <code>GET /products</code> pour obtenir la liste complète</li>
        </ul>
    </div>

    <div class='section'>
        <h2>📝 Exemple de réponse d'erreur</h2>
        <div class='example'><pre>{{
  ""type"": ""{ProductNotFoundType}"",
  ""title"": ""Product not found"",
  ""status"": 404,
  ""detail"": ""No product found with ID 999"",
  ""instance"": ""/products/999"",
  ""traceId"": ""00-abc123def456...""
}}</pre></div>
    </div>

    <div class='section'>
        <h2>🔧 Comment gérer cette erreur (JavaScript)</h2>
        <div class='example'><pre>try {{
  const response = await fetch('/products/123');
  
  if (response.status === 404) {{
    const problem = await response.json();
    console.log('Produit introuvable:', problem.detail);
  }}
}} catch (error) {{
  console.error('Erreur réseau:', error);
}}</pre></div>
    </div>

    <hr style='margin: 40px 0;'>
    <p><a href='/errors'>← Tous les types d'erreurs</a> | <a href='/'>Retour à l'API</a></p>
</body>
</html>
", "text/html"))
    .WithName("ProductNotFoundDoc")
    .ExcludeFromDescription();

    // Documentation pour validation-error
    app.MapGet("/errors/validation-error", () => Results.Content(@$"
<!DOCTYPE html>
<html lang='fr'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Erreur : Validation échouée</title>
    <style>
        body {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; max-width: 800px; margin: 50px auto; padding: 20px; line-height: 1.6; color: #333; }}
        .header {{ border-bottom: 3px solid #f39c12; padding-bottom: 20px; margin-bottom: 30px; }}
        h1 {{ color: #f39c12; margin: 0; }}
        .type-uri {{ color: #7f8c8d; font-size: 0.9em; font-family: monospace; background: #ecf0f1; padding: 5px 10px; border-radius: 3px; display: inline-block; margin-top: 10px; }}
        .section {{ margin: 30px 0; padding: 20px; background: #f8f9fa; border-left: 4px solid #3498db; border-radius: 4px; }}
        .section h2 {{ color: #2c3e50; margin-top: 0; }}
        code {{ background: #2c3e50; color: #ecf0f1; padding: 2px 6px; border-radius: 3px; font-family: monospace; }}
        .example {{ background: #2c3e50; color: #ecf0f1; padding: 15px; border-radius: 5px; overflow-x: auto; }}
        .example pre {{ margin: 0; }}
        .status-badge {{ display: inline-block; background: #f39c12; color: white; padding: 5px 10px; border-radius: 3px; font-weight: bold; font-size: 0.9em; }}
        ul {{ padding-left: 20px; }}
        li {{ margin: 10px 0; }}
        a {{ color: #3498db; text-decoration: none; }}
        a:hover {{ text-decoration: underline; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>⚠️ Erreur de validation</h1>
        <div class='type-uri'>{ValidationErrorType}</div>
        <p><span class='status-badge'>HTTP 400 Bad Request</span></p>
    </div>

    <div class='section'>
        <h2>📋 Description</h2>
        <p>Cette erreur se produit lorsque les données que vous avez soumises ne respectent pas les règles de validation de l'API.</p>
    </div>

    <div class='section'>
        <h2>🎯 Causes possibles</h2>
        <ul>
            <li>Champs obligatoires manquants</li>
            <li>Format de données incorrect (ex: prix négatif)</li>
            <li>Valeurs hors limites acceptables</li>
            <li>Types de données incompatibles</li>
        </ul>
    </div>

    <div class='section'>
        <h2>💡 Solutions recommandées</h2>
        <ul>
            <li><strong>Vérifier les champs obligatoires :</strong> Assurez-vous que tous les champs requis sont présents</li>
            <li><strong>Vérifier les formats :</strong> Consultez la documentation de l'API pour les formats attendus</li>
            <li><strong>Lire les messages d'erreur :</strong> Chaque champ en erreur est détaillé dans la réponse</li>
        </ul>
    </div>

    <div class='section'>
        <h2>📝 Exemple de réponse d'erreur</h2>
        <div class='example'><pre>{{
  ""type"": ""{ValidationErrorType}"",
  ""title"": ""Validation failed"",
  ""status"": 400,
  ""detail"": ""One or more validation errors occurred"",
  ""instance"": ""/products"",
  ""errors"": {{
    ""name"": [""Name is required""],
    ""price"": [""Price must be greater than 0""]
  }},
  ""traceId"": ""00-abc123def456...""
}}</pre></div>
    </div>

    <div class='section'>
        <h2>🔧 Comment gérer cette erreur (JavaScript)</h2>
        <div class='example'><pre>try {{
  const response = await fetch('/products', {{
    method: 'POST',
    headers: {{ 'Content-Type': 'application/json' }},
    body: JSON.stringify({{ name: '', price: -10 }})
  }});
  
  if (response.status === 400) {{
    const problem = await response.json();
    
    // Afficher les erreurs de validation
    for (const [field, errors] of Object.entries(problem.errors)) {{
      console.error(`${{field}}: ${{errors.join(', ')}}`);
    }}
  }}
}} catch (error) {{
  console.error('Erreur réseau:', error);
}}</pre></div>
    </div>

    <hr style='margin: 40px 0;'>
    <p><a href='/errors'>← Tous les types d'erreurs</a> | <a href='/'>Retour à l'API</a></p>
</body>
</html>
", "text/html"))
    .WithName("ValidationErrorDoc")
    .ExcludeFromDescription();
  }
}
